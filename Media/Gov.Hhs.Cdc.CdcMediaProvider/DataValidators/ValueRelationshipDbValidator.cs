// Copyright [2015] [Centers for Disease Control and Prevention] 
// Licensed under the CDC Custom Open Source License 1 (the 'License'); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
// 
//   http://t.cdc.gov/O4O
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    ///The ValueRelationshipObjectDbValidator class is an extension of the ValueRelationshipObjectValidator class.
    ///Normally all validation logic exists in the normal validator class (i.e., ValueRelationshipObjectValidator),
    ///but due to the amount of code required to implement the ValueRelationship validator, the DB logic was split
    ///into a separate class.
    public class ValueRelationshipDbValidator
    {
        private MediaObjectContext media { get; set; }
        private ValidationMessages validationMessages { get; set; }

        public ValueRelationshipDbValidator(MediaObjectContext media, ValidationMessages validationMessages)
        {
            this.media = media;
            this.validationMessages = validationMessages;

        }

        public void ValidateDelete(IEnumerable<IValidationObject> items)
        {
            foreach (ValueRelationshipValidationObject set in items)
            {
                ValidateLanguages(set);
                if (set.Relationship.ValueId == 0 || set.Relationship.RelatedValueId == 0)
                { validationMessages.AddError(set.Relationship.ValidationKey, 
                    "ValueRelationshipObject Id is 0.  To Delete the Id must be valid"); }
            }
        }

        public void ValidateSave(IEnumerable<IValidationObject> items)
        {
            foreach (ValueRelationshipValidationObject set in items)
            {
                //Validate ValueLanguageCode and RelatedValueLanguageCode
                ValidateLanguages(set);

                RelationshipTypeItem relationshipType = (!string.IsNullOrEmpty(set.Relationship.RelationshipTypeName))
                    ? MediaCacheController.GetRelationshipType(media, set.Relationship.RelationshipTypeName)
                    : MediaCacheController.GetRelationshipTypeByShortType(media, set.Relationship.ShortType);

                if (set.Relationship.ValueId == set.Relationship.RelatedValueId && set.Relationship.ValueLanguageCode == set.Relationship.RelatedValueLanguageCode)
                { validationMessages.AddError(set.Relationship.ValidationKey, "Value cannot have a reference to itself"); }

                ValidateValueItemsExist(set.Relationship);

                if (!validationMessages.Errors().Any())
                {
                    ValidateRelationshipBetweenValueSetsIsAllowed(set.Relationship);
                    if (set.Inverse != null)
                    { ValidateRelationshipBetweenValueSetsIsAllowed(set.Inverse); }
                }
            }
            ValidateUseRelationships(items);
        }

        public void PostSaveValidate(IEnumerable<IValidationObject> items)
        {
            foreach (ValueRelationshipValidationObject set in items)
            {
                ValidateRecursiveAndReverseRelationships(set.Relationship);
                if (set.Inverse != null)
                { ValidateRecursiveAndReverseRelationships(set.Inverse); }
            }
        }

        private void ValidateRecursiveAndReverseRelationships(ValueRelationshipObject relationship)
        {
            string inverseRelationshipTypeName =
                MediaCacheController.GetRelationshipType(media, relationship.RelationshipTypeName).InverseRelationshipTypeName;
            if (string.IsNullOrEmpty(inverseRelationshipTypeName))
            { return; }
            bool exists = PotentialValueRelationshipCtl.DoesInheritedRelationshipExists(media, relationship.ValueId, inverseRelationshipTypeName, relationship.RelatedValueId);
            if (exists)
            {
                validationMessages.AddError(relationship.ValidationKey,
                       string.Format("Relationship already has an inherited {0} relationship", inverseRelationshipTypeName));
            }
            bool recursiveLoopExists = PotentialValueRelationshipCtl.DoesRecursiveLoopRelationshipExist(
                media, relationship.ValueId, inverseRelationshipTypeName);
            if (recursiveLoopExists)
            {
                validationMessages.AddError(relationship.ValidationKey,
                      string.Format("Relationship implies a recursive loop",
                      inverseRelationshipTypeName));
            }
        }

        private void ValidateValueItemsExist(ValueRelationshipObject relationship)
        {
            IQueryable<ValueObject> valueItems = ValueCtl.Get(media);
            if (!valueItems.Where(v => new { v.ValueId, v.LanguageCode }
                    == new { relationship.ValueId, LanguageCode = relationship.ValueLanguageCode })
                    .Any())
            { validationMessages.AddError(relationship.ValidationKey, "Value not found"); }
            if (!valueItems.Where(v => new { v.ValueId, v.LanguageCode }
                    == new { ValueId = relationship.RelatedValueId, LanguageCode = relationship.RelatedValueLanguageCode })
                    .Any())
            { validationMessages.AddError(relationship.ValidationKey, "Related value not found"); }
        }

        private void ValidateLanguages(ValueRelationshipValidationObject set)
        {
            ValidateLanguageCode(set.Relationship.ValueLanguageCode, "ValueLanguageCode", set.Relationship);
            ValidateLanguageCode(set.Relationship.RelatedValueLanguageCode, "RelatedValueLanguageCode", set.Relationship);
        }

        private void ValidateLanguageCode(string languageCode, string field, DataSourceBusinessObject theObject)
        {
            LanguageItem language = MediaCacheController.GetLanguage(media, languageCode);
            if (language == null || !language.IsActive)
            { validationMessages.AddError(theObject.ValidationKey, field + " is invalid"); }
        }


        private void ValidateRelationshipBetweenValueSetsIsAllowed(ValueRelationshipObject relationship)
        {
            PotentialValueRelationshipItem invalidRelationship = PotentialValueRelationshipCtl.Get(media)
                .Where(r =>
                    r.ValueValueSetItem.ValueId == relationship.ValueId
                 && r.ValueValueSetItem.ValueLanguageCode == relationship.ValueLanguageCode
                 && r.RelatedValueValueSetItem.ValueId == relationship.RelatedValueId
                 && r.RelatedValueValueSetItem.ValueLanguageCode == relationship.RelatedValueLanguageCode
                 && r.ValueSetRelationItem.RelationshipTypeName == relationship.RelationshipTypeName
                 && !r.ValueSetRelationItem.IsRelationshipAllowed)
                 .FirstOrDefault();

            if (invalidRelationship != null)
            {
                validationMessages.AddError(relationship.ValidationKey,
                    string.Format("ValueSet {0} cannot have a relationship of '{1}' with value set '{2}'",
                        invalidRelationship.ValueValueSetItem.ValueSetKeyToString(),
                        invalidRelationship.ValueSetRelationItem.RelationshipTypeName,
                        invalidRelationship.RelatedValueValueSetItem.ValueSetKeyToString())
                    );
            }

        }

        private void ValidateUseRelationships(IEnumerable<IValidationObject> items)
        {
            IEnumerable<ValueRelationshipValidationObject> allNewRelationships = GetAllNewRelationships(items);
            var useRelationships = allNewRelationships.Where(r => r.Relationship.RelationshipTypeName == "Use");
            List<int> valueIds = useRelationships.Select(u => u.Relationship.ValueId).Distinct().ToList();  //Get value ids to limit select from DB

            IQueryable<ValueRelationshipObject> savedRelationships = ValueRelationshipCtl.Get(media, forUpdate: false)
                .Where(s => s.RelationshipTypeName == "Use" && valueIds.Contains(s.ValueId));


            //This will filter by language code as well, and will be executed in memory because 
            //the join with a LINQ to Objects and LINQ to Entities is done in memory.
            IEnumerable<ValueRelationshipValidationObject> ourSavedRelationships =
                from u in useRelationships
                join s in savedRelationships
                    on new { u.Relationship.ValueId, u.Relationship.ValueLanguageCode } equals
                        new { s.ValueId, s.ValueLanguageCode }
                select new ValueRelationshipValidationObject()
                {
                    Relationship = s
                };

            ////TAC temp
            ////string sql1 = ((ObjectQuery)ourSavedRelationships).ToTraceString();
            //var ourSavedRelationshipsAsList = ourSavedRelationships.ToList();
            //var useRelationshipsAsList = useRelationships.ToList();
            //var newInverseUseRelationshipsAsList = newInverseRelationships.ToList();
            //var itemsAsList = items.ToList();

            useRelationships = useRelationships.Union(ourSavedRelationships);

            var groups = from r in useRelationships
                         //Must do active here.  If only in DB, then take its active flag.  If the use is in new list
                         //as well as DB, then we take the active flag of the new one only (which is all that matters).
                         where r.Relationship.IsActive
                         group r by new { r.Relationship.ValueId, r.Relationship.ValueLanguageCode } into g
                         select new { Count = g.Count(), Relationship = g.Where(r => r.Relationship.ValidationKey != null).FirstOrDefault(), g };

            foreach (var groupItem in groups.Where(g => g.Count > 1))
            {
                validationMessages.AddError(groupItem.Relationship.Relationship.ValidationKey, "There is more than one instance of 'Use' relationships for this relationship");
            }
        }

        public static IEnumerable<ValueRelationshipValidationObject> GetAllNewRelationships(IEnumerable<IValidationObject> items)
        {
            IEnumerable<ValueRelationshipValidationObject> newRelationships = items.Select(i => (ValueRelationshipValidationObject)i);
            IEnumerable<ValueRelationshipValidationObject> newInverseRelationships = newRelationships.Where(t => t.Inverse != null)
                .Select(t => new ValueRelationshipValidationObject { Relationship = t.Inverse, Inverse = t.Relationship });

            IEnumerable<ValueRelationshipValidationObject> allNewRelationships = newRelationships.Union(newInverseRelationships);
            return allNewRelationships;
        }

    }
}
