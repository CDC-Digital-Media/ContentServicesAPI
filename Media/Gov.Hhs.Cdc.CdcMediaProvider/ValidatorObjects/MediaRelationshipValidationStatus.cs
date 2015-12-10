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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class MediaRelationshipValidationObject : IValidationObject
    {
        public MediaRelationshipObject Relationship { get; set; }
        public MediaRelationshipObject OriginalRelationship { get; set; }
        public MediaRelationshipObject Inverse { get; set; }
        public MediaRelationshipObject OriginalInverse { get; set; }

        public override bool Equals(object obj)
        {
            MediaRelationshipValidationObject relationship = obj as MediaRelationshipValidationObject;
            return IsEqualTo(relationship);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Relationship.MediaId.GetHashCode();
                hash = hash * 23 + Relationship.RelatedMediaId.GetHashCode();
                hash = hash * 23 + Relationship.RelationshipTypeName.GetHashCode();
                return hash;
            }
        }

        public bool Equals(MediaRelationshipValidationObject compareTo)
        {
            return IsEqualTo(compareTo);
        }

        private bool IsEqualTo(MediaRelationshipValidationObject compareTo)
        {
            if (compareTo == null)
            {
                return false;
            }
            return Relationship.MediaId == compareTo.Relationship.MediaId
                && Relationship.RelatedMediaId == compareTo.Relationship.RelatedMediaId
                && Relationship.RelationshipTypeName == compareTo.Relationship.RelationshipTypeName;
        }
    }
}
