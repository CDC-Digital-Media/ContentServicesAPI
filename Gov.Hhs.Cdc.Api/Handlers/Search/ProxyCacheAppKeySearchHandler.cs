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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataProvider;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class ProxyCacheAppKeySearchHandler : HandlerSearchBase, ISearchHandler
    {
        public ProxyCacheAppKeySearchHandler(ICallParser param)
            : base(param)
        { }

        public override SearchParameters BuildSearchParameters()
        {
            Sorting sorting = GetSortColumns(Parser.SortColumns, ApiParam.proxycacheappkeyid.ToString().Direction(SortOrderType.Desc));
            SearchParam = new SearchParameters("Data", "ProxyCacheAppKey", Parser.Query.GetPaging(), Parser.SecondsToLive, sorting, GetFilterCriteria().TheFilterCriteria);
            return SearchParam;
        }

        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {

            ServiceUtility.GetDataSet(Parser, searchParam, Response);

            Response.results = Response.dataset.Records.Cast<ProxyCacheAppKeyObject>().Select(a => new SerialProxyCacheAppKey()
            {
                proxyCacheAppKeyId = a.ProxyCacheAppKeyId,
                description = a.Description,
                active = a.IsActive.ToString()
            }).ToList();

            return Response;
        }

        private ProxyCacheAppKeyFilterCriteria GetFilterCriteria()
        {
            //TODO: What's being done w/ messages???
            ValidationMessages messages = new ValidationMessages();

            ProxyCacheAppKeyFilterCriteria criteria = new ProxyCacheAppKeyFilterCriteria();

            criteria.IsActive = Parser.GetBoolParm(messages, ApiParam.isactive);
            criteria.DescriptionContains = Parser.GetStringParm(ApiParam.descriptioncontains);

            return criteria;
        }

    }
}
