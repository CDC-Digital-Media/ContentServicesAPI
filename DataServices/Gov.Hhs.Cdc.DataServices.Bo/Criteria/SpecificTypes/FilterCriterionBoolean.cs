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
using System.Xml.Linq;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class FilterCriterionBoolean : FilterCriterion
    {
        public bool Value { get; set; }

        public override string ParameterType { get { return "Boolean"; } }

        public FilterCriterionBoolean()
            : base()
        {
        }


        public FilterCriterionBoolean(bool? value)
            : base()
        {
            if (value == null)
            {
                IsFiltered = false;
            }
            else
            {
                IsFiltered = true;
                Value = (bool)value;
            }
        }

        public override string ValueToString()
        {
            return Value.ToString();
        }

        protected override void SetCriteriaValues(XElement values)
        {
            string value = values.Attribute("Value").Value;
            Value = string.IsNullOrEmpty(value) ? false : string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        public override void SetCriteriaValues(FilterCriterion criterion)
        {
            base.SetCriteriaValues(criterion);
            FilterCriterionBoolean cb = (FilterCriterionBoolean) criterion;
            Value = cb.Value;
        }

        public override void SetCriteriaValues(Criterion criterion)
        {
            base.SetCriteriaValues(criterion);
            bool isChecked = false;
            if (bool.TryParse(criterion.Value.ToLower(), out isChecked))
            {
                Value = isChecked;
            }
            else
            {
                throw new ApplicationException("Invalid checked value from Boolean criteria");
            }
        }

        public override string ToString()
        {
            return Code + ": " + (Value ? "True" : "False");
        }

        public override void SetValues(string value)
        {
            base.SetValues(value);
            bool isChecked = false;
            bool.TryParse(value, out isChecked);
            Value = isChecked;
        }

        public override bool GetBoolValue()
        {
            return Value;
        }


    }
}
