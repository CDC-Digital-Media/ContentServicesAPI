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
using System.Xml.Linq;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class FilterCriterionMultiSelect : FilterCriterion
    {
        public List<ListItem> SelectedItems { get; set; }

        public override string ParameterType { get { return "MultiSelect"; } }
        public ListItem.KeyType KeyType { get; set; }

        public FilterCriterionMultiSelect()
            : base()
        {
            CreateItems();
            SelectedItems = new List<ListItem>();
        }

        public FilterCriterionMultiSelect(IEnumerable<string> keys)
            : base()
        {
            CreateItems();
            KeyType = ListItem.KeyType.StringKey;
            if (keys == null)
            {
                IsFiltered = false;
            }
            else
            {
                IsFiltered = true;
                SelectedItems = keys.Select(k => new ListItem(k, k)).ToList();
            }
        }

        public FilterCriterionMultiSelect(IEnumerable<int> keys)
            : base()
        {
            CreateItems();
            KeyType = ListItem.KeyType.IntKey;
            if (keys == null)
            {
                IsFiltered = false;
            }
            else
            {
                IsFiltered = true;
                SelectedItems = keys.Select(k => new ListItem(k, k.ToString())).ToList();
            }
        }

        private void CreateItems()
        {
            if (SelectedItems == null)
            {
                SelectedItems = new List<ListItem>();
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
            if (KeyType != ListItem.KeyType.IntKey)
            {
                throw new ApplicationException("Filter Criterion " + Name + " key type is not integer");
            }
            return (from i in SelectedItems
                    select i.IntKey).ToList();
        }

        public override List<string> GetStringKeys()
        {
            if (KeyType == ListItem.KeyType.StringKey)
            {
                return (from i in SelectedItems
                        select i.Value).ToList();
            }
            else
            {
                return null;
            }
        }

        protected override void SetCriteriaValues(XElement values)
        {
            SelectedItems = new List<ListItem>();
            foreach (XElement item in values.Elements("Item"))
            {
                ListItem listItem = new ListItem(KeyType);
                listItem.Value = item.Value;
                SelectedItems.Add(listItem);
            }
        }

        public override void SetCriteriaValues(FilterCriterion criterion)
        {
            base.SetCriteriaValues(criterion);
            FilterCriterionMultiSelect ms = (FilterCriterionMultiSelect)criterion;
            foreach (ListItem item in ms.SelectedItems)
            {
                ListItem listItem = new ListItem(KeyType);
                listItem.Value = item.Value;
                SelectedItems.Add(listItem);
            }
        }

        public override void SetCriteriaValues(Criterion criterion)
        {
            if (criterion.Values != null)
            {
                SelectedItems = criterion.Values.Select(v => new ListItem(KeyType) { Value = v }).ToList();
                IsFiltered = true;
            }
            else if (!string.IsNullOrEmpty(criterion.Value))
            {
                SelectedItems.Add(new ListItem(KeyType) { Value = criterion.Value });
                IsFiltered = true;
            }
            else
            {
                IsFiltered = false;
            }
        }

        public override string ToString()
        {
            string results = base.ToString();
            string[] names = (from i in SelectedItems
                              select i.ToString()).ToArray();
            results += string.Join(", ", names);
            return Code + ": " + results;
        }

        public override void SetValues(string value)
        {
            base.SetValues(value);
        }
    }
}
