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
using Gov.Hhs.Cdc.Search.Controller;

namespace Gov.Hhs.Cdc.Api
{
    public abstract class HandlerSearchBase
    {
        protected readonly ICallParser Parser;
        public static ISearchControllerFactory SearchControllerFactory;
        protected static IMediaProvider MediaProvider { get; set; }
        protected SearchParameters SearchParam { get; set; }
        protected SerialResponse Response { get; set; }

        public static void Inject(IMediaProvider mediaProvider, ISearchControllerFactory searchControllerFactory)
        {
            MediaProvider = mediaProvider;
            SearchControllerFactory = searchControllerFactory;
        }

        public HandlerSearchBase(ICallParser parser)
        {
            this.Parser = parser;
            this.SearchParam = new SearchParameters();
            this.Response = new SerialResponse();
        }

        public HandlerSearchBase(ICallParser parser, SearchParameters searchParam, SerialResponse response)
        {
            this.Parser = parser;
            this.SearchParam = searchParam ?? new SearchParameters();
            this.Response = response ?? new SerialResponse();
        }

        public virtual SearchParameters BuildSearchParametersForIdSearch()
        {
            throw new NotImplementedException();
        }
        
        public virtual SearchParameters BuildSearchParameters()
        {
            throw new NotImplementedException();
        }

        public virtual SearchParameters BuildSearchParameters(int id)
        {
            throw new NotImplementedException();
        }

        public abstract SerialResponse BuildResponse(SearchParameters searchParam);

        public static Sorting GetSorting(ICallParser parser, SortColumn defaultSortColumn)
        {
            if (parser.SortColumns.Count > 0)
                return new Sorting(parser.SortColumns);
            else
                return new Sorting(defaultSortColumn);
        }

        // recursive function
        protected SerialVocab GetValueItemRecursively(HierVocabValueItem a)
        {
            return new SerialVocab
            {
                id = a.ValueKey.Id,
                name = a.ValueName,
                description = a.Description,
                mediaUsageCount = a.DescendantMediaUsageCount,
                language = a.ValueKey.LanguageCode,
                items = a.Children == null ? new List<SerialVocab>() : a.Children.Select(c => GetValueItemRecursively(c)).ToList()
            };
        }

        public static Sorting GetSortColumns(List<SortColumn> sortColumns, params SortColumn[] defaultSortColumns)
        {
            if (sortColumns != null && sortColumns.Count > 0)
            {
                return new Sorting(ApiColumnMap.MapSortColumns(sortColumns));
            }
            else
            {
                return new Sorting(ApiColumnMap.MapSortColumns(defaultSortColumns.ToList()));
            }
        }

    }
}
