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
    public class ValueToValueSetObject : DataSourceBusinessObject, IValidationObject
    {
        #region BusinessObjectProperties
        public int ValueId { get; set; }
        public string ValueLanguageCode { get; set; }
        public int ValueSetId { get; set; }
        public string ValueSetLanguageCode { get; set; }
        public string ValueToValueSetName { get; set; }
        
        public int DisplayOrdinal { get; set; }
        public bool IsDefault { get; set; }
        #endregion

        #region OtherProperties
        public string ValueSetName { get; set; }
        #endregion

        public string ValueSetKeyToString()
        {
            return string.Format("{0}({1},{2})", ValueSetName, ValueSetId, ValueSetLanguageCode);
        }
    }
}
