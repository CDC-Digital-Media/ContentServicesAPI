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
    [FilteredDataSet]
    public class ValueObject : DataSourceBusinessObject, IValidationObject
    {
        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        public int ValueId { get; set; }

        [FilterSelection(Code="ValueNameStartsWith", CriterionType = FilterCriterionType.Text, TextType=FilterTextType.StartsWith)]
        public string ValueName { get; set; }

        [FilterSelection(Code = "Language", CriterionType = FilterCriterionType.MultiSelect)]
        public string LanguageCode { get; set; }
        public string Description { get; set; }
        public int DisplayOrdinal { get; set; }
        public bool IsActive { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Boolean)]


        public IEnumerable<ValueToValueSetObject> ValueToValueSetObjects { get; set; }
        public IEnumerable<ValueRelationshipObject> ValueRelationships { get; set; }
        public IEnumerable<ValueRelationshipObject> ValueRelationships2 { get; set; }

        public ValueObject()
        {
        }

        public ValueObject(int id)
        {
            ValueId = id;
        }
        //public IEnumerable<string> ValueSetKeys { get; set; }
        //public IEnumerable<ValueSetKey> ValueSetKeys { get; set; }
        //public IEnumerable<ValueToValueSet> ValueToValueSets { get; set; }

        
    }
}
