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

using System.IO;
using System.Web;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class ExportHandler : MediaHandlerBase
    {

        public static void Perform(string id, string action, ICallParser parser, IOutputWriter writer)
        {
            int exportId = ServiceUtility.ParsePositiveInt(id);
            if (exportId > 0 && action.Trim().ToLower() == "performexport")
            {
                ValidationMessages messages = new ValidationMessages();

                // get the feedExport object
                var feedExportObject = CsMediaProvider.GetFeedExport(exportId, out messages);

                if (feedExportObject != null && feedExportObject.MediaId > 0)
                {
                    // setup parser
                    parser.Version = 2;

                    switch (feedExportObject.FeedFormatName.ToLower())
                    {
                        case "json feed export format":
                            parser.Query.Format = FormatType.json;
                            break;
                        case "rss export format":
                            parser.Query.Format = FormatType.rss;
                            break;
                        case "atom export format":
                            parser.Query.Format = FormatType.atom;
                            break;
                    }

                    SerialResponse response = null;
                    var mediaType = new MediaTypeParms(feedExportObject.MediaType);
                    if (mediaType.IsFeedAggregate)
                    {
                        response = GetMedia(parser, feedExportObject.MediaId);
                    }
                    else
                    {
                        response = GetMediaWithChildren(parser, feedExportObject.MediaId);
                    }

                    WriteResponseToOutputAndFile(writer, messages, feedExportObject.FilePath, response);
                }
            }
        }

        private static void WriteResponseToOutputAndFile(IOutputWriter writer, ValidationMessages messages, string filePath, SerialResponse response)
        {
            // write to file
            writer.WriteToFile(response, filePath);

            // write to response output
            writer.Write(response, messages);
        }

        private static SerialResponse GetMedia(ICallParser parser, int id)
        {
            var criteria = new SearchCriteria { ParentId = id.ToString() };

            // get the mediaObject
            var mediaObjects = CsMediaSearchProvider.Search(criteria);

            // create the output to serialize
            var transformation = TransformationFactory.GetMediaTransformation(parser.Version, true);
            var results = transformation.CreateSerialResponse(mediaObjects, true).results;

            SerialResponse response = new SerialResponse();
            response.dataset = null;
            response.results = results;
            response.mediaObjects = mediaObjects;

            var first = mediaObjects.Any() ? mediaObjects.First() : new MediaObject();
            SetPaging(first, response, parser, id);

            //response.meta = ServiceUtility.CreateResponseMetaDataForOneItem(parser, id, true);

            //// write the response to the file
            //if (response.results is List<SerialMediaV2>)
            //{
            //    var item = CsMediaProvider.GetGeo(id);
            //    var resultsMutation = ((List<SerialMediaV2>)response.results);
            //    if (resultsMutation.Count > 0)
            //    {
            //        resultsMutation[0].geoTags = GeoTagTransformation.CreateSeialObjectList(item);
            //    }
            //}

            return response;
        }

        private static SerialResponse GetMediaWithChildren(ICallParser parser, int id)
        {
            // used to get children items
            parser.Query.ShowChildLevel = 1;

            CompositeMedia mediaObject =
                    MediaProvider.GetMediaCollection<MediaObject>(id, new Sorting(parser.SortColumns),
                    parser.Query.ShowChildLevel, parser.Query.ShowParentLevel, onlyIsPublishedHidden: true, OnlyDisplayableMediaTypes: false); //parser.IsPublicFacing);

            var transformation = TransformationFactory.GetRelatedMediaTransformation(parser.Version, true);

            SerialResponse response = new SerialResponse();
            if (mediaObject != null)
            {
                response = transformation.CreateSerialResponse(mediaObject, true);
                response.meta = ServiceUtility.CreateResponseMetaDataForOneItem(parser, id, true);

                mediaObject.TheMediaObject.Children = mediaObject.Children;
                response.mediaObjects = new List<MediaObject>() { mediaObject.TheMediaObject }.AsEnumerable();
            }

            return response;
        }

        private static void SetPaging(MediaObject first, SerialResponse response, ICallParser parser, int parentId)
        {
            response.meta.resultSet.id = Guid.NewGuid().ToString();
            response.meta.pagination = new SerialPagination(parser, new Sorting(parser.SortColumns), first.TotalRows, first.RowsPerPage, first.RowsPerPage, first.PageOffset,
                first.PageNumber, first.TotalPages, 0, parentId, true);
        }

    }
}
