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
using System.Xml.Linq;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    [Serializable]
    public class FilterCriterionText : FilterCriterion
    {
        #region dynamicProperties
        public string Value { get; set; }
        #endregion

        #region configurationProperties
        public override string ParameterType { get { return "Text"; }}
        public FilterTextType TextType { get; set; }
        #endregion

        public FilterCriterionText()
            : base()
        {
        }

        public FilterCriterionText(string value)
            : base()
        {
            if (value == null)
            {
                IsFiltered = false;
            }
            else
            {
                IsFiltered = true;
                Value = value;
            }
        }

        public static FilterCriterionText CreateWithTextType(string textType)
        {
            FilterCriterionText criterion = new FilterCriterionText();
            criterion.TextType = (FilterTextType)Enum.Parse(typeof(FilterTextType), textType);
            return criterion;
        }

        public override string ValueToString()
        {
            return Value;
        }

        protected override void SetCriteriaValues(XElement values)
        {
            Value = values.Attribute("Value").Value;
        }
        
        public override void SetCriteriaValues(FilterCriterion criterion)
        {
            base.SetCriteriaValues(criterion);
            FilterCriterionText tb = (FilterCriterionText)criterion;
            Value = tb.Value;
        }

        public override void SetCriteriaValues(Criterion criterion)
        {
            base.SetCriteriaValues(criterion);
            Value = criterion.Value;
        }

        public override string ToString()
        {
            return Code + ": " + Value;
        }

        public override void SetValues(string value)
        {
            base.SetValues(value);
            Value = value;
        }

        public override string GetStringKey()
        {
            return Value;
        }

    }
}
