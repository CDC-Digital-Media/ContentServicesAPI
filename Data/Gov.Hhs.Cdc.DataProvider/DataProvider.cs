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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.DataProvider
{
    public class DataProvider : IDataProvider
    {
        protected static IObjectContextFactory ObjectContextFactory { get; set; }
        protected static IValidator<ProxyCacheObject, ProxyCacheObject> proxyCacheObjectValidator { get; set; }

        public static void Inject(IObjectContextFactory objectContextFactory)
        {
            ObjectContextFactory = objectContextFactory;
            proxyCacheObjectValidator = new ProxyCacheObjectValidator();
        }

        public ProxyCacheObject ProxyRequest(string url, string datasetId, out ValidationMessages messages) 
        {
            messages = new ValidationMessages();
            ProxyCacheObject proxyCacheObject = null;
            if (string.IsNullOrEmpty(url))
            {
                messages.AddError("Url", "A valid url is required to proxy data");
                return proxyCacheObject;
            }

            Boolean dataNeedsUpdating = false;
            
            using (IDataServicesObjectContext dataObjectContext = ObjectContextFactory.Create())
            {
                var dbContext = dataObjectContext as DataObjectContext;                

                proxyCacheObject = ProxyCacheCtl.Get(dbContext, forUpdate: true)
                    .Where(d => d.Url == url).FirstOrDefault();

                if (proxyCacheObject != null)
                {
                    if (DateTime.Compare(DateTime.UtcNow, proxyCacheObject.ExpirationDateTime) > 0 && proxyCacheObject.Failures < DataUpdater.CONSECUTIVE_FAILURE_LIMIT)
                    {
                        dataNeedsUpdating = true;                        
                    }
                }
                else
                {
                    proxyCacheObject = BuildNewProxyCacheObject(url, datasetId);
                    proxyCacheObjectValidator.PreSaveValidate(ref messages, 
                        ValidatorHelper.GetWithValidationKey(proxyCacheObject, "Proxy Cache Object"));
                    if (!messages.Errors().Any())
                    {
                        ProxyCacheCtl.Create(dbContext, proxyCacheObject).AddToDb();
                        dbContext.SaveChanges();

                        dataNeedsUpdating = true;
                    }
                }
            }

            if (dataNeedsUpdating)
            {
                //Async update of data
                DataUpdater dataUpdater = new DataUpdater(ObjectContextFactory.Create(), url);
                dataUpdater.UpdateData();
            }

            return proxyCacheObject; 
        }

        public ProxyCacheObject GetProxyData(int id, out ValidationMessages messages)
        {
            messages = new ValidationMessages();

            ProxyCacheObject retObject = null;
            using (IDataServicesObjectContext dataObjectContext = ObjectContextFactory.Create())
            {
                var dbContext = dataObjectContext as DataObjectContext;
                retObject = ProxyCacheCtl.Get(dbContext, forUpdate: true)
                                    .Where(d => d.Id == id).FirstOrDefault();
            }

            retObject.Url = DataUtil.PercentDecodeRfc3986(retObject.Url);

            return retObject;
        }

        public ValidationMessages SaveProxyData(ProxyCacheObject proxyCacheObject)
        {
            ValidationMessages messages = new ValidationMessages();

            proxyCacheObject.Url = DataUtil.PercentEncodeRfc3986(proxyCacheObject.Url);

            using (IDataServicesObjectContext dataObjectContext = ObjectContextFactory.Create())
            {
                var dbContext = dataObjectContext as DataObjectContext;

                proxyCacheObjectValidator.PreSaveValidate(ref messages, new List<ProxyCacheObject>() { proxyCacheObject });
                if (!messages.Errors().Any())
                {
                    if (proxyCacheObject.Id != 0)
                    {
                        ProxyCacheObject oldProxyCacheObject = ProxyCacheCtl.Get(dbContext, forUpdate: true)
                            .Where(d => d.Id == proxyCacheObject.Id).FirstOrDefault();
                        ProxyCacheCtl.Update(dbContext, oldProxyCacheObject, proxyCacheObject, enforceConcurrency: false);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        ProxyCacheCtl ctl = ProxyCacheCtl.Create(dbContext, proxyCacheObject);
                        ctl.AddToDb();
                        dbContext.SaveChanges();
                        proxyCacheObject.Id = ctl.PersistedDbObject.ProxyCacheID;
                    }
                    
                }
            }
            
            return messages;
        }

        public ValidationMessages DeleteProxyData(int id)
        {
            ValidationMessages messages = new ValidationMessages();

            using (IDataServicesObjectContext dataObjectContext = ObjectContextFactory.Create())
            {
                var dbContext = dataObjectContext as DataObjectContext;

                ProxyCacheObject proxyCacheObject = ProxyCacheCtl.Get(dbContext, forUpdate: true)
                    .Where(d => d.Id == id).FirstOrDefault();

                if (proxyCacheObject == null)
                {
                    messages.AddWarning("data", "ID of proxy cache data for deletion does not exist");
                }
                else
                {
                    ProxyCacheCtl.Delete(dbContext, proxyCacheObject);
                    dbContext.SaveChanges();
                }
            }

            return messages;
        }

        public Boolean ValidateAppKey(string key)
        {
            using (IDataServicesObjectContext dataObjectContext = ObjectContextFactory.Create())
            {
                var dbContext = dataObjectContext as DataObjectContext;

                ProxyCacheAppKeyObject appKeyObj = ProxyCacheAppKeyCtl.Get(dbContext, forUpdate: true)
                    .Where(k => k.ProxyCacheAppKeyId == key && k.IsActive == true).FirstOrDefault();

                if (appKeyObj != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }            
        }

        public ProxyCacheAppKeyObject GetProxyCacheAppKey(string key, out ValidationMessages messages)
        {
            messages = new ValidationMessages();
            ProxyCacheAppKeyObject retVal = null;

            using (IDataServicesObjectContext dataObjectContext = ObjectContextFactory.Create())
            {
                var dbContext = dataObjectContext as DataObjectContext;

                retVal = ProxyCacheAppKeyCtl.Get(dbContext, forUpdate: true)
                    .Where(k => k.ProxyCacheAppKeyId == key).FirstOrDefault();
            }

            return retVal;
        }

        public ValidationMessages SaveProxyCacheAppKey(ProxyCacheAppKeyObject proxyCacheAppKeyObject)
        {
            ValidationMessages messages = new ValidationMessages();

            using (IDataServicesObjectContext dataObjectContext = ObjectContextFactory.Create())
            {
                var dbContext = dataObjectContext as DataObjectContext;

                if (!string.IsNullOrEmpty(proxyCacheAppKeyObject.ProxyCacheAppKeyId))
                {
                    ProxyCacheAppKeyObject oldProxyCacheAppKeyObject = ProxyCacheAppKeyCtl.Get(dbContext, forUpdate: true)
                        .Where(d => d.ProxyCacheAppKeyId == proxyCacheAppKeyObject.ProxyCacheAppKeyId).FirstOrDefault();

                    ProxyCacheAppKeyCtl.Update(dbContext, oldProxyCacheAppKeyObject, proxyCacheAppKeyObject, enforceConcurrency: false);
                    dbContext.SaveChanges();
                }
                else
                {
                    proxyCacheAppKeyObject.ProxyCacheAppKeyId = GenerateNewProxyCacheAppKey();
                    ProxyCacheAppKeyCtl ctl = ProxyCacheAppKeyCtl.Create(dbContext, proxyCacheAppKeyObject);
                    ctl.AddToDb();
                    dbContext.SaveChanges();
                    proxyCacheAppKeyObject.ProxyCacheAppKeyId = ctl.PersistedDbObject.ProxyCacheAppKeyID;
                }
                
            }

            return messages;
        }

        public ValidationMessages DeleteProxyCacheAppKey(string key)
        {
            ValidationMessages messages = new ValidationMessages();

            using (IDataServicesObjectContext dataObjectContext = ObjectContextFactory.Create())
            {
                var dbContext = dataObjectContext as DataObjectContext;

                ProxyCacheAppKeyObject proxyCacheAppKeyObject = ProxyCacheAppKeyCtl.Get(dbContext, forUpdate: true)
                    .Where(d => d.ProxyCacheAppKeyId == key).FirstOrDefault();

                if (proxyCacheAppKeyObject == null)
                {
                    messages.AddWarning("data", "ID of proxy cache app key for deletion does not exist");
                }
                else
                {
                    ProxyCacheAppKeyCtl.Delete(dbContext, proxyCacheAppKeyObject);
                    dbContext.SaveChanges();
                }
            }

            return messages;
        }



        private ProxyCacheObject BuildNewProxyCacheObject(string url, string datasetId)
        {
            ProxyCacheObject newObject = new ProxyCacheObject();
            newObject.Url = url;
            newObject.DatasetId = datasetId;
            newObject.ExpirationInterval = "default";
            newObject.GetExpirationIntervalTimeSpan();
            newObject.ExpirationDateTime = DateTime.UtcNow;
            newObject.NeedsRefresh = true;
            newObject.Failures = 0;
            return newObject;
        }

        private ProxyCacheAppKeyObject BuildNewProxyCacheAppKeyObject(string description)
        {
            ProxyCacheAppKeyObject newObject = new ProxyCacheAppKeyObject();
            newObject.ProxyCacheAppKeyId = GenerateNewProxyCacheAppKey();
            newObject.Description = description;
            newObject.IsActive = true;
            return newObject;
        }

        private string GenerateNewProxyCacheAppKey()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
