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

namespace Gov.Hhs.Cdc.MediaProvider
{
    public class ValueKey
    {
        public int Id { get; set; }
        public string LanguageCode { get; set; }

        public ValueKey()
        {
        }

        public ValueKey(int id, string languageCode)
        {
            Id = id;
            LanguageCode = languageCode;
        }

        /// </summary>
        /// <param name="obj">Object to compare to the current Person</param>
        /// <returns>True if equal, false if not</returns>
        public override bool Equals(object obj)
        {
            ValueKey compareTo = obj as ValueKey;
            if (compareTo == null)
            {
                return false;
            }
            return Id == compareTo.Id
                && LanguageCode == compareTo.LanguageCode;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + LanguageCode.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Id, LanguageCode);
        }

    }

}
