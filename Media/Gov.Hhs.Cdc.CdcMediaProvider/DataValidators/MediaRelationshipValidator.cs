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

using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class MediaRelationshipValidator : IValidator<MediaRelationshipObject, MediaRelationshipValidationObject>  
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<MediaRelationshipObject> items)
        {
            foreach (MediaRelationshipObject relationship in items)
            {
                RegExValidator v = new RegExValidator(validationMessages, relationship.ValidationKey);
                v.IsValid(v.AlphanumericSpaces, relationship.RelationshipTypeName, required: false, message: "RelationshipTypeName is invalid");
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<MediaRelationshipObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, 
            IList<MediaRelationshipValidationObject> items)
        {
            MediaObjectContext media = (MediaObjectContext)objectContext;
            foreach (MediaRelationshipValidationObject set in items)
            {
                RelationshipTypeItem relationshipType = MediaCacheController.GetRelationshipType(media, set.Relationship.RelationshipTypeName);

                if (set.Relationship.MediaId == set.Relationship.RelatedMediaId)
                { validationMessages.AddError(set.Relationship.ValidationKey, "Media cannot have a reference to itself"); }

            }
            ValidateMediaItemsExist(objectContext, validationMessages, items, true);
        }


        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, 
            IList<MediaRelationshipValidationObject> items)
        {
            foreach (MediaRelationshipValidationObject set in items)
            {
                if (set.Relationship.MediaId == 0 || set.Relationship.RelatedMediaId == 0)
                { validationMessages.AddError(set.Relationship.ValidationKey, 
                    "MediaRelationshipObject Id is 0.  To Delete the Id must be valid"); }
            }
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, 
            IList<MediaRelationshipValidationObject> items)
        {
            MediaObjectContext media = (MediaObjectContext)objectContext;
            foreach (MediaRelationshipValidationObject set in items)
            {
                //Note: some relationships might be deleted, so they will be null
                if (set.Relationship != null)
                { ValidateRecursiveAndReverseRelationships(media, validationMessages, set.Relationship); }
                if (set.Inverse != null)
                { ValidateRecursiveAndReverseRelationships(media, validationMessages, set.Inverse); }
            }
        }

        public MediaRelationshipValidationObject GetValidationObject(IDataServicesObjectContext objectContext, 
            ValidationMessages validationMessages, MediaRelationshipObject theObject)
        {
            var allRelationships = MediaRelationshipCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                .Where(mr => mr.MediaId == theObject.MediaId || mr.RelatedMediaId == theObject.MediaId).ToList();

            RelationshipTypeItem relationshipType = MediaCacheController.GetRelationshipType((MediaObjectContext)objectContext, theObject.RelationshipTypeName);

            var parentRelationships = allRelationships.Where(r => r.RelationshipTypeName == relationshipType.RelationshipTypeName).ToList();
            var childRelationships = allRelationships.Where(r => r.RelationshipTypeName == relationshipType.InverseRelationshipTypeName).ToList();
                        
            return new MediaRelationshipValidationObject()
           {
               Relationship = theObject,
               OriginalRelationship = parentRelationships.Where(oldMr =>
                                  theObject.RelatedMediaId == oldMr.RelatedMediaId).FirstOrDefault(),
               Inverse = GetInverseRelationship(objectContext, theObject, validationMessages),
               OriginalInverse = childRelationships.Where(oldMr =>
                                 theObject.RelatedMediaId == oldMr.MediaId).FirstOrDefault()
           };

        }

        public static MediaRelationshipObject GetInverseRelationship(IDataServicesObjectContext objectContext, 
            MediaRelationshipObject theObject, ValidationMessages validationMessages)
        {
            var db = objectContext as MediaObjectContext;
            MediaRelationshipObject inverseObject = null;
            RelationshipTypeItem relationshipType = MediaCacheController.GetRelationshipType(db, theObject.RelationshipTypeName);
            if (relationshipType != null && relationshipType.IsActive)
            {
                theObject.RelationshipTypeName = relationshipType.RelationshipTypeName;
                RelationshipTypeItem inverseType = MediaCacheController.GetRelationshipType(db, relationshipType.InverseRelationshipTypeName);
                if (inverseType != null && inverseType.IsActive)
                { inverseObject = new MediaRelationshipObject(theObject, inverseType); }
            }
            else
            { validationMessages.AddError(theObject.ValidationKey, "Invalid relationship type"); }
            return inverseObject;
        }


        private static void ValidateRecursiveAndReverseRelationships(MediaObjectContext media, ValidationMessages validationMessages, 
            MediaRelationshipObject relationship)
        {
            string inverseRelationshipTypeName =
                MediaCacheController.GetRelationshipType(media, relationship.RelationshipTypeName).InverseRelationshipTypeName;
            if (string.IsNullOrEmpty(inverseRelationshipTypeName))
            { return; }
            bool exists = media.MediaDbEntities.HasInheritedMediaRelationship(relationship.MediaId, inverseRelationshipTypeName, 
                relationship.RelatedMediaId).First() == "Yes";
            if (exists)
            {
                validationMessages.AddError(relationship.ValidationKey,
                      string.Format("Relationship already has an inherited {0} relationship", inverseRelationshipTypeName));
            }
            bool recursiveLoopExists = media.MediaDbEntities.HasRecursiveLoopMediaRelationship(relationship.MediaId, 
                inverseRelationshipTypeName).First() == "Yes";
            if (recursiveLoopExists)
            {
                validationMessages.AddError(relationship.ValidationKey,
                      string.Format("Relationship implies a recursive loop",
                      inverseRelationshipTypeName));
            }

        }

        private static void ValidateMediaItemsExist(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, 
            IList<MediaRelationshipValidationObject> items, bool forUpdate = false)
        {

            var allRelationshipsForExistingMedias = items
                //We only need the relationship, because the inverse uses the same media items
                //.SelectMany(s => new List<MediaRelationshipObject>() { s.Relationship, s.Inverse })
                .Select(s => s.Relationship)
                .SelectMany(r => new List<Tuple<string, int>>(){
                    new Tuple<string, int>(r.ValidationKey, r.MediaId),
                    new Tuple<string, int>(r.ValidationKey, r.RelatedMediaId)
                })
                .Where(t => t.Item2 != 0).ToList();
            List<int> allRelatedMedias = allRelationshipsForExistingMedias.Select(r => r.Item2).Distinct().ToList();
            var medias = MediaCtl.Get((MediaObjectContext)objectContext, forUpdate);
            List<MediaObject> referencedMedias = medias.Where(m => allRelatedMedias.Contains(m.Id)).ToList();
            var referencesWithoutAMediaId = allRelationshipsForExistingMedias
                .Where(r => !referencedMedias.Select(m => m.Id).Contains(r.Item2)).Distinct().ToList();
            foreach (var missingReference in referencesWithoutAMediaId)
            {
                validationMessages.AddError(missingReference.Item1, "Media item not found");
            }
        }

    }
}
