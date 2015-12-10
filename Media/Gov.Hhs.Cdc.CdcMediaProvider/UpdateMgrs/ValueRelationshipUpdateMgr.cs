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
    public class ValueRelationshipUpdateMgr : IUpdateMgr 
    {
        protected ValueRelationshipValidator Validator { get; set; }
        public IList<ValueRelationshipObject> Items;
        public List<ValueRelationshipValidationObject> RelationshipAndInverse;

        public int ValueId { get; set; }
        private List<IDataCtl> _dataControls;
        /// <summary>
        /// This is used by the UpdateIdsAfterInsert method to find the persisted items and update the new ids into the business objects
        /// </summary>
        public List<IDataCtl> InsertedDataControls
        {
            get
            {
                if (_dataControls == null) { _dataControls = new List<IDataCtl>(); }
                return _dataControls;
            }
        }
        List<ValueRelationshipValidationStatus> setsWithStatus;
        
        public bool RequiresLogicalCommit { get { return true; } }

        public string ObjectName { get { return "ValueRelationship"; } }

        /// <summary>
        /// Constructor for Value Save, so that the valueid is available for delete of value relationships
        /// </summary>
        /// <param name="items"></param>
        /// <param name="valueId"></param>
        public ValueRelationshipUpdateMgr(IList<ValueRelationshipObject> items, int valueId)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new ValueRelationshipValidator();
            ValueId = valueId;
        }

        public ValueRelationshipUpdateMgr(IList<ValueRelationshipObject> items)
            : this(items, 0)
        {
        }

        public ValueRelationshipUpdateMgr(ValueRelationshipObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new ValueRelationshipValidator();
        }

        public void Save(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            SetCommitted(RelationshipAndInverse, false);
            setsWithStatus = RelationshipAndInverse.Select(r => Save(media, r)).ToList();
        }

        private ValueRelationshipValidationStatus Save(IDataServicesObjectContext media, ValueRelationshipValidationObject relationshipSet)
        {
            ValueRelationshipValidationStatus results = new ValueRelationshipValidationStatus { Relationship = relationshipSet.Relationship, Inverse = relationshipSet.Inverse };
            results.RelationshipUpdateStatus = Save(media, relationshipSet.Relationship);
            results.InverseUpdateStatus = Save(media, relationshipSet.Inverse);
            return results;
        }

        private ValueRelationshipValidationObject.UpdateStatus Save(IDataServicesObjectContext media, ValueRelationshipObject relationship)
        {
            if (relationship == null)
            {
                return null;
            }
            ValueRelationshipObject persistedRelationship = GetForUpdate(media, relationship);
            ValueRelationshipValidationObject.UpdateStatus updateStatus = new ValueRelationshipValidationObject.UpdateStatus()
            {
                Existed = persistedRelationship != null,
                WasActive = persistedRelationship == null ? false : persistedRelationship.IsActive
            };
            if (persistedRelationship == null)
            {
                ValueRelationshipCtl insertedCtl = ValueRelationshipCtl.Create(media, relationship);
                insertedCtl.AddToDb();
            }
            else
            {
                ValueRelationshipCtl.Update(media, persistedRelationship, relationship, enforceConcurrency: true);
            }
            return updateStatus;
        }

        public void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (ValueRelationshipValidationObject set in RelationshipAndInverse)
            {
                Delete(media, validationMessages, set.Relationship, "Relationship");
                Delete(media, validationMessages, set.Inverse, "Inverse relationship");
            }
        }

        private void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages, ValueRelationshipObject relationship, string description)
        {
            ValueRelationshipObject persistedInverse = GetForUpdate(media, relationship);
            if (persistedInverse == null)
            {
                if (validationMessages != null)
                {
                    validationMessages.AddWarning(relationship.ValidationKey, description + " not found to delete");
                }
            }
            else
            //TODO: Do we need to check to make sure there are no children before we delete?  Should this be part of validation?
            {
                ValueRelationshipCtl.Delete(media, persistedInverse);
            }
        }

        public void PostSaveValidate(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.PostSaveValidate(media, validationMessages, RelationshipAndInverse);
            SetCommitted(RelationshipAndInverse, true);
        }

        public void LogicalRollback(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (ValueRelationshipValidationStatus set in setsWithStatus)
            {
                RollBack(media, set.Relationship, set.RelationshipUpdateStatus, "Relationship");
                if (set.Inverse != null)
                {
                    RollBack(media, set.Inverse, set.InverseUpdateStatus, "Inverse relationship");
                }
            }
        }

        private void RollBack(IDataServicesObjectContext media, ValueRelationshipObject relationship, ValueRelationshipValidationObject.UpdateStatus updateStatus, string description)
        {
            if (updateStatus.Existed)
            {
                relationship.IsActive = updateStatus.WasActive;
                Save(media, relationship);
            }
            else
            {
                Delete(media, null, relationship, description);
            }
        }

        public void LogicalCommit(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (var relationshipSet in RelationshipAndInverse)
            {
                ValidationMessages messages = new ValidationMessages();
                ValueRelationshipValidationStatus results = new ValueRelationshipValidationStatus { Relationship = relationshipSet.Relationship, Inverse = relationshipSet.Inverse };
                SaveCommittedRelationship(media, relationshipSet.Relationship);
                SaveCommittedRelationship(media, relationshipSet.Inverse);
            }
        }

        private void SaveCommittedRelationship(IDataServicesObjectContext media, ValueRelationshipObject relationship)
        {
            if (relationship == null)
            { return;
                }
            ValueRelationshipObject persistedRelationship = GetForUpdate(media, relationship);
            if (persistedRelationship == null)
            {
                if (!relationship.IsBeingDeleted)
                {
                    ValueRelationshipCtl.Create(media, relationship).AddToDb();
                }
            }
            else
            {
                if (relationship.IsBeingDeleted)
                {
                    ValueRelationshipCtl.Delete(media, persistedRelationship);
                }
                else
                {
                    ValueRelationshipCtl.Update(media, persistedRelationship, relationship, enforceConcurrency: true);
                }
            }
        }


        private ValueRelationshipObject GetForUpdate(IDataServicesObjectContext media, ValueRelationshipObject relationship)
        {
            return ValueRelationshipCtl.Get((MediaObjectContext)media, forUpdate: true)
                .Where(m =>
                    m.RelationshipTypeName == relationship.RelationshipTypeName && 
                        m.ValueId == relationship.ValueId && m.RelatedValueId == relationship.RelatedValueId 
                ).FirstOrDefault();
        }

        private static void SetCommitted(IList<ValueRelationshipValidationObject> sets, bool committed)
        {
            foreach (ValueRelationshipValidationObject set in sets)
            {
                set.Relationship.IsCommitted = committed;
                if (set.Inverse != null)
                {
                    set.Inverse.IsCommitted = committed;
                }
            }
        }

        public void BuildValidationObjects(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            RelationshipAndInverse = Items.Select(i => Validator.GetValidationObject(media, validationMessages, i)).ToList();
        }

        public void AddDeletedRelationships(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            var relationships = RelationshipAndInverse.Select(r => r.Relationship);
            List<int> existingRelatedValueIds = relationships.Select(r => r.RelatedValueId).ToList();
                
            var deletedRelationships = ValueRelationshipCtl.Get((MediaObjectContext)media, forUpdate: false)
                .Where(r => r.ValueId == ValueId && !existingRelatedValueIds.Contains(r.RelatedValueId)).ToList();
            if (deletedRelationships.Any())
            {
                AddDeletedRelationships(media, validationMessages, deletedRelationships);
            }
        }

        public void AddDeletedRelationships(IDataServicesObjectContext media, ValidationMessages validationMessages, List<ValueRelationshipObject> deletedRelationships )
        {
            var deletedValidationRelationships = deletedRelationships.Select(i => Validator.GetValidationObject(media, validationMessages, i))
                .ToList();
            foreach (var d in deletedValidationRelationships)
            {
                d.Relationship.IsBeingDeleted = true;
                d.Inverse.IsBeingDeleted = true;
            }
            RelationshipAndInverse.AddRange(deletedValidationRelationships);
        }

        public void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
        }

        public void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
        }

        public void ValidateSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(media, validationMessages, RelationshipAndInverse);
        }

        public void ValidateDelete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(media, validationMessages, RelationshipAndInverse);
        }

        public void AdditionalSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public void UpdateIdsAfterInsert()
        {
            foreach (IDataCtl dataCtl in InsertedDataControls)
            {
                dataCtl.UpdateIdsAfterInsert();
            }
        }


        public void Insert(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

        public void Update(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

    }
}
