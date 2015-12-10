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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web;
using System.Web.Script.Serialization;
using System.Configuration;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.TransactionLogProvider;
using Gov.Hhs.Cdc.Commons.Api.Key.Utils;
using Newtonsoft.Json;

namespace Gov.Hhs.Cdc.Api
{
    /// <summary>
    /// This utility is used to provide a single interface to executing the API code, while also providing the capability
    /// to execute the API through the restful web service as well as being able to easily step into the code by calling
    /// the API methods directly.  
    /// 
    /// Normally, to debug using the web service, you must start the API without debugging, start the unit test with a breakpoint
    /// on the call to the web service, and then attach the API to IIS once you hit the break point.
    /// 
    /// With this utility, you can just modify the UseWebService flag below to false, and the call will then call directly into
    /// the API.  What follows is sample code to call the API in a test.
    /// 
    ///     IApiServiceFactory adminService = new AdminApiServiceFactory();
    ///     var url = adminService.CreateSecureUrl("valuesets", id, "", "");
    ///     List<SerialValueSetObj> createdValueSets;
    ///     var messages = TestApiUtility.ApiPost<SerialValueSetObj>(adminService,
    ///         url, valueSet, out createdValueSets, WindowsIdentity.GetCurrent().Name);
    /// </summary>
    public static class TestApiUtility
    {
        public static string ConsumerKey = "XIdBaBpCDarLCEQAERgxF0szEakjd65KtvU362sitXo";
        private static string consumerSecret = "Bpd3IJPt6JrU/aUK7/T0hU2nTnpZx1U5yRRt/xaMOgkk/BlFSpX5Hb9JzbPc3G4TfbbE7UhARI3JbT9MtmR/iA==";

        /// <summary>
        /// Modify this property to step into the code directly in a unit test
        /// </summary>
        public static bool UseWebService = true;

        public static ValidationMessages ApiPost<T>(IApiServiceFactory serviceFactory, TestUrl theUrl, T newSerialData, out List<T> outputObjects, string adminUser = "")
        {
            return ApiPostSerializedData<T>(serviceFactory, theUrl, Serialize(newSerialData), out outputObjects, adminUser);
        }

        public static ValidationMessages ApiPostWithoutOutput<T>(IApiServiceFactory serviceFactory, TestUrl theUrl, T newSerialData, string adminUser = "")
        {
            List<T> outputObjects;
            var serialized = Serialize(newSerialData);
            return ApiPostSerializedData<T>(serviceFactory, theUrl, serialized, out outputObjects, adminUser);
        }

        /// <summary>
        /// The preferred method to execute a Put
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceFactory"></param>
        /// <param name="theUrl"></param>
        /// <param name="newSerialData"></param>
        /// <param name="outputObjects"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static ValidationMessages ApiPut<T>(IApiServiceFactory serviceFactory, TestUrl theUrl, T newSerialData, out List<T> outputObjects, string adminUser = "")
        {
            return ApiPutSerializedData<T>(serviceFactory, theUrl, Serialize(newSerialData), out outputObjects, adminUser);
        }

        public static ValidationMessages ApiPutSerializedData<T>(IApiServiceFactory serviceFactory, TestUrl theUrl, string theData,
            out List<T> outputObjects, string adminUser = "")
        {
            SerialResponse serialResponse;
            if (TestApiUtility.UseWebService)
            {
                var response = CallAPI(theUrl.ToString(), theData, "PUT", adminUser);
                serialResponse = DeserializeResponse<T>(response);
            }
            else
            {
                serialResponse = (SerialResponse)ApiPut(serviceFactory, theUrl, theData);
            }
            return ConvertSerialResponse<T>(serialResponse, out outputObjects);
        }

