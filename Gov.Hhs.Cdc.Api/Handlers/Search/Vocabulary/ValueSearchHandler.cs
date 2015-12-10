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

    public sealed class ValueSearchHandler : HandlerSearchBase, ISearchHandler
    {

        public ValueSearchHandler(ICallParser parser)
            : base(parser, null, null)
        {
        }

        public Criterion[] GetFilterCriteria()
        {
            List<Criterion> criteria = new List<Criterion>()
            {
                Parser.GetListCriterion(ApiParam.valuesetid, "ValueSet"),
                Parser.GetListCriterion(ApiParam.valueset, "ValueSetName"),
                Parser.GetCriterion(ApiParam.q, "FullSearch"),
                Parser.GetListCriterion(ApiParam.Language, "Language"),
                Parser.GetCriterion(ApiParam.startswith, "ValueNameStartsWith")
            };

            if (Parser.IsPublicFacing)
            {
                criteria.Add(new Criterion("IsActive", true));
            }

            Criterion[] results = criteria
                .Union(Parser.Criteria.List)        //For now, add the deprecated Criteria until we have moved everything over to the new style
                .Where(c => c != null).ToArray();

            return results;
        }

        public static List<string> OuterSetJoin(IEnumerable<string> leftValues, string separator, IEnumerable<string> rightValues)
        {
            List<string> results = new List<string>();
            foreach (string left in leftValues)
            {
                foreach (string right in rightValues)
                {
                    results.Add(left + separator + right);
                }
            }
            return results;
        }

        public override SearchParameters BuildSearchParameters()
        {
            SearchParam = new SearchParameters("Media", "FlatVocabValue", Parser.GetStringParm(ApiParam.action), Parser.Query.GetPaging(), Parser.SecondsToLive,
                GetSortColumns(Parser.SortColumns, "DisplayOrdinal".Direction(SortOrderType.Asc)), GetFilterCriteria());
            return SearchParam;
        }

        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {
            ServiceUtility.GetDataSet(Parser, searchParam, Response);
            Response.results = Response.dataset.Records.Cast<FlatVocabValueItem>()
                .Select(c => new SerialValueItemAdmin
                {
                    valueId = c.ValueObject.ValueId,
                    valueName = c.ValueObject.ValueName,
                    languageCode = c.ValueObject.LanguageCode,
                    description = c.ValueObject.Description,
                    displayOrdinal = c.ValueObject.DisplayOrdinal,
                    isActive = c.ValueObject.IsActive,
                    relationships = c.Relations.Select(r =>
                        new SerialVocabularyRelation()
                        {
                            valueId = r.ValueId,
                            type = r.TypeName,
                            description = r.Description,
                            relatedValueId = r.RelatedValueId,
                            relatedValueLanguageCode = r.RelatedValueLanguageCode,
                            relatedValueName = r.RelatedValueName
                        }).ToList()
                }).ToList();

            return Response;
        }
    }
}
