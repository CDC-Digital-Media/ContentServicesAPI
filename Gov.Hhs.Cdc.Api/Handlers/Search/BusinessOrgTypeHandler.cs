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
    public sealed class BusinessOrgTypeHandler : HandlerSearchBase, ISearchHandler
    {
        public BusinessOrgTypeHandler(ICallParser parser)
            : base(parser)
        {
        }

        public Criterion[] GetFilterCriteria()
        {
            List<Criterion> criteria = new List<Criterion>()
            {
                new Criterion("Active", true)
            };
            return criteria.Where(c => c != null).ToArray();
        }

        public override SearchParameters BuildSearchParameters()
        {
            SearchParam = new SearchParameters("Media", "BusinessUnitType", Parser.Query.GetPaging(),
                GetSorting(Parser, "DisplayOrdinal".Direction(SortOrderType.Asc)), GetFilterCriteria());
            return SearchParam;
        }

        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {
            ServiceUtility.GetDataSet(Parser, searchParam, Response);
            Response.results = Response.dataset.Records.Cast<BusinessUnitTypeItem>().Select(a => new SerialBusinessOrgTypeItem()
            {
                typeCode = a.TypeCode,
                description = a.Description,
                displayOrdinal = a.DisplayOrdinal,
                active = a.IsActive
            }).ToList();

            return Response;
        }
    }
}