        public static ValidationMessages ApiPostSerializedData<T>(IApiServiceFactory serviceFactory, TestUrl theUrl, string stream, out List<T> outputObjects, string adminUser = "")
        {
            outputObjects = null;
            SerialResponse serialResponse;
            if (TestApiUtility.UseWebService)
            {
                string callResults = CallAPI(theUrl.ToString(), stream, "POST", adminUser);
                serialResponse = DeserializeResponse<T>(callResults);
                return ConvertSerialResponse<T>(serialResponse, out outputObjects);
            }
            else
            {
                SerialResponse response;
                ValidationMessages messages = ExecuteApiPost(serviceFactory, theUrl, stream, out response);
                ConvertSerialResponse<T>(response, out outputObjects);
                return messages;
            }
        }

        private static ValidationMessages ExecuteApiPost(IApiServiceFactory serviceFactory, TestUrl theUrl, string stream, out SerialResponse response)
        {
            SetCurrentHttpContext(theUrl);
            TestOutputWriter outputWriter = new TestOutputWriter();
            RestServiceBase service = serviceFactory.CreateNewService(theUrl.Version);
            service.Post(outputWriter, stream, ConsumerKey, theUrl.Resource, theUrl.Id, theUrl.Action);
            response = (SerialResponse)outputWriter.TheObject;
            return outputWriter.ValidationMessages;
        }

        private static ValidationMessages ExecuteApiPut(IApiServiceFactory serviceFactory, TestUrl theUrl, string stream, out SerialResponse response)
        {
            SetCurrentHttpContext(theUrl);
            TestOutputWriter outputWriter = new TestOutputWriter();
            RestServiceBase service = serviceFactory.CreateNewService(theUrl.Version);
            service.Put(outputWriter, stream, ConsumerKey, theUrl.Resource, theUrl.Id, theUrl.Action);
            response = (SerialResponse)outputWriter.TheObject;
            return outputWriter.ValidationMessages;
        }

        private static object ApiPut(IApiServiceFactory apiServiceFactory, TestUrl url, string stream)
        {
            SetCurrentHttpContext(url);
            TestOutputWriter outputWriter = new TestOutputWriter();
            RestServiceBase service = apiServiceFactory.CreateNewService(url.Version);
            service.Put(outputWriter, stream, ConsumerKey, url.Resource, url.Id, url.Action);
            return outputWriter.TheObject;
        }

        /// <summary>
        /// The preferred method to execute a Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceFactory"></param>
        /// <param name="theUrl"></param>
        /// <param name="outputObjects"></param>
        /// <returns></returns>
        public static ValidationMessages ApiGet<T>(IApiServiceFactory serviceFactory, TestUrl theUrl, out List<T> outputObjects)
        {
            SerialResponse serialResponse;
            if (UseWebService)
            {
                Console.WriteLine("API call: " + theUrl.ToString());
                serialResponse = DeserializeResponse<T>(CallAPI(theUrl.ToString(), string.Empty, "GET"));
            }
            else
                serialResponse = (SerialResponse)ApiGet(serviceFactory, theUrl);
            return ConvertSerialResponse<T>(serialResponse, out outputObjects);
        }

