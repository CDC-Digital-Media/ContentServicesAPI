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
using System.Data;
using System.Data.Objects;
using System.Reflection;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServicesCacheProvider;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    //DataEntities inherit from IDataContext to provide any DataContext methods, like Save Changes.  However
    //The real data context is stored in the instance variable dbDataContext, and must be used for DB access.
    //For anything other than generic methods like SaveChanges, the dataContext will be passed to the appropriate 
    //IDataCtl method to be used.
    public abstract class DataServicesObjectContext : IDataServicesObjectContext  
    {

        #region InjectedProperties
        public static IDataServicesCacheProviderFactory CacheProviderFactory { get; set; }

        #endregion

        public bool IsConnected { get; set; }

        public abstract string ApplicationCode { get; }

        //The list of assemblies are needed to parse the business object cache attributes and persiste them in a static variable
        private List<Assembly> _assembliesWithCachedBusinessObjects = null;
        public List<Assembly> AssembliesWithCachedBusinessObjects
        {
            get { return _assembliesWithCachedBusinessObjects ?? (_assembliesWithCachedBusinessObjects = GetAssembliesWithCachedBusinessObjects()); }
        }

        protected virtual List<Assembly> GetAssembliesWithCachedBusinessObjects() { return null; }

        protected ObjectContext efObjectContext = null;
        protected string ConnectionString { get; set; }

        public IDataServicesCacheProvider DataServicesCacheProvider { get; set; }

        public TransactionSettings TransactionSettings { get; set; }

        public abstract ObjectContext GetEfObjectContext();

        public DataServicesObjectContext(string connectionString)
        {
            Initialize(connectionString, new TransactionSettings());
            if (CacheProviderFactory != null)
            {
                DataServicesCacheProvider = CacheProviderFactory.Create(ApplicationCode);
            }
        }

        private void Initialize(string connectionString, TransactionSettings transactionSettings)
        {
            ConnectionString = connectionString;
            TransactionSettings = transactionSettings;
            IsConnected = true;
            if (DataServicesCacheProvider != null)
            {
                DataServicesCacheProvider.InitializePersistedCacheItems(this);
            }
        }

        public void Dispose()
        {
            if (DataServicesCacheProvider != null)
            {
                DataServicesCacheProvider.ReleaseCacheGroup();
            }
            if (efObjectContext != null)
            {
                try
                {
                    efObjectContext.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        public void SaveChanges()
        {
            if (efObjectContext != null)
            {
                try
                {
                    efObjectContext.SaveChanges();
                }
                catch (UpdateException ex)
                {
                    foreach (var entry in ex.StateEntries)
                    {
                        AuditLogger.LogAuditEvent(string.Format("Entity {0} in state {1} has validation errors", 
                            entry.Entity.GetType().Name, entry.State), ex.Message);
                    }
                    throw;
                }
            }
        }

        public void SaveChanges(SaveOptions saveOptions)
        {
            if (efObjectContext != null)
            {
                //Look at efObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added) if issues
                efObjectContext.SaveChanges(saveOptions);
            }
        }

        public void AcceptAllChanges()
        {
            if (efObjectContext != null)
            {
                efObjectContext.AcceptAllChanges();
            }
        }

        public virtual ICachedDataControl CreateNewCachedDataControl<T>()
        {
            throw new NotImplementedException();
        }
    }
}
