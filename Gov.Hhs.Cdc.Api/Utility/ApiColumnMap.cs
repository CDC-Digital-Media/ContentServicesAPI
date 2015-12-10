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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public static class ApiColumnMap
    {
        private static Dictionary<string, string> _map;       
        private static Dictionary<string, string> Map
        {
            get
            {
                return _map ?? (_map = GetColumnMappingDictionary());
            }
        }

        private static Dictionary<string, string> GetColumnMappingDictionary()
        {
            Dictionary<string, string>  map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            map.Add("mediatype", "mediatypecode");
            map.Add("languagename", "languagecode");
            map.Add("language", "languagecode");
            map.Add("active", "isactive");
            map.Add("datepublished", "publisheddatetime");
            map.Add("datelastreviewed", "lastrevieweddate");
            map.Add("datecreated", "createddate");
            map.Add("datemodified", "modifieddatetime");
            map.Add("popularity", "popularity");
            map.Add("status", "mediastatuscode");
            //map.Add("id", "mediaid");

            return map;
        }

        public static List<SortColumn> MapSortColumns(IEnumerable<SortColumn> sources)
        {
            return sources.Select(c => new SortColumn(GetValue(c.Column), c.SortOrder)).ToList();
        }

        public static string GetKey(string value)
        {
            value = value.ToLower();
            if (Map.ContainsValue(value.ToLower()))
                value = Map.Where(x => x.Value == value).FirstOrDefault().Key;
            
            return value;
        }

        public static string GetValue(string key)
        {
            key = key.ToLower();
            if (Map.ContainsKey(key))
                return Map[key];
            else
                return key;
        }
              
    }
}
