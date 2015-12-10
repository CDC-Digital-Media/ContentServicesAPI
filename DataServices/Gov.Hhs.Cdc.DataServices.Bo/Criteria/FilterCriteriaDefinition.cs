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
using System.Xml.Serialization;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class CriteriaDefinitionsForFilter
    {
        public string FilterCode { get; set; }
        public string NameSpace {get; set;}
        public List<FilterCriteriaDefinition> Criteria { get; set; }

        public CriteriaDefinitionsForFilter(string filterCode, string nameSpace, List<FilterCriteriaDefinition> criteria)
        {
            FilterCode = filterCode;
            NameSpace = nameSpace;
            Criteria = criteria;
        }

        public override string ToString()
        {
            return FilterCode + ": " + string.Join(", ", Criteria.Select(c => c.Code).ToArray());
        }
    }

    public class CriteriaForApplication
    {
        public string ApplicationCode { get; set; }
        public List<CriteriaDefinitionsForFilter> Criteria { get; set; }

        public CriteriaForApplication(string applicationCode, List<CriteriaDefinitionsForFilter> criteria)
        {
            ApplicationCode = applicationCode;
            Criteria = criteria;
        }
    }

    [CacheSelection(ReturnsList = true)]
    public class FilterCriteriaDefinition : IDefaultableBusinessObject, IEquatable<FilterCriteriaDefinition>
    {

        [CacheSelectionKey]
        public string ApplicationCode { get; set; }

        [CacheSelectionKey] //Need an alternate comparison with BaseFilterCode here when building the Dictionary
        public string FilterCode { get; set; }

        public string Code { get; set; }
        [CacheOrderBy(Index = 1)]
        public int DisplayOrder { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNote { get; set; }

        public string Type { get; set; }
        //public FilterCriterionType CriterionType { get { return (FilterCriterionType)Enum.Parse(typeof(FilterCriterionType), Type); } }
        public string TextType { get; set; }
        public string ListKeyType { get; set; }
        //public ListItem.KeyType ListItemKeyType { get { return (ListItem.KeyType)Enum.Parse(typeof(ListItem.KeyType), ListKeyType); } }
        public string DbColumnName { get; set; }
        public string GroupCode { get; set; }
        
        //public List<string> FilterByParameterCodes { get; set; }
        public bool IsRequired { get; set; }
        //public object AdditionalInfo { get; set; }
        public bool AllowDateInPast { get; set; }
        public bool AllowDateInFuture { get; set; }
        public bool IsIncludedByDefault { get; set; }

        public string InheritsTypeName { get; set; }
        public Type InheritsType { get; set; }

        public bool IsDefault { get; set; }


        //Derived Attributes
        [CacheOrderBy(Index = 0)]
        public int GroupOrder { get; set; }
        public string GroupName { get; set; }

        //[XmlIgnore]
        //public IEnumerable<FilterControlCriteria> ControlCriterionEnumerable {get; set;}

        //[XmlIgnore]
        //private List<FilterControlCriteria> controlCriterion;

        //public List<FilterControlCriteria> ControlCriterion {
        //    get 
        //    { 
        //        return controlCriterion ?? 
        //            (controlCriterion = ControlCriterionEnumerable != null ? ControlCriterionEnumerable.ToList()
        //                : new List<FilterControlCriteria>()); 
        //    }
        //    set { controlCriterion = value; }
        //}


        public FilterCriteriaDefinition()
        {
        }

        public bool Equals(FilterCriteriaDefinition other)
        {
            return Code.Equals(other.Code);
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
