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
using System.Data;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.DataServices
{
    public abstract class BaseCtl<t, dT, ct, oc> : IDataCtl
        where t : DataSourceBusinessObject
        where oc : DataServicesObjectContext
        where ct : IDataCtl, new()
    {
        public t NewBusinessObject { get; set; }
        public t PersistedBusinessObject { get; set; }
        protected oc TheObjectContext { get { return (oc)DataEntities; } }
        public dT PersistedDbObject
        {
            get { return (dT)PersistedBusinessObject.DbObject; }
            set { PersistedBusinessObject.DbObject = value; }
        }

        public oc DataEntities { get; set; }

        public BaseCtl()
        {
        }

        //Methods Implemented by the derived class
        public abstract void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid);
        public abstract void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid);

        /// <summary>
        /// Return true if there are no changes and the update should be skipped.  Return 
        /// false to always force the update to occur
        /// </summary>
        /// <returns></returns>
        public abstract bool DbObjectEqualsBusinessObject();
        public abstract bool VersionMatches();

        //Methods to consider removing because they are not required in the interface
        public abstract void UpdateIdsAfterInsert();
        public abstract void Delete();
        public abstract void AddToDb();

        //New methods to begin using, implemented by the base control
        public static ct Update(IDataServicesObjectContext dataEntities, t persistedBusinessObject, t newBusinessObject, bool enforceConcurrency)
        {
            ct dataCtl = new ct();
            dataCtl.Update(dataEntities, persistedBusinessObject, newBusinessObject, enforceConcurrency);
            return dataCtl;
        }

        public void Update(IDataServicesObjectContext dataEntities, object persistedBusinessObject, object newBusinessObject, bool enforceConcurrency)
        {
            DataEntities = (oc)dataEntities;
            PersistedBusinessObject = (t) persistedBusinessObject;
            NewBusinessObject = (t)newBusinessObject;
            if (enforceConcurrency || !DbObjectEqualsBusinessObject())
            {
                if (!VersionMatches())
                {
                    throw new OptimisticConcurrencyException("Update conflict on table " + ToString());
                }
                SetUpdatableValues(DataEntities.TransactionSettings.TransactionDateTime, DataEntities.TransactionSettings.OwnerGuid);
            }
        }

        public static ct Create(IDataServicesObjectContext dataEntities, t newBusinessObject)
        {
            ct dataCtl = new ct();
            dataCtl.Create(dataEntities, newBusinessObject);
            return dataCtl;
        }

        public void Create(IDataServicesObjectContext dataEntities, object newBusinessObject)
        {
            DataEntities = (oc)dataEntities;
            PersistedBusinessObject = (t)newBusinessObject;
            NewBusinessObject = (t)newBusinessObject;
            SetInitialValues(DataEntities.TransactionSettings.TransactionDateTime, DataEntities.TransactionSettings.OwnerGuid);
            SetUpdatableValues(DataEntities.TransactionSettings.TransactionDateTime, DataEntities.TransactionSettings.OwnerGuid);
        }

        public static ct Delete(IDataServicesObjectContext dataEntities, t persistedBusinessObject)
        {
            ct dataCtl = new ct();
            dataCtl.Delete(dataEntities, persistedBusinessObject);
            return dataCtl;
        }

        public void Delete(IDataServicesObjectContext dataEntities, object persistedBusinessObject)
        {
            DataEntities = (oc)dataEntities;
            PersistedBusinessObject = (t)persistedBusinessObject;
            Delete();
        }


        //Deprecated Methods
        //public void Update(object persistedBusinessObject, object newBusinessObject, bool enforceConcurrency)
        //{
        //    SetBusinessObjects(persistedBusinessObject, newBusinessObject);
        //    Update(enforceConcurrency);
        //}

        //public void Update(object newBusinessObject, bool enforceConcurrency)
        //{
        //    SetBusinessObjects(newBusinessObject, newBusinessObject);
        //    Update(enforceConcurrency);
        //}

        public void Delete(object persistedBusinessObject)
        {
            SetBusinessObjects(persistedBusinessObject, persistedBusinessObject);
            Delete();
        }        

        protected static bool AsBool(string value)
        {
            return string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        //protected virtual void Update(bool enforceConcurrency)
        //{
        //    if (enforceConcurrency || !DbObjectEqualsBusinessObject())
        //    {
        //        if (!VersionMatches())
        //        {
        //            throw new OptimisticConcurrencyException("Update conflict on table " + ToString());
        //        }
        //        SetUpdatableValues(DataEntities.TransactionSettings.TransactionDateTime, DataEntities.TransactionSettings.OwnerGuid);
        //    }
        //}

        public void SetBusinessObjects(object persistedBusinessObject, object newBusinessObject)
        {
            PersistedBusinessObject = (t)persistedBusinessObject;
            NewBusinessObject = (t)newBusinessObject;
        }

        public abstract object Get(bool forUpdate);

    }
}
