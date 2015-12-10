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
using System.Data.Objects;
using System.Reflection;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServicesCacheProvider;
//using CS.BusinessEntities;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    //DataEntities inherit from IDataContext to provide any DataContext methods, like Save Changes.  However
    //The real data context is stored in the instance variable dbDataContext, and must be used for DB access.
    //For anything other than generic methods like SaveChanges, the dataContext will be passed to the appropriate 
    //IDataCtl method to be used.
    public abstract class DataServicesObjectContext : ICsObjectContext  // : CS.DataServices.MediaDataContext   //, IDisposable
    {

        public bool IsConnected { get; set; }
        

        protected ObjectContext efObjectContext = null;
        protected string ConnectionString { get; set; }
        
        public IDataServicesCacheProvider CacheProvider { get; set; }
        ISearchProviders _allSearchProviders;
        public ISearchProviders AllSearchProviders { get { return _allSearchProviders; } }

        #region TransactionControllerProperties
        public TransactionSettings TransactionSettings { get; set; }

        //IDataFactory dataFactory;
        #endregion



        #region abstractMethods
        public abstract ObjectContext GetEfObjectContext();
        public abstract IDataCtl Create<t, dt>();
        #endregion

        public virtual IDataCtl Create<t>()
        {
            return CreateDynamically<t>();
        }

        protected IDataCtl CreateDynamically<t>()
        {
            Type requestedType = typeof(t);
            string ctlName = typeof(t).FullName + "Ctl";
            try
            {
                Type thisType = this.GetType();
                //Type ctlType = Assembly.GetCallingAssembly().GetType(ctlName);
                Type ctlType = thisType.Assembly.GetType(ctlName);
                return (IDataCtl)Activator.CreateInstance(ctlType, new object[] { this });
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("MediaDataFactory Create: " + ctlName + " is not implemented", ex);
            }


            throw new NotImplementedException("MediaDataFactory Create " + ctlName + " is not implemented");
        }

        public DataServicesObjectContext(string connectionStrings, IDataServicesCacheProvider cacheProvider, ISearchProviders allSearchProviders)
        {
            Initialize(connectionStrings, new TransactionSettings());
            _allSearchProviders = allSearchProviders;
            CacheProvider = cacheProvider;
            //this.dataFactory = dataFactory;
            //efObjectContext = dataFactory.CreateNewObjectContext();
        }

        //public DataServicesObjectContext(EntitiesConfigurationItems configItems, Guid ownerGuid)
        //{
        //    Initialize(configItems, new TransactionSettings(ownerGuid));
        //    //TransactionSettings = new TransactionSettings(ownerGuid);
        //    //this.dataFactory = dataFactory;
        //    //efObjectContext = dataFactory.CreateNewObjectContext();
        //}

        private void Initialize(string connectionString, TransactionSettings transactionSettings)
        {
            ConnectionString = connectionString;
            TransactionSettings = transactionSettings;
            IsConnected = true;
            if (CacheProvider != null)
                CacheProvider.InitializeCacheItems(this);
        }

        //private void InitializeCacheItems(EntitiesConfigurationItems configItems)
        //{
        //    ApplicationManagedCacheItems applicationItems = ConfigItems.CacheProvider.Persistence.GetApplicationManagedCacheItems(ApplicationCode);

        //    if (applicationItems != null && applicationItems.CacheGroups.Count > 0 && configItems.DataSourceCacheItemsAsGroup)
        //    {
        //        PersistedCacheGroup = ConfigItems.CacheProvider.Persistence.GetCurrentCacheGroup(ApplicationCode, PersistedCacheGroupCode);
        //        if (PersistedCacheGroup == null)
        //        {
        //            RefreshCache(applicationItems);
        //            PersistedCacheGroup = ConfigItems.CacheProvider.Persistence.GetCurrentCacheGroup(ApplicationCode, PersistedCacheGroupCode);
        //        }
        //        if (PersistedCacheGroup == null)
        //            throw new ApplicationException("The required cache group '" + ApplicationCode + "' is missing");
        //    }
        //}


        public IQueryable<t> Get<t>()
        {
            return Get<t>(/*forUpdate=*/false);
        }

        public IQueryable<t> Get<t>(bool forUpdate)
        {
            return (IQueryable<t>)this.Create<t>().Get(forUpdate);
        }

        public IQueryable<t> Get<t>(bool forUpdate, out IDataCtl dataCtl)
        {
            dataCtl = this.Create<t>();
            return (IQueryable<t>)dataCtl.Get(forUpdate);
        }

        public IDataCtl Insert<t>(t newData)
        {
            return Insert<t>(newData, null);
        }

        public IDataCtl Insert<t>(t newData, List<object> addedReferences)
        {
            IDataCtl dataCtl = this.Create<t>();
            dataCtl.Create(newData, addedReferences);
            dataCtl.AddToDb();
            return dataCtl;
        }

        public IDataCtl Create<t>(t newData)
        {
            IDataCtl dataCtl = this.Create<t>();
            dataCtl.Create(newData, null);
            return dataCtl;
        }

        public void CreateDbObject<t>(t newData)
        {
            IDataCtl dataCtl = this.Create<t>();
            dataCtl.Create(newData, null);
        }

        public void CreateDbObject<t, dt>(t newData)
        {
            IDataCtl dataCtl = this.Create<t, dt>();
            dataCtl.Create(newData, null);
        }


        public IDataCtl Save<t>(t persistedObject, t newData, bool enforceConcurrency)
        {
            if (persistedObject == null)
                return Insert<t>(newData);
            else
            {
                Update<t>(persistedObject, newData, enforceConcurrency);
                return null;
            }
        }


        public IDataCtl Update<t>(t persistedObject, t newData, bool enforceConcurrency)
        {
            return Update<t>(persistedObject, newData, enforceConcurrency, null);
        }

        public IDataCtl Update<t>(t persistedObject, t newData, bool enforceConcurrency, List<object> addedReferences)
        {
            IDataCtl dataCtl = this.Create<t>();
            dataCtl.Update(persistedObject, newData, enforceConcurrency, addedReferences);
            return dataCtl;
        }

        public IDataCtl Update<t, dt>(t persistedObject, t newData, bool enforceConcurrency)
        {
            IDataCtl dataCtl = this.Create<t, dt>();
            dataCtl.Update(persistedObject, newData, enforceConcurrency, null);
            return dataCtl;
        }


        public void DeleteObject<t>(t persistedObject)
        {
            IDataCtl dataCtl = this.Create<t>();
            dataCtl.Delete(persistedObject);
        }



        public void Dispose()
        {
            if (CacheProvider != null)
                CacheProvider.ReleaseCacheGroup();
            if (efObjectContext != null)
                try
                {
                    efObjectContext.Dispose();
                }
                catch (Exception)
                {
                }
        }

        public void SaveChanges()
        {
            if( efObjectContext != null)
                efObjectContext.SaveChanges();
        }

        
    }
}
