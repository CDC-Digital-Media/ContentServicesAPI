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

namespace Gov.Hhs.Cdc.Bo
{
    /// <summary>
    /// The key to be used to populate the dictionary key must also be identified, by using the CacheSelectionKey.  
    /// The SelectionTypeCode must be specified for all CacheSelections other than the first one.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CacheSelectionKey : System.Attribute
    {
        /// <summary>
        /// Defines which selection this key is used for.
        /// </summary>
        public string SelectionTypeCode { get; set; }

        
        /// <summary>
        /// If true, select items based only on the other keys in this business object if this property is null.  
        /// If this property is not null, then the property will be used in the dictionary key.
        /// </summary>
        public bool DefaultIfNull { get; set; }

        public CacheSelectionKey()
        {
            SelectionTypeCode = "";
            DefaultIfNull = false;
        }

    }
}
