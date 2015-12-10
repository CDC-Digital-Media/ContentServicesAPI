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


    public class RelatedMediaObject : IRelatedMediaItemContainer
    {
        //Parameters that are set correctly
        public int Id { get; set; }
        
        public string RelationshipTypeName { get; set; }
        public int? RelatedMediaId { get; set; }
        public int? Level { get; set; }

        public List<MediaObject> Parents { get; set; }
        public List<MediaObject> Children { get; set; }
        public string MediaTypeCode { get; set; }
        public string MimeTypeCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LanguageCode { get; set; }
        public string SourceUrl { get; set; }
        public string TargetUrl { get; set; }

        public string EffectiveStatusCode { get; set; }
        public string MediaStatusCode { get; set; }
        public bool DisplayOnSearch { get; set; }

        public DateTime? ActiveDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        
        public override string ToString()
        {
            return string.Format("{0} {1} Title: {2}, Description: {3}", RelationshipTypeName, Id, Title, Description);
        }
    }

}
