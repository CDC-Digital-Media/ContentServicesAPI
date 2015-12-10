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

namespace Gov.Hhs.Cdc.CsBusinessObjects.Media
{
    public class SearchCriteria
    {
        public string Title { get; set; }
        public string MediaDisplay { get; set; } //use to get active media types only (for storefront)
        public string FullTextSearch{ get; set; }
        public string Topic { get; set; }
        public string ContentGroup { get; set; }
        public string MediaType { get; set; }
        public string Status { get; set; }
        public string PublishDateFrom { get; set; }
        public string PublishDateTo { get; set; }
        public string ModifiedDateFrom { get; set; }
        public string ModifiedDateTo { get; set; }
        public string MediaId { get; set; }
        public string Language { get; set; }
        public string Source { get; set; }
        public string Audience { get; set; }
        public string OwningOrganization { get; set; }
        public string MaintainingOrganization { get; set; }
        public string Url { get; set; }
        public string PersistentUrl { get; set; }

        //From public API spec:  https://.....[productionToolsServer]...../api/docs/info.aspx#search_media
        public string ExactTitle { get; set; }

        public string Description { get; set; }

        public string ExactDescription { get; set; }
        public string TopicId { get; set; }
        public string LanguageIsoCode { get; set; }
        public string SourceName { get; set; }
        public string SourceNameExact { get; set; }
        public string SourceAcronym { get; set; }
        public string SourceAcronymExact { get; set; }
        public string SourceUrl { get; set;  }
        public string SourceUrlExact { get; set; }

        public string ShowChildLevel { get; set; }
        public string ShowParentLevel { get; set; }
        public string GeoName { get; set; }
        public string GeoNameId { get; set; }
        public string GeoParentId { get; set; }
        public string CountryCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string DateContentAuthored { get; set; }
        public string ContentAuthoredSinceDate { get; set; }
        public string ContentAuthoredBeforeDate { get; set; }
        public string ContentAuthoredInRange { get; set; }
        public string DateContentUpdated { get; set; }
        public string ContentUpdatedSinceDate { get; set; }
        public string ContentUpdatedBeforeDate { get; set; }
        public string ContentUpdatedInRange { get; set; }
        public string DateContentPublished { get; set; }
        public string ContentPublishedSinceDate { get; set; }
        public string ContentPublishedBeforeDate { get; set; }
        public string ContentPublishedInRange { get; set;  }
        public string DateContentReviewed { get; set;  }
        public string ContentReviewedSinceDate { get; set; }
        public string ContentReviewedBeforeDate { get; set; }
        public string ContentReviewedInRange { get; set; }
        public string DateSyndicationCaptured { get; set; }

        public string SyndicationCapturedSinceDate { get; set; }

        public string SyndicationCapturedBeforeDate { get; set; }
        public string SyndicationCapturedInRange { get; set; }
        public string DateSyndicationUpdated { get; set; }

        public string SyndicationUpdatedSinceDate { get; set; }

        public string SyndicationUpdatedBeforeDate { get; set; }
        public string SyndicationUpdatedInRange { get; set; }
        public string DateSyndicationVisible { get; set; }

        public string SyndicationVisibleSinceDate { get; set; }
        public string SyndicationVisibleBeforeDate { get; set; }
        public string SyndicationVisibleInRange { get; set; }

        //For Paging
        public string PageNumber { get; set; }      //pagenum
        public string RowsPerPage { get; set; }     //max
        public string PageOffset { get; set; }      //offset

        //For sorting, can be multiple fields with direction: title|DESC,mediaType|ASC
        public string Sort { get; set; }

        public string AdminUserGuid { get; set; }

        public string SearchedFrom { get; set; }

        public string ParentId { get; set; }
    }
}
