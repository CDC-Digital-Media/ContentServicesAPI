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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    [KnownType(typeof(SerialECardDetail))]
    [KnownType(typeof(List<SerialHtmlPreferences>))]

    [DataContract(Name = "mediaAdmin")]
    //[Serializable]
    public class SerialMediaAdmin : ISerialRelatedMediaAdmin    //, ISerializable
    {
        
        public SerialMediaAdmin() { }

        public string url { get; set; }
        public string embedUrl { get; set; }
        public string syndicateUrl { get; set; }
        public string contentUrl { get; set; }

        //1
        //[Deprecated("In version 2")]
        //[Remove("In version 3")]
        [DataMember(Order = 100)]
        public string mediaId { get; set; }

        ////[Added("In version 2")]
        [DataMember(Order = 101)]
        public string id { get; set; }

        [DataMember(Order = 102)]
        public string title { get; set; }

        [DataMember(Order = 103)]
        public string subTitle { get; set; }

        [DataMember(Order = 104)]
        public string description { get; set; }

        [DataMember(Order = 105)]
        public string language { get; set; }

        [DataMember(Order = 106)]
        public string mediaType { get; set; }

        //[admin only]
        [DataMember(Order = 107)]
        public string mimeType { get; set; }

        [DataMember(Order = 108)]
        public string encoding { get; set; }

        [DataMember(Order = 109)]

        public string mediaGuid { get; set; }


        //2
        [DataMember(Order = 200)]
        public string sourceUrl { get; set; }

        [DataMember(Order = 201)]
        public string targetUrl { get; set; }

        [DataMember(Order = 202)]
        public string persistentUrlToken { get; set; }

        [DataMember(Order = 203)]
        public string persistentUrl { get; set; }

        //3
        [DataMember(Order = 301)]
        public string sourceCode { get; set; }

        [DataMember(Order = 302)]
        public int? owningOrgId { get; set; }

        [DataMember(Order = 303)]
        public int? maintainingOrgId { get; set; }

        //4
        [DataMember(Order = 400)]
        public List<SerialAlternateImage> alternateImages { get; set; }

        [DataMember(Order = 401)]
        public string alternateText { get; set; }

        [DataMember(Order = 402)]
        public string noScriptText { get; set; }

        [DataMember(Order = 403)]
        public string featuredText { get; set; }

        [DataMember(Order = 404)]
        public string author { get; set; }

        [DataMember(Order = 405)]
        public string length { get; set; }

        [DataMember(Order = 406)]
        public int? size { get; set; }

        [DataMember(Order = 407)]
        public int? height { get; set; }

        [DataMember(Order = 408)]
        public int? width { get; set; }

        //41
        [DataMember(Order = 410)]
        public int childRelationshipCount { get; set; }

        [DataMember(Order = 411)]
        public List<SerialMediaRelationship> childRelationships { get; set; }


        [DataMember(Order = 420)]
        public int childCount { get; set; }

        [DataMember(Order = 421)]
        public List<SerialMediaAdmin> children { get; set; }

        [DataMember(Order = 422)]
        public int parentCount { get; set; }

        [DataMember(Order = 423)]
        public List<SerialMediaAdmin> parents { get; set; }

        [DataMember(Order = 424)]
        public List<SerialMediaRelationship> parentRelationships { get; set; }


        //5
        [DataMember(Order = 501)]
        public int? ratingOverride { get; set; }

        [DataMember(Order = 502)]
        public int ratingMinimum { get; set; }

        [DataMember(Order = 503)]
        public int ratingMaximum { get; set; }

        [DataMember(Order = 504)]
        public string comments { get; set; }

        //6
        [DataMember(Order = 602)]
        public string status { get; set; }

        [DataMember(Order = 603)]
        public string datePublished { get; set; }

        //[admin only]
        [DataMember(Order = 604)]
        public string dateLastReviewed { get; set; }

        [DataMember(Order = 605)]
        public string rowVersion { get; set; }


        [DataMember(Order = 701)]
        public Dictionary<string, List<SerialValueItem>> tags { get; set; }

        [DataMember(Order = 702)]
        public List<SerialGeoTag> geoTags { get; set; }

        [DataMember(Order = 703)]
        public List<int> topics { get; set; }

        [DataMember(Order = 704)]
        public List<int> audiences { get; set; }


        //8
        //[admin only]
        [DataMember(Order = 801)]
        public bool canManage { get; set; }

        //[admin only]
        [DataMember(Order = 802)]
        public bool canShare { get; set; }


        [DataMember(Order = 1001)]
        public SerialECardDetail eCard { get; set; }

        [DataMember(Order = 1002)]
        public SerialFeedDetail feed { get; set; }

        //#region mediaFeedback   //TODO: Implement save of these

        //[DataMember(Name = "rating", Order = 12)]
        //public string rating { get; set; }

        //[DataMember(Name = "ratingCount", Order = 13)]
        //public string ratingCount { get; set; }

        //[DataMember(Name = "commentCount", Order = 14)]
        //public string ratingCommentCount { get; set; }

        //#endregion

        [DataMember(Order = 1200)]
        public string dateContentAuthored { get; set; }

        [DataMember(Order = 1201)]
        public string dateContentUpdated { get; set; }

        [DataMember(Order = 1202)]
        public string dateContentPublished { get; set; }

        [DataMember(Order = 1203)]
        public string dateContentReviewed { get; set; }

        [DataMember(Order = 1204)]
        public string dateSyndicationCaptured { get; set; }

        [DataMember(Order = 1205)]
        public string dateSyndicationUpdated { get; set; }

        [DataMember(Order = 1206)]
        public string dateSyndicationVisible { get; set; }

        [DataMember(Order = 1300)]
        public Dictionary<string, string> extendedAttributes { get; set; }

        [DataMember(Order = 1400)]
        public string pageCount { get; set; }

        [DataMember(Order = 1500)]
        public string dataSize { get; set; }

        [DataMember(Order = 1600)]
        public string omnitureChannel { get; set; }

        [DataMember(Order = 2000)]
        public List<SerialEnclosure> enclosures { get; set; }

        #region setBySystemFields

        //21
        [DataMember(Order = 2101)]
        public string domainName { get; set; }

        [DataMember(Order = 2102)]
        public string dateModified { get; set; }

        [DataMember(Order = 2103)]
        public string dateCreated { get; set; }

        [DataMember(Order = 2104)]
        public string createdBy { get; set; }

        [DataMember(Order = 2105)]
        public string modifiedBy { get; set; }

        #endregion

        #region derivedFields

        //31
        [DataMember(Order = 3101)]
        public string thumbnailUrl { get; set; }

        [DataMember(Order = 3103)]
        public string owningOrgName { get; set; }

        [DataMember(Order = 3104)]
        public string maintainingOrgName { get; set; }

        [DataMember(Order = 3105)]
        public List<SerialPreferenceSet> effectivePreferences { get; set; }

        [DataMember(Order = 3106)]
        public List<SerialPreferenceSet> preferences { get; set; }

        [DataMember(Order = 3207)]
        public string xPath { get; set; }

        [DataMember(Order = 3208)]
        public string elementIds { get; set; }

        [DataMember(Order = 3109)]
        public string classNames { get; set; }

        [DataMember(Order = 3110)]
        public string attribution { get; set; }

        [DataMember(Order = 3111)]
        public string embedcode { get; set; }

        #endregion

        [DataMember(Order = 4000)]
        public List<SerialAggregate> feedAggregates { get; set; }


    }
}
