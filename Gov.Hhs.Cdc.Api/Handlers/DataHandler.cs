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
using System.Web;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataProvider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.CsCaching;

namespace Gov.Hhs.Cdc.Api
{
    public static class DataHandler
    {
        private const string APP_KEY = "appkey";
        private const string CACHE_KEY_SUFFIX = "&proxyCache=true";

        #region Public

        public static void ProxyRequest(string datasetId, ICallParser parser, IOutputWriter writer, HttpContext httpContext)
        {
            ValidationMessages messages = new ValidationMessages();

            if (datasetId == null)
            {
                messages.AddError("Url", "Invalid Url.  Could not find dataset information.");
            }

            IDictionary<string, string> queryParamsDict = GetQueryStringDictionary(httpContext);

            string appKey = "";
            if (queryParamsDict.ContainsKey(APP_KEY))
            {
                appKey = queryParamsDict[APP_KEY];
                queryParamsDict.Remove(APP_KEY);
            }
            if (!DataProviderInstance.ValidateAppKey(appKey))
            {
                messages.AddError("AppKey", "Invalid appkey parameter.");
            }

            string proxyCacheUrl = datasetId + GetQueryString(queryParamsDict);

            SerialProxyData serialProxyData = new SerialProxyData();
            if (!messages.Errors().Any())
            {
                var proxyCacheObject = CacheManager.CachedValue<ProxyCacheObject>(parser.Query.CacheKey + CACHE_KEY_SUFFIX);
                if (proxyCacheObject == null)
                {
                    proxyCacheObject = DataProviderInstance.ProxyRequest(proxyCacheUrl, datasetId, out messages);                    
                }
                
                if (messages.Errors().Any())
                {
                    Logger.LogError(messages.Errors().First().DeveloperMessage, "DataHandler.ProxyRequest");
                    serialProxyData.status = ProxyCacheObject.DataStatus.Error.ToString();
                    serialProxyData.data = new JavaScriptSerializer().Serialize(messages.Errors().First().Message);
                }
                else if (proxyCacheObject == null || String.IsNullOrEmpty(proxyCacheObject.Data))
                {
                    serialProxyData.status = ProxyCacheObject.DataStatus.Error.ToString();
                    serialProxyData.data = new JavaScriptSerializer().Serialize("The data you requested is currently unavailable.  Please verify your url and try again at a later time");
                }
                else
                {
                    parser.Query.CacheKey = parser.Query.CacheKey + CACHE_KEY_SUFFIX;
                    ServiceUtility.AddApplicationCachePublicApi(proxyCacheObject, parser);
                    serialProxyData = ProxyCacheTransformation.CreateSerialProxyData(proxyCacheObject);
                }
            }
            else
            {
                Logger.LogError(messages.Errors().First().DeveloperMessage, "DataHandler.ProxyRequest");
                serialProxyData.status = ProxyCacheObject.DataStatus.Error.ToString();
                serialProxyData.data = new JavaScriptSerializer().Serialize(messages.Errors().First().Message);
            }

            WriteSerialProxyData(serialProxyData, parser);
        }

        public static void GetProxyData(string id, ICallParser parser, IOutputWriter writer)
        {
            ValidationMessages messages = new ValidationMessages();
            SerialResponse response = new SerialResponse();

            if (id == null)
            {
                messages.AddError("Id", "Invalid Proxy Data Id");
                writer.Write(messages);
                return;
            }

            try
            {
                int intId = ServiceUtility.ParseInt(id);
                ProxyCacheObject proxyCacheObject = DataProviderInstance.GetProxyData(intId, out messages);
                SerialProxyCache serialProxyData = new SerialProxyCache();
                if (proxyCacheObject != null)
                {
                    serialProxyData = ProxyCacheTransformation.CreateSerialProxyCache(proxyCacheObject);
                    response.results = serialProxyData;
                    response.meta = ServiceUtility.CreateResponseMetaDataForOneItem(parser);
                }
                writer.Write(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                writer.Write(ValidationMessages.CreateError("data", "Exception has been logged getting proxy data object"));
            }
        }

        public static void DeleteProxyData(string id, string appKey, IOutputWriter writer)
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.LogError("ID required to delete proxy data", "DeleteProxyData");
                writer.Write(ValidationMessages.CreateError("data", "ID required to delete proxy data"));
                return;
            }

