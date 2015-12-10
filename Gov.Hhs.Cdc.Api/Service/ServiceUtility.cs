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
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsCaching;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;


namespace Gov.Hhs.Cdc.Api
{
    public static class ServiceUtility
    {
        /// <summary>
        /// Parse the id into an integer only if a positive integer id is required.  This will
        /// fail the API call with an invalid ID error if the id is not a valid integer.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int ParsePositiveInt(string id)
        {
            int results = 0;
            if (int.TryParse(id, out results) && results > 0)
            {
                return results;
            }
            else
            {
                throw new InvalidResourceIdException();
            }
        }

        public static int ParseInt(string id)
        {
            int results = 0;
            if (int.TryParse(id, out results) && results > 0)
            {
                return results;
            }
            else
            {
                return 0;
            }
        }

        private static readonly object CacheAddLockObject = new object();
        public static void AddApplicationCachePublicApi<T>(T objectToCache, ICallParser parser)
        {
            if (HttpContext.Current.Request.HttpMethod != WebRequestMethods.Http.Get)
            {
                return;
            }
            if (HttpContext.Current.Response.StatusCode != (int)HttpStatusCode.OK)
            {
                if (HttpContext.Current.Response.StatusCode != (int)HttpStatusCode.MovedPermanently)
                {
                    return;
                }
            }

            if (!parser.IsPublicFacing)
            {
                return;
            }

            string[] DoNotCacheTheseParams =
                ConfigurationManager.AppSettings["PublicAPICacheExemptionList"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string param in DoNotCacheTheseParams)
            {
                if (parser.Query.CacheKey.Contains(param))
                {
                    return;
                }
            }

            // add object to cache
            AddBasicApplicationCache(parser.Query.CacheKey, objectToCache);
        }

        public static void AddBasicApplicationCache<T>(string key, T objectToCache)
        {
            int ttl = GetApiCacheTTL("PublicApiDefaultCacheInSeconds");
            if (ttl > 0)
            {
                lock (CacheAddLockObject)
                {
                    var result = CacheManager.CachedValue<T>(key);
                    if (result == null)
                    {
                        CacheManager.Cache(key, objectToCache, DateTime.Now.AddSeconds(ttl));
                    }
                    SetCacheForBrowser(ttl);
                }
            }
        }

        public static int GetApiCacheTTL(string configName)
        {
            int APIPublicApiDefaultCacheInSeconds = ParseInt(ConfigurationManager.AppSettings[configName]);
            return APIPublicApiDefaultCacheInSeconds;
        }

        private static void SetCacheForBrowser(int ttl_in_seconds)
        {
            TimeSpan refresh = new TimeSpan(0, 0, ttl_in_seconds);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.Add(refresh));
            HttpContext.Current.Response.Cache.SetMaxAge(refresh);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
            HttpContext.Current.Response.CacheControl = HttpCacheability.Public.ToString();
            HttpContext.Current.Response.Cache.SetValidUntilExpires(true);
        }

        public static SerialMeta CreateOutputMeta(ICallParser parser, DataSetResult results, SearchParameters searchParameters)
        {
            SerialMeta meta = new SerialMeta();
            if (results == null || results.RecordCount == 0)
            {
                return meta;
            }

            meta.pagination = CreatePagination(parser, results, searchParameters.Sorting);
            meta.resultSet.id = results.Id.ToString();

            return meta;
        }

        public static SerialMeta CreateResponseMetaDataForOneItem(ICallParser parser, int currentId = 0, bool forExport = false)
        {
            SerialMeta meta = new SerialMeta();
            meta.pagination = new SerialPagination(parser, new Sorting(parser.SortColumns), 1, 1, 1, 0, 1, 1, currentId, 0, forExport);
            meta.resultSet.id = Guid.NewGuid().ToString();

            return meta;
        }

        public static void DeprecatedSetOutputMeta(SerialMeta meta, ICallParser parser, DataSetResult results, SearchParameters searchParameters)
        {
            if (results == null || results.RecordCount == 0)
            {
                return;
            }
            DeprecatedSetPagination(meta, parser, results, searchParameters);
            meta.resultSet.id = results.Id.ToString();
        }

        private static SerialPagination CreatePagination(ICallParser parser, DataSetResult results, Sorting sort)
        {
            SerialPagination pagination = new SerialPagination(parser, sort, results.TotalRecordCount, results.RecordCount, results.PageSize,
                 parser.Query.Offset, results.PageNumber, results.TotalPages);

            return pagination;
        }

        private static void DeprecatedSetPagination(SerialMeta meta, ICallParser parser, DataSetResult results, SearchParameters searchParameters)
        {
            SerialPagination pagination = new SerialPagination(parser, searchParameters.Sorting, results.TotalRecordCount, results.RecordCount, results.PageSize,
                parser.Query.Offset, results.PageNumber, results.TotalPages);

            meta.pagination = pagination;
        }

