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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class ValueRelationshipValidationStatus : ValueRelationshipValidationObject
    {
        public UpdateStatus RelationshipUpdateStatus { get; set; }
        public UpdateStatus InverseUpdateStatus { get; set; }
    }
    public class ValueRelationshipValidationObject : IValidationObject
    {
        public class UpdateStatus
        {
            public bool Existed { get; set; }
            public bool WasActive { get; set; }
        }
        public ValueRelationshipObject Relationship { get; set; }
        public ValueRelationshipObject Inverse { get; set; }
        
        /// </summary>
        /// <param name="obj">Object to compare to the current Person</param>
        /// <returns>True if equal, false if not</returns>
        public override bool Equals(object obj)
        {

            // Try to cast the object to compare to to be a Person
            ValueRelationshipValidationObject relationship = obj as ValueRelationshipValidationObject;
            return IsEqualTo(relationship);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Relationship.ValueId.GetHashCode();
                hash = hash * 23 + Relationship.ValueLanguageCode.GetHashCode();
                hash = hash * 23 + Relationship.RelatedValueId.GetHashCode();
                hash = hash * 23 + Relationship.RelatedValueLanguageCode.GetHashCode();
                hash = hash * 23 + Relationship.RelationshipTypeName.GetHashCode();
                return hash;
            }
        }

        public bool Equals(ValueRelationshipValidationObject compareTo)
        {
            return IsEqualTo(compareTo);
        }

        private bool IsEqualTo(ValueRelationshipValidationObject compareTo)
        {
            if (compareTo == null)
            {
                return false;
            }
            return Relationship.ValueId == compareTo.Relationship.ValueId
                && Relationship.ValueLanguageCode == compareTo.Relationship.ValueLanguageCode
                && Relationship.RelatedValueId == compareTo.Relationship.RelatedValueId
                && Relationship.RelatedValueLanguageCode == compareTo.Relationship.RelatedValueLanguageCode
                && Relationship.RelationshipTypeName == compareTo.Relationship.RelationshipTypeName;
        }
    }
}
