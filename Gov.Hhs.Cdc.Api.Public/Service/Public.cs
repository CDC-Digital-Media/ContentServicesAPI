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
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;

using System.ServiceModel;

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.CsCaching;

namespace Gov.Hhs.Cdc.Api.Public
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Public : RestServiceBase
    {
        #region "contructor"

        // constructor
        public Public()
            : base(new ApiParser(HttpContext.Current))
        {
        }

        public Public(int version)
            : base(new ApiParser(HttpContext.Current))
        {
            _parser.Version = version;
        }

        #endregion

        #region "GET"

        public override void ExecuteGet(IOutputWriter writer, string resource, string id, string action)
        {
            if (LogTransactions)
                LogTransaction("GET", "api", resource, id, action);

            switch (resource.ToLower())
            {
                case "syndicationlists":
                    if (!string.IsNullOrEmpty(id))
                    {
                        SyndicationListHandler.Get(id, writer);
                    }
                    break;
                case "media":
                    ExecuteGetActionForMedia(writer, resource, id, action);
                    break;
                case "locations":
                    ExecuteSearch(writer, resource, "ParentId", id);
                    break;
                case "organizations":
                    ExecuteSearch(writer, resource, "Id", id);
                    break;
                case "topics":
                    ExecuteSearch(writer, resource, "Id", id);
                    break;
                case "ecards":
                    var messages = new ValidationMessages();
                    if (string.IsNullOrEmpty(id)) { messages.AddError("Url", "eCard Receipt Id is missing"); }
                    if (string.IsNullOrEmpty(action)) { messages.AddError("Url", "action parameter is missing"); }
                    if (!string.Equals(action, "view", StringComparison.OrdinalIgnoreCase)) { messages.AddError("Url", "invalid action parameter"); }

                    if (messages.Messages.Count == 0)
                    {
                        ECardHandler.ViewECard(writer, id);
                    }
                    else
                    {
                        writer.Write(messages);
                    }

                    break;
                case "tags":
                    if (_parser.Version < 2)
                    {
                        writer.Write(ValidationMessages.CreateError("Tags", "Tags endpoint is only version 2 and later"));
                    }
                    else if (string.IsNullOrEmpty(action))
                    {
                        TagSearchHandler.GetById(_parser, writer, id);
                    }
                    else if (string.Equals(action, "related", StringComparison.OrdinalIgnoreCase))
                    {
                        TagSearchHandler.GetByRelatedId(_parser, writer, id);
                    }
                    else if (string.Equals(action, "media", StringComparison.OrdinalIgnoreCase))
                    {
                        MediaGetHandler.GetMediaForTag(_parser, writer, id);
                    }
                    break;
                case "tagtypes":
                    TagTypeSearchHandler.Get(_parser, writer);
                    break;
                case "atoz":
                    AToZSearchHandler.Get(_parser, writer);
                    break;
                case "links":
                    _parser.ReplaceIntParmInCriteria("StorageId", ServiceUtility.ParsePositiveInt(id));
                    new StorageHandler(_parser).GetContent(writer);
                    break;
                case "languages":
                    _parser.Criteria.Add("OnlyInUseItems", true);
                    ExecuteSearch(writer, resource);
                    break;
                case "mediatypes":
                    _parser.Criteria.Add("IsPublicFacing", _parser.IsPublicFacing);
                    ExecuteSearch(writer, resource);
                    break;
                case "version":
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    writer.Write(fvi.ProductVersion);
                    break;
                case "data":
                    DataHandler.ProxyRequest(id, _parser, writer, HttpContext.Current);
                    break;
                case "cache":
                    var keys = CacheManager.CachedKeys();
                    writer.CreateAndWriteSerialResponse(keys);
                    break;
                case "clearcache":
                    CacheManager.ClearAll();
                    writer.CreateAndWriteSerialResponse("Cache has been cleared.");
                    break;
                default:
                    ExecuteSearch(writer, resource);
                    break;
            };
        }

        protected void ExecuteSearch(IOutputWriter writer, string resource, string idParamName, string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int parentId = _parser.ReplaceIntParmInCriteria(idParamName, id);
                if (parentId > 0)
                {
                    ExecuteSearch(writer, resource);
                }
            }
            else
            {
                ExecuteSearch(writer, resource);
            }
        }

        private void ExecuteSearch(IOutputWriter writer, string resource)
        {
            ISearchHandler searchHandler = CreateSearchHandler(resource, _parser);
            if (searchHandler != null)
            {
                SearchParameters = searchHandler.BuildSearchParameters();
                Response = searchHandler.BuildResponse(SearchParameters);

                ServiceUtility.DeprecatedSetOutputMeta(Response.meta, _parser, Response.dataset, SearchParameters);
                writer.Write(Response);
                return;
            }

            //Handle the 1 offs 
            ExecuteCustomSearch(writer, resource);
        }

        private void ExecuteCustomSearch(IOutputWriter writer, string resource)
        {
            switch (resource.ToLower())
            {
                case "topics":
                    SearchForHierarchicalValues(writer, "Topics");
                    return;
                case "audiences":
                    SearchForHierarchicalValues(writer, "Audiences");
                    return;
                default:
                    throw new InvalidOperationException("Invalid API Request");
            }
        }

        private void SearchForHierarchicalValues(IOutputWriter writer, string valueSetName)
        {
            var searchHandler = new HierarchicalValueSearchHandler(_parser, new SearchParameters(), new SerialResponse());
            SearchParameters = searchHandler.BuildSearchParameters(valueSetName);

            if (_parser.Query.ShowChild)
            {
                Response = searchHandler.BuildResponse(SearchParameters, Param.SearchType.VocabValueItem);
            }
            else
            {
                Response = searchHandler.BuildResponse(SearchParameters, Param.SearchType.VocabValue);
            }

            ServiceUtility.DeprecatedSetOutputMeta(Response.meta, _parser, Response.dataset, SearchParameters);
            writer.Write(Response);
        }

        private void ExecuteGetActionForMedia(IOutputWriter writer, string resource, string id, string action)
        {
            switch ((action ?? "").ToLower())
            {
                case "":
                    MediaGetHandler.Get(writer, _parser, id);
                    return;
                case "content":
                    SyndicationHandler.GetContent(writer, _parser, id);
                    return;
                case "syndicate":
                    AuditLogger.LogAuditEvent(_parser.Query.CacheKey);
                    SyndicationHandler.GetSyndication(writer, _parser, id);
                    return;
                case "thumbnail":
                    ThumbnailHandler.GetThumbnail(id, _parser, writer);
                    return;
                case "embed":
                    MediaEmbedSearchHandler mediaEmbedService = new MediaEmbedSearchHandler(_parser, id);
                    SearchParameters = mediaEmbedService.BuildSearchParameters();
                    Response = mediaEmbedService.BuildResponse(SearchParameters);
                    ServiceUtility.DeprecatedSetOutputMeta(Response.meta, _parser, Response.dataset, SearchParameters);
                    writer.Write(Response);
                    return;
                //case "noscript":
                //    //redirect to actual content
                //    MediaGetHandler.RedirectToMediaContent(writer, _parser, id);
                //    return;
                default:
                    break;
            }
        }

        private ISearchHandler CreateSearchHandler(string service, ICallParser parser)
        {
            switch (service.ToLower())
            {
                //Media Search is handled separately
                //case "media":
                //    return new MediaSearchHandler(parser);
                case "values":
                    return new ValueSearchHandler(parser);
                case "languages":
                    return new LanguageSearchHandler(parser);
                case "organizations":
                    return new UserOrgHandler(parser);
                case "organizationtypes":
                    return new UserOrgTypeHandler(parser);
                case "sources":
                    return new SourceHandler(parser);
                case "businessorgs":
                    return new BusinessOrgHandler(parser);
                case "businessorgtypes":
                    return new BusinessOrgTypeHandler(parser);
                case "locations":
                    return new LocationSearchHandler(parser);
                case "mediatypes":
                    return new MediaTypeSearchHandler(parser);
                case "mediastatus":
                    return new MediaStatusHandler(parser);
                default:
                    return null;
            }
        }

        //public void AuthKeyGenSlashGet()
        //{
        //    AuthKeyGenGet(Writer);
        //}

        //public void AuthKeyGenGet(IOutputWriter writer)
        //{
        //    try
        //    {
        //        if (HttpContext.Current.Request.Url.Scheme.ToLower() == "http")
        //            throw new Exception("A secure connection is required.");

        //        _kh = new Author().GenerateKeyPair();

        //        writer.Write(_kh);
        //    }
        //    catch (Exception ex)
        //    {
        //        ProcessError(ex);
        //    }
        //}

        #endregion

        #region PUT
        public override void Put(IOutputWriter writer, string stream, string appKey, string resource, string id, string action)
        {
            if (LogTransactions)
                LogTransaction("PUT", "api", resource, id, action, stream);

            switch (SafeTrimAndLower(resource))
            {
                case "media":
                    if (string.IsNullOrEmpty(action))
                    {
                        UpdateMedia(writer, id, action);
                    }
                    else if (string.Equals(action, "thumbnail", StringComparison.OrdinalIgnoreCase))
                    {
                        ThumbnailHandler.UpdateThumbnail(id, _parser, writer);
                    }
                    break;
                case "users":
                    if (!string.IsNullOrEmpty(id))
                    {
                        if (string.IsNullOrEmpty(action))
                        {
                            OutputActionResults(writer, RegistrationHandler.Update(new JavaScriptSerializer() { MaxJsonLength = int.MaxValue }.Deserialize<SerialUser>(stream)));
                        }
                        else if (string.Equals(action, "usageagreements", StringComparison.OrdinalIgnoreCase))
                        {
                            OutputActionResults(writer, RegistrationHandler.SetUserAgreement(stream, id));
                        }
                    }
                    break;
                default:
                    writer.Write(ValidationMessages.CreateError("Resource", "Resource is invalid: " + resource));
                    break;
            }
        }

        private void UpdateMedia(IOutputWriter writer, string id, string action)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(action))
            {
                switch (action.ToLower())
                {
                    case "refreshtoken":
                        ServiceUtility.WriteDoNotCacheResponseToHeader(writer);
                        ValidationMessages messages = new ValidationMessages();
                        int resourceId = 0;
                        int.TryParse(id, out resourceId);
                        if (resourceId > 0)
                        {
                            var returnObj = new SerialMediaAdmin() { id = resourceId.ToString() };
                            messages = MediaMgrHandler.RegeneratePersistentUrl(returnObj);
                            Response.results = returnObj;
                            writer.Write(Response, messages);
                        }
                        else
                        {
                            writer.Write(ValidationMessages.CreateError("MediaId", "Media Id is invalid"));
                        }
                        break;
                }
            }
        }
        #endregion

        #region "POST"

        public override void Post(IOutputWriter writer, string stream, string appKey, string resource, string id, string action)
        {
            if (LogTransactions)
            {
                LogTransaction("POST", "api", resource, id, action, stream);
            }

            ServiceUtility.WriteDoNotCacheResponseToHeader(writer);
            switch (SafeTrimAndLower(resource))
            {
                case "syndicationlists":
                    if (string.IsNullOrEmpty(id))
                    {
                        SyndicationListHandler.Create(stream, writer);
                    }
                    else if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(action) && string.Equals(action, "media", StringComparison.OrdinalIgnoreCase))
                    {
                        SyndicationListHandler.AddMediaItemsToSyndicationList(stream, id, writer);
                    }
                    else
                    {
                        writer.Write(ValidationMessages.CreateError("Url", "Invalid syndicationlists call"));
                    }
                    break;
                case "ecards":
                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(action) && string.Equals(action, "send", StringComparison.OrdinalIgnoreCase))
                    {
                        ECardHandler.SubmitCard(stream, id, writer, GetUserAgent());
                    }
                    else
                    {
                        writer.Write(ValidationMessages.CreateError("Url", "Invalid ecard call"));
                    }
                    break;
                case "reset_user_passwords":
                    RegistrationHandler.ResetUserPassword(stream, appKey, writer);
                    break;
                case "update_user_passwords":
                    RegistrationHandler.UpdateUserPassword(stream, appKey, writer);
                    break;
                case "user_email_exists":
                    RegistrationHandler.ValidateUsers(stream, appKey, writer);
                    break;
                default:
                    PostInOldFormat(writer, stream, appKey, resource, id, action);
                    break;
            }
        }

        private void PostInOldFormat(IOutputWriter writer, string stream, string appKey, string resource, string id, string action)
        {
            switch (SafeTrimAndLower(resource))
            {
                case "users":
                    break;  //Let it fall through to the "old code" to be rewritten later
                case "serviceusers":
                    break;  //Let it fall through to the "old code" to be rewritten later
                case "logins":
                    break;  //Let it fall through to the "old code" to be rewritten later
                default:
                    writer.Write(ValidationMessages.CreateError("Resource", "Resource is invalid: " + resource));
                    return;
            }

            OutputActionResults(writer, Post2(writer, stream, appKey, resource, id, action));
        }

        private void OutputActionResults(IOutputWriter writer, ActionResults actionResults)
        {
            Response.results = actionResults.Results;
            WebOperationContext.Current.OutgoingResponse.Headers.Add(actionResults.HeaderValues);
            writer.Write(Response, actionResults.ValidationMessages);
        }

        //Get rid of ActionResults and push the writing down to the individual methods
        private ActionResults Post2(IOutputWriter writer, string stream, string appKey, string resource, string id, string action)
        {
            switch (resource.Trim().ToLower())
            {
                case "users":
                    return RegistrationHandler.CreateUser(stream, appKey);
                case "serviceusers":
                    return RegistrationHandler.CreateServiceUser(stream);
                case "logins":
                    return RegistrationHandler.Login(stream, appKey);
            }
            throw new ApplicationException("Resource was not recognized: " + resource);
        }

        private static string GetUserAgent()
        {
            string userAgent = null;
            try
            {
                userAgent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
                //userAgent = WebOperationContext.Current.IncomingRequest.Headers["User-Agent"];
            }
            catch (ObjectDisposedException)
            {

            }
            return userAgent;
        }

        #endregion

        #region "DELETE"
        public override void Delete(IOutputWriter writer, string stream, string appKey, string resource, string id, string action)
        {
            if (string.IsNullOrEmpty(id))
            {
                writer.Write(ValidationMessages.CreateError("Id", "Id must be passed"));
            }
            else
            {
                switch (SafeTrimAndLower(resource))
                {
                    case "syndicationlists":
                        if (string.IsNullOrEmpty(action))
                        {
                            SyndicationListHandler.Delete(id, writer);
                        }
                        else if (string.Equals(action.ToLower(), "media", StringComparison.OrdinalIgnoreCase))
                        {
                            SyndicationListHandler.DeleteMediaItemsFromSyndicationList(stream, id, writer);
                        }
                        else
                        {
                            writer.Write(ValidationMessages.CreateError("Url", "Invalid syndicationlists call"));
                        }
                        break;
                    case "users":
                        writer.Write(RegistrationHandler.Delete(id));
                        break;
                    default:
                        writer.Write(ValidationMessages.CreateError("Resource", "Resource is invalid: " + resource));
                        break;
                }
            }
        }

        //        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(action))
        //{
        //    switch (action.ToLower())
        //    {
        //        case "refreshtoken":
        //            DoNotCacheResponse();
        //            ValidationMessages messages = new ValidationMessages();
        //            int resourceId = 0;
        //            int.TryParse(id, out resourceId);
        //            if (resourceId > 0)
        //            {
        //                var returnObj = new SerialMediaObj() { id = resourceId };
        //                messages = MediaMgrHandler.RegeneratePersistentUrl(returnObj);
        //                Response.results = returnObj;
        //                writer.Write(Response, messages);
        //            }
        //            else
        //                writer.Write(ValidationMessages.CreateError("MediaId", "Media Id is invalid"));

        //            break;
        //    }
        //}
        #endregion
    }
}
