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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;



namespace Gov.Hhs.Cdc.Api
{
    public sealed class ValueSetHandler : HandlerSearchBase
    {
        public ValueSetHandler(ICallParser parser, SearchParameters searchParam, SerialResponse response)
            : base(parser, searchParam, response)
        {
        }

        public Criterion[] GetFilterCriteria()
        {

            List<Criterion> criteria = new List<Criterion>()
            {
                Parser.GetCriterion(ApiParam.id, "ValueSet")
            };

            Criterion[] results = criteria
                .Union(Parser.Criteria.List)        //For now, add the deprecated Criteria until we have moved everything over to the new style
                .Where(c => c != null).ToArray();
            return results;
        }

        public override SearchParameters BuildSearchParameters()
        {
            // sort columns
            List<SortColumn> columnList = Parser.SortColumns;

            // add default sort if no columns were supplied
            if (columnList.Count == 0)
            {
                columnList.Add(new SortColumn() { Column = "DisplayOrdinal", SortOrder = SortOrderType.Asc });
            }

            // build searchParamaters
            SearchParam.ApplicationCode = "Media";
            //If passed ValueSet, or ValueSetName then 
            SearchParam.DataSetCode = ValueSetSpecifiedAsAFilter() ? "FlatVocabValue" : "ValueSet";
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
                case Param.SearchType.Valueset:
                    Response.results = Response.dataset.Records.Cast<ValueSetObject>()
                        .Select(c => new SerialValueSetItem
                        {
                            id = c.Id,
                            languageCode = c.LanguageCode,
                            //LanguageDescription = c.LanguageDescription,
                            name = c.Name,
                            description = c.Description,
                            attributeId = c.AttributeId,
                            attributeName = c.AttributeName,
                            attributeDescription = c.AttributeDescription,
                            displayOrdinal = c.DisplayOrdinal,
                            isActive = c.IsActive,
                            isDefaultable = c.IsDefaultable,
                            isOrderable = c.IsOrderable,
                            isHierachical = c.IsHierachical
                        }).ToList();
                    break;
                case Param.SearchType.ValuesetRelation:
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
                    break;
            }

            return Response;
        }

        private bool ValueSetSpecifiedAsAFilter()
        {
            return Parser.Criteria.GetCriterion("ValueSet") != null || Parser.Criteria.GetCriterion("ValueSetName") != null;
        }
    }
}
