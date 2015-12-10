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

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public class AttributeSelection
    {
        public string Value { get; set; }
        public AttributeSelection()
        {
            Value = "";
        }

        public AttributeSelection(string attributeName)
        {
            if (attributeName.First() == '[')
            {
                Value = attributeName;  //hardcoded attribute list
            }
            else
            {
                Value = @"[@" + attributeName.ToLower() + "]";
            }
        }

        public AttributeSelection(string attributeName, string attributeValue)
        {
            Value = @"[@" + attributeName.ToLower() + @"='" + attributeValue + "']";
        }

        public AttributeSelection(string attributeName, List<string> attributeValues)
        {
            string[] selections;
            
            // case insensitive
            if (attributeName.ToLower() == "class" || attributeName.ToLower() == "id")
            {
                attributeValues = new List<string> { attributeValues.First() }; //the content is rendered incorrectly if mulple class names
                selections = (from v in attributeValues
                              select @"translate(@" + attributeName.ToLower() + ",'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')" + @"='" + v.ToLower() + "'").ToArray();
            }
            else
            {
                selections = (from v in attributeValues
                              select @"@" + attributeName.ToLower() + @"='" + v + "'").ToArray();
            }

            Value = "[" + string.Join(" or ", selections) + "]";
        }

        public override string ToString()
        {
            return Value;
        }


    }
}
