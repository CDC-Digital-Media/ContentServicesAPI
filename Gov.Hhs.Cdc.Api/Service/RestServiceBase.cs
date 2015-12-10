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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsCaching;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.TransactionLogProvider;
using Gov.Hhs.Cdc.Commons.Api.Key.Utils;
using Gov.Hhs.Cdc.CdcRegistrationProvider;

namespace Gov.Hhs.Cdc.Api
{
    public abstract class RestServiceBase : IApi
    {
        #region "Members"

        protected SearchParameters SearchParameters { get; set; }
        private SerialResponse _res;
        protected SerialResponse Response
        {
            get { return _res ?? (_res = new SerialResponse()); }
            set { _res = value; }
        }
        protected Author.KeyHolder _kh { get; set; }

        protected readonly ICallParser _parser;
        private OutputWriter Writer { get; set; }

        // TODO: Remove after all API clients are using the credentials in the database
        private Dictionary<string, string> temp_client_credentials
        {
            get
            {
                Dictionary<string, string> map = new Dictionary<string, string>();
                try
                {
                    string temp_client_credentialStr = ConfigurationManager.AppSettings["temp_client_credentials"];
                    if (!string.IsNullOrEmpty(temp_client_credentialStr))
                        map = temp_client_credentialStr.Split(',').ToDictionary(item => item.Split('|')[0], item => item.Split('|')[1]);
                }
                catch (Exception)
                { }

                return map;
            }
        }

        #endregion

        #region "constructor"

        protected RestServiceBase(ICallParser parser)
        {
            _parser = parser;
            Writer = new OutputWriter(_parser);
        }

        #endregion

        #region "Error Handler"

        protected void ProcessError(Exception ex, string query)
        {
            // don't cache error response
            ServiceUtility.WriteDoNotCacheResponseToHeader(Writer);

            HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Response.meta.message.Add(new SerialValidationMessage().Error(ex));
            Logger.LogError(ex, ex.Message, query);

            Writer.Write(Response);
        }

        #endregion


        public abstract void ExecuteGet(IOutputWriter writer, string resource, string id, string action);
        protected void Get(string resource, string id, string action, string format)
        {
            if (format != null)
                _parser.Query.Format = ServiceUtility.GetFormatType(format);

            try
            {
                var cache = CacheManager.CachedValue<object>(_parser.Query.CacheKey);
                if (cache == null)
                    ExecuteGet(Writer, resource, id, action);
                else
                    Writer.WriteFromCache(cache);
            }
            catch (InvalidResourceIdException)
            {
                Writer.Write(ValidationMessages.CreateError("Id", "Invalid Resource Id for " + resource + " of " + id));
            }
            catch (Exception ex)
            {
                ProcessError(ex, _parser.Query.CacheKey);
            }
        }

        public abstract void Post(IOutputWriter writer, string stream, string appKey, string resource, string id, string action);
        private void Post(string resource, string id, string action, string format, Stream data)
        {
            if (format != null)
                _parser.Query.Format = ServiceUtility.GetFormatType(format);

            try
            {
                string stream = AuthDataValidation(data);

                if (!string.IsNullOrEmpty(resource))
                {
                    string appKey = (string)HttpContext.Current.Items["app_public_key"];
                    Post(Writer, stream, appKey, resource, id, action);
                }
            }
            catch (InvalidResourceIdException)
            {
                Writer.Write(ValidationMessages.CreateError("Id", "Invalid Resource Id for " + resource + " of " + id));
            }
            catch (Exception ex)
            {
                ProcessError(ex, _parser.Query.CacheKey);
            }
        }

        public abstract void Put(IOutputWriter writer, string stream, string appKey, string resource, string id, string action);
        private void Put(string resource, string id, string action, string format, Stream data)
        {
            if (format != null)
                _parser.Query.Format = ServiceUtility.GetFormatType(format);

            try
            {
                string stream = AuthDataValidation(data);

                if (!string.IsNullOrEmpty(resource))
                {
                    string appKey = (string)HttpContext.Current.Items["app_public_key"];
                    Put(Writer, stream, appKey, resource, id, action);
                }
            }
            catch (InvalidResourceIdException)
            {
                Writer.Write(ValidationMessages.CreateError("Id", "Invalid Resource Id for " + resource + " of " + id));
            }
            catch (Exception ex)
            {
                ProcessError(ex, _parser.Query.CacheKey);
            }
        }

