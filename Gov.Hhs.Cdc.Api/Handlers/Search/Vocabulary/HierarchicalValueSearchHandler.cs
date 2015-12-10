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
using System.Configuration;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class HierarchicalValueSearchHandler : HandlerSearchBase
    {
        public HierarchicalValueSearchHandler(ICallParser param, SearchParameters searchParam, SerialResponse response)
            : base(param, searchParam, response)
        {
        }

        public override SearchParameters BuildSearchParameters()
        {
            throw new NotImplementedException();
        }

        public SearchParameters BuildSearchParameters(string valueSetName)
        {
            // sort columns
            List<SortColumn> columnList = Parser.SortColumns;

            // add default sort if no columns were supplied
            if (columnList.Count == 0)
            {
                columnList.Add(new SortColumn() { Column = "DisplayOrdinal", SortOrder = SortOrderType.Asc });
            }

            //Parser.Criteria.List.Remove(Parser.Criteria.GetCriterion("MediaType"));

            var crit = Parser.GetListCriterion(ApiParam.Language, ApiParam.Language.ToString());
            if (crit != null)
            {
                Parser.Criteria.Add("ValueSetName", crit.Values.Select(a => valueSetName + "|" + a).ToList());
            }
            else
            {
                Parser.Criteria.Add("ValueSetName", new List<string> { valueSetName + "|" + ConfigurationManager.AppSettings["TopicsAudiencesDefaultLanguage"] });
            }


            Parser.Criteria.Add("IsActive", true);

            // build searchParamaters
            SearchParam.ApplicationCode = "Media";
            SearchParam.DataSetCode = "HierVocabValue";
            SearchParam.BasicCriteria = Parser.Criteria;
            SearchParam.Sorting = new Sorting(columnList);

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
            throw new NotImplementedException();
        }

        public SerialResponse BuildResponse(SearchParameters searchParam, Param.SearchType searchType)
        {
            ServiceUtility.GetDataSet(Parser, searchParam, Response);

            switch (searchType)
            {
                case Param.SearchType.VocabValue:
                    Response.results = Response.dataset.Records.Cast<HierVocabValueItem>()
                        .Select(c => new SerialVocab
                        {
                            id = c.ValueKey.Id,
                            name = c.ValueName,
                            description = c.Description,
                            mediaUsageCount = c.DescendantMediaUsageCount,
                            language = c.ValueKey.LanguageCode,
                        }).ToList();
                    break;
                case Param.SearchType.VocabValueItem:
                    Response.results = Response.dataset.Records.Cast<HierVocabValueItem>()
                        .Select(c => GetValueItemRecursively(c)).ToList();
                    break;
            }

            return Response;
        }

    }
}
