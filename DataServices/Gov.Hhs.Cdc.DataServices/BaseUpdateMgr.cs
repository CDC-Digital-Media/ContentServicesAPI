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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices
{
    public abstract class BaseUpdateMgr<T> : BaseUpdateMgr<T, T> where T : IValidationObject
    {
        public override void BuildValidationObjects(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            ValidationItems = Items;        //By default, the items are already what we need to validate
        }
    }

    public abstract class BaseUpdateMgr<T, vT> : IUpdateMgr where vT : IValidationObject
    {
        protected IValidator<T, vT> Validator { get; set; }
        public IList<T> Items;
        protected IList<vT> ValidationItems;

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

        public virtual bool RequiresLogicalCommit { get { return false; } }

        public virtual void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
        }

        public virtual void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
        }

        public virtual void ValidateSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(media, validationMessages, ValidationItems);
        }

        public virtual void ValidateDelete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(media, validationMessages, ValidationItems);
        }

        public virtual void AdditionalSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public virtual void PostSaveValidate(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public virtual void LogicalCommit(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public virtual void LogicalRollback(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public virtual void UpdateIdsAfterInsert()
        {
            foreach (IDataCtl dataCtl in InsertedDataControls)
            {
                dataCtl.UpdateIdsAfterInsert();
            }
        }

        public abstract string ObjectName { get; }

        public virtual void Save(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public virtual void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public abstract void BuildValidationObjects(IDataServicesObjectContext media, ValidationMessages validationMessages);


        public virtual void Insert(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }
    }


}
