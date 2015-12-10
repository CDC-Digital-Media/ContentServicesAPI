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
    public sealed class BusinessOrgHandler : HandlerSearchBase, ISearchHandler
    {
        public BusinessOrgHandler(ICallParser parser)
            : base(parser)
        {
        }

        public Criterion[] GetFilterCriteria()
        {
            List<Criterion> criteria = new List<Criterion>()
            {
                Parser.GetCriterion(ApiParam.source, "SourceCode"),
                new Criterion("Active", true)
            };
            return criteria.Where(c => c != null).ToArray();
        }



        public override SearchParameters BuildSearchParameters()
        {
            SearchParameters searchParms = new SearchParameters("Media", "BusinessUnit", Parser.Query.GetPaging(),
                GetSorting(Parser, "Name".Direction(SortOrderType.Asc)), GetFilterCriteria());


            //Parser.Criteria.Add("ValueSetName", "Topics|English");

            return searchParms;
        }



        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {
            ValidationMessages messages; 
            Response.dataset = ServiceUtility.GetDataSet(Parser, searchParam, out messages);
            if (messages.Errors().Any())
            {
                Response.meta.message = OutputWriter.SerializeValidationMessages(messages);
            }
            if (Response.dataset.Records == null) return Response;
            Response.results = Response.dataset.Records.Cast<BusinessUnitItem>().Select(a => new SerialBusinessOrgItem()
            {
                id = a.Id,
                name = a.Name,
                sourceCode = a.SourceCode,
                active = a.IsActive,
                typeCode = a.TypeCode,
                geoNameId = a.GeoNameId,
                parentId = a.ParentId
            }).ToList();

            return Response;
        }
    }
}
