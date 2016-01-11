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
using System.Text.RegularExpressions;
using System.Web;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.CsCaching;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.Api
{
    public abstract class ParserBase : ICallParser
    {
        #region "Public Properties"

        public Criteria Criteria { get; set; }
        public CsBusinessObjects.Media.SearchCriteria Criteria2 { get; set; }
        public bool CanUseXmlSearch { get; set; }
        public IRequestOptions ReqOptions { get; set; }

        public string ContentNamespace { get; set; }
        public IDictionary<string, string> ParamDictionary { get; set; }
        public List<SortColumn> SortColumns { get; set; }

        public QueryParams Query { get; set; }
        public bool IsPublicFacing { get; set; }

        public int Version { get; set; }
        public int SecondsToLive { get { return 0; } }    // Query.PageSize > 0 ? ServiceUtility.GetPublicApiCacheTTL() : 0; } }

        #endregion

        #region "Protected Properties"

        protected string _servicePath { get; set; }
        protected string _queryString { get; set; }
        protected string _protocol { get; set; }

        #endregion

        public string QueryString { get { return _queryString; } }

        #region "Constructor"

        public ParserBase(HttpContext context)
        {
            this._servicePath = context.Request.Path.Trim('/').ToLower();
            this._queryString = context.Request.QueryString.ToString();
            this._protocol = context.Request.Url.Scheme;

            InitParser();

            SetConnectionStringName();
            SetCacheKey();
        }

        #endregion

        #region "Methods"

        private void InitParser()
        {
            this.Query = new QueryParams();
            this.SortColumns = new List<SortColumn>();
            this.Criteria = new Criteria();
            this.Criteria2 = new CsBusinessObjects.Media.SearchCriteria();
            this.ReqOptions = new RequestOptions();

            CreateApiDictionary();
        }

        public virtual void Parse()
        {
            if (ParamDictionary.Count > 0)
            {
                int param_int = 0;
                CanUseXmlSearch = true;

                for (int i = 0; i < ParamDictionary.Count(); i++)
                {
                    string entry = string.Empty, value = string.Empty;
                    entry = ParamDictionary.ElementAt(i).Key.ToLower();
                    value = ParamDictionary.ElementAt(i).Value;

                    switch (entry)
                    {
                        case Param.FullSearch:
                            if (value.IsUrl())
                            {
                                Query.Url = value.SafeUrlContainsSearch();
                                Criteria.Add("SourceUrlContains", Query.Url);
                                Criteria2.SourceUrl = Query.Url;
                            }
                            else
                            {
                                Criteria.Add("FullSearch", value);
                                Criteria2.FullTextSearch = value;
                            }

                            CanUseXmlSearch = true;
                            break;
                        case Param.URL_PARAM:
                            Query.Url = value.Trim('/');
                            Criteria.Add("SourceUrl", Query.Url);
                            Criteria2.SourceUrlExact = Query.Url;
                            CanUseXmlSearch = true;
                            break;
                        case "sourceurl":
                            Criteria.Add("SourceUrl", value);
                            Criteria2.SourceUrlExact = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.URL_PARAM_CONTAINS:
                            Query.Url = value.SafeUrlContainsSearch();
                            Criteria.Add("SourceUrlContains", Query.Url);
                            Criteria2.SourceUrl = Query.Url;
                            CanUseXmlSearch = true;
                            break;
                        case "sourceurlcontains":
                            Query.Url = value.SafeUrlContainsSearch();
                            Criteria.Add("SourceUrlContains", Query.Url);
                            Criteria2.SourceUrl = Query.Url;
                            CanUseXmlSearch = true;
                            break;
                        case "language":
                            Criteria2.Language = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.PAGE_NUMBER:
                            param_int = 1;
                            int.TryParse(value, out param_int);

                            param_int = param_int < 1 ? Param.DEFAULT_PAGE_NUMBER : param_int;
                            Query.PageNumber = param_int;

                            ParamDictionary[Param.PAGE_NUMBER] = param_int.ToString();
                            Criteria2.PageNumber = param_int.ToString();
                            break;
                        case Param.PAGE_OFFSET:
                            if (!ParamDictionary.ContainsKey(Param.PAGE_NUMBER))
                            {
                                param_int = 0;
                                int.TryParse(value, out param_int);

                                param_int = param_int < 0 ? Param.DEFAULT_PAGE_OFFSET : param_int;
                                Query.Offset = param_int;

                                ParamDictionary[Param.PAGE_OFFSET] = param_int.ToString();
                                Criteria2.PageOffset = param_int.ToString();
                            }
                            break;
                        case Param.PAGE_RECORD_MAX:
                            param_int = -1;
                            int.TryParse(value, out param_int);

                            if (param_int < 0)
                            {
                                param_int = Param.DEFAULT_PAGE_RECORD_MAX;
                            }

                            //used to make sure that requests do not use too much memory
                            if (param_int == 0)
                            {
                                param_int = int.MaxValue;
                            }

                            Query.PageSize = param_int;
                            ParamDictionary[Param.PAGE_RECORD_MAX] = param_int.ToString();

                            Criteria2.RowsPerPage = param_int.ToString();
                            break;
                        case Param.FORMAT:      // format set using parameter
                            Query.Format = ServiceUtility.GetFormatType(value);
                            break;
                        case Param.RESULT_SET_ID:
                            Query.ResultSetId = value;
                            break;
                        case Param.SHOW_CHILD:
                            Query.ShowChild = StringToBool(value);
                            Criteria.Add("ShowChild", Query.ShowChild);
                            break;
                        case Param.SHOW_CHILD_LEVEL:
                            param_int = 0;
                            int.TryParse(value, out param_int);

                            param_int = param_int < 0 ? 0 : param_int;
                            Query.ShowChildLevel = param_int;
                            Criteria2.ShowChildLevel = param_int.ToString();
                            CanUseXmlSearch = false;
                            break;
                        case Param.SHOW_PARENT_LEVEL:
                            param_int = 0;
                            int.TryParse(value, out param_int);

                            param_int = param_int < 0 ? 0 : param_int;
                            Query.ShowParentLevel = param_int;
                            Criteria2.ShowParentLevel = param_int.ToString();
                            break;
                        case Param.MEDIA_TYPE_V2:
                        case Param.MEDIA_TYPE:
                            string[] strMT = value.Split(',');
                            Query.MediaType = strMT.ToList<string>();
                            Criteria.Add("MediaType", strMT.ToList<string>());
                            Criteria2.MediaType = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.CAMPAIGN:
                            string[] strCamp = value.Split(',');
                            Query.Campaign = strCamp.ToList<string>();
                            Criteria.Add("Campaign", strCamp.ToList<string>());
                            break;

                        case Param.TOPIC:
                            Criteria.Add("Topic", value);
                            Criteria2.Topic = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.CONTENT_GROUP:
                            Criteria.Add("ContentGroup", value);
                            Criteria2.ContentGroup = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.AUDIENCE:
                            Criteria.Add("Audience", value);
                            Criteria2.Audience = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.TOPIC_ID_V2: //topicids
                        case Param.TOPIC_ID:    //topicd
                            Criteria.AddList("TopicId", value, ',');
                            Criteria2.TopicId = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.TITLE:
                            Criteria.Add("Title", value);
                            Criteria2.Title = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.PERSISTENT_URL:
                            Criteria.AddList("PersistentUrlToken", value, ',');
                            Criteria2.PersistentUrl = value;
                            CanUseXmlSearch = true;
                            break;
                        case Param.CONTENT_NAMESPACE:
                            ReqOptions.ContentNamespace = ValidateNamespace(value);
                            break;
                        case Param.SCALE:
                            double scale = 1;
                            double.TryParse(value, out scale);
                            Query.Scale = scale;
                            break;
                        case Param.HEIGHT:
                            Query.Height = SetIntFromParam(value);
                            break;
                        case Param.WIDTH:
                            Query.Width = SetIntFromParam(value);
                            break;
                        case Param.BROWSER_HEIGHT:
                            Query.BrowserHeight = SetIntFromParam(value);
                            break;
                        case Param.BROWSER_WIDTH:
                            Query.BrowserWidth = SetIntFromParam(value);
                            break;
                        case Param.CROP_H:
                            Query.CropH = SetIntFromParam(value);
                            break;
                        case Param.CROP_W:
                            Query.CropW = SetIntFromParam(value);
                            break;
                        case Param.CROP_X:
                            Query.CropX = SetIntFromParam(value);
                            break;
                        case Param.CROP_Y:
                            Query.CropY = SetIntFromParam(value);
                            break;
                        case Param.PAUSE:
                            Query.Pause = SetIntFromParam(value);
                            break;
                        case Param.APIROOT:
                            Query.ApiRoot = value;
                            break;
                        case Param.WEBROOT:
                            Query.WebRoot = value;
                            break;
                        //additional parameters for media search 2
                        case "status":
                            Criteria2.Status = value;
                            CanUseXmlSearch = true;
                            break;
                        case "fromdatepublished":
                            Criteria2.PublishDateFrom = value;
                            CanUseXmlSearch = true;
                            break;
                        case "todatepublished":
                            Criteria2.PublishDateTo = value;
                            CanUseXmlSearch = true;
                            break;
                        case "todatemodified":
                            Criteria2.ModifiedDateTo = value;
                            CanUseXmlSearch = true;
                            break;
                        case "fromdatemodified":
                            Criteria2.ModifiedDateFrom = value;
                            CanUseXmlSearch = true;
                            break;
                        case "owningorg":
                            Criteria2.OwningOrganization = value;
                            CanUseXmlSearch = true;
                            break;
                        case "maintainingorg":
                            Criteria2.MaintainingOrganization = value;
                            CanUseXmlSearch = true;
                            break;
                        //parameters that were missing
                        case "sourcename":
                            Criteria2.SourceName = value;
                            CanUseXmlSearch = true;
                            break;
                        case "syndicationlistid":
                            CanUseXmlSearch = false;
                            break;
                        case "description":
                            CanUseXmlSearch = true;
                            Criteria2.Description = value;
                            break;
                        case "contentauthoredbeforedate":
                            CanUseXmlSearch = true;
                            Criteria2.ContentAuthoredBeforeDate = value;
                            break;
                        case "adminuserguid":
                            CanUseXmlSearch = true;
                            Criteria2.AdminUserGuid = value;
                            break;
                        case "mediaid":
                            CanUseXmlSearch = true;
                            Criteria2.MediaId = value;
                            break;
                        case "parentid":
                            CanUseXmlSearch = true;
                            Criteria2.ParentId = value;
                            break;
                        case "name":
                            CanUseXmlSearch = true;
                            Criteria2.ExactTitle = value;
                            break;
                        case "namecontains":
                            CanUseXmlSearch = true;
                            Criteria2.Title = value;
                            break;

                        default:
                            break;
                    }
                }

                if (IsPublicFacing)
                {
                    Criteria2.Status = "Published";
                    Criteria2.PublishDateFrom = "1/2/1753"; //SQL Server min date plus a day.  .NET min date is too early
                    Criteria2.PublishDateTo = DateTime.UtcNow.ToString();
                    Criteria2.MediaDisplay = "Yes";
                    Criteria2.SearchedFrom = "PublicApi";
                }
                else
                {
                    Criteria2.SearchedFrom = "AdminApi";
                }

                SetSort();
                ParamDictionary = ParamDictionary.Where(a => a.Value != "").ToDictionary(a => a.Key, a => a.Value, StringComparer.OrdinalIgnoreCase);
            }
        }

        private void SetSort()
        {
            if (ParamDictionary.ContainsKey(Param.SORT))
            {
                if (ParamDictionary[Param.SORT].Contains("popularity"))
                {
                    CanUseXmlSearch = false;
                }
                string[] col = ParamDictionary[Param.SORT].Split(',');
                if (col.Length == 1)
                {
                    if (ParamDictionary.ContainsKey(Param.ORDER))
                    {
                        col[0] = col[0].TrimStart('-');
                        if (string.Equals(ParamDictionary[Param.ORDER], SortOrderType.Desc.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            col[0] = "-" + col[0];
                        }
                    }
                }

                if (CanUseXmlSearch)
                {
                    var sorts = col.Select(c => ParseSortAttribute2(c));
                    Criteria2.Sort = string.Join(",", sorts);
                    SortColumns = ParseSortColumns(col);
                }
                else
                {
                    SortColumns = ParseSortColumns(col);
                }
            }
        }

        private static string ValidateNamespace(string nameSpace)
        {
            // now we need to handle the content namespace parameter
            if (!String.IsNullOrEmpty(nameSpace))
            {
                nameSpace.TrimEnd('_');

                // make sure it is alpha_only
                if (Regex.IsMatch(nameSpace, "^[a-zA-Z]*$"))
                {
                    return Util.StripTrim(nameSpace, 255);
                }
            }
            return "cdc";
        }

        protected static int SetIntFromParam(string value)
        {
            int i = 0;
            int.TryParse(value, out i);
            return i;
        }

        protected static bool StringToBool(string value)
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parse column from QueryString
        /// </summary>
        /// <param name="col"></param>
        protected List<SortColumn> ParseSortColumns(string[] col)
        {
            return ApiColumnMap.MapSortColumns(col.Select(c => ParseSortAttribute(c)));
        }

        private static SortColumn ParseSortAttribute(string source)
        {
            return source.TrimStart('-').Direction(source.StartsWith("-") ? SortOrderType.Desc : SortOrderType.Asc);
        }

        private static string ParseSortAttribute2(string source)
        {
            var column = source.TrimStart('-');
            var direction = source.StartsWith("-") ? "DESC" : "ASC";
            return ApiColumnMap.GetValue(column) + "|" + direction;
        }

        private void CreateApiDictionary()
        {
            if (ParamDictionary == null)
            {
                ParamDictionary = RestHelper.CreateDictionary(_queryString);
            }

            // fix for appscan issue COD7 10/23/2015
            // filter callback for problem characters - it's the one param that we write directly back to the output.
            if(ParamDictionary.ContainsKey(Param.CALLBACK)){
                if (ParamDictionary[Param.CALLBACK].ToString().IndexOf('%') > -1
                    || ParamDictionary[Param.CALLBACK].ToString().ToLower().IndexOf("content-type") > -1
                    ) {
                    ParamDictionary[Param.CALLBACK] = "invalidCharactersInCallback";
                }
            }

            if (!ParamDictionary.ContainsKey(Param.PAGE_RECORD_MAX))
            {
                ParamDictionary.Add(Param.PAGE_RECORD_MAX, Param.DEFAULT_PAGE_RECORD_MAX.ToString());
            }
            else
            {
                int max_page = int.MaxValue;
                if (int.Parse(ParamDictionary[Param.PAGE_RECORD_MAX]) > max_page)
                {
                    ParamDictionary[Param.PAGE_RECORD_MAX] = max_page.ToString();
                }
            }

            if (!ParamDictionary.ContainsKey(Param.PAGE_NUMBER))
            {
                if (!ParamDictionary.ContainsKey(Param.PAGE_OFFSET))
                {
                    ParamDictionary.Add(Param.PAGE_NUMBER, Param.DEFAULT_PAGE_NUMBER.ToString());
                }
            }

            if (!ParamDictionary.ContainsKey(Param.PAGE_OFFSET))
            {
                if (!ParamDictionary.ContainsKey(Param.PAGE_NUMBER))
                {
                    ParamDictionary.Add(Param.PAGE_OFFSET, Param.DEFAULT_PAGE_OFFSET.ToString());
                }
            }

        }

        private void SetConnectionStringName()
        {
            Connection.CurrentConnection.Name = ConfigurationManager.AppSettings["CommonDbConnectionStringName"];

            //if (ParamDictionary.ContainsKey(Param.API_KEY))
            //    this.Query.ApiKey = ParamDictionary[Param.API_KEY].ToLower();

            //if (string.IsNullOrEmpty(this.Query.ApiKey))
            //    this.Query.ApiKey = ConfigurationManager.AppSettings["DefaultApiKey"];

            //var dict = GetApiKeyDictionaryFromCache();
            //if (dict.ContainsKey(this.Query.ApiKey))
            //    Connection.CurrentConnection.Name = dict[this.Query.ApiKey];
            //else
            //    Connection.CurrentConnection.Name = "";
        }

        private void SetCacheKey()
        {
            this.Query.Format = ServiceUtility.GetFormatType(GetFormatFromServicePath());
            this.Query.CacheKey = this._protocol + "|" + this._servicePath.Replace("/", "|");
            this.Query.CacheKey = this.Query.CacheKey.Replace("." + this.Query.Format.ToString(), string.Empty) + "|" + this.Query.Format.ToString();

            // Get the canonical query string
            IDictionary<string, string> copyDict = RemoveDynamicParamsFromDictionary();
            this._queryString = RestHelper.ConstructCanonicalQueryStringFromDictionary(copyDict);
            this.Query.CacheKey += "|?" + this._queryString;
        }

        /// <summary>
        /// Check for resource id
        /// </summary>
        /// <returns></returns>
        private bool IsSingleItemSearch()
        {
            bool isTrue = false;
            string[] segments = this.Query.CacheKey.Split('|');

            if (segments.Length > 6)
            {
                isTrue = ServiceUtility.ParseInt(segments[5]) > 0;
            }

            return isTrue;
        }

        public IDictionary<string, string> RemoveDynamicParamsFromDictionary()
        {
            IDictionary<string, string> copyDict = ParamDictionary.ToDictionary(a => a.Key, a => a.Value);

            if (copyDict.ContainsKey(Param.CALLBACK))
            {
                copyDict.Remove(Param.CALLBACK);
            }

            if (copyDict.ContainsKey(Param.TIME_TO_LIVE))
            {
                copyDict.Remove(Param.TIME_TO_LIVE);
            }

            if (copyDict.ContainsKey(Param.FORMAT))
            {
                copyDict.Remove(Param.FORMAT);
            }

            if (copyDict.ContainsKey(Param.FIELDS))
            {
                copyDict.Remove(Param.FIELDS);
            }

            if (copyDict.ContainsKey(Param.API_KEY))
            {
                copyDict.Remove(Param.API_KEY);
            }

            //TODO: parse explicit params to create cacheKey
            if (copyDict.ContainsKey("_"))
            {
                copyDict.Remove("_");
            }

            if (IsSingleItemSearch())
            {
                return RemovePagingParamsFromDictionary(copyDict);
            }
            else
            {
                return copyDict;
            }
        }

        public IDictionary<string, string> RemoveDynamicParamsFromDictionaryForPaging()
        {
            IDictionary<string, string> copyDict = ParamDictionary.ToDictionary(a => a.Key, a => a.Value);

            if (copyDict.ContainsKey(Param.PAGE_NUMBER))
            {
                if (copyDict.ContainsKey(Param.PAGE_OFFSET))
                {
                    copyDict.Remove(Param.PAGE_OFFSET);
                }
            }

            if (copyDict.ContainsKey(Param.CALLBACK))
            {
                copyDict.Remove(Param.CALLBACK);
            }

            if (copyDict.ContainsKey(Param.TIME_TO_LIVE))
            {
                copyDict.Remove(Param.TIME_TO_LIVE);
            }

            if (copyDict.ContainsKey(Param.FORMAT))
            {
                copyDict.Remove(Param.FORMAT);
            }

            //TODO: parse explicit params to create cacheKey
            if (copyDict.ContainsKey("_"))
            {
                copyDict.Remove("_");
            }

            if (IsSingleItemSearch())
            {
                return RemovePagingParamsFromDictionary(copyDict);
            }
            else
            {
                return copyDict;
            }
        }

        private IDictionary<string, string> RemovePagingParamsFromDictionary(IDictionary<string, string> copyDict)
        {
            if (copyDict.ContainsKey(Param.PAGE_NUMBER))
            {
                copyDict.Remove(Param.PAGE_NUMBER);
            }

            if (copyDict.ContainsKey(Param.PAGE_OFFSET))
            {
                copyDict.Remove(Param.PAGE_OFFSET);
            }

            if (copyDict.ContainsKey(Param.PAGE_RECORD_MAX))
            {
                copyDict.Remove(Param.PAGE_RECORD_MAX);
            }

            return copyDict;
        }

        private string GetFormatFromServicePath()
        {
            int startIndex = this._servicePath.LastIndexOf(".") + 1;

            if (startIndex == 0)
            {
                if (ParamDictionary.ContainsKey(Param.FORMAT))
                {
                    return ParamDictionary[Param.FORMAT];
                }
            }
            return this._servicePath.Substring(startIndex, this._servicePath.Length - startIndex);
        }

        #endregion

        public Criterion GetCriterion(ApiParam uriCode, string criteriaCode, string defaultValue = null)
        {
            string code = uriCode.ToString();
            if (ParamDictionary.ContainsKey(code))
            {
                return new Criterion(criteriaCode, ParamDictionary[code]);
            }
            if (defaultValue != null)
            {
                return new Criterion(criteriaCode, defaultValue);
            }
            else
            {
                return null;
            }
        }

        public Criterion GetListCriterion(ApiParam uriCode, string criteriaCode, string defaultValue = null)
        {
            string code = uriCode.ToString();
            if (ParamDictionary.ContainsKey(code))
            {
                return new Criterion(criteriaCode, ParamDictionary[code].Split(',').ToList());
            }
            if (defaultValue != null)
            {
                return new Criterion(criteriaCode, defaultValue);
            }
            else
            {
                return null;
            }
        }

        public List<string> GetStringListParm(ApiParam uriCode, List<string> defaultValue = null)
        {
            string code = uriCode.ToString();
            if (ParamDictionary.ContainsKey(code))
            {
                return ParamDictionary[code].Split(',').ToList();
            }
            if (defaultValue != null)
            {
                return defaultValue;
            }
            else
            {
                return null;
            }
        }

        public string GetStringParm(ApiParam uriCode, string defaultValue = null)
        {
            string code = uriCode.ToString();
            if (ParamDictionary.ContainsKey(code) && !string.IsNullOrEmpty(ParamDictionary[code]))
            {
                return ParamDictionary[code];
            }
            if (defaultValue != null)
            {
                return defaultValue;
            }
            else
            {
                return null;
            }
        }

        public FilterCriterionDateRange GetDateRangeParm(ApiParam fromUriCode, ApiParam toUriCode)
        {
            string fromCode = fromUriCode.ToString().ToLower();
            string toCode = toUriCode.ToString().ToLower();
            DateTime? fromDate = ParamDictionary.ContainsKey(fromCode) ? SafeGetDateFromDictionary(ParamDictionary, fromCode) : null;
            DateTime? toDate = ParamDictionary.ContainsKey(toCode) ? SafeGetDateFromDictionary(ParamDictionary, toCode) : null;

            return (fromDate == null && toDate == null) ? null : new FilterCriterionDateRange(fromDate, toDate);
        }

        public bool? GetBoolParm(ValidationMessages messages, ApiParam uriCode, bool? defaultValue = null)
        {
            try
            {
                string code = uriCode.ToString().ToLower();
                if (ParamDictionary.ContainsKey(code))
                {
                    return bool.Parse(ParamDictionary[code]);
                }
            }
            catch
            {
                messages.AddError(uriCode.ToString(), "Invalid string value for parameter " + uriCode);
            }
            return defaultValue;
        }

        public int? GetIntParm(ValidationMessages messages, ApiParam uriCode, int? defaultValue = null)
        {
            string code = uriCode.ToString();
            if (ParamDictionary.ContainsKey(code) && !string.IsNullOrEmpty(code))
            {
                return ServiceUtility.ParseInt(ParamDictionary[code]);
            }
            else
            {
                return defaultValue;
            }
        }

        public List<int> GetIntListParm(ValidationMessages messages, ApiParam uriCode, List<int> defaultValue = null)
        {
            try
            {
                string code = uriCode.ToString();
                if (ParamDictionary.ContainsKey(code))
                {
                    return ParamDictionary[code].Split(',').Select(s => int.Parse(s)).ToList();
                }
            }
            catch
            {
                messages.AddError(uriCode.ToString(), "Invalid string value for parameter " + uriCode);
            }
            return defaultValue;
        }

        public Criterion GetIntListCriterion(ApiParam uriCode, List<int> defaultValue = null)
        {
            string code = uriCode.ToString().ToLower();
            List<int> value = (ParamDictionary.ContainsKey(code) && !string.IsNullOrEmpty(code))
                ? ParamDictionary[code].Split(',').Select(s => int.Parse(s)).ToList()
                : defaultValue;
            return value == null ? null : new Criterion(uriCode.ToString(), value);
        }

        public Criterion GetDateRangeCriterion(ApiParam fromUriCode, ApiParam toUriCode, string criteriaCode,
            DateTime? fromDefaultValue = null, DateTime? toDefaultValue = null)
        {
            string fromCode = fromUriCode.ToString().ToLower();
            string toCode = toUriCode.ToString().ToLower();
            if (ParamDictionary.ContainsKey(fromCode) || ParamDictionary.ContainsKey(toCode))
            {
                return new Criterion(criteriaCode, SafeGetDateFromDictionary(ParamDictionary, fromCode), SafeGetDateFromDictionary(ParamDictionary, toCode));
            }
            if (fromDefaultValue != null || toDefaultValue != null)
            {
                return new Criterion(criteriaCode, fromDefaultValue.Value.Date, toDefaultValue.Value.Date);
            }
            else
            {
                return null;
            }
        }

        public Criterion GetDateRangeParamCriterion(ApiParam uriCode, string criteriaCode,
            DateTime? fromDefaultValue = null, DateTime? toDefaultValue = null)
        {
            string[] range = ParamDictionary.ContainsKey(uriCode.ToString()) ? ParamDictionary[uriCode.ToString()].Split(',') : null;

            if (range != null && range.Length == 2)
            {
                return new Criterion(criteriaCode, ParseDate(range[0]), ParseDate(range[1]));
            }

            if (fromDefaultValue != null || toDefaultValue != null)
            {
                return new Criterion(criteriaCode, fromDefaultValue.Value.Date, toDefaultValue.Value.Date);
            }
            else
            {
                return null;
            }
        }

        public Criterion GetDateRangeCriterion(ValidationMessages messages, ApiParam singleUriCode, ApiParam rangeUriCode, ApiParam fromUriCode, ApiParam toUriCode, string criteriaCode)
        {
            string singleCode = singleUriCode.ToString();
            if (ParamDictionary.ContainsKey(singleCode))
            {
                return new Criterion(criteriaCode, SafeGetDateFromDictionary(ParamDictionary, singleCode), SingleDateType.SingleDateOperator.Eq);
            }

            string fromCode = fromUriCode.ToString();
            string toCode = toUriCode.ToString();
            string rangeCode = rangeUriCode.ToString();
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (ParamDictionary.ContainsKey(rangeCode))
            {
                List<DateTime> dates = GetDateListFromDictionary(messages, ParamDictionary, rangeCode);
                if (dates.Count != 2)
                {
                    messages.AddError(rangeCode, "Invalid date range");
                }
                else
                {
                    startDate = dates[0];
                    endDate = dates[1];
                }
            }

            if (ParamDictionary.ContainsKey(fromCode))
            {
                startDate = SafeGetDateFromDictionary(ParamDictionary, fromCode);
            }
            if (ParamDictionary.ContainsKey(toCode))
            {
                endDate = SafeGetDateFromDictionary(ParamDictionary, toCode);
            }

            if (messages.Errors().Any())
            {
                return null;
            }

            if (startDate != null && endDate != null)
            {
                return new Criterion(criteriaCode, startDate, endDate);
            }
            else if (startDate != null)
            {
                return new Criterion(criteriaCode, startDate, SingleDateType.SingleDateOperator.GtEq);
            }
            else if (endDate != null)
            {
                return new Criterion(criteriaCode, endDate, SingleDateType.SingleDateOperator.Lt);
            }

            return null;
        }

        private static List<DateTime> GetDateListFromDictionary(ValidationMessages messages, IDictionary<string, string> dictionary, string key)
        {
            var value = dictionary.ContainsKey(key) ? dictionary[key] : "";
            var dates = value.Split(',').Select(v => ParseDate(v)).ToList();
            if (dates.Where(d => d == null).Any())
            {
                messages.AddError(key, "Invalid date range");
            }

            return dates.Where(d => d != null).Select(d => (DateTime)d).ToList();
        }

        private static DateTime? SafeGetDateFromDictionary(IDictionary<string, string> dictionary, string key)
        {
            return dictionary.ContainsKey(key) ? ParseDate(dictionary[key]) : null;
        }

        public static DateTime? ParseDate(string strDate)
        {
            DateTime result;
            return DateTime.TryParse(strDate, out result) ? result.Date : (DateTime?)null;
        }

        public int? ParseInt(string parmName)
        {
            if (ParamDictionary.ContainsKey(parmName))
            {
                int i = 0;
                int.TryParse(ParamDictionary[parmName], out i);
                if (i > 0)
                {
                    return i;
                }
            }
            return null;
        }

        public int ReplaceIntParmInCriteria(string parmName, string id)
        {
            int idAsInt = 0;
            int.TryParse(id, out idAsInt);
            ReplaceIntParmInCriteria(parmName, idAsInt);
            return idAsInt;
        }

        public void ReplaceIntParmInCriteria(string parmName, int id)
        {
            if (id > 0)
            {
                Criteria.List.Remove(Criteria.GetCriterion(parmName));
                Criteria.Add(parmName, id);
            }
        }

        public void UpdateOrAddParmInDictionaryAsInt(string key, int value)
        {
            if (ParamDictionary.ContainsKey(key))
            {
                ParamDictionary[key] = value.ToString();
            }
            else
            {
                ParamDictionary.Add(key, value.ToString());
            }
        }

        public void UpdateOrAddParmInDictionaryAsString(string key, string value)
        {
            if (ParamDictionary.ContainsKey(key))
            {
                ParamDictionary[key] = value;
            }
            else
            {
                ParamDictionary.Add(key, value);
            }
        }

        public void RemoveParmInDictionaryAsString(string key)
        {
            if (ParamDictionary.ContainsKey(key))
            {
                ParamDictionary.Remove(key);
            }
        }

    }
}