        public abstract void Delete(IOutputWriter writer, string stream, string appKey, string resource, string id, string action);
        public void Delete(string resource, string id, string action, string format, Stream data)
        {
            if (format != null)
                _parser.Query.Format = ServiceUtility.GetFormatType(format);
            try
            {
                string stream = AuthDataValidation(data);

                if (!string.IsNullOrEmpty(resource))
                {
                    string appKey = (string)HttpContext.Current.Items["app_public_key"];
                    Delete(Writer, stream, appKey, resource, id, action);
                }
            }
            catch (InvalidResourceIdException)
            {
                Writer.Write(ValidationMessages.CreateError("Id", "Invalid Resource Id for " + resource + " of " + id));
            }
            catch (Exception ex)
            {
                ProcessError(ex, _parser.Query.CacheKey);
            }
        }

        //#region "dataset Behavior"

        //public void AddBindingParameters(ServiceDescription serviceDescription,
        //    ServiceHostBase serviceHostBase,
        //    System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints,
        //    System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        //{
        //    //throw new NotImplementedException();
        //}

        //public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        //{
        //    foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
        //    {
        //        foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
        //        {
        //            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        //        }
        //    }
        //}

        //public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        //{
        //    //throw new NotImplementedException();
        //}

        //#endregion

        //#region "Message Dispatcher"

        //public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
        //{
        //    try
        //    {
        //        _srchParam = this.Parser();

        //        MessageProperties mp = request.Properties;
        //        HttpRequestMessageProperty reqProp = (HttpRequestMessageProperty)mp["httpRequest"];

        //        _jsonify = new JavaScriptSerializer();
        //        reqProp.Headers.Remove("SearchCrit");
        //        reqProp.Headers.Add("SearchCrit", _jsonify.Serialize(_srchParam));

        //        mp.Remove("httpRequest");  
        //        mp.Add("httpRequest", reqProp);

        //        Message replacedMessage = Message.CreateMessage(request.Version, null);
        //        replacedMessage.Headers.CopyHeadersFrom(request.Headers);

        //        replacedMessage.Properties.CopyProperties(mp);
        //        request = replacedMessage;
        //    }
        //    catch (Exception)
        //    {
        //    }            

        //    return null;
        //}

        //public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        //{            
        //}

        //#endregion

        #region "Protected Methods"

