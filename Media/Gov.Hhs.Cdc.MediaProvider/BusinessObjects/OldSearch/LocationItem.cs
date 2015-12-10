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

namespace Gov.Hhs.Cdc.MediaProvider
{
    [FilterSelection(Code = "FullSearch", CriterionType = FilterCriterionType.Text)]

    [CacheSelection]
    [FilteredDataSet]
    public class LocationItem : DefaultableBusinessObject
    {
        [CacheSelectionKey]        
        public int GeoNameId { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string Admin1Code { get; set; }
        public string Admin2Code { get; set; }

        [FilterSelection(Code = "ParentId", CriterionType = FilterCriterionType.SingleSelect)]
        public int? ParentId { get; set; }

        public int? previousId { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Timezone { get; set; }
    }
}
