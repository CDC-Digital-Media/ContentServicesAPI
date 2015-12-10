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
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.DataServices.Bo;

using System.ServiceModel;
using Gov.Hhs.Cdc.CsCaching;
using Gov.Hhs.Cdc.Api.Handlers.Admin;

namespace Gov.Hhs.Cdc.Api.Admin
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Admin : RestServiceBase
    {
        public Admin()
            : base(new ApiParser(HttpContext.Current))
        {
        }

        public Admin(int version)
            : base(new ApiParser(HttpContext.Current))
        {
            _parser.Version = version;
        }

        #region "GET"

        public override void ExecuteGet(IOutputWriter writer, string resource, string id, string action)
        {
            if (LogTransactions)
            {
                LogTransaction("GET", "adminapi", resource, id, action);
            }

            switch (resource.ToLower())
            {
                case "syndicationlists":
                    break;
                case "media":
                    ExecuteGetActionForMedia(writer, resource, id, action);
                    break;
                case "mediadupes":
                    MediaGetHandler.GetDuplicateHtml(writer);
                    break;
                case "valuesets":
                    ExecuteSearch(writer, resource, "ValueSet", id);
                    break;
                case "values":
                    ExecuteSearch(writer, resource, "ValueId", id);
                    break;
                case "validations":
                    HtmlValidationHandler.ValidateHtml(writer, id, _parser);
                    break;
                case "adminusers":
                    AuthorizationHandler.Get(writer, id);
                    break;
                case "roles":
                    AuthorizationHandler.WriteRoles(writer);
                    break;
                case "languages":
                    _parser.Criteria.Add("OnlyInUseItems", false);
                    ExecuteSearch(writer, resource);
                    break;
                case "mediatypes":
                    _parser.Criteria.Add("IsPublicFacing", _parser.IsPublicFacing);
                    ExecuteSearch(writer, resource);
                    break;
                case "links":
                    _parser.ReplaceIntParmInCriteria("StorageId", ServiceUtility.ParsePositiveInt(id));
                    new StorageHandler(_parser).GetContent(writer);
                    break;
                case "schedules":
                    ScheduleHandler.Get(writer, id, action, _parser);
                    break;
                case "data":
                    ExecuteGetActionForData(writer, _parser, resource, id, action);
                    break;
                case "dataappkey":
                    ExecuteGetActionForDataAppKey(writer, _parser, resource, id, action);
                    break;
                case "cache":
                    var keys = CacheManager.CachedKeys();
                    writer.CreateAndWriteSerialResponse(keys);
                    break;
                case "clearcache":
                    CacheManager.ClearAll();
                    writer.CreateAndWriteSerialResponse("Cache has been cleared.");
                    break;
                case "stats":
                    var stats = StatsHandler.Stats();
                    writer.CreateAndWriteSerialResponse(stats);
                    break;
                case "exports":
                    ExportHandler.Perform(id, action, _parser, writer);
                    break;
                case "logs":
                    var log = LogHandler.Log();
                    writer.CreateAndWriteSerialResponse(log);
                    break;
                default:
                    ExecuteSearch(writer, resource);
                    break;
            }
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
                ExecuteSearch(writer, searchHandler);
            }
            else
            {
                ExecuteCustomSearch(writer, resource);
            }
        }

        private void ExecuteSearch(IOutputWriter writer, ISearchHandler searchHandler)
        {
            SearchParameters = searchHandler.BuildSearchParameters();
            Response = searchHandler.BuildResponse(SearchParameters);
            ServiceUtility.DeprecatedSetOutputMeta(Response.meta, _parser, Response.dataset, SearchParameters);

            writer.Write(Response);
        }

        private void ExecuteCustomSearch(IOutputWriter writer, string service)
        {
            switch (service.ToLower())
            {
                case "valuesets":
                    //This is in use by:  /adminapi/v1/resources/valuesets
                    ValueSetHandler valueSetService = new ValueSetHandler(_parser, new SearchParameters(), new SerialResponse());
                    SearchParameters = valueSetService.BuildSearchParameters();

                    if (_parser.Criteria.GetCriterion("ValueSet") == null)
                    {
                        Response = valueSetService.BuildResponse(SearchParameters, Param.SearchType.Valueset);
                    }
                    else
                    {
                        Response = valueSetService.BuildResponse(SearchParameters, Param.SearchType.ValuesetRelation);
                    }
                    ServiceUtility.DeprecatedSetOutputMeta(Response.meta, _parser, Response.dataset, SearchParameters);

                    writer.Write(Response);
                    return;
                default:
                    throw new NotImplementedException("Invalid API Request");
            }
        }

        private void ExecuteGetActionForMedia(IOutputWriter writer, string resource, string id, string action)
        {
            switch ((action ?? "").ToLower())
            {
                case "":
                    ValidationMessages messages = new ValidationMessages();

                    //TODO:  Turn authorization back on for GETs once complete
                    AdminUser adminUser = null;
                    //var adminUser = AuthorizationHandler.Authorize(ref messages, HttpContext.Current);
                    //if (messages.Errors().Any())
                    //{
                    //    writer.Write(messages);
                    //    return;
                    //}
                    //else
                    //{
                    //    if (adminUser == null)
                    //    {
                    //        writer.Write(ValidationMessages.CreateError("auth", "User not authorized for the admin system."));
                    //        return;
                    //    }
                    //}

                    MediaGetHandler.Get(writer, _parser, id, adminUser);
                    break;
                case "embed":
                    MediaEmbedSearchHandler mediaEmbedService = new MediaEmbedSearchHandler(_parser, id);
                    SearchParameters = mediaEmbedService.BuildSearchParameters();
                    Response = mediaEmbedService.BuildResponse(SearchParameters);
                    ServiceUtility.DeprecatedSetOutputMeta(Response.meta, _parser, Response.dataset, SearchParameters);

                    //*ServiceUtility.CacheResponse(Response, _parser.Query);
                    writer.Write(Response);

                    break;
                case "thumbnail":
                    ThumbnailHandler.GetThumbnail(id, _parser, writer);
                    break;
                default:
                    writer.Write(ValidationMessages.CreateError("Action", "Invalid Action"));
                    break;
                    throw new InvalidOperationException("Invalid action");
            }
        }

        private ISearchHandler CreateSearchHandler(string service, ICallParser parser)
        {
            switch (service.ToLower())
            {
                //This is handled in the MediaGetAction
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
                case "data":
                    return new ProxyCacheSearchHandler(parser);
                case "dataappkey":
                    return new ProxyCacheAppKeySearchHandler(parser);
                case "feedformats":
                    return new FeedFormatSearchHandler(parser);
                default:
                    return null;
            }
        }

        private void ExecuteGetActionForData(IOutputWriter writer, ICallParser parser, string resource, string id, string action)
        {
            if (string.IsNullOrEmpty(id))
            {
                ExecuteSearch(writer, resource);
            }
            else
            {
                DataHandler.GetProxyData(id, parser, writer);
            }
        }

        private void ExecuteGetActionForDataAppKey(IOutputWriter writer, ICallParser parser, string resource, string id, string action)
        {
            if (string.IsNullOrEmpty(id))
            {
                ExecuteSearch(writer, resource);
            }
            else
            {
                DataHandler.GetProxyCacheAppKey(id, parser, writer);
            }
        }

        #endregion

        #region "POST"

        public override void Post(IOutputWriter writer, string stream, string appKey, string resource, string id, string action)
        {
            if (LogTransactions)
            {
                LogTransaction("POST", "adminapi", resource, id, action, stream);
            }

            ValidationMessages messages = new ValidationMessages();

            var adminUser = AuthorizationHandler.Authorize(ref messages, HttpContext.Current);
            if (messages.Errors().Any())
            {
                writer.Write(messages);
                return;
            }
            else
            {
                if (adminUser == null)
                {
                    writer.Write(ValidationMessages.CreateError("auth", "User not authorized for the admin system."));
                    return;
                }
            }

            ServiceUtility.WriteDoNotCacheResponseToHeader(writer);
            switch (SafeTrimAndLower(resource))
            {
                case "thumbnail":
                    ThumbnailHandler.UpdateThumbnail(id, _parser, writer);
                    break;

                case "media":
                    if (!string.IsNullOrEmpty(id))
                        messages.AddError("id", "Url id must be blank for a media post");

                    if (!string.IsNullOrEmpty(action))
                        messages.AddError("action", "Url action must be blank for a media post");

                    if (messages.Errors().Any())
                    {
                        writer.Write(messages);
                    }
                    else
                    {
                        MediaMgrHandler.Insert(writer, stream, appKey, adminUser, _parser);
                    }
                    break;
                case "links":
                    StorageMgrHandler.Insert(writer, stream, appKey, adminUser);
                    break;
                case "adminusers":
                    AuthorizationHandler.UpdateUser(writer, stream, adminUser);
                    break;
                case "dataadmin":
                    DataHandler.InsertProxyData(stream, appKey, writer);
                    break;
                case "schedules":
                    //insert ScheduleHandler(IOutputWriter writer, string data, string params);
                    //end with: writer.Write(Response); SerialResponse  
                    break;
                case "validations":
                    HtmlValidationHandler.ValidateResources(_parser, writer, stream);
                    break;
                case "values":
                    messages.Add(ValueMgrHandler.UpdateValue(0, stream, adminUser, out id));
                    if (messages.Errors().Any())
                    {
                        writer.Write(messages);
                    }
                    else
                    {
                        ExecuteSearch(writer, resource, "ValueId", id);
                    }
                    break;
                case "data":
                    DataHandler.InsertProxyData(stream, appKey, writer);
                    break;
                case "dataappkey":
                    DataHandler.InsertProxyCacheAppKey(stream, appKey, writer);
                    break;
                default:
                    //These are the old format of the call where we return the validation messages
                    messages.Add(PostInOldFormat(resource, stream, appKey, adminUser));
                    writer.Write(Response, messages);
                    break;
            }

        }

        private ValidationMessages PostInOldFormat(string resource, string stream, string appKey, AdminUser adminUser)
        {
            switch (SafeTrimAndLower(resource))
            {
                case "valuesets":
                    return ValueMgrHandler.InsertValueSet(stream, adminUser);
                default:
                    return ValidationMessages.CreateError("Dataset", "Dataset is invalid: " + resource);
            }
        }


        #endregion

        #region "PUT"

        public override void Put(IOutputWriter writer, string stream, string appKey, string resource, string id, string action)
        {
            if (LogTransactions)
            {
                LogTransaction("PUT", "adminapi", resource, id, action, stream);
            }
            ValidationMessages messages = new ValidationMessages();
            var auth = AuthorizationHandler.Authorize(ref messages, HttpContext.Current);
            if (messages.Errors().Any())
            {
                writer.Write(messages);
                return;
            }
            else
            {
                if (auth == null)
                {
                    writer.Write(ValidationMessages.CreateError("", "User not authorized for the admin system."));
                    return;
                }
            }

            switch (SafeTrimAndLower(resource))
            {
                case "media":
                    if (string.IsNullOrEmpty(action) || string.Equals(action, "update", StringComparison.OrdinalIgnoreCase))
                    {
                        MediaMgrHandler.Update(writer, stream, appKey, GetIdAsInt(id), auth);
                    }
                    else if (string.Equals(action, "thumbnail", StringComparison.OrdinalIgnoreCase))
                    {
                        ThumbnailHandler.UpdateThumbnail(id, _parser, writer);
                    }
                    else if (string.Equals(action, "importfeeditems", StringComparison.OrdinalIgnoreCase))
                    {
                        FeedImportMgrHandler.Update(writer, stream, appKey, GetIdAsInt(id), auth, _parser);
                    }
                    break;
                case "syndicationlists":
                    SyndicationListHandler.Update(stream, writer);
                    break;
                case "valuesets":
                    ValueMgrHandler.UpdateValueSet(stream, GetIdAsInt(id), writer, auth);
                    break;
                case "values":
                    if (!string.IsNullOrEmpty(action) && string.Equals(action, "addRelationships", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(ValueMgrHandler.AddRelationship(GetIdAsInt(id), stream, writer, auth));
                    }
                    else if (!string.IsNullOrEmpty(action)
                        && string.Equals(action, "deleteRelationships", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(ValueMgrHandler.DeleteRelationship(GetIdAsInt(id), stream, writer, auth));
                    }
                    else
                    {
                        messages.Add(ValueMgrHandler.UpdateValue(GetIdAsInt(id), stream, writer, auth));
                    }
                    if (messages.Errors().Any())
                    {
                        writer.Write(messages);
                    }
                    else
                    {
                        ExecuteSearch(writer, resource, "ValueId", id);
                    }
                    break;
                case "schedules":
                    //insert ScheduleHandler(IOutputWriter writer, string data, string params);
                    //end with: writer.Write(Response); SerialResponse 
                    break;
                case "data":
                    DataHandler.UpdateProxyData(id, stream, appKey, writer);
                    break;
                case "dataappkey":
                    DataHandler.UpdateProxyCacheAppKey(id, stream, appKey, writer);
                    break;
                default:
                    writer.Write(ValidationMessages.CreateError("Resource", "Resource is invalid: " + resource));
                    break;
            }
        }

        #endregion

        #region "DELETE"

        public override void Delete(IOutputWriter writer, string stream, string appKey, string resource, string id, string action)
        {
            ValidationMessages messages = new ValidationMessages();
            var auth = AuthorizationHandler.Authorize(ref messages, HttpContext.Current);
            if (messages.Errors().Any())
            {
                writer.Write(messages);
                return;
            }
            else
            {
                if (auth == null)
                {
                    writer.Write(ValidationMessages.CreateError("", "User not authorized for the admin system."));
                    return;
                }
            }

            if (string.IsNullOrEmpty(id))
            {
                writer.Write(ValidationMessages.CreateError("Id", "Id must be passed"));
            }
            else
            {
                switch (SafeTrimAndLower(resource))
                {
                    case "media":
                        if (string.IsNullOrEmpty(action))
                        {
                            writer.Write(MediaMgrHandler.Delete(GetIdAsInt(id)));
                        }
                        else if (string.Equals(action, "thumbnail", StringComparison.OrdinalIgnoreCase))
                        {
                            ThumbnailHandler.Delete(id, _parser, writer);
                        }
                        break;
                    case "syndicationlists":
                        SyndicationListHandler.Delete(id, writer);
                        break;
                    case "valuesets":
                        writer.Write(ValueMgrHandler.DeleteValueSet(GetIdAsInt(id), auth));
                        break;
                    case "values":
                        writer.Write(ValueMgrHandler.DeleteValue(GetIdAsInt(id), auth));
                        break;
                    case "data":
                        DataHandler.DeleteProxyData(id, appKey, writer);
                        break;
                    case "dataappkey":
                        DataHandler.DeleteProxyCacheAppKey(id, appKey, writer);
                        break;
                    case "links":
                        writer.Write(StorageHandler.Delete(GetIdAsInt(id), appKey));
                        break;
                    default:
                        writer.Write(ValidationMessages.CreateError("Resource", "Resource is invalid: " + resource));
                        break;
                }
            }
        }


        #endregion

    }
}
