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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class AToZColumnMapFactory : ColumnMapFactory<SerialAToZ>
    {
        /// <summary>
        /// Create the appropriate mapping depending on the version of the API
        /// </summary>
        protected override Dictionary<string, string> CreateMapping(int version, bool isPublicFacing)
        {
            Dictionary<string, string> mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            mapping.Add("letter", "Letter");
            mapping.Add("language", "LanguageCode");
            mapping.Add("valueSetName", "ValueSetName");
            mapping.Add("mediaCount", "MediaCount");
            mapping.Add("vocabCount", "VocabCount");
            return mapping;
        }

        /// <summary>
        /// Create the appropriate default sort columns, depending on the version of the API
        /// </summary>
        protected override List<SortColumn> CreateDefaultSortList(int version, bool isPublicFacing)
        {
            return new List<SortColumn>(){
                "DisplayOrdinal".Direction(SortOrderType.Asc)
            };
        }
    }
}
