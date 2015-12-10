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
    [FilteredDataSet]
    public class BusinessUnitItem : DefaultableBusinessObject
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Text, TextType = FilterTextType.StartsWith)]
        public string Name { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.SingleSelect)]
        public string SourceCode { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        public string TypeCode { get; set; }

        public bool IsActive { get; set; }
        public int? GeoNameId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