        private static SerialResponse DeserializeResponse<T>(string jsonStream)
        {
            if (jsonStream == string.Empty) throw new ApplicationException("no results from API");

            try
            {
                Console.WriteLine(jsonStream);
                var obj = JsonConvert.DeserializeObject<SerialResponseWithType<List<T>>>(jsonStream);
                return new SerialResponse(obj.meta, obj.results);
            }
            catch (JsonSerializationException)
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<SerialResponseWithType<T>>(jsonStream);
                    return new SerialResponse(obj.meta, obj.results);
                }
                catch (JsonSerializationException)
                {
                    return new JavaScriptSerializer().Deserialize<SerialResponse>(jsonStream);
                }
            }
        }

        private static ValidationMessages ConvertSerialResponse<T>(SerialResponse serialResponse, out List<T> outputObjects)
        {
            outputObjects = null;
            if (serialResponse == null)
                return new ValidationMessages();
            else
            {
                if (serialResponse.results.GetType() == typeof(List<T>))
                    outputObjects = (List<T>)serialResponse.results;

                return serialResponse.meta.GetUnserializedMessages();
            }
        }

        /// <summary>
        /// The preferred method to execute a Delete
        /// </summary>
        /// <param name="serviceFactory"></param>
        /// <param name="theUrl"></param>
        /// <param name="stream"></param>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public static ValidationMessages ApiDelete(IApiServiceFactory serviceFactory, TestUrl theUrl, string stream, string adminUser)
        {
            SerialResponse serialResponse;
            if (TestApiUtility.UseWebService)
            {
                string callResults = CallAPI(theUrl.ToString(), string.Empty, "DELETE", adminUser);
                SerialResponse obj = new JavaScriptSerializer().Deserialize<SerialResponse>(callResults);
                serialResponse = new SerialResponse(obj.meta, obj.results);
                return serialResponse.meta.GetUnserializedMessages();
            }
            else
            {
                SetCurrentHttpContext(theUrl);
                TestOutputWriter outputWriter = new TestOutputWriter();
                RestServiceBase service = serviceFactory.CreateNewService(theUrl.Version);
                service.Delete(outputWriter, stream, TestApiUtility.ConsumerKey, theUrl.Resource, theUrl.Id, theUrl.Action);
                return outputWriter.ValidationMessages;
            }

        }

        private static string Serialize(object data)
        {
            return data == null ? string.Empty : new JavaScriptSerializer().Serialize(data);
        }

        //TODO: Add a HttpContextFactory, and use it in the application to be able to set/use header value (i.e., admin_user)
        //For more information, see:
        //    http://stackoverflow.com/questions/9624242/setting-the-httpcontext-current-session-in-unit-test/18982320#18982320
        private static void SetCurrentHttpContext(TestUrl url)
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest(string.Empty, string.Join(@"/", url.ToString()), string.Empty),
                new HttpResponse(new StringWriter())
                );
        }

        public static byte[] ApiGetBytes(IApiServiceFactory serviceFactory, TestUrl theUrl)
        {
            if (TestApiUtility.UseWebService)
            {
                return TestApiUtility.GetBytes(theUrl.ToString());
            }
            else
            {
                return (byte[])ApiGet(serviceFactory, theUrl);
            }
        }

        private static object ApiGet(IApiServiceFactory serviceFactory, TestUrl url)
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", string.Join(@"/", url.ToString()), string.Empty),
                new HttpResponse(new StringWriter())
                );
            TestOutputWriter outputWriter = new TestOutputWriter();

            //Get a media item as an examples
            RestServiceBase service = serviceFactory.CreateNewService(url.Version);
            service.ExecuteGet(outputWriter, url.Resource, url.Id, url.Action);
            object results = outputWriter.TheObject;
            return results;
        }

        public static string CallAPIPut(string apiUrl, string data, string adminUser)
        {
            return CallAPI(apiUrl, data, WebRequestMethods.Http.Put, adminUser);
        }

        public static string CallAPIPost(string apiUrl, string data, string adminUser)
        {
            return CallAPI(apiUrl, data, WebRequestMethods.Http.Post, adminUser);
        }

        public static string CallAPIDelete(string apiUrl, string data, string adminUser)
        {
            return CallAPI(apiUrl, data, "DELETE", adminUser);
        }

        public static string CallAPI(string apiUrl, string data, string method = WebRequestMethods.Http.Post, string adminUser = "")
        {
            Console.WriteLine(apiUrl);
            if (method == WebRequestMethods.Http.Get)
                return Get(apiUrl);
            if (data != null)
                data = data.Replace("\\u0027", "'");

            string response = string.Empty;

            var webClient = new WebClient();
            if (!string.IsNullOrEmpty(adminUser))
            {
                if (adminUser.StartsWith("CDC\\"))
                {
                    adminUser = adminUser.Substring(4);
                }
                webClient.Headers.Add("admin_user", adminUser);
            }
            webClient.Headers.Add("Authorization", GetAuthorizationHeader(ref webClient, apiUrl, method, data));

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            try
            {
                response = webClient.UploadString(apiUrl, method, data);
                Console.WriteLine(response);
            }
            catch (WebException)
            {
                webClient.Dispose();
                //timeout
            }

            webClient.Dispose();
            return response;
        }

        private static string GetAuthorizationHeader(ref WebClient request, string url, string method, string requestBody)
        {
            AuthorizationHeaderGenerator.KeyAgreement keyAgreement = new AuthorizationHeaderGenerator.KeyAgreement();
            keyAgreement.publicKey = ConsumerKey;
            keyAgreement.secret = consumerSecret;

            AuthorizationHeaderGenerator generator = new AuthorizationHeaderGenerator("syndication_api_key", keyAgreement);

            var headers = new NameValueCollection();
            headers.Add("X-Syndication-Date", DateTime.UtcNow.ToString());
            headers.Add("Content-Type", "application/json");

            // Add headers to request
            request.Headers.Add(headers);

            headers.Add("Content-Length", requestBody.Length.ToString());
            string apiKeyHeaderValue = generator.GetApiKeyHeaderValue(headers, url, method, requestBody);

            return apiKeyHeaderValue;
        }

        //Above method does not work for GET requests because it is not valid to send data
        public static string Get(string apiUrl)
        {
            string nonCache = Guid.NewGuid().ToString();
            var parmChar = "&";
            if (!apiUrl.Contains("?")) parmChar = "?";
            apiUrl += parmChar + nonCache + "=" + nonCache;
            var uri = new Uri(apiUrl);
            Console.WriteLine(apiUrl);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            using (var webClient = new WebClient())
            {
                try
                {
                    return webClient.DownloadString(apiUrl);
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message); //generally a timeout
                }

                return string.Empty;
            }
        }

        public static string Get(string apiUrl, string adminUser)
        {
            string nonCache = Guid.NewGuid().ToString();
            var parmChar = "&";
            if (!apiUrl.Contains("?")) parmChar = "?";
            apiUrl += parmChar + nonCache + "=" + nonCache;
            var uri = new Uri(apiUrl);
            Console.WriteLine(apiUrl);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            using (var webClient = new WebClient())
            {
                if (!string.IsNullOrEmpty(adminUser))
                {
                    webClient.Headers.Add("admin_user", adminUser);
                }
                try
                {
                    return webClient.DownloadString(apiUrl);
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message); //generally a timeout
                }

                return string.Empty;
            }
        }

        //Above method does not work for GET requests because it is not valid to send data
        public static byte[] GetBytes(string apiUrl)
        {
            var uri = new Uri(apiUrl);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadData(apiUrl);
                return response;
            }
        }

        //for testing purpose only, accept any dodgy certificate... 
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static ValidationMessages DeserializeResults(string data)
        {
            if (data == string.Empty) throw new ApplicationException("no results from API");

            SerialResponse mediaObj = new JavaScriptSerializer().Deserialize<SerialResponse>(data);
            ValidationMessages messages = new ValidationMessages()
            {
                Messages = mediaObj.meta.message.Select(m => new ValidationMessage(m.type, m.code, m.userMessage)).ToList()
            };
            return messages;
        }

        public static SerialMediaV2 PublicHtml()
        {
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}?{4}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "media", "status=published&mediatype=html");
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            var action = new ActionResultsWithType<List<SerialMediaV2>>(result);
            var errors = "";
            if (action.Meta.message.Count() > 0) errors = action.Meta.message.First().developerMessage;
            if (!string.IsNullOrEmpty(errors)) throw new InvalidOperationException(errors);
            if (action.ResultObject.Count < 1) throw new InvalidOperationException("No results from API call");
            return action.ResultObject[0];
        }

        //test data
        public static SerialMediaAdmin SinglePublishedHtml()
        {
            var criteria = "?mediatype=HTML&status=published";
            var results = AdminApiMediaSearch(criteria);
            Console.WriteLine(criteria);
            if (results.Count() < 1) throw new InvalidOperationException("No results from API call");
            return results.First();
        }

        public static List<SerialMediaAdmin> GetAllPublishedFeeds()
        {
            List<SerialMediaAdmin> resultList = new List<SerialMediaAdmin>();
            var criteria = "?mediatype=feed&status=published";
            var results = AdminApiMediaSearch(criteria);
            if (results.Count() < 1) throw new InvalidOperationException("No results from API call");
            //var result = results.Where(i => i.feed != null && i.feed.imageTitle != null);
            foreach (SerialMediaAdmin media in results)
            {
                resultList.Add(media);
            }

            return resultList;
        }

        public static SerialMediaAdmin SinglePublishedFeed()
        {
            var criteria = "?mediatype=feed&status=published";
            var results = AdminApiMediaSearch(criteria);
            if (results.Count() < 1) throw new InvalidOperationException("No results from API call");
            var result = results.Where(i => i.feed != null && i.feed.imageTitle != null);

            return result.First();
        }

        public static SerialMediaAdmin SinglePublishedFeedWithChildren()
        {
            //SerialMediaAdmin sendBack = null;
            var criteria = "?mediatype=feed&status=published";
            var results = AdminApiMediaSearch(criteria);
            if (results.Count() < 1) throw new InvalidOperationException("No results from API call");
            var result = results.Where(i => i.feed != null && i.childCount > 0).First();

            return result;
        }

        public static SerialMediaAdmin SinglePublishedFeedWithChildrenAndAltImages()
        {
            SerialMediaAdmin sendBack = null;
            var criteria = "?mediatype=feed&status=published";
            var results = AdminApiMediaSearch(criteria);
            if (results.Count() < 1) throw new InvalidOperationException("No results from API call");
            var result = results.Where(i => i.feed != null && i.childCount > 0).ToList();
            foreach (SerialMediaAdmin serialAdmin in result)
            {
                for (int i = 0; i < serialAdmin.children.Count; i++)
                {
                    if (serialAdmin.children[i].alternateImages == null)
                    {
                        continue;
                    }

                    if (serialAdmin.children[i].alternateImages.Count > 0)
                    {
                        sendBack = serialAdmin;
                        break;
                    }
                }

                if (sendBack != null)
                {
                    break;
                }
            }

            return sendBack;
        }

        public static IEnumerable<SerialMediaAdmin> AdminApiMediaSearch(string criteria)
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, criteria);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            //Console.WriteLine(result);
            var action = new ActionResultsWithType<List<SerialMediaAdmin>>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }
            return action.ResultObject as IEnumerable<SerialMediaAdmin>;
        }

        public static IList<string> AdminApiCache()
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/cache", TestUrl.Protocol, TestUrl.AdminApiServer);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<List<string>>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }
            return action.ResultObject as IList<string>;
        }

        public static IList<string> PublicApiCache()
        {
            var url = string.Format("{0}://{1}/api/v2/resources/cache", TestUrl.Protocol, TestUrl.PublicApiServer);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<List<string>>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }
            return action.ResultObject as IList<string>;
        }

        public static IEnumerable<SerialMediaV2> PublicApiV2MediaSearch(string criteria)
        {
            var url = string.Format("{0}://{1}/api/v2/resources/media?{2}", TestUrl.Protocol, TestUrl.PublicApiServer, criteria);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);

            var action = new ActionResultsWithType<List<SerialMediaV2>>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }
            return action.ResultObject as IEnumerable<SerialMediaV2>;

        }

        public static SerialMeta AdminApiSearchMeta(string criteria)
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, criteria);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<List<SerialMediaAdmin>>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }

            return action.Meta;
        }

        public static SerialMediaAdmin SinglePublishedEcard()
        {
            var criteria = "?mediatype=eCard&status=published";
            var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, criteria);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            //Console.WriteLine(result);
            var action = new ActionResultsWithType<List<SerialMediaAdmin>>(result);
            if (action.ResultObject.Count < 1) throw new InvalidOperationException("No results from API call");
            return action.ResultObject[0];
        }

        public static List<SerialMediaAdmin> ExistingBySourceUrl(string sourceUrl)
        {
            var criteria = "?urlcontains=" + sourceUrl;
            var url = string.Format("{0}://{1}/adminapi/{2}/resources/{3}/{4}", TestUrl.Protocol, TestUrl.AdminApiServer, "v2", "media", criteria);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<List<SerialMediaAdmin>>(result);
            if (action.ResultObject.Count < 1) return new List<SerialMediaAdmin>();
            return action.ResultObject;
        }

        public static SerialMediaAdmin SingleAdminMedia(int mediaId)
        {
            var url = string.Format("{0}://{1}/adminapi/{2}/resources/{3}/{4}", TestUrl.Protocol, TestUrl.AdminApiServer, "v2", "media", mediaId);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<List<SerialMediaAdmin>>(result);
            if (action.ResultObject.Count < 1) throw new InvalidOperationException("No results from API call");
            return action.ResultObject[0];
        }

        public static ActionResultsWithType<List<SerialMediaAdmin>> CreateAdminMedia(SerialMediaAdmin media, string adminUser)
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/media", TestUrl.Protocol, TestUrl.AdminApiServer);
            var ser = JsonConvert.SerializeObject(media);
            var results = TestApiUtility.CallAPIPost(url, ser, adminUser);
            return new ActionResultsWithType<List<SerialMediaAdmin>>(results);
        }

        public static ActionResultsWithType<List<SerialMediaAdmin>> UpdateAdminMedia(SerialMediaAdmin media, string adminUser)
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, media.id);
            var ser = JsonConvert.SerializeObject(media);
            var results = TestApiUtility.CallAPIPut(url, ser, adminUser);
            return new ActionResultsWithType<List<SerialMediaAdmin>>(results);
        }

        public static ActionResultsWithType<List<SerialMediaValidation>> ValidateUrl(string url)
        {
            var adminApi = string.Format("{0}://{1}/adminapi/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, "v1");
            var api = string.Format("{0}/resources/{1}/?url={2}", adminApi, "validations", url);
            var result = Get(api);
            var results = new ActionResultsWithType<List<SerialMediaValidation>>(result);
            return results;
        }

        public static ActionResultsWithType<List<SerialMediaValidation>> ValidateUrl(string url, string className)
        {
            var adminApi = string.Format("{0}://{1}/adminapi/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, "v1");
            var api = string.Format("{0}/resources/{1}/?url={2}&clsids={3}", adminApi, "validations", url, className);
            var result = Get(api);
            var results = new ActionResultsWithType<List<SerialMediaValidation>>(result);
            return results;
        }


        public static bool Matches(this DateTime? d1, DateTime? d2)
        {
            if (!d1.HasValue && !d2.HasValue) return true;

            TimeSpan? res = d1 - d2;
            if (!res.HasValue) return false;
            TimeSpan ts = res.Value;
            return ts.Days == 0 && ts.Milliseconds < 1000;// less than a second?  close enough.
        }

        public static void ClearStorefrontApiCache()
        {
            var url = string.Format("{0}://{1}/api/v1/resources/clearcache", TestUrl.Protocol, TestUrl.PublicApiServer);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<string>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }
        }

        public static void ClearAdminApiCache()
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/clearcache", TestUrl.Protocol, TestUrl.AdminApiServer);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<string>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }
        }

        public static SerialAdminStats AdminApiStats()
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/stats", TestUrl.Protocol, TestUrl.AdminApiServer);
            Console.WriteLine(url);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<SerialAdminStats>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }
            return action.ResultObject;
        }

        public static IEnumerable<SerialMediaAdmin> Dupes()
        {
            var url = string.Format("{0}://{1}/adminapi/v1/resources/mediadupes", TestUrl.Protocol, TestUrl.AdminApiServer);
            var result = Get(url);
            //Console.WriteLine(result);
            var action = new ActionResultsWithType<List<SerialMediaAdmin>>(result);
            if (action.ValidationMessages.Errors().Any())
            {
                var err = action.ValidationMessages.Errors().First();
                throw new InvalidOperationException(string.IsNullOrEmpty(err.DeveloperMessage) ? err.Message : err.DeveloperMessage);
            }
            Console.WriteLine(action.ResultObject.Count);
            return action.ResultObject;
        }
    }
}
