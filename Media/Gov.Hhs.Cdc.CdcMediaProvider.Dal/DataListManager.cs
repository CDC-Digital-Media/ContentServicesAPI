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
using System.Text.RegularExpressions;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class DataListManager
    {
        public IObjectContextFactory ObjectContextFactory { get; set; }

        public DataListManager(IObjectContextFactory objectContextFactory)
        {
            ObjectContextFactory = objectContextFactory;
        }

        public List<ListItem> GetListItems(FilterCriterion parameter, bool getOnlySelectedItems)
        {
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                IQueryable<ListItem> result = GetFullListItems(media, parameter);
                if( getOnlySelectedItems )
                    result = parameter.Filter(result);
                return result.ToList();
            }
        }

        public ListItem RemoveSpecialCharacters(ListItem item)
        {
            item.Value = RemoveSpecialCharacters(item.Value);
            return item;
        }

        private IQueryable<ListItem> GetFullListItems(MediaObjectContext media, FilterCriterion parameter)
        {
            switch (parameter.Code.ToLower())
            {
                case "mediatype":
                    return (new MediaTypeItemCtl()).GetDataList(media);
                //case "language":
                    //return LanguageItemCtl.GetDataList(media, getOnlyInUseItems:true);
                case "valueset":
                    return ValueSetDataListCtl.GetDataList(media);
                case "valuesetname":
                    return ValueSetDataListCtl.GetDataListByKey(media).ToList()
                        .Select(v => RemoveSpecialCharacters(v)).AsQueryable();
                default:
                    return new List<ListItem>().AsQueryable();
            }
        }

        public static string RemoveSpecialCharacters(string input)
        {
            Regex r = new Regex("(?:[^a-z0-9 |]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return r.Replace(input, String.Empty);
        }

    }
}
