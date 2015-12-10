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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;

namespace Gov.Hhs.Cdc.MediaProvider
{
    public enum MediaStatusCodeValue { Invalid, Archived, Hidden, Published, Staged };
    [FilterSelection(Code = "FullSearch", CriterionType = FilterCriterionType.Text)]
    //[FilterSelection(Code = "GetAttributes", CriterionType = FilterCriterionType.Boolean)]
    [FilterSelection(Code = "ShowChild", CriterionType = FilterCriterionType.Boolean)]

    //[FilterSelection(Code = "GeoLocationSearch", CriterionType = FilterCriterionType.Boolean)]
    [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey, Code = "geoname")]
    [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey, Code = "countrycode")]
    [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey, Code = "geonameid")]
    [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey, Code = "geoparentid")]
    [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey, Code = "latitude")]
    [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey, Code = "longitude")]
    [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey, Code = "timezone")]
    [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey, Code = "admin1code")]

    [FilterSelection(Code = "SyndicationListGuid", CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey)]
    [FilterSelection(Code = "OnlyDisplayableMediaTypes", CriterionType = FilterCriterionType.Boolean)]

    //[FilterSelection(Code = "HasAttribute", CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey)]
    [FilterSelection(Code = "VocabNameValuePair", CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.StringKey)]

    //TODO: Make these parameters generic
    [FilterSelection(Code = "TopicId", CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.IntKey)]  //Need to change Code from "TopicId" to "Topic"
    [FilterSelection(Code = "Topic", CriterionType = FilterCriterionType.Text)]
    [FilterSelection(Code = "Audience", CriterionType = FilterCriterionType.Text)]

    //v2
    [FilterSelection(Code = "TagIds", CriterionType = FilterCriterionType.MultiSelect, ListKeyType = ListItem.KeyType.IntKey)]

    [FilteredDataSet]
    public class MediaObject : DataSourceBusinessObject, IValidationObject, IRelatedMediaItemContainer
    {
        public MediaObject ShallowCopy()
        {
            return (MediaObject)this.MemberwiseClone();
        }

        #region BusinessObjectProperties

        public string RelationshipTypeName { get; set; }
        public int? RelatedMediaId { get; set; }
        public int? Level { get; set; }
        public int? DisplayOrdinal { get; set; }

        public List<MediaObject> Parents { get; set; }
        public List<MediaObject> Children { get; set; }

        public IEnumerable<EnclosureObject> Enclosures { get; set; }

        //Parameters that are set correctly
        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        public int Id { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        public int MediaId { get; set; }

        public Guid MediaGuid { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Text)]
        public string Title { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        [FilterSelection(Code = "NameContains", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Contains)]
        public string Name { get; set; }

        public string SubTitle { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        [FilterSelection(Code = "DescriptionContains", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Contains)]
        public string Description { get; set; }

        [FilterSelection(Code = "Language", CriterionType = FilterCriterionType.MultiSelect)]    //Need to change Code from "Language" to "LanguageCode"
        [FilterSelection(Code = "LanguageName", CriterionType = FilterCriterionType.MultiSelect)]
        public string LanguageCode { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        public string LanguageIsoCode { get; set; }

        public LanguageItem LangItem { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, Code = "MediaType")]  //Need to change Code from "MediaType" to "MediaTypeCode"
        public string MediaTypeCode { get; set; }

        public string MimeTypeCode { get; set; }
        public string CharacterEncodingCode { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Text)]
        [FilterSelection(Code = "SourceUrlContains", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Contains)]
        public string SourceUrl { get; set; }

        private string _eCardHtml5SourceUrl
        {
            get
            {
                return MediaTypeSpecificDetail != null && MediaTypeSpecificDetail.GetType() == typeof(ECardDetail) ?
                    ((ECardDetail)MediaTypeSpecificDetail).Html5Source :
                    null;
            }
        }

        public string SourceUrlForSave
        {
            get { return !string.IsNullOrEmpty(SourceUrl) ? SourceUrl : _eCardHtml5SourceUrl; }
        }

        public string SourceUrlForApi
        {
            get
            {
                //Temporarily, don't return SourceUrl
                if (string.IsNullOrEmpty(_eCardHtml5SourceUrl))
                {
                    return SourceUrl;
                }
                else if (string.Equals(_eCardHtml5SourceUrl, SourceUrl))
                { return ""; }
                else
                { return SourceUrl ?? ""; }
            }
            set
            {
                SourceUrl = value;
            }
        }

        public string TargetUrl { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        public string PersistentUrlToken { get; set; }


        [FilterSelection(Code = "SourceName", CriterionType = FilterCriterionType.MultiSelect)]
        [FilterSelection(Code = "SourceNameContains", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Contains)]
        public string SourceCode { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        [FilterSelection(Code = "SourceAcronymContains", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Contains)]
        public string SourceAcronym { get; set; }

        public SourceItem Source { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        public int? OwningBusinessUnitId { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect)]
        public int? MaintainingBusinessUnitId { get; set; }

        public string AlternateText { get; set; }
        public string NoScriptText { get; set; }
        public string FeaturedText { get; set; }
        //To validate that it is unique
        public string Author { get; set; }

        public int? Length { get; set; }
        public int? Size { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }

        public int? RatingOverride { get; set; }
        public int RatingMinimum { get; set; }
        public int RatingMaximum { get; set; }
        public string Comments { get; set; }

        public string MediaStatusCode { get; set; }
        public DateTime? LastReviewedDateTime { get; set; }

        public Guid CreatedByGuid { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Guid ModifiedByGuid { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.DateRange)]
        public DateTime? DateContentAuthored { get; set; }

        [FilterSelection(Code = "DateContentUpdated", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentUpdatedSinceDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentUpdatedBeforeDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentUpdatedInRange", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? DateContentUpdated { get; set; }

        [FilterSelection(Code = "DateContentPublished", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentPublishedSinceDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentPublishedBeforeDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentPublishedInRange", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? DateContentPublished { get; set; }

        [FilterSelection(Code = "DateContentReviewed", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentReviewedSinceDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentReviewedBeforeDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "ContentReviewedInRange", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? DateContentReviewed { get; set; }

        [FilterSelection(Code = "DateSyndicationCaptured", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationCapturedSinceDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationCapturedBeforeDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationCapturedInRange", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? DateSyndicationCaptured { get; set; }

        [FilterSelection(Code = "DateSyndicationUpdated", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationUpdatedSinceDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationUpdatedBeforeDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationUpdatedInRange", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? DateSyndicationUpdated { get; set; }

        [FilterSelection(Code = "DateSyndicationVisible", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationVisibleSinceDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationVisibleBeforeDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "SyndicationVisibleInRange", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? DateSyndicationVisible { get; set; }

        public PreferencesSet Preferences { get; set; }

        public string ExtractionClasses { get; set; }
        public string ExtractionElementIds { get; set; }
        public string ExtractionXpath { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Text, TextType = FilterTextType.StartsWith)]
        public string DomainName { get; set; }

        public IEnumerable<AttributeValueObject> AttributeValues { get; set; }

        public string AttributeValuesAsString
        {
            get
            {
                if (AttributeValues == null) { return string.Empty; }
                return string.Join("; ", AttributeValues.Select(t => t.ValueName).ToArray());
            }
        }

        #endregion

        private MediaTypeParms _mediaTypeParms = null;
        public MediaTypeParms MediaTypeParms
        {
            get
            {
                if (_mediaTypeParms == null)
                {
                    _mediaTypeParms = new MediaTypeParms(MediaTypeCode);
                }
                return _mediaTypeParms;
            }
        }

        ////Following is from MediaFeedback table.  Hold off for now
        //public string RatingCount { get; set; }
        //public string Rating { get; set; }
        //public string RatingCommentCount { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, Code = "EffectiveStatus")]
        public string EffectiveStatusCode { get; set; }
        public MediaStatusCodeValue MediaStatusCodeValue
        {
            get
            {
                MediaStatusCodeValue theStatusCode;
                if (!string.IsNullOrEmpty(MediaStatusCode) &&
                    Enum.TryParse<MediaStatusCodeValue>(MediaStatusCode, /*ignoreCase:*/true, out theStatusCode))
                { return theStatusCode; }
                else
                { return MediaStatusCodeValue.Invalid; }
            }
            set
            {
                MediaStatusCode = value.ToString();
            }
        }

        public string OwningBusinessUnitName { get; set; }

        public string MaintainingBusinessUnitName { get; set; }

        public int Popularity { get; set; }

        [FilterSelection(Code = "LastReviewedDate", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? LastReviewedDate { get; set; }

        [FilterSelection(Code = "PublishedDateTime", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? PublishedDateTime { get; set; }

        [FilterSelection(Code = "CreatedDate", CriterionType = FilterCriterionType.DateRange)]
        public DateTime CreatedDate { get; set; }
        [FilterSelection(Code = "ModifiedDateTime", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? ModifiedDateTime { get; set; }

        [FilterSelection(Code = "SyndicationListModifiedDateTime", CriterionType = FilterCriterionType.DateRange)]
        public DateTime? SyndicationListModifiedDateTime { get; set; }

        public bool DisplayOnSearch { get; set; }

        //public IEnumerable<AttributeValueObject> Campaign { get; set; }

        public string Attribution { get; set; }

        public IEnumerable<MediaRelationshipObject> MediaRelationships { get; set; }
        public bool HasRelationships { get { return MediaRelationships != null && MediaRelationships.Count() > 0; } }

        public IEnumerable<MediaGeoDataObject> MediaGeoData { get; set; }
        public bool HasGeoData { get { return MediaGeoData != null && MediaGeoData.Count() > 0; } }

        public IEnumerable<ExtendedAttribute> ExtendedAttributes { get; set; }

        #region EmbedParameterProperties
        public string UserOverriddenEmbedParametersForMediaItem { get; set; }
        public string UserOverriddenEmbedParametersForMediaType { get; set; }
        public string EmbedParametersForMediaItem { get; set; }
        public string EmbedParametersForMediaType { get; set; }


        public string EffectiveEmbedParameters
        {
            get
            {
                return UserOverriddenEmbedParametersForMediaItem ??
                       UserOverriddenEmbedParametersForMediaType ??
                       EmbedParametersForMediaItem ??
                       EmbedParametersForMediaType;
            }
        }
        #endregion

        public string Embedcode { get; set; }

        //For PDFs
        public int? DocumentPageCount { get; set; }
        public string DocumentDataSize { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: Title: {1}, Description: {2}, attributeValues: {3}", Id, Title, Description, AttributeValuesAsString);
        }

        public bool NeedsPersistentUrl()
        {
            List<string> ContentTypesWithPersistentUrl = Gov.Hhs.Cdc.Bo.MediaTypeParms.ContentTypesWithPersistentUrl;
            return ContentTypesWithPersistentUrl.Where(s => s.Equals(MediaTypeCode, StringComparison.OrdinalIgnoreCase)).Any();
        }

        public IEnumerable<AttributeValueObject> SafeGetAttributes()
        {
            if (AttributeValues == null)
            {
                AttributeValues = new List<AttributeValueObject>();
            }
            return AttributeValues;
        }

        public IEnumerable<AttributeValueObject> GetAttributeValues(string attributeName)
        {
            return SafeGetAttributes()
                .Where(a => string.Equals(a.AttributeName, attributeName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        //  output
        public IMediaTypeSpecificDetail MediaTypeSpecificDetail { get; set; }

        public IEnumerable<StorageObject> AlternateImages { get; set; }

        //for paging in newSearch
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int RowsPerPage { get; set; }
        public int PageOffset { get; set; }

        public string OmnitureChannel { get; set; }

        public MediaImage AssociatedImage { get; set; }

        // used to save input
        public IEnumerable<FeedExportObject> ExportSettings { get; set; }

        public bool HasFeedEditorData
        {
            get
            {
                var feed = MediaTypeSpecificDetail as FeedDetailObject;
                if (feed == null)
                { return false; }
                else
                {
                    return
                           !string.IsNullOrEmpty(feed.EditorialManager) ||
                           !string.IsNullOrEmpty(feed.WebMasterEmail) ||
                           !string.IsNullOrEmpty(feed.Copyright);
                }
            }
        }

        public bool HasFeedImage
        {
            get
            {
                return (MediaTypeParms.IsFeed || MediaTypeParms.IsFeedItem) && HasImage();
            }
        }

        public bool HasFeedItemImage
        {
            get
            {
                return MediaTypeParms.IsFeedItem && HasImage();
            }
        }

        public bool HasImage()
        {
            var feed = MediaTypeSpecificDetail as FeedDetailObject;
            if (feed == null)
            {
                return false;
            }
            if (feed.AssociatedImage == null)
            {
                return false;

            }
            return
                  !string.IsNullOrEmpty(feed.AssociatedImage.Title) ||
                  !string.IsNullOrEmpty(feed.AssociatedImage.SourceUrl) ||
                  !string.IsNullOrEmpty(feed.AssociatedImage.TargetUrl) ||
                  (feed.AssociatedImage.Height == null ? false : !string.IsNullOrEmpty(feed.AssociatedImage.Height.Value.ToString())) ||
                  (feed.AssociatedImage.Width == null ? false : !string.IsNullOrEmpty(feed.AssociatedImage.Width.Value.ToString()));
        }

        public IEnumerable<AggregateObject> Aggregates { get; set; }
    }
}
