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
using System.Xml.Linq;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class FilterCriterionSingleSelect : FilterCriterion
    {
 
        public ListItem SelectedItem { get; set; }

        public override string ParameterType { get { return "SingleSelect"; } }
        public ListItem.KeyType KeyType { get; set; }

        public FilterCriterionSingleSelect()
            : base()
        {
        }

        public FilterCriterionSingleSelect(int? key)
            : base()
        {
            KeyType = ListItem.KeyType.IntKey;
            if (key == null)
            {
                IsFiltered = false;
            }
            else
            {
                int theValue = (int)key;
                IsFiltered = true;
                SelectedItem = new ListItem(theValue, theValue.ToString());
            }
        }



        public override string ValueToString()
        {
            return SelectedItem.Value;
        }

        public override int GetIntKey()
        {
            if (KeyType == ListItem.KeyType.IntKey)
            {
                return SelectedItem.IntKey;
            }
            else
            {
                return 0;
            }
        }
        public override string GetStringKey()
        {
            return SelectedItem.Value;
        }

        public override ListItem.KeyType GetKeyType()
        {
            return KeyType;
        }

        public override List<int> GetIntKeys()
        {
            if (KeyType != ListItem.KeyType.IntKey)
            {
                return null;
            }
            List<int> keys = new List<int>();
            if (SelectedItem != null)
            {
                keys.Add(SelectedItem.IntKey);
            }
            return keys;

        }

        public override List<string> GetStringKeys()
        {
            List<string> keys = new List<string>();
            if (SelectedItem != null)
            {
                keys.Add(SelectedItem.Value);
            }
            return keys;
        }


        protected override void SetCriteriaValues(XElement values)
        {
            SelectedItem = new ListItem(KeyType);
            SelectedItem.Value = values.Attribute("WriteElementString").Value;
        }

        public override void SetCriteriaValues(FilterCriterion criterion)
        {
            base.SetCriteriaValues(criterion);
            FilterCriterionSingleSelect ss = (FilterCriterionSingleSelect)criterion;
            SelectedItem = new ListItem(KeyType);
            SelectedItem.Value = ss.SelectedItem == null ? null : ss.SelectedItem.Value;
        }

        public override void SetCriteriaValues(Criterion criterion)
        {
            base.SetCriteriaValues(criterion);
            SelectedItem = new ListItem(KeyType);
            SelectedItem.Value = criterion.Value;
        }

        public override string ToString()
        {
            return Code + ": " + SelectedItem.ToString();
        }

        public override void SetValues(string value)
        {
            base.SetValues(value);
        }
    }
}