        public static string GetCurrentUrlPath(bool specificProtocol, string appendAction, bool forExport)
        {
            string url = "";

            if (forExport == false)
            {
                url = GetServerBaseOnProtocol(specificProtocol);
                if (!string.IsNullOrEmpty(appendAction))
                {
                    url += HttpContext.Current.Request.Url.AbsolutePath.Substring(0, HttpContext.Current.Request.Url.AbsolutePath.LastIndexOf("/") + 1) + appendAction;
                }
                else
                {
                    url += HttpContext.Current.Request.Url.AbsolutePath;
                }
            }
            else
            {
                string PublicApiForExport = HttpContext.Current.Request.Url.Scheme + "://" + ConfigurationManager.AppSettings["PublicApiServer"] + ConfigurationManager.AppSettings["PublicApiForExport"];
                if (!string.IsNullOrEmpty(appendAction))
                {
                    url = PublicApiForExport.Substring(0, PublicApiForExport.LastIndexOf("/") + 1) + appendAction;
                }
                else
                {
                    url = PublicApiForExport;
                }
            }

            return url;
        }

        public static string GetServerBaseOnProtocol(bool specificProtocol)
        {
            string server = string.Empty;

            if (HttpContext.Current.Request.Url.Scheme == Uri.UriSchemeHttp)
            {
                server = ConfigurationManager.AppSettings["HttpServer"];
            }
            else if (HttpContext.Current.Request.Url.Scheme == Uri.UriSchemeHttps)
            {
                server = ConfigurationManager.AppSettings["HttpsServer"];
            }

            if (!specificProtocol)
            {
                server = "//" + server;
            }
            else
            {
                server = HttpContext.Current.Request.Url.Scheme + "://" + server;
            }

            return server;
        }

        public static string GetResourceUrl(int id, string action, bool forExport = false)   //, string resource = "")
        {
            if (id < 1)
            {
                return null;
            }

            Uri url = new Uri(GetCurrentUrlPath(true, null, forExport));
            string resourceUrl = url.Scheme + "://" + url.Authority +
                url.Segments[0] + url.Segments[1] + url.Segments[2] + url.Segments[3];

            //if (string.IsNullOrEmpty(resource))
            resourceUrl += url.Segments[4].TrimEnd('/') + "/" + id.ToString();
            //else
            //    resourceUrl += resource + "/" + id;

            FormatType format = GetFormatType(GetFormatFromServicePath(url.AbsolutePath));
            // remove format
            resourceUrl = resourceUrl.Replace("." + format.ToString(), string.Empty);

            if (!string.IsNullOrEmpty(action))
            {
                resourceUrl += "/" + action;

                if (action != Param.MEDIA_THUMBNAIL && action != Param.MEDIA_CONTENT)
                {
                    resourceUrl += "." + format;
                }
            }
            else
            {
                resourceUrl += "." + format;
            }

            return resourceUrl;
        }

        private static string GetFormatFromServicePath(string urlPath)
        {
            int startIndex = urlPath.LastIndexOf(".") + 1;

            if (startIndex == 0)
            {
                var dict = RestHelper.CreateDictionary(HttpContext.Current.Request.Url.Query);
                if (dict.ContainsKey(Param.FORMAT))
                {
                    return dict[Param.FORMAT];
                }
            }
            return urlPath.Substring(startIndex, urlPath.Length - startIndex);
        }

        public static string GetPersistentUrl(string persistentUrlToken)
        {
            if (string.IsNullOrEmpty(persistentUrlToken))
            {
                return null;
            }
            else
            {
                string server = string.Empty;

                if (HttpContext.Current.Request.Url.Scheme == Uri.UriSchemeHttp)
                {
                    server = ConfigurationManager.AppSettings["PersistentUrlHttp"];
                }
                else if (HttpContext.Current.Request.Url.Scheme == Uri.UriSchemeHttps)
                {
                    server = ConfigurationManager.AppSettings["PersistentUrlHttps"];
                }

                server = string.Format(server, ConfigurationManager.AppSettings["PublicApiVersion"], persistentUrlToken);

                return server;
            }
        }

        public static string GetStorageUrl(string file, bool forExport)
        {
            Uri url = new Uri(GetCurrentUrlPath(true, null, forExport));

            return url.Scheme + "://" + url.Authority +
                url.Segments[0] + url.Segments[1] + url.Segments[2] + url.Segments[3] + Param.API_STORAGE
                + "/" + file;
        }

        private static string GetQueryStringForPaging(string queryString, bool removePageNum)
        {
            IDictionary<string, string> dict = RestHelper.CreateDictionary(queryString);

            if (removePageNum)
            {
                dict.Remove(Param.RESULT_SET_ID);
                dict.Remove(Param.PAGE_NUMBER);

                return RestHelper.ConstructQueryStringFromDictionary(dict);
            }
            else
            {
                return RestHelper.ConstructCanonicalQueryStringFromDictionary(dict);
            }
        }