            try
            {
                ValidationMessages messages = new ValidationMessages();

                if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(appKey) != null)
                {
                    int intId = ServiceUtility.ParseInt(id);
                    messages = DataProviderInstance.DeleteProxyData(intId);
                    SerialResponse response = new SerialResponse();
                    writer.Write(response);
                }
                else
                {
                    writer.Write(ValidationMessages.CreateError("data", "Invalid API Key"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                writer.Write(ValidationMessages.CreateError("data", "Exception has been logged deleting proxy data object"));
            }

        }

        public static void UpdateProxyData(string id, string stream, string appKey, IOutputWriter writer)
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.LogError("ID required to update proxy data", "UpdateProxyData");
                writer.Write(ValidationMessages.CreateError("data", "ID required to update proxy data"));
                return;
            }
            SaveProxyData(stream, appKey, writer);
        }

        public static void InsertProxyData(string stream, string appKey, IOutputWriter writer)
        {
            SaveProxyData(stream, appKey, writer);
        }

        public static void SaveProxyData(string stream, string appKey, IOutputWriter writer)
        {
            SerialProxyCache serialProxyData = null;
            try
            {
                serialProxyData = new JavaScriptSerializer().Deserialize<SerialProxyCache>(stream);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Logger.LogError("Invalid format of proxy data object: " + stream, "SaveProxyData");
                writer.Write(ValidationMessages.CreateError("data", "Invalid format of serialized proxy data object"));
                return;
            }

            try
            {
                ValidationMessages messages = new ValidationMessages();

                if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(appKey) != null)
                {
                    ProxyCacheObject proxyCacheObject = ProxyCacheTransformation.CreateProxyCacheObject(serialProxyData);
                    messages = DataProviderInstance.SaveProxyData(proxyCacheObject);

                    if (messages.Errors().Any())
                    {
                        writer.Write(messages);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(serialProxyData.id))
                        {
                            serialProxyData = ProxyCacheTransformation.CreateSerialProxyCache(proxyCacheObject);
                        }
                        SerialResponse response = new SerialResponse(serialProxyData);
                        writer.Write(response);
                    }
                }
                else
                {
                    writer.Write(ValidationMessages.CreateError("data", "Invalid API Key"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                writer.Write(ValidationMessages.CreateError("data", "Exception has been logged saving proxy data object"));
            }                        
        }

        public static void GetProxyCacheAppKey(string id, ICallParser parser, IOutputWriter writer)
        {
            ValidationMessages messages = new ValidationMessages();
            SerialResponse response = new SerialResponse();

            if (id == null)
            {
                messages.AddError("Id", "Invalid Proxy Cache App Key Id");
                writer.Write(messages);
                return;
            }

            try
            {
                ProxyCacheAppKeyObject proxyCacheAppKeyObject = DataProviderInstance.GetProxyCacheAppKey(id, out messages);
                SerialProxyCacheAppKey serialProxyCacheAppKey = new SerialProxyCacheAppKey();
                if (proxyCacheAppKeyObject != null)
                {
                    serialProxyCacheAppKey = ProxyCacheAppKeyTransformation.CreateSerialProxyCacheAppKey(proxyCacheAppKeyObject);
                    response.results = serialProxyCacheAppKey;
                    response.meta = ServiceUtility.CreateResponseMetaDataForOneItem(parser);
                }
                writer.Write(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                writer.Write(ValidationMessages.CreateError("data", "Exception has been logged getting proxy cache app key"));
            }
        }

        public static void DeleteProxyCacheAppKey(string id, string appKey, IOutputWriter writer)
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.LogError("ID required to delete proxy cache app key", "DeleteProxyCacheAppKey");
                writer.Write(ValidationMessages.CreateError("data", "ID required to delete proxy cache app key"));
                return;
            }

            try
            {
                ValidationMessages messages = new ValidationMessages();

                if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(appKey) != null)
                {
                    messages = DataProviderInstance.DeleteProxyCacheAppKey(id);
                    SerialResponse response = new SerialResponse();
                    writer.Write(response);
                }
                else
                {
                    writer.Write(ValidationMessages.CreateError("data", "Invalid API Key"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                writer.Write(ValidationMessages.CreateError("data", "Exception has been logged deleting proxy cache app key"));
            }
        }

        public static void UpdateProxyCacheAppKey(string id, string stream, string appKey, IOutputWriter writer)
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.LogError("ID required to update proxy cache app key", "UpdateProxyCacheAppKey");
                writer.Write(ValidationMessages.CreateError("data", "ID required to update proxy cache app key"));
                return;
            }
            SaveProxyCacheAppKey(stream, appKey, writer);
        }

