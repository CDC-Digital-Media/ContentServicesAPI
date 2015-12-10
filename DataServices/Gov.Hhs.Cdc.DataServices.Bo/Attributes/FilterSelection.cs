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

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public enum FilterCriterionType { TextBox, Boolean, DateRange, DropDownList, SingleSelect, MultiSelect, HierMultiSelect };
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class FilterSelection : System.Attribute
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }
        public string DisplayNote { get; set; }
        public int DisplayOrder { get; set; }

        public FilterCriterionType	CriterionType { get; set; }
        public string GroupCode { get; set; }
        public bool	IsRequired { get; set; }
        public ListItem.KeyType ListKeyType { get; set; }
        
        public bool IsIncludedByDefault { get; set; }

        public bool AllowDateInFuture { get; set; }
        public bool AllowDateInPast { get; set; }

        public bool Inherit { get; set; }

        public FilterSelection()
        {
            IsIncludedByDefault = true;
            AllowDateInFuture = true;
            AllowDateInPast = true;
            IsRequired = false;
            ListKeyType = ListItem.KeyType.Null;
        }

    }
}