        public static string HtmlEncodeByFormat(string url, string format)
        {
            return string.Equals(format, "xml", StringComparison.OrdinalIgnoreCase)
                ? Util.HtmlEncodeOutput(url) : HttpUtility.UrlDecode(url);
        }

        public static void WriteDoNotCacheResponseToHeader(IOutputWriter writer)
        {
            //disable cache
            writer.AddHeader("Pragma", "no-cache");
            writer.AddHeader("Expires", "Tue, 31 Mar 1981 05:00:00 GMT");
            writer.AddHeader("Cache-Control", "max-age=0, no-cache, no-store, must-revalidate, pre-check=0, post-check=0");
        }

        public static void SetResponse(ICallParser parser, IOutputWriter writer, SearchParameters searchParameters, DataSetResult dataSet, SerialResponse response1)
        {
            SerialResponse response = new SerialResponse()
            {
                results = response1.results,
                meta = ServiceUtility.CreateOutputMeta(parser, dataSet, searchParameters)
            };

            writer.Write(response);
        }

        public static DataSetResult GetDataSet(SearchParameters searchParameters, out ValidationMessages messages)
        {
            messages = new ValidationMessages();

            try
            {
                var getController = HandlerSearchBase.SearchControllerFactory.GetSearchController(searchParameters);
                return getController.Get();
            }
            catch (Exception ex)
            {
                var message = "An error occurred while retrieving data";
                Logger.LogError(ex.GetBaseException(), message);
                messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "Parameter", message));
                return new DataSetResult();
            }
        }

        public static void GetDataSet(ICallParser parser, SearchParameters searchParameters, SerialResponse response)
        {
            ValidationMessages messages;
            response.dataset = ServiceUtility.GetDataSet(parser, searchParameters, out messages);
            if (messages.Errors().Any())
            {
                response.meta.message = OutputWriter.SerializeValidationMessages(messages);
            }
        }

        public static void GetDataSet(ICallParser parser, SearchParameters searchParameters, SerialResponse response, IList<int> allowableMediaIds)
        {
            ValidationMessages messages;
            response.dataset = ServiceUtility.GetDataSet(parser, searchParameters, allowableMediaIds, out messages);
            if (messages.Errors().Any())
            {
                response.meta.message = OutputWriter.SerializeValidationMessages(messages);
            }
        }

        public static DataSetResult GetDataSet(ICallParser parser, SearchParameters searchParameters, out ValidationMessages messages)
        {
            messages = new ValidationMessages();
            if (parser.Query.HasResultSetIdAndPagingOrOffset)
            {

                try
                {
                    // cached paging
                    var navigateController = HandlerSearchBase.SearchControllerFactory.GetSearchController(Guid.Parse(parser.Query.ResultSetId));
                    return navigateController.NavigatePages(parser.Query.PageNumber, parser.Query.Offset);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Invalid result set id " + parser.Query.ResultSetId + ".  " + ex.ToString(), "GetDataSet");
                    messages.AddError("ResultSetId", "Expired result set");
                    return new DataSetResult();
                }
            }

            try
            {
                var getController = HandlerSearchBase.SearchControllerFactory.GetSearchController(searchParameters);
                return getController.Get();
            }
            catch (Exception ex)
            {
                var message = "An error occurred while retrieving data";
                Logger.LogError(ex.GetBaseException(), message, parser.Query.CacheKey);
                messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "Parameter", message));
                return new DataSetResult();
            }
        }

        public static DataSetResult GetDataSet(ICallParser parser, SearchParameters searchParameters, IList<int> allowableMediaIds, out ValidationMessages messages)
        {
            messages = new ValidationMessages();
            if (parser.Query.HasResultSetIdAndPagingOrOffset)
            {

                try
                {
                    // cached paging
                    var navigateController = HandlerSearchBase.SearchControllerFactory.GetSearchController(Guid.Parse(parser.Query.ResultSetId));
                    return navigateController.NavigatePages(parser.Query.PageNumber, parser.Query.Offset);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Invalid result set id " + parser.Query.ResultSetId + ".  " + ex.ToString(), "GetDataSet");
                    messages.AddError("ResultSetId", "Expired result set");
                    return new DataSetResult();
                }
            }

            try
            {
                var getController = HandlerSearchBase.SearchControllerFactory.GetSearchController(searchParameters);
                return getController.Get(allowableMediaIds);
            }
            catch (Exception ex)
            {
                var message = "An error occurred while retrieving data";
                Logger.LogError(ex.GetBaseException(), message);
                messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "Parameter", message));
                return new DataSetResult();
            }
        }

        public static FormatType GetFormatType(string format)
        {
            FormatType type;
            bool isValid = Enum.TryParse(format, true, out type);

            if (isValid)
            {
                return type;
            }
            else
            {
                return FormatType.json;
            }
        }

        public static string[] GetFormatTypes()
        {
            return Enum.GetNames(typeof(FormatType));
        }

        internal static Guid CreateGuidFromString(string str)
        {
            Guid theGuid;
            Guid.TryParse(str, out theGuid);

            return theGuid;
        }
    }


}