        public static void InsertProxyCacheAppKey(string stream, string appKey, IOutputWriter writer)
        {
            SaveProxyCacheAppKey(stream, appKey, writer);
        }

        public static void SaveProxyCacheAppKey(string stream, string appKey, IOutputWriter writer)
        {
            SerialProxyCacheAppKey serialProxyCacheAppKey = null;
            try
            {
                serialProxyCacheAppKey = new JavaScriptSerializer().Deserialize<SerialProxyCacheAppKey>(stream);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Logger.LogError("Invalid format of proxy cache app key: " + stream, "SaveProxyCacheAppKey");
                writer.Write(ValidationMessages.CreateError("data", "Invalid format of serialized proxy cache app key"));
                return;
            }

            try
            {
                ValidationMessages messages = new ValidationMessages();

                if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(appKey) != null)
                {
                    ProxyCacheAppKeyObject proxyCacheAppKeyObject = ProxyCacheAppKeyTransformation.CreateProxyCacheAppKeyObject(serialProxyCacheAppKey);
                    messages = DataProviderInstance.SaveProxyCacheAppKey(proxyCacheAppKeyObject);

                    if (messages.Errors().Any())
                    {
                        writer.Write(messages);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(serialProxyCacheAppKey.proxyCacheAppKeyId))
                        {
                            serialProxyCacheAppKey = ProxyCacheAppKeyTransformation.CreateSerialProxyCacheAppKey(proxyCacheAppKeyObject);
                        }
                        SerialResponse response = new SerialResponse(serialProxyCacheAppKey);
                        writer.Write(response);
                    }
                }
                else
                {
                    writer.Write(ValidationMessages.CreateError("data", "Invalid API Key"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                writer.Write(ValidationMessages.CreateError("data", "Exception has been logged saving proxy cache app key"));
            } 
        }


        #endregion

        #region Private
        //TODO: BW - Need to move this to some IOC implementation???
        private static IDataProvider DataProviderInstance = new Gov.Hhs.Cdc.DataProvider.DataProvider();

        private static void WriteSerialProxyData(SerialProxyData serialProxyData, ICallParser parser)
        {
            string responseBody = "{\"status\":\"" + serialProxyData.status + "\",\"data\":" + serialProxyData.data + "}";
            if (parser.ParamDictionary.ContainsKey(Param.CALLBACK) && !string.IsNullOrEmpty(parser.ParamDictionary[Param.CALLBACK]))
            {
                HttpContext.Current.Response.ContentType = "application/javascript; charset=UTF-8";
                responseBody = parser.ParamDictionary[Param.CALLBACK] + "(" + responseBody + ")";
            }
            else
            {
                HttpContext.Current.Response.ContentType = "application/json; charset=UTF-8";
            }
            HttpContext.Current.Response.Write(responseBody);
        }

        private static IDictionary<string, string> GetQueryStringDictionary(HttpContext httpContext)
        {
            IDictionary<string, string> copyDict = RestHelper.CreateUnfilteredDictionary(httpContext.Request.QueryString.ToString());
            if (copyDict.ContainsKey(Param.PAGE_RECORD_MAX))
            {
                copyDict.Remove(Param.PAGE_RECORD_MAX);
            }
            if (copyDict.ContainsKey(Param.PAGE_NUMBER))
            {
                copyDict.Remove(Param.PAGE_NUMBER);
            }
            if (copyDict.ContainsKey(Param.PAGE_OFFSET))
            {
                copyDict.Remove(Param.PAGE_OFFSET);
            }
            if (copyDict.ContainsKey(Param.CALLBACK))
            {
                copyDict.Remove(Param.CALLBACK);
            }

            return copyDict;
        }

        private static string GetQueryString(IDictionary<string, string> dictionary)
        {
            //TODO: Don't use resthelper here, use someting in DataUtil so can have consistent encoding/decoding of url.
            string queryString = RestHelper.ConstructCanonicalQueryStringFromDictionary(dictionary);
            return String.IsNullOrEmpty(queryString) ? queryString : "?" + queryString;
        }

        #endregion

    }
}
