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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class ValueRelationshipValidator : IValidator<ValueRelationshipObject, ValueRelationshipValidationObject>  //<ValueRelationshipObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<ValueRelationshipObject> items)
        {
            foreach (ValueRelationshipObject relationship in items)
            {
                RegExValidator v = new RegExValidator(validationMessages, relationship.ValidationKey);
                //if(relationship.ShortType == "RT")
                //{
                //    validationMessages.AddWarning(relationship.ValidationKey, "This is a sample validation warning");
                //    validationMessages.AddWarning(relationship.ValidationKey, "This is another sample validation warning");
                //}
                v.IsValid(v.AlphanumericSpaces, relationship.RelationshipTypeName, required: false, message: "RelationshipTypeName is invalid");
                v.IsValid(v.AlphanumericSpaces, relationship.ShortType, required: false, message: "ShortType is invalid");
                v.IsValid(v.AlphanumericSpaces, relationship.ValueLanguageCode, required: true, message: "ValueLanguageCode is invalid");
                v.IsValid(v.AlphanumericSpaces, relationship.RelatedValueLanguageCode, required: true, message: "RelatedValueLanguageCode is invalid");

                if (string.IsNullOrEmpty(relationship.ShortType) && string.IsNullOrEmpty(relationship.RelationshipTypeName))
                    validationMessages.AddError(relationship.ValidationKey, "Either RelationshipTypeName or ShortType must be passed");
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<ValueRelationshipObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueRelationshipValidationObject> items)
        {
            ValueRelationshipDbValidator validator = new ValueRelationshipDbValidator((MediaObjectContext)objectContext, validationMessages);
            validator.ValidateSave(items);
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueRelationshipValidationObject> items)
        {
            ValueRelationshipDbValidator validator = new ValueRelationshipDbValidator((MediaObjectContext)objectContext, validationMessages);
            validator.ValidateDelete(items);
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<ValueRelationshipValidationObject> items)
        {
            ValueRelationshipDbValidator validator = new ValueRelationshipDbValidator((MediaObjectContext)objectContext, validationMessages);
            validator.PostSaveValidate(items);
        }

        public ValueRelationshipValidationObject GetValidationObject(IDataServicesObjectContext context, ValidationMessages validationMessages, ValueRelationshipObject theObject)
        {
            ValueRelationshipValidationObject set = new ValueRelationshipValidationObject()
            {
                Relationship = theObject
            };

            RelationshipTypeItem relationshipType =  (!string.IsNullOrEmpty(theObject.RelationshipTypeName))
                ? MediaCacheController.GetRelationshipType((MediaObjectContext) context, theObject.RelationshipTypeName)
                : MediaCacheController.GetRelationshipTypeByShortType((MediaObjectContext)context, theObject.ShortType);


            if (relationshipType != null && relationshipType.IsActive)
            {
                set.Relationship.RelationshipTypeName = relationshipType.RelationshipTypeName;
                set.Relationship.ShortType = relationshipType.ShortType;
                RelationshipTypeItem inverseType = MediaCacheController.GetRelationshipType((MediaObjectContext)context, relationshipType.InverseRelationshipTypeName);
                if (inverseType != null && inverseType.IsActive)
                { set.Inverse = new ValueRelationshipObject(set.Relationship, inverseType); }
            }
            else
            { validationMessages.AddError(theObject.ValidationKey, "Invalid relationship type"); }

            return set;
        }


    }
}
