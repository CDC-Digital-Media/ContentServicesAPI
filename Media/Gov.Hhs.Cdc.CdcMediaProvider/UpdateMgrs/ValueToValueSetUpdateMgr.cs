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
using System.Text;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class ValueToValueSetUpdateMgr : IUpdateMgr
    {
        public string ObjectName { get { return "ValueToValueSet"; } }

        protected ValueToValueSetObjectValidator Validator { get; set; }
        public IList<ValueToValueSetObject> Items;
        protected IList<ValueToValueSetObject> ValidationItems;

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
        public bool RequiresLogicalCommit { get { return false; } }

        public ValueToValueSetUpdateMgr(IList<ValueToValueSetObject> items)
        {
            this.Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            this.Validator = new ValueToValueSetObjectValidator();
        }

        public ValueToValueSetUpdateMgr(ValueToValueSetObject item)
        {
            this.Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            this.Validator = new ValueToValueSetObjectValidator();
        }
        
        public void BuildValidationObjects(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            ValidationItems = Items;        //By default, the items are already what we need to validate
        }

        public void Save(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            //valueToValueSetObject
            foreach (ValueToValueSetObject obj in Items)
            {
                ValueToValueSetObject persistedValueToValueSet = ValueToValueSetCtl.Get((MediaObjectContext)media, forUpdate:true)
                    .Where(m => m.ValueId == obj.ValueId && m.ValueSetId == obj.ValueSetId)
                    .FirstOrDefault();

                if (persistedValueToValueSet == null)
                {
                    ValueToValueSetCtl vocabValueCtl = ValueToValueSetCtl.Create(media, obj);
                    vocabValueCtl.AddToDb();
                }
                else
                {
                    ValueToValueSetCtl.Update(media, persistedValueToValueSet, obj, enforceConcurrency: true);
                }
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
        
        public void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (ValueToValueSetObject valueToValueSetObject in Items)
            {
                ValueToValueSetObject persistedValueObject = ValueToValueSetCtl.Get((MediaObjectContext)media, forUpdate:true)
                    .Where(m =>
                            m.ValueId == valueToValueSetObject.ValueId && m.ValueLanguageCode == valueToValueSetObject.ValueLanguageCode &&
                            m.ValueSetId == valueToValueSetObject.ValueSetId && m.ValueSetLanguageCode == valueToValueSetObject.ValueSetLanguageCode
                    ).FirstOrDefault();

                //TODO: Do we need to check to make sure there are no children before we delete?  Should this be part of validation?
                if (persistedValueObject != null)
                    ValueToValueSetCtl.Delete(media, persistedValueObject);
            }
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
            Validator.ValidateSave(media, validationMessages, ValidationItems);
        }

        public void ValidateDelete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(media, validationMessages, ValidationItems);
        }


        public void AdditionalSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public void PostSaveValidate(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public void LogicalCommit(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public void LogicalRollback(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public void UpdateIdsAfterInsert()
        {
            foreach (IDataCtl dataCtl in InsertedDataControls)
                dataCtl.UpdateIdsAfterInsert();
        }


    }
}
