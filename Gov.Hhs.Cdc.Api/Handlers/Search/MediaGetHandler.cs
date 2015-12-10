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
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class MediaGetHandler : MediaHandlerBase
    {
        public static void Get(IOutputWriter writer, ICallParser parser, string id)
        {
            Get(writer, parser, id, null);
        }

        public static void Get(IOutputWriter writer, ICallParser parser, string id, AdminUser adminUser)
        {
            bool selectingById = !string.IsNullOrEmpty(id);
            ValidationMessages messages = new ValidationMessages();
            SerialResponse response = new SerialResponse();

            // set showChildLevel as default for feed formats

            if (selectingById && parser.Query.IsKindOfFeedFormatType && parser.Query.ShowChildLevel <= 0)
            {
                parser.Query.ShowChildLevel = 1;
            }

            if (selectingById && (parser.Query.ShowChildLevel > 0 || parser.Query.ShowParentLevel > 0))
            {
                CompositeMedia mediaObject =
                    MediaProvider.GetMediaCollection<MediaObject>(ServiceUtility.ParsePositiveInt(id), new Sorting(parser.SortColumns),
                    parser.Query.ShowChildLevel, parser.Query.ShowParentLevel, onlyIsPublishedHidden: parser.IsPublicFacing, OnlyDisplayableMediaTypes: false); //parser.IsPublicFacing);

                var transformation = TransformationFactory.GetRelatedMediaTransformation(parser.Version, parser.IsPublicFacing);

                if (mediaObject != null)
                {
                    response = transformation.CreateSerialResponse(mediaObject);
                    response.meta = ServiceUtility.CreateResponseMetaDataForOneItem(parser);

                    mediaObject.TheMediaObject.Children = mediaObject.Children;
                    response.mediaObjects = new List<MediaObject>() { mediaObject.TheMediaObject }.AsEnumerable();
                }
                writer.Write(response, messages);

            }
            else if (selectingById)  // && !parser.IsPublicFacing)    //Is Admin
            {
                var mediaId = ServiceUtility.ParsePositiveInt(id);
                Logger.LogInfo("Selecting by id: " + id + ", new search: " + parser.CanUseXmlSearch.ToString());
                parser.CanUseXmlSearch = true;
                if (parser.Version == 1)
                {
                    parser.ReplaceIntParmInCriteria("MediaId", mediaId);
                }
                else
                {
                    parser.ReplaceIntParmInCriteria("Id", mediaId);
                }

                // used to exclude or include secondary mediatype i.e. transcript, etc
                parser.Criteria.Add("OnlyDisplayableMediaTypes", false);

                MediaSearchHandler searchHandler = new MediaSearchHandler(parser);
                SearchParameters searchParameters = searchHandler.BuildSearchParameters();
                if (parser.CanUseXmlSearch)
                {
                    parser.Criteria2.MediaId = mediaId.ToString();
                    parser.Criteria2.MediaDisplay = ""; //experiment
                    response = searchHandler.Search();
                }
                else
                {
                    response = searchHandler.BuildResponse(searchParameters);
                }

                var transformation = TransformationFactory.GetMediaTransformation(parser.Version, parser.IsPublicFacing);
                response.results = transformation.CreateSerialResponse(response.mediaObjects.Where(r => r != null)).results;

                if (response.results is List<SerialMediaV2>)
                {
                    var item = CsMediaProvider.GetGeo(mediaId);
                    var results = ((List<SerialMediaV2>)response.results);
                    if (results.Count > 0)
                    {
                        results[0].geoTags = GeoTagTransformation.CreateSeialObjectList(item);
                    }
                }

                ServiceUtility.DeprecatedSetOutputMeta(response.meta, parser, response.dataset, searchParameters);
                writer.Write(response, messages);
            }
            else //Standard Search
            {
                //if (parser.ParamDictionary.Keys.Contains(ApiParm.importfeedurl.ToString()))
                //{
                //    GetMediaFromFeedUrl(writer, parser);
                //    return;
                //}

                var start = DateTime.Now;

                // used to exclude or include secondary mediatype i.e. transcript, etc
                parser.Criteria.Add("OnlyDisplayableMediaTypes", parser.IsPublicFacing);

                MediaSearchHandler searchHandler = new MediaSearchHandler(parser);
                SearchParameters searchParameters = searchHandler.BuildSearchParameters();

                if (adminUser != null)
                {
                    var ids = Authorization.AuthorizationManager.AllowedMediaIds(adminUser);
                    ExecuteSearch(writer, parser, messages, searchHandler, searchParameters, ids);
                }
                else
                {
                    ExecuteSearch(writer, parser, messages, searchHandler, searchParameters);
                }

                var executionTime = DateTime.Now.Subtract(start);
                AuditLogger.LogAuditEvent(string.Format("Media Get took {0} milliseconds", executionTime.TotalMilliseconds));
            }
        }

        //private static void GetMediaFromFeedUrl(IOutputWriter writer, ICallParser parser)
        //{
        //    var mediaItem = FeedImport.CreateMedia(new MediaObject { SourceUrl = parser.ParamDictionary[ApiParm.importfeedurl.ToString()]});

        //    var response = new SerialResponse();
        //    var transformation = TransformationFactory.GetRelatedMediaTransformation(parser.Version, parser.IsPublicFacing);

        //    CompositeMedia theObject = new CompositeMedia()
        //    {
        //        TheMediaObject = mediaItem,
        //        Parents = null,
        //        Children = mediaItem.Children
        //    };

        //    response = transformation.CreateSerialResponse(theObject);
        //    response.meta = ServiceUtility.CreateResponseMetaDataForOneItem(parser);

        //    theObject.TheMediaObject.Children = theObject.Children;
        //    response.mediaObjects = new List<MediaObject>() { theObject.TheMediaObject }.AsEnumerable();

        //    writer.Write(response);
        //}

        private static void ExecuteSearch(IOutputWriter writer, ICallParser parser, ValidationMessages messages, MediaSearchHandler searchHandler, SearchParameters searchParameters)
        {
            ExecuteSearch(writer, parser, messages, searchHandler, searchParameters, null);
        }

        private static void ExecuteSearch(IOutputWriter writer, ICallParser parser, ValidationMessages messages, MediaSearchHandler searchHandler, SearchParameters searchParameters, IList<int> allowableMediaIds)
        {
            SerialResponse response = null;
            if (parser.CanUseXmlSearch)
            {
                AuditLogger.LogAuditEvent("New Search " + parser.Query.CacheKey + " MediaGetHandler.ExecuteSearch()");
                response = searchHandler.Search();
            }
            else
            {
                AuditLogger.LogAuditEvent("Old search " + parser.Query.CacheKey);
                response = searchHandler.BuildResponse(searchParameters, allowableMediaIds);
                ServiceUtility.DeprecatedSetOutputMeta(response.meta, parser, response.dataset, searchParameters);
            }
            writer.Write(response, messages);
        }

        public static ValidationMessages GetMediaForTag(ICallParser parser, IOutputWriter writer, string id)
        {
            ValidationMessages messages = new ValidationMessages();
            parser.UpdateOrAddParmInDictionaryAsInt(ApiParam.TagIds.ToString(), ServiceUtility.ParsePositiveInt(id));
            MediaSearchHandler searchHandler = new MediaSearchHandler(parser);
            SearchParameters searchParameters = searchHandler.BuildSearchParameters();
            parser.RemoveParmInDictionaryAsString(ApiParam.TagIds.ToString());

            ExecuteSearch(writer, parser, messages, searchHandler, searchParameters);
            return messages;
        }

        public static void GetDuplicateHtml(IOutputWriter writer)
        {
            var dupes = CsMediaProvider.Dupes();
            writer.CreateAndWriteSerialResponse(dupes);
        }
    }
}
