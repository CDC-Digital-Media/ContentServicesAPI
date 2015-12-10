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
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public partial class ValueSetUpdateMgr : BaseUpdateMgr<ValueSetObject>
    {
        //List<ValueSetObject> theObjects;
        //List<OrganizationUpdateMgr> organizationUpdateMgrs;

        public override string ObjectName { get { return "User"; } }
        private List<ValueSetObjectCtl> InsertedValueSets = new List<ValueSetObjectCtl>();

        public ValueSetUpdateMgr(IList<ValueSetObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new ValueSetObjectValidator();
        }

        public ValueSetUpdateMgr(ValueSetObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);;
            Validator = new ValueSetObjectValidator();
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {

            foreach (ValueSetObject valueSet in Items)
            {

                ValueSetObject persistedValueSet =
                    ValueSetObjectCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(v => v.Id == valueSet.Id).FirstOrDefault();

                if (persistedValueSet == null)
                {

                    //First time saving the object, we must generate the salt
                    ValueSetObjectCtl InsertedValueSet = ValueSetObjectCtl.Create(objectContext, valueSet);
                    InsertedValueSet.AddToDb();
                    InsertedValueSets.Add(InsertedValueSet);
                }
                else
                    ValueSetObjectCtl.Update(objectContext, persistedValueSet, valueSet, enforceConcurrency: true);
            }
        }

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (ValueSetObject valueSet in Items)
            {
                ValueSetObject persistedValueSet =
                    ValueSetObjectCtl.Get((MediaObjectContext)objectContext, forUpdate: true)
                    .Where(v => v.Id == valueSet.Id).FirstOrDefault();

                if (persistedValueSet != null)
                {
                    ValueSetObjectCtl.Delete(objectContext, persistedValueSet);
                }
            }
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (ValueSetObjectCtl valueSetObjectCtl in InsertedValueSets)
            {
                valueSetObjectCtl.UpdateIdsAfterInsert();
            }
        }
    }
}
