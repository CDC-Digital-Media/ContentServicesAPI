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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class FilterCriterionHierarchicalMultiSelect : FilterCriterion
    {
        public List<HierarchicalItem> SelectedItems { get; set; }

        public override string ParameterType { get { return "HierMultiSelect"; } }
        public ListItem.KeyType KeyType { get; set; }
        public List<HierarchicalItem> DataSource { get; set; }


        public FilterCriterionHierarchicalMultiSelect()
            : base()
        {
            CreateItems();
        }


        private void CreateItems()
        {
            if (SelectedItems == null)
            {
                SelectedItems = new List<HierarchicalItem>();
            }
        }

        public override string ValueToString()
        {
            return string.Join(",", (from i in SelectedItems
                                     select i.Value).ToArray());
        }


        public override ListItem.KeyType GetKeyType()
        {
            return KeyType;
        }

        public override List<int> GetIntKeys()
        {
            if (KeyType == ListItem.KeyType.IntKey)
            {
                return (from i in SelectedItems
                        select i.IntKey).ToList();
            }
            else
            {
                return null;
            }
        }


        public override List<string> GetStringKeys()
        {
            if (SelectedItems == null)
            {
                return new List<string>();
            }
            else
            {
                return (from i in SelectedItems
                        select i.Value).ToList();
            }
        }

        protected override void SetCriteriaValues(XElement values)
        {
            SelectedItems = new List<HierarchicalItem>();
            foreach (XElement item in values.Elements("HierItem"))
            {
                HierarchicalItem listItem = new HierarchicalItem(KeyType);
                listItem.Value = item.Value;
                bool searchChildren = false;
                bool.TryParse(item.Attribute("SearchChildren").Value, out searchChildren);
                listItem.SearchChildren = searchChildren;
                SelectedItems.Add(listItem);
            }

        }

        public override void SetCriteriaValues(FilterCriterion criterion)
        {
            base.SetCriteriaValues(criterion);
            FilterCriterionHierarchicalMultiSelect hs = (FilterCriterionHierarchicalMultiSelect)criterion;
            foreach (HierarchicalItem item in hs.SelectedItems)
            {
                HierarchicalItem listItem = new HierarchicalItem(KeyType);
                listItem.Value = item.Value;
                listItem.SearchChildren = item.SearchChildren;
                SelectedItems.Add(listItem);
            }
        }

        public override void SetCriteriaValues(Criterion criterion)
        {
            if (criterion.Values != null)
            {
                SelectedItems = criterion.Values.Select(v => new HierarchicalItem(KeyType) { Value = v, SearchChildren = true }).ToList();
                IsFiltered = true;
            }
            else if (!string.IsNullOrEmpty(criterion.Value))
            {
                SelectedItems.Add(new HierarchicalItem(KeyType) { Value = criterion.Value, SearchChildren = true });
                IsFiltered = true;
            }
            else
            {
                IsFiltered = false;
            }
        }

        public override void SetValues(string value)
        {
            base.SetValues(value);
        }

        public override string ToString()
        {
            string results = base.ToString();
            string[] names = (from i in SelectedItems
                              select i.ToString()).ToArray();
            results += string.Join(", ", names);
            return results;
        }

    }
}
