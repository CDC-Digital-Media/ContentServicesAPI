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
    public class ValueUpdateMgr : IUpdateMgr
    {
        private List<ValueRelationshipUpdateMgr> RelationshipUpdateMgrs { get; set; }
        private List<ValueToValueSetUpdateMgr> ValueToValueSetUpdateMgrs { get; set; }

        protected ValueObjectValidator Validator { get; set; }

        public bool RequiresLogicalCommit { get { return true; } }
        public string ObjectName { get { return "Value"; } }

        public IList<ValueObject> Items;

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
        public ValueUpdateMgr(IList<ValueObject> items)
        {
            Initialize(ValidatorHelper.GetWithValidationKey(items, ObjectName));
        }

        public ValueUpdateMgr(ValueObject item)
        {
            Initialize(ValidatorHelper.GetWithValidationKey(item, ObjectName));
        }

        private void Initialize(IList<ValueObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new ValueObjectValidator();
            RelationshipUpdateMgrs =
                items.Select(i => new ValueRelationshipUpdateMgr(
                        (i.ValueRelationships ?? new List<ValueRelationshipObject>()).ToList(), i.ValueId
                    )).ToList();

            ValueToValueSetUpdateMgrs =
                    items.Select(i => new ValueToValueSetUpdateMgr(
                            (i.ValueToValueSetObjects ?? new List<ValueToValueSetObject>()).ToList()
                        )).ToList();
        }

        public void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.PreSaveValidate(ref validationMessages);
            }
            foreach (ValueToValueSetUpdateMgr v2vsUpdateMgr in ValueToValueSetUpdateMgrs)
            {
                v2vsUpdateMgr.PreSaveValidate(ref validationMessages);
            }
        }

        public void ValidateSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Items = ValueObjectValidator.ValidateSaveReturningUpdatedValues(media, validationMessages, Items);
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {   
                relationshipUpdateMgr.ValidateSave(media, validationMessages);
            }

            foreach (ValueToValueSetUpdateMgr v2vsUpdateMgr in ValueToValueSetUpdateMgrs)
            {
                v2vsUpdateMgr.ValidateSave(media, validationMessages);
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.PreDeleteValidate(validationMessages);
            }
            foreach (ValueToValueSetUpdateMgr v2vsUpdateMgr in ValueToValueSetUpdateMgrs)
            {
                v2vsUpdateMgr.PreDeleteValidate(validationMessages);
            }
        }

        public void ValidateDelete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(media, validationMessages, Items);
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.ValidateDelete(media, validationMessages);
            }
            foreach (ValueToValueSetUpdateMgr v2vsUpdateMgr in ValueToValueSetUpdateMgrs)
            {
                v2vsUpdateMgr.ValidateDelete(media, validationMessages);
            }
        }

        public void Save(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {

            foreach (ValueObject value in Items)
            {
                if (value.ValueId == 0)
                    InsertedDataControls.Add(Insert(media, value));
                else
                    Update(media, value);
            }
            
            //Handling deletes gets a bit complicated because there is a 2 stage commit/rollback process
            //due to the post save validations
            foreach (var relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.AddDeletedRelationships(media, validationMessages);
            }

            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.Save(media, validationMessages);
            }
            foreach (ValueToValueSetUpdateMgr v2vsUpdateMgr in ValueToValueSetUpdateMgrs)
            {
                v2vsUpdateMgr.Save(media, validationMessages);
            }
        }


        internal static IDataCtl Insert(IDataServicesObjectContext media, ValueObject valueObject)
        {
            ValueCtl vocabValueCtl = ValueCtl.Create(media, valueObject);
            vocabValueCtl.AddToDb();

            return vocabValueCtl;
        }

        public void Insert(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

        public void Update(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

        internal static void Update(IDataServicesObjectContext media, ValueObject value)
        {
            ValueObject persistedValueObject = ValueCtl.GetWithChildren((MediaObjectContext)media, forUpdate:true).Where(m => m.ValueId == value.ValueId).FirstOrDefault();
            ValueCtl vocabValueCtl = ValueCtl.Update(media, persistedValueObject, value, enforceConcurrency: true);
        }

        public void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            DeleteAllRelationships(media);

            foreach (ValueToValueSetUpdateMgr v2vsUpdateMgr in ValueToValueSetUpdateMgrs)
            {
                v2vsUpdateMgr.Delete(media, validationMessages);
            }
            foreach (ValueObject value in Items)
            {
                Delete(media, validationMessages, value);
            }
        }

        private void DeleteAllRelationships(IDataServicesObjectContext media)
        {
            var valueIds = Items.Select(i => i.ValueId).ToList();
            List<ValueRelationshipObject> persistedRelationships = ValueRelationshipCtl.Get((MediaObjectContext)media, forUpdate: true)
                .Where(vr => valueIds.Contains(vr.ValueId) || valueIds.Contains(vr.RelatedValueId)).ToList();
            foreach (ValueRelationshipObject persistedRelationship in persistedRelationships)
            {
                ValueRelationshipCtl.Delete(media, persistedRelationship);
            }
        }

        private static void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages, ValueObject value)
        {
            ValueObject persistedValueObject = ValueCtl.GetWithChildren((MediaObjectContext) media, forUpdate: true).Where(m => m.ValueId == value.ValueId).FirstOrDefault();
            if (persistedValueObject == null)
                validationMessages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, value.ValidationKey,
                    "Value Id was not found to delete"));
            DeleteChildrenOfAValue(media, value.ValueId);
            ValueCtl.Delete(media, persistedValueObject);
        }

        internal static void DeleteChildrenOfAValue(IDataServicesObjectContext media, int valueId)
        {

            List<ValueRelationshipObject> relationships = ValueRelationshipCtl.Get((MediaObjectContext)media, forUpdate: true)
                .Where(r => r.ValueId == valueId || r.RelatedValueId == valueId).ToList();
            foreach (ValueRelationshipObject persistedRelationship in relationships)
                ValueRelationshipCtl.Delete(media, persistedRelationship);
        }


        public void PostSaveValidate(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.PostSaveValidate(media, validationMessages, Items);
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.PostSaveValidate(media, validationMessages);
            }
        }

        public void LogicalRollback(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.LogicalRollback(media, validationMessages);
            }
        }

        public void LogicalCommit(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.LogicalCommit(media, validationMessages);
            }
        }

        //private void RollBack(IBaseObjectContext media, ValueRelationshipObject relationship, ValueRelationshipValidationObject.UpdateStatus updateStatus, string description)
        //{
        //    throw new NotImplementedException();
        //}

        public void UpdateIdsAfterInsert()
        {
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.UpdateIdsAfterInsert();
            }
            foreach(IDataCtl dataCtl in InsertedDataControls)
                dataCtl.UpdateIdsAfterInsert();
        }

        public void BuildValidationObjects(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (ValueRelationshipUpdateMgr relationshipUpdateMgr in RelationshipUpdateMgrs)
            {
                relationshipUpdateMgr.BuildValidationObjects(media, validationMessages);
            }
        }

        public virtual void AdditionalSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

    }
}
