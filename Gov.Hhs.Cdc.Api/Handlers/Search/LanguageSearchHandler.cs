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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;



namespace Gov.Hhs.Cdc.Api
{
    public sealed class LanguageSearchHandler : HandlerSearchBase, ISearchHandler
    {
        public LanguageSearchHandler(ICallParser param)
            : base(param, null, null)
        {
        }

        public override SearchParameters BuildSearchParameters()
        {
            // sort columns
            List<SortColumn> columnList = Parser.SortColumns;

            //Parser.Criteria.Add("OnlyInUseItems", true);

            // build searchParamaters
            SearchParam.ApplicationCode = "Media";
            SearchParam.DataSetCode = "Language";
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

            if (Parser.IsPublicFacing)
            {
                Response.results = TransformationFactory.CreateLanguage(Parser.Version, Response.dataset);
            }
            else
            {
                Response.results = TransformationAdminFactory.CreateLanguage(Parser.Version, Response.dataset);
            }

            return Response;
        }

    }
}
