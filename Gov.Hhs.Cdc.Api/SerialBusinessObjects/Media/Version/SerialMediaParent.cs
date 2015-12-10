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
    [KnownType(typeof(SerialECardDetail))]
    [KnownType(typeof(List<SerialECardDetail>))]

    [DataContract(Name = "mediaItemParent")]
    public sealed class SerialMediaParent : ISerialRelatedMediaV2
    {
        public SerialMediaParent()
        {
            alternateImages = new List<SerialAlternateImage>();

            campaigns = new List<SerialCampaign>();
            children = new List<SerialMediaChildren>();
            parents = new List<SerialMediaParent>();
            extendedAttributes = new Dictionary<string, string>();
            geoTags = new List<SerialGeoTag>();
        }

        [DataMember(Order = 0)]
        public int id { get; set; }

        [DataMember(Order = 1)]
        public string url { get; set; }

        [DataMember(Order = 10)]
        public string name { get; set; }

        [DataMember(Order = 12)]
        public string description { get; set; }

        [DataMember(Order = 13)]
        public string mediaType { get; set; }

        [DataMember(Order = 14)]
        public SerialLanguageV2 language { get; set; }

        [DataMember(Order = 15)]
        public List<SerialValueItemTag> tags { get; set; }

        [DataMember(Order = 26)]
        public List<SerialGeoTag> geoTags { get; set; }

        [DataMember(Order = 27)]
        public List<SerialCampaign> campaigns { get; set; }

        [DataMember(Order = 28)]
        public SerialSource source { get; set; }

        [DataMember(Order = 29)]
        public string attribution { get; set; }

        //21
        [DataMember(Order = 30)]
        public string domainName { get; set; }

        //31
        [DataMember(Order = 31)]
        public string owningOrgName { get; set; }

        [DataMember(Order = 32)]
        public int? owningOrgId { get; set; }

        [DataMember(Order = 33)]
        public string maintainingOrgName { get; set; }

        [DataMember(Order = 34)]
        public int? maintainingOrgId { get; set; }

        [DataMember(Order = 35)]
        public string sourceUrl { get; set; }

        [DataMember(Order = 45)]
        public string targetUrl { get; set; }

        [DataMember(Order = 46)]
        public string persistentUrlToken { get; set; }
        
        [DataMember(Order = 47)]
        public string persistentUrl { get; set; }

        [DataMember(Order = 48)]
        public string embedUrl { get; set; }

        [DataMember(Order = 49)]
        public string syndicateUrl { get; set; }

        [DataMember(Order = 50)]
        public string contentUrl { get; set; }

        [DataMember(Order = 51)]
        public string thumbnailUrl { get; set; }

        [DataMember(Order = 52)]
        public List<SerialAlternateImage> alternateImages { get; set; }

        //4
        [DataMember(Order = 54)]
        public string alternateText { get; set; }

        [DataMember(Order = 55)]
        public string noScriptText { get; set; }

        [DataMember(Order = 56)]
        public string featuredText { get; set; }

        [DataMember(Order = 57)]
        public string embedCode { get; set; }

        [DataMember(Order = 58)]
        public string author { get; set; }

        [DataMember(Order = 59)]
        public string length { get; set; }

        [DataMember(Order = 66)]
        public int? size { get; set; }

        [DataMember(Order = 67)]
        public int? height { get; set; }

        [DataMember(Order = 68)]
        public int? width { get; set; }

        //41
        [DataMember(Order = 69)]
        public int childCount { get; set; }

        [DataMember(Order = 70)]
        public List<SerialMediaChildren> children { get; set; }

        [DataMember(Order = 71)]
        public int parentCount { get; set; }

        [DataMember(Order = 72)]
        public List<SerialMediaParent> parents { get; set; }

        #region propertiesNotInUseYet

        [DataMember(Order = 73)]
        public string rating { get; set; }

        [DataMember(Order = 74)]
        public string ratingCount { get; set; }

        [DataMember(Order = 75)]
        public string ratingCommentCount { get; set; }

        #endregion

        [DataMember(Order = 86)]
        public string status { get; set; }

        [DataMember(Order = 87)]
        public string datePublished { get; set; }

        [DataMember(Order = 88)]
        public string dateModified { get; set; }

        [DataMember(Order = 89)]
        public string dateContentAuthored { get; set; }

        [DataMember(Order = 90)]
        public string dateContentUpdated { get; set; }

        [DataMember(Order = 91)]
        public string dateContentPublished { get; set; }

        [DataMember(Order = 92)]
        public string dateContentReviewed { get; set; }

        [DataMember(Order = 93)]
        public string dateSyndicationCaptured { get; set; }

        [DataMember(Order = 94)]
        public string dateSyndicationUpdated { get; set; }

        [DataMember(Order = 95)]
        public string dateSyndicationVisible { get; set; }

        [DataMember(Order = 106)]
        public Dictionary<string, string> extendedAttributes { get; set; }

        [DataMember(Order = 107)]
        public object extension { get; set; }

        [DataMember(Order = 120)]
        public List<SerialEnclosure> enclosures { get; set; }
        
    }

}
