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
    public class AttributeValueObject : DataSourceBusinessObject
    {
        public int MediaId { get; set; }
        public ValueKey ValueKey { get; set; }
        public int AttributeId { get; set; }
        public int DisplayOrdinal { get; set; }

        //Derived Values
        public string AttributeName { get; set; }
        public string ValueName { get; set; }
        public bool IsActive { get; set; }


        public AttributeValueObject()
        {
            IsActive = true;
        }

        public AttributeValueObject(string attributeName, string lang, int valueId, int displayOrder)
        {
            ValueKey = new ValueKey(valueId, lang);
            AttributeId = 0;
            DisplayOrdinal = displayOrder;
            IsActive = true;

            AttributeName = attributeName;  //We will have to derive the attributeId from the name
        }
    }

}