        protected string AuthDataValidation(Stream data)
        {
#if (DEBUG)
#else
            if (!HttpContext.Current.Request.IsSecureConnection)
                throw new Exception("A secured connection is required.");
#endif

            if (!data.CanRead)
                return "";
            string stream;
            try
            {
                stream = (data == null) ? "" : GetDataStream(data);
            }
            catch (ArgumentException)
            {
                //Stream not readable.  Just return "" because we may not have data
                return "";
            }

            try
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                string unicodeString = stream;
                byte[] encodedBytes = utf8.GetBytes(unicodeString);

                foreach (byte b in encodedBytes)
                {
                    if (b > 191)
                    {
                        throw new Exception("Invalid UTF-8 character was found.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (!IsAuthenticated(stream))
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                throw new Exception("Unauthorized Request",
                    new Exception(
                        "Step: " + HttpContext.Current.Items["step"] + " | " +
                        "Content-Type: " + HttpContext.Current.Request.Headers["Content-Type"] + " | " +
                        "Content-Length: " + HttpContext.Current.Request.Headers["Content-Length"] + " | " +
                        "X-Syndication-Date: " + HttpContext.Current.Request.Headers["X-Syndication-Date"] + " | " +
                        "Date: " + HttpContext.Current.Request.Headers["Date"] + " | " +                        
                        "Authorization: " + HttpContext.Current.Request.Headers["Authorization"] + " | " +
                        "Data: " + stream)
                        );
            }

            return stream;
        }

        protected static string GetDataStream(Stream data)
        {
            string postData = "";
            using (StreamReader reader = new StreamReader(data, Encoding.UTF8))
            {
                postData = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
            return postData;
        }

        protected static int GetIdAsInt(string id)
        {
            int objId = 0;
            if (!string.IsNullOrEmpty(id))
                if (!int.TryParse(id, out objId))
                    throw new ApplicationException("Invalid Id");

            return objId;
        }

        #endregion

        #region "Private Methods"

        private bool IsAuthenticated(string data)
        {
            bool authValue = false;

            try
            {
                //context.Headers
                if (HttpContext.Current.Request.Headers.AllKeys.Any(a => a == "Authorization"))
                {
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers["Authorization"]))
                    {
                        HttpContext.Current.Items["step"] = 1;
                        return authValue;
                    }
                    else
                    {
                        if (!HttpContext.Current.Request.Headers["Authorization"].StartsWith("syndication_api_key "))
                        {
                            HttpContext.Current.Items["step"] = 2;
                            return authValue;
                        }
                    }
                }
                else
                {
                    HttpContext.Current.Items["step"] = 3;
                    return authValue;
                }

                //// check if header date is valid
                //string dateStr = GetDateFromHeader();
                //if (!ValidDate(dateStr))
                //{
                //    HttpContext.Current.Items["step"] = 4;
                //    return authValue;
                //}

                string appKey = HttpContext.Current.Request.Headers["Authorization"].Substring("syndication_api_key ".Length).Split(':')[0];
                HttpContext.Current.Items["app_public_key"] = appKey;

                // used to disable authentication
                if (ConfigurationManager.AppSettings["Secure"] == "false")
                    return true;                              

                string consumersecret = GetApiClientSecret(appKey);
                if (string.IsNullOrEmpty(consumersecret))       //return false if consumer string is empty or null
                {
                    HttpContext.Current.Items["step"] = 5;
                    return false;
                }

                authValue = HttpContext.Current.Request.Headers["Authorization"] == GetAuthorizationHeader(HttpContext.Current.Request.Url.AbsolutePath,
                    HttpContext.Current.Request.HttpMethod,
                    data, appKey, consumersecret);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Items["step"] = 6;
                Logger.LogError(ex);
            }

            HttpContext.Current.Items["step"] = 7;
            return authValue;
        }

        private string GetApiClientSecret(string appKey)
        {
            string consumersecret = temp_client_credentials.ContainsKey(appKey) ? temp_client_credentials[appKey] : "";

            if (string.IsNullOrEmpty(consumersecret))
            {
                var apiClient = RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(appKey);
                if (apiClient != null)
                    consumersecret = CredentialManager.CreateHash(ConfigurationManager.AppSettings["ApiClientPassword"], apiClient.Salt);
            }
            return consumersecret;
        }

        private static string GetDateFromHeader()
        {
            string dateStr = "";
            dateStr = HttpContext.Current.Request.Headers["X-Syndication-Date"];
            if (string.IsNullOrEmpty(dateStr))
                dateStr = HttpContext.Current.Request.Headers["Date"];
            return dateStr;
        }

        private string GetAuthorizationHeader(string url, string method, string requestBody, string api_publicKey, string secret)
        {
            AuthorizationHeaderGenerator.KeyAgreement keyAgreement = new AuthorizationHeaderGenerator.KeyAgreement();
            keyAgreement.publicKey = api_publicKey;
            keyAgreement.secret = secret;

            AuthorizationHeaderGenerator generator = new AuthorizationHeaderGenerator("syndication_api_key", keyAgreement);

            string apiKeyHeaderValue = generator.GetApiKeyHeaderValue(HttpContext.Current.Request.Headers, url, method, requestBody);
            return apiKeyHeaderValue;
        }

        /// <summary>
        /// Check if header date is valid.
        /// Date is invalid when the header datetime stamp is: 
        /// a) 5 minutes greater than the current UTC datetime.
        /// b) 15 minutes less than the current UTC datetime.
        ///                                                    
        /// </summary>
        /// <returns>bool</returns>
        //public static bool ValidDate(string date)
        //{
        //    try
        //    {
        //        DateTime dt;
        //        if (DateTime.TryParse(date, out dt))
        //        {
        //            TimeSpan ts = DateTime.UtcNow - dt;

        //            if (ts.Minutes < 0)
        //            {
        //                if (ts.Minutes < -5)
        //                    return false;
        //            }
        //            else
        //            {
        //                if (ts.Minutes > 15)
        //                    return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //        return false;
        //    }
        //}        

        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------

        #region GetWebMethods

        private static int GetVersion(string version)
        {
            int num = Convert.ToInt32(ConfigurationManager.AppSettings["PublicApiVersion"]);
            if (!string.IsNullOrEmpty(version))
                int.TryParse(version, out num);

            return num;
        }

        public void WebMethodGet(string version, string resource)
        {
            _parser.Version = GetVersion(version);
            Get(resource, id: null, action: null, format: null);
        }

        public void WebMethodGetSlash(string version, string resource)
        {
            WebMethodGet(version, resource);
        }

        public void WebMethodGetFormat(string version, string resource, string format)
        {
            _parser.Version = GetVersion(version);
            Get(resource, id: null, action: null, format: format);
        }

        public void WebMethodGetFormatSlash(string version, string resource, string format)
        {
            WebMethodGetFormat(version, resource, format);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodGetId(string version, string resource, string id)
        {
            _parser.Version = GetVersion(version);
            Get(resource, id: id, action: null, format: null);
        }

        public void WebMethodGetIdSlash(string version, string resource, string id)
        {
            WebMethodGetId(version, resource, id);
        }

        public void WebMethodGetIdFormat(string version, string resource, string id, string format)
        {
            _parser.Version = GetVersion(version);
            Get(resource, id, action: null, format: format);
        }

        public void WebMethodGetIdFormatSlash(string version, string resource, string id, string format)
        {
            WebMethodGetIdFormat(version, resource, id, format);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodGetIdAction(string version, string resource, string id, string action)
        {
            _parser.Version = GetVersion(version);
            Get(resource, id: id, action: action, format: null);
        }

        public void WebMethodGetIdActionSlash(string version, string resource, string id, string action)
        {
            WebMethodGetIdAction(version, resource, id, action);
        }

        public void WebMethodGetIdActionFormat(string version, string resource, string id, string action, string format)
        {
            _parser.Version = GetVersion(version);
            Get(resource, id, action, format);
        }

        public void WebMethodGetIdActionFormatSlash(string version, string resource, string id, string action, string format)
        {
            WebMethodGetIdActionFormat(version, resource, id, action, format);
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------

        #region PutWebMethods

        public void WebMethodPut(string version, string resource, Stream data)
        {
            _parser.Version = GetVersion(version);
            Put(resource, id: null, action: null, format: null, data: data);
        }

        public void WebMethodPutSlash(string version, string resource, Stream data)
        {
            WebMethodPut(version, resource, data);
        }

        public void WebMethodPutFormat(string version, string resource, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Put(resource, id: null, action: null, format: format, data: data);
        }

        public void WebMethodPutFormatSlash(string version, string resource, string format, Stream data)
        {
            WebMethodPutFormat(version, resource, format, data);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodPutId(string version, string resource, string id, Stream data)
        {
            _parser.Version = GetVersion(version);
            Put(resource, id: id, action: null, format: null, data: data);
        }

        public void WebMethodPutIdSlash(string version, string resource, string id, Stream data)
        {
            WebMethodPutId(version, resource, id, data);
        }

        public void WebMethodPutIdFormat(string version, string resource, string id, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Put(resource, id: id, action: null, format: format, data: data);
        }

        public void WebMethodPutIdFormatSlash(string version, string resource, string id, string format, Stream data)
        {
            WebMethodPutIdFormat(version, resource, id, format, data);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodPutIdAction(string version, string resource, string id, string action, Stream data)
        {
            _parser.Version = GetVersion(version);
            Put(resource, id: id, action: action, format: null, data: data);
        }

        public void WebMethodPutIdActionSlash(string version, string resource, string id, string action, Stream data)
        {
            WebMethodPutIdAction(version, resource, id, action, data);
        }

        public void WebMethodPutIdActionFormat(string version, string resource, string id, string action, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Put(resource, id, action, format, data);
        }

        public void WebMethodPutIdActionFormatSlash(string version, string resource, string id, string action, string format, Stream data)
        {
            WebMethodPutIdActionFormat(version, resource, id, action, format, data);
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodDelete(string version, string resource, Stream data)
        {
            _parser.Version = GetVersion(version);
            Delete(resource, id: null, action: null, format: null, data: data);
        }

        public void WebMethodDeleteSlash(string version, string resource, Stream data)
        {
            WebMethodDelete(version, resource, data);
        }

        public void WebMethodDeleteFormat(string version, string resource, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Delete(resource, id: null, action: null, format: format, data: data);
        }

        public void WebMethodDeleteFormatSlash(string version, string resource, string format, Stream data)
        {
            WebMethodDeleteFormat(version, resource, format, data);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodDeleteId(string version, string resource, string id, Stream data)
        {
            _parser.Version = GetVersion(version);
            Delete(resource, id: id, action: null, format: null, data: data);
        }

        public void WebMethodDeleteIdSlash(string version, string resource, string id, Stream data)
        {
            WebMethodDeleteId(version, resource, id, data);
        }

        public void WebMethodDeleteIdFormat(string version, string resource, string id, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Delete(resource, id: id, action: null, format: format, data: data);
        }

        public void WebMethodDeleteIdFormatSlash(string version, string resource, string id, string format, Stream data)
        {
            WebMethodDeleteIdFormat(version, resource, id, format, data);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodDeleteIdAction(string version, string resource, string id, string action, Stream data)
        {
            _parser.Version = GetVersion(version);
            Delete(resource, id: id, action: action, format: null, data: data);
        }

        public void WebMethodDeleteIdActionSlash(string version, string resource, string id, string action, Stream data)
        {
            WebMethodDeleteIdAction(version, resource, id, action, data);
        }

        public void WebMethodDeleteIdActionFormat(string version, string resource, string id, string action, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Delete(resource, id: id, action: action, format: format, data: data);
        }

        public void WebMethodDeleteIdActionFormatSlash(string version, string resource, string id, string action, string format, Stream data)
        {
            WebMethodDeleteIdActionFormat(version, resource, id, action, format, data);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        //public void DefaultPostDelete(string resource, string id, Stream data)
        //{
        //    Delete(resource, id: id, action: null, format: null, data: data);
        //}

        //public void DefaultPostDeleteSlash(string resource, string id, Stream data)
        //{
        //    DefaultPostDelete(resource, id, data);
        //}

        //public void DefaultPostDeleteFormat(string resource, string id, string format, Stream data)
        //{
        //    Delete(resource, id: id, action: null, format: format, data: data);
        //}

        //public void DefaultPostDeleteFormatSlash(string resource, string id, string format, Stream data)
        //{
        //    DefaultPostDeleteFormat(resource, id, format, data);
        //}

        //---------------------------------------------------------------------------------------------------------------------------------------------

        //public void DefaultPostUpdate(string resource, string id, Stream data)
        //{
        //    Put(resource, id: id, action: null, format: null, data: data);
        //}

        //public void DefaultSlashPostUpdate(string resource, string id, Stream data)
        //{
        //    Put(resource, id: id, action: null, format: null, data: data);
        //}

        //public void DefaultPostUpdateFormat(string resource, string id, string format, Stream data)
        //{
        //    Put(resource, id: id, action: null, format: format, data: data);
        //}

        //public void DefaultSlashPostUpdateFormat(string resource, string id, string format, Stream data)
        //{
        //    Put(resource, id: id, action: null, format: format, data: data);
        //}

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodPost(string version, string resource, Stream data)
        {
            _parser.Version = GetVersion(version);
            Post(resource, id: null, action: null, format: null, data: data);
        }

        public void WebMethodPostSlash(string version, string resource, Stream data)
        {
            WebMethodPost(version, resource, data);
        }

        public void WebMethodPostFormat(string version, string resource, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Post(resource, id: null, action: null, format: format, data: data);
        }

        public void WebMethodPostFormatSlash(string version, string resource, string format, Stream data)
        {
            WebMethodPostFormat(version, resource, format, data);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodPostId(string version, string resource, string id, Stream data)
        {
            _parser.Version = GetVersion(version);
            Post(resource, id: id, action: null, format: null, data: data);
        }

        public void WebMethodPostIdSlash(string version, string resource, string id, Stream data)
        {
            WebMethodPostId(version, resource, id, data);
        }

        public void WebMethodPostIdFormat(string version, string resource, string id, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Post(resource, id: id, action: null, format: format, data: data);
        }

        public void WebMethodPostIdFormatSlash(string version, string resource, string id, string format, Stream data)
        {
            WebMethodPostIdFormat(version, resource, id, format, data);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public void WebMethodPostIdAction(string version, string resource, string id, string action, Stream data)
        {
            _parser.Version = GetVersion(version);
            Post(resource, id: id, action: action, format: null, data: data);
        }

        public void WebMethodPostIdActionSlash(string version, string resource, string id, string action, Stream data)
        {
            WebMethodPostIdAction(version, resource, id, action, data);
        }

        public void WebMethodPostIdActionFormat(string version, string resource, string id, string action, string format, Stream data)
        {
            _parser.Version = GetVersion(version);
            Post(resource, id: id, action: action, format: format, data: data);
        }

        public void WebMethodPostIdActionFormatSlash(string version, string resource, string id, string action, string format, Stream data)
        {
            WebMethodPostIdActionFormat(version, resource, id, action, format, data);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------

        public string SafeTrimAndLower(string value)
        {
            return value == null ? "" : value.Trim().ToLower();
        }

        private static CsTransactionLogProvider _transactionLogProvider = null;
        public CsTransactionLogProvider TransactionLogProvider
        {
            get
            {
                return _transactionLogProvider =
                    _transactionLogProvider ?? (_transactionLogProvider = new CsTransactionLogProvider());
            }
        }

        private static bool? _logTransactions = null;
        protected static bool LogTransactions
        {
            get { return (bool)(_logTransactions ?? (_logTransactions = GetLogTransactions())); }
        }
        private static bool? GetLogTransactions()
        {
            try
            {
                return string.Equals("yes", ConfigurationManager.AppSettings["LogTransactions"]);
            }
            catch
            {
                return false;
            }
        }

        private static string CreatePath(ICallParser parser, string api, string resource, string id, string action)
        {
            string theResource = resource ?? "";
            string theId = id ?? "";
            string theAction = action ?? "";

            if (theId != "" || theAction != "")
                theId = "/" + theId;
            if (theAction != "")
                theAction = "/" + theAction;
            return "/" + api + "/v" + parser.Version.ToString() + "/" + Param.API_ROOT + "/" + theResource + theId + theAction;
        }

        protected void LogTransaction(string httpMethod, string api, string resource, string id, string action, string inputData = null)
        {
            //Don't Fail on log
            try
            {
                new CsTransactionLogProvider().LogTransaction(new TransactionEntryObject()
                {
                    HttpMethod = httpMethod,
                    InputData = inputData,
                    QueryString = _parser.QueryString,
                    ServicePath = CreatePath(_parser, api, resource, id, action),
                    Resource = resource,
                    ResourceId = id,
                    ResourceAction = action
                });
            }
            catch (Exception ex)
            {
                try
                {
                    Logger.LogError(ex);
                }
                catch { };
            }
        }


    }

}
