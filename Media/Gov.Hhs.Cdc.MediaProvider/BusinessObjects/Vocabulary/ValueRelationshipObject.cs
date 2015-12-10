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
//using System.ComponentModel.DataAnnotations;

namespace Gov.Hhs.Cdc.MediaProvider
{
    public class ValueRelationshipObject : DataSourceBusinessObject
    {
        #region BusinessObjectProperties

        public string RelationshipTypeName { get; set; }
        public int ValueId { get; set; }
        public string ValueLanguageCode { get; set; }
        public int RelatedValueId { get; set; }
        public string RelatedValueLanguageCode { get; set; }

        public int DisplayOrdinal { get; set; }
        public bool IsCommitted { get; set; }
        public bool IsActive { get; set; }
        #endregion

        #region OtherProperties
        public string ShortType { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }

        public string RelatedValueName { get; set; }
        public bool IsBeingDeleted { get; set; }
        #endregion

        public ValueRelationshipObject()
        {
        }
        public ValueRelationshipObject(ValueRelationshipObject relationship, RelationshipTypeItem inverseType)
        {
            RelationshipTypeName = inverseType.RelationshipTypeName;
            ShortType = inverseType.ShortType;
            ValueId = relationship.RelatedValueId;
            RelatedValueId = relationship.ValueId;
            ValueLanguageCode = relationship.RelatedValueLanguageCode;
            RelatedValueLanguageCode = relationship.ValueLanguageCode;
            DisplayOrdinal = relationship.DisplayOrdinal;
            IsActive = relationship.IsActive;
            IsCommitted = relationship.IsCommitted;
            DbObject = relationship.DbObject;
            ValidationKey = relationship.ValidationKey + ".Inverse";
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", ValueId, TypeName, RelatedValueId);
        }
        
    }
}
