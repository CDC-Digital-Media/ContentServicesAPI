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

using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.MediaProvider
{
    [FilterSelection(Code = "IsPublicFacing", CriterionType = FilterCriterionType.Boolean)]

    [CacheSelection(IsPersisted = true)]
    [FilteredDataSet]
    public class MediaTypeItem : DefaultableBusinessObject
    {
        [CacheSelectionKey]
        public string MediaTypeCode{ get; set; }
        public string Description { get; set; }
        public int DisplayOrdinal { get; set; }
        public bool IsActive { get; set; }
        public bool Display { get; set; }
        public int MediaCount { get; set; }
        public PreferencesSet Preferences { get; set; }
    }
}
