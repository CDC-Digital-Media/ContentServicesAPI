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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServices;
using System.Data.Objects;

namespace Gov.Hhs.Cdc.DataSource
{

    public abstract class DataManager<t> : IDataManager where t : DataSourceBusinessObject
    {
        //public EntitiesConfigurationItems ConfigItems { get; set; }
        //protected IPersistenceCacheController PersistenceCacheController { get; set; }

        //public IValidator Validator {get; set;}

        //public DataManager(EntitiesConfigurationItems configItems, IValidator validator)
        //{
        //    this.ConfigItems = configItems;
        //    this.Validator = validator;
        //}

        //public virtual ValidationMessages Save(IList<t> objects, IUpdateMgr updateMgr)
        //{
        //    return SaveObjects(GetWithValidationKey(objects, Validator), updateMgr);
        //}



        //public virtual ValidationMessages Save(t theObject, IUpdateMgr updateMgr)
        //{
        //    return SaveObjects(GetWithValidationKey(theObject, Validator), updateMgr);
        //}

        //protected ValidationMessages SaveObjects(IList<t> objects, IUpdateMgr updateMgr)
        //{
        //    IList<t> updatedObjects = GetWithValidationKey(objects, Validator);
        //    ValidationMessages validationMessages = new ValidationMessages();
        //    Validator.PreValidate(validationMessages, updatedObjects);
        //    if (!validationMessages.Errors().Any())
        //        SaveObjects(validationMessages, updatedObjects, updateMgr);
        //    return validationMessages;
        //}

        //public virtual ValidationMessages Delete(IList<t> objects, IUpdateMgr updateMgr)
        //{
        //    return DeleteObjects(GetWithValidationKey(objects, Validator), updateMgr);
        //}

        //public virtual ValidationMessages Delete(t theObject, IUpdateMgr updateMgr)
        //{
        //    return DeleteObjects(GetWithValidationKey(theObject, Validator), updateMgr);
        //}

        //protected static IList<t> GetWithValidationKey(IList<t> items, IValidator validator) //where t : DataSourceBusinessObject
        //{
        //    for (int i = 0; i < items.Count(); ++i)
        //    {
        //        items[i].ValidationKey = string.Format("{0}[{1}]", validator.ObjectName, i);
        //    }
        //    return items;
        //}

        //protected ValidationMessages DeleteObjects(IList<t> objects, IUpdateMgr updateMgr)
        //{
        //    IList<t> updatedObjects = GetWithValidationKey(objects, Validator);
        //    ValidationMessages validationMessages = new ValidationMessages();
        //    Validator.PreValidate(validationMessages, updatedObjects);
        //    if (!validationMessages.Errors().Any())
        //        DeleteObjects(validationMessages, updatedObjects, updateMgr);
        //    return validationMessages;
        //}

        //protected static IList<t> GetWithValidationKey(t item, IValidator validator) //where t : DataSourceBusinessObject
        //{
        //    item.ValidationKey = validator.ObjectName;
        //    return new List<t>() { item };
        //}

        //protected abstract void SaveObjects(ValidationMessages validationMessages, IList<t> items, IUpdateMgr updateMgr);
        //protected abstract void DeleteObjects(ValidationMessages validationMessages, IList<t> items, IUpdateMgr updateMgr);


    }
}
