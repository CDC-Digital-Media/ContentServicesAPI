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

using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "mediaItem")]
    public sealed class SerialMediaV1 : ISerialRelatedMediaV1
    {
        [DataMember(Order = 1)]
        public string mediaId { get; set; }

        //[DataMember(Name = "parentId", Order = 2)]
        //public string parentId { get; set; }

        [DataMember(Order = 2)]
        public string url { get; set; }

        [DataMember(Order = 3)]
        public string title { get; set; }

        [DataMember(Order = 4)]
        public string description { get; set; }

        [DataMember(Order = 5)]
        public string mediaType { get; set; }

        //[DataMember(Name = "mimeType", Order = 6)]
        //public string mimeType { get; set; }

        [DataMember(Order = 7)]
        public SerialTags tags { get; set; }

        //[DataMember(Order = 7)]
        //public List<SerialValueItemList> valueItemLists { get; set; }

        [DataMember(Order = 10)]
        public string language { get; set; }

        [DataMember(Order = 19)]
        public string sourceUrl { get; set; }

        [DataMember(Order = 20)]
        public string targetUrl { get; set; }

        [DataMember(Order = 21)]
        public string persistentUrlToken { get; set; }

        [DataMember(Order = 22)]
        public string persistentUrl { get; set; }

        [DataMember(Order = 23)]
        public string embedUrl { get; set; }

        [DataMember(Order = 24)]
        public string syndicateUrl { get; set; }

        [DataMember(Order = 25)]
        public string contentUrl { get; set; }

        [DataMember(Order = 30)]
        public string thumbnailUrl { get; set; }

        [DataMember(Order = 31)]
        public string rating { get; set; }

        [DataMember(Order = 32)]
        public string ratingCount { get; set; }

        [DataMember(Order = 33)]
        public string ratingCommentCount { get; set; }

        [DataMember(Order = 34)]
        public string embedCode { get; set; }

        [DataMember(Order = 35)]
        public string attribution { get; set; }

        //[DataMember(Name = "active", Order = 18)]
        //public bool active { get; set; }

        [DataMember(Order = 40)]
        public string status { get; set; }

        [DataMember(Order = 41)]
        public string datePublished { get; set; }

        //[DataMember(Name = "dateLastReviewed", Order = 21)]
        //public string dateLastReviewed { get; set; }

        [DataMember(Order = 42)]
        public string dateModified { get; set; }

        //[DataMember(Name = "dateCreated", Order = 25)]
        //public string dateCreated { get; set; }

        //[DataMember(Name = "canManage", Order = 26)]
        //public bool canManage { get; set; }

        //[DataMember(Name = "canShare", Order = 27)]
        //public bool canShare { get; set; }

        //[DataMember(Name = "rowVersion", Order = 28)]
        //public string rowVersion { get; set; }

        //[DataMember(Name = "itemCount", Order = 29)]
        //public int itemCount { get; set; }

        //[DataMember(Name = "items", Order = 30)]
        //public List<SerialCombineMediaItem> items { get; set; }

        [DataMember(Order = 43)]
        public int childCount { get; set; }

        [DataMember(Order = 44)]
        public List<SerialMediaV1> children { get; set; }

        [DataMember(Order = 45)]
        public int parentCount { get; set; }

        [DataMember(Order = 50)]
        public List<SerialMediaV1> parents { get; set; }

        [DataMember(Order = 51)]
        public string sourceCode { get; set; }

        [DataMember(Order = 52)]
        public string domainName { get; set; }

        [DataMember(Order = 53)]
        public int? owningOrgId { get; set; }

        [DataMember(Order = 54)]
        public string owningOrgName { get; set; }

        [DataMember(Order = 55)]
        public int? maintainingOrgId { get; set; }

        [DataMember(Order = 60)]
        public string maintainingOrgName { get; set; }

        [DataMember(Order = 120)]
        public List<SerialEnclosure> enclosures { get; set; }
    }
}
