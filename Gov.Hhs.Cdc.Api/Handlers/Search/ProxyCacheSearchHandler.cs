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
    public sealed class ProxyCacheSearchHandler : HandlerSearchBase, ISearchHandler
    {
        public ProxyCacheSearchHandler(ICallParser param)
            : base(param)
        { }

        public override SearchParameters BuildSearchParameters()
        {
            Sorting sorting = GetSortColumns(Parser.SortColumns, ApiParam.id.ToString().Direction(SortOrderType.Desc));
            SearchParam = new SearchParameters("Data", "ProxyCache", Parser.Query.GetPaging(), Parser.SecondsToLive, sorting, GetFilterCriteria().TheFilterCriteria);
            return SearchParam;
        }

        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {

            ServiceUtility.GetDataSet(Parser, searchParam, Response);

            Response.results = Response.dataset.Records.Cast<ProxyCacheObject>().Select(a => new SerialProxyCache()
            {
                id = a.Id.ToString(),
                url = DataUtil.PercentDecodeRfc3986(a.Url),
                datasetId = a.DatasetId,
                data = a.Data,
                expirationInterval = a.ExpirationInterval,
                expirationDateTime = a.ExpirationDateTime.FormatAsUtcString(),
                needsRefresh = a.NeedsRefresh.ToString(),
                status = a.GetDataStatus().ToString(),
                failures = a.Failures.ToString()
            }).ToList();

            return Response;
        }

        private ProxyCacheFilterCriteria GetFilterCriteria()
        {
            //TODO: What's being done w/ messages???
            ValidationMessages messages = new ValidationMessages();

            ProxyCacheFilterCriteria criteria = new ProxyCacheFilterCriteria();

            criteria.Url = Parser.GetStringParm(ApiParam.url);
            criteria.UrlContains = Parser.GetStringParm(ApiParam.urlcontains);
            criteria.DatasetId = Parser.GetStringListParm(ApiParam.datasetid);
            criteria.ExpirationDate = Parser.GetDateRangeParm(ApiParam.expiresafterdate, ApiParam.expiresbeforedate);
            bool? filterIsExpired = Parser.GetBoolParm(messages, ApiParam.isexpired);
            if (filterIsExpired == true)
            {
                criteria.IsExpired = new FilterCriterionDateRange(null, DateTime.UtcNow);
            }
            else if (filterIsExpired == false)
            {
                criteria.IsExpired = new FilterCriterionDateRange(DateTime.UtcNow, null);
            }
            criteria.NeedsRefresh = Parser.GetBoolParm(messages, ApiParam.needsrefresh);

            return criteria;
        }

    }
}
