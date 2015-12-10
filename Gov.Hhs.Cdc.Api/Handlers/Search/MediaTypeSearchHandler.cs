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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class MediaTypeSearchHandler : HandlerSearchBase, ISearchHandler
    {
        public MediaTypeSearchHandler(ICallParser parser)
            : base(parser)
        {
        }

        public override SearchParameters BuildSearchParameters()
        {
            List<SortColumn> columnList = Parser.SortColumns;

            // build searchParamaters
            SearchParam.ApplicationCode = "Media";
            SearchParam.DataSetCode = "MediaType";
            SearchParam.BasicCriteria = Parser.Criteria;
            SearchParam.Sorting = new Sorting(new List<SortColumn>()                        
            {
                new SortColumn("DisplayOrdinal", SortOrderType.Asc)
            });

            if (Parser.Query.PageSize > 0 && (Parser.Query.PageNumber > 0 || Parser.Query.Offset > 0))
            {
                SearchParam.Paging = new Paging(Parser.Query.PageSize, Parser.Query.PageNumber, Parser.Query.Offset);
            }

            if (columnList.Count > 0)
            {
                SearchParam.Sorting = new Sorting(columnList);
            }

            return SearchParam;
        }

        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {
            ServiceUtility.GetDataSet(Parser, searchParam, Response);
            Response.results = Response.dataset.Records.Cast<MediaTypeItem>()
                .Select(a => CreateSerialMediaType(a)).ToList();

            return Response;
        }

        private static SerialMediaType CreateSerialMediaType(MediaTypeItem mediaType)
        {
            return
                new SerialMediaType()
                {
                    name = mediaType.MediaTypeCode,
                    description = mediaType.Description,
                    displayOrdinal = mediaType.DisplayOrdinal,
                    mediaCount = mediaType.MediaCount
                };
        }
    }
}
