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
using System.Linq;
using System.Web;
using System.Configuration;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.CsCaching;

namespace Gov.Hhs.Cdc.Api
{
    public class MediaTransformationV2 : MediaTransformationBase, IMediaTransformation
    {
        public MediaTransformationV2()
        {
        }

        public SerialResponse CreateSerialResponse(IEnumerable<MediaObject> mediaObjects, bool forExport = false)
        {
            return new SerialResponse(mediaObjects.Select(a => CreateSerialObject(a, forExport)).ToList());
        }

        public SerialMediaV2 CreateSerialObject(MediaObject obj, bool forExport)
        {
            var media = new SerialMediaV2()
            {
                //1
                id = obj.Id,
                //id = mediaItem.Id.ToString(),

                name = obj.Title,
                //subTitle = mediaItem.SubTitle,
                description = obj.Description,
                //language2 = obj.LanguageCode,
                language = new LanguageTransformationV2().CreateSerialObject(obj.LangItem),
                mediaType = obj.MediaTypeCode,
                //mimeType = mediaItem.MimeTypeCode,    //[admin only]
                //encoding = mediaItem.CharacterEncodingCode,

                //2
                sourceUrl = obj.SourceUrlForApi,
                targetUrl = obj.TargetUrl,
                persistentUrlToken = obj.PersistentUrlToken,

                persistentUrl = ServiceUtility.GetPersistentUrl(obj.PersistentUrlToken),
                url = ServiceUtility.GetResourceUrl(obj.Id, null, forExport),
                embedUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_EMBED, forExport),
                syndicateUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_SYNDICATE, forExport),
                contentUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_CONTENT, forExport),
                thumbnailUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_THUMBNAIL, forExport),
                omnitureChannel = obj.OmnitureChannel,

                //3
                //sourceCode = obj.SourceCode,
                source = new SourceTransformationV2().CreateSerialObject(obj.Source),
                owningOrgId = obj.OwningBusinessUnitId,
                maintainingOrgId = obj.MaintainingBusinessUnitId,

                //4
                alternateText = obj.AlternateText,
                noScriptText = obj.NoScriptText,
                featuredText = obj.FeaturedText,
                author = obj.Author,
                length = obj.Length.ToString(),
                size = obj.Size,
                height = obj.Height,
                width = obj.Width,

                //5
                //ratingOverride = mediaItem.RatingOverride,
                //ratingMinimum = mediaItem.RatingMinimum,
                //ratingMaximum = mediaItem.RatingMaximum,
                //comments = mediaItem.Comments,

                //6
                //active = mediaItem.IsActive,
                status = obj.MediaStatusCode,
                datePublished = obj.PublishedDateTime.FormatAsUtcString(),
                //dateLastReviewed = mediaItem.LastReviewedDate.FormatAsUtcString(),
                //rowVersion = mediaItem.RowVersion.ToBase64String(),      //[admin only],

                //7
                //TODO: Modify this to dynamically set the "topic" and "audience" name
                //tags = MediaTransformationHelper.BuildSerialTags(obj.AttributeValues),
                tags = MediaTransformationHelper.BuildSerialHashTags(obj.AttributeValues),
                geoTags = MediaTransformationHelper.BuildSerialGeoTags(obj.MediaGeoData),

                //8
                //canManage = mediaItem.CanManage,    //[admin only]
                //canShare = mediaItem.CanShare,      //[admin only]

                //21    setBySystemFields
                domainName = obj.DomainName,
                dateModified = obj.ModifiedDateTime.FormatAsUtcString(),
                //dateCreated = mediaItem.CreatedDate.FormatAsUtcString(),    //[admin only]

                //31    derivedFields
                owningOrgName = obj.OwningBusinessUnitName,
                maintainingOrgName = obj.MaintainingBusinessUnitName,
                embedCode = obj.Embedcode,  //PreferenceTransformation.CreateNewSerialPreference(mediaItem.EffectiveEmbedParameters),
                attribution = obj.Attribution,

                //rating = mediaItem.Rating,
                //ratingCount = mediaItem.RatingCount,
                //ratingCommentCount = mediaItem.RatingCommentCount,

                dateContentAuthored = obj.DateContentAuthored.FormatAsUtcString(),
                dateContentPublished = obj.DateContentPublished.FormatAsUtcString(),
                dateContentReviewed = obj.DateContentReviewed.FormatAsUtcString(),
                dateContentUpdated = obj.DateContentUpdated.FormatAsUtcString(),

                dateSyndicationCaptured = obj.DateSyndicationCaptured.FormatAsUtcString(),
                dateSyndicationUpdated = obj.DateSyndicationUpdated.FormatAsUtcString(),
                dateSyndicationVisible = obj.DateSyndicationVisible.FormatAsUtcString(),

                alternateImages = MediaTransformationHelper.BuildSerialAlternateImage(obj.AlternateImages, forExport),

                extendedAttributes = MediaTransformationHelper.BuildExtendedAttributes(obj.ExtendedAttributes),

                extension = BuildMediaExtension(obj),

                pageCount = obj.DocumentPageCount.ToString(),
                dataSize = obj.DocumentDataSize,
                enclosures = MediaTransformationHelper.BuildSerialEnclosures(obj.Enclosures)
            };

            UpdateDynamicUrls(media);
            return media;
        }

        public SerialMediaChildren CreateSerialMediaChildren(MediaObject obj, bool forExport)
        {
            var media = new SerialMediaChildren()
            {
                //1
                id = obj.Id,
                //id = mediaItem.Id.ToString(),

                name = obj.Title,
                //subTitle = mediaItem.SubTitle,
                description = obj.Description,
                //language2 = obj.LanguageCode,
                language = new LanguageTransformationV2().CreateSerialObject(obj.LangItem),
                mediaType = obj.MediaTypeCode,
                //mimeType = mediaItem.MimeTypeCode,    //[admin only]
                //encoding = mediaItem.CharacterEncodingCode,

                //2
                sourceUrl = obj.SourceUrlForApi,
                targetUrl = obj.TargetUrl,
                persistentUrlToken = obj.PersistentUrlToken,

                persistentUrl = ServiceUtility.GetPersistentUrl(obj.PersistentUrlToken),
                url = ServiceUtility.GetResourceUrl(obj.Id, null, forExport),
                embedUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_EMBED, forExport),
                syndicateUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_SYNDICATE, forExport),
                contentUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_CONTENT, forExport),
                thumbnailUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_THUMBNAIL, forExport),

                //3
                //sourceCode = obj.SourceCode,
                source = new SourceTransformationV2().CreateSerialObject(obj.Source),
                owningOrgId = obj.OwningBusinessUnitId,
                maintainingOrgId = obj.MaintainingBusinessUnitId,

                //4
                alternateText = obj.AlternateText,
                noScriptText = obj.NoScriptText,
                featuredText = obj.FeaturedText,
                author = obj.Author,
                length = obj.Length.ToString(),
                size = obj.Size,
                height = obj.Height,
                width = obj.Width,

                //5
                //ratingOverride = mediaItem.RatingOverride,
                //ratingMinimum = mediaItem.RatingMinimum,
                //ratingMaximum = mediaItem.RatingMaximum,
                //comments = mediaItem.Comments,

                //6
                //active = mediaItem.IsActive,
                status = obj.MediaStatusCode,
                datePublished = obj.PublishedDateTime.FormatAsUtcString(),
                //dateLastReviewed = mediaItem.LastReviewedDate.FormatAsUtcString(),
                //rowVersion = mediaItem.RowVersion.ToBase64String(),      //[admin only],

                //7
                tags = MediaTransformationHelper.BuildSerialHashTags(obj.AttributeValues),
                geoTags = MediaTransformationHelper.BuildSerialGeoTags(obj.MediaGeoData),

                //8
                //canManage = mediaItem.CanManage,    //[admin only]
                //canShare = mediaItem.CanShare,      //[admin only]

                //21    setBySystemFields
                domainName = obj.DomainName,
                dateModified = obj.ModifiedDateTime.FormatAsUtcString(),
                //dateCreated = mediaItem.CreatedDate.FormatAsUtcString(),    //[admin only]

                //31    derivedFields
                owningOrgName = obj.OwningBusinessUnitName,
                maintainingOrgName = obj.MaintainingBusinessUnitName,
                embedCode = obj.Embedcode,  //PreferenceTransformation.CreateNewSerialPreference(mediaItem.EffectiveEmbedParameters),
                attribution = obj.Attribution,

                //rating = mediaItem.Rating,
                //ratingCount = mediaItem.RatingCount,
                //ratingCommentCount = mediaItem.RatingCommentCount,

                dateContentAuthored = obj.DateContentAuthored.FormatAsUtcString(),
                dateContentPublished = obj.DateContentPublished.FormatAsUtcString(),
                dateContentReviewed = obj.DateContentReviewed.FormatAsUtcString(),
                dateContentUpdated = obj.DateContentUpdated.FormatAsUtcString(),
                dateSyndicationCaptured = obj.DateSyndicationCaptured.FormatAsUtcString(),
                dateSyndicationUpdated = obj.DateSyndicationUpdated.FormatAsUtcString(),
                dateSyndicationVisible = obj.DateSyndicationVisible.FormatAsUtcString(),

                alternateImages = MediaTransformationHelper.BuildSerialAlternateImage(obj.AlternateImages, forExport),

                extendedAttributes = MediaTransformationHelper.BuildExtendedAttributes(obj.ExtendedAttributes),

                extension = BuildMediaExtension(obj),

                enclosures = MediaTransformationHelper.BuildSerialEnclosures(obj.Enclosures)
            };

            UpdateDynamicUrls(media);
            return media;
        }

        public SerialMediaParent CreateSerialMediaParent(MediaObject obj, bool forExport)
        {
            var media = new SerialMediaParent()
            {
                //1
                id = obj.Id,
                //id = mediaItem.Id.ToString(),

                name = obj.Title,
                //subTitle = mediaItem.SubTitle,
                description = obj.Description,
                //language2 = obj.LanguageCode,
                language = new LanguageTransformationV2().CreateSerialObject(obj.LangItem),
                mediaType = obj.MediaTypeCode,
                //mimeType = mediaItem.MimeTypeCode,    //[admin only]
                //encoding = mediaItem.CharacterEncodingCode,

                //2
                sourceUrl = obj.SourceUrlForApi,
                targetUrl = obj.TargetUrl,
                persistentUrlToken = obj.PersistentUrlToken,

                persistentUrl = ServiceUtility.GetPersistentUrl(obj.PersistentUrlToken),
                url = ServiceUtility.GetResourceUrl(obj.Id, null, forExport),
                embedUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_EMBED, forExport),
                syndicateUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_SYNDICATE, forExport),
                contentUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_CONTENT, forExport),
                thumbnailUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_THUMBNAIL, forExport),

                //3
                //sourceCode = obj.SourceCode,
                source = new SourceTransformationV2().CreateSerialObject(obj.Source),
                owningOrgId = obj.OwningBusinessUnitId,
                maintainingOrgId = obj.MaintainingBusinessUnitId,

                //4
                alternateText = obj.AlternateText,
                noScriptText = obj.NoScriptText,
                featuredText = obj.FeaturedText,
                author = obj.Author,
                length = obj.Length.ToString(),
                size = obj.Size,
                height = obj.Height,
                width = obj.Width,

                //5
                //ratingOverride = mediaItem.RatingOverride,
                //ratingMinimum = mediaItem.RatingMinimum,
                //ratingMaximum = mediaItem.RatingMaximum,
                //comments = mediaItem.Comments,

                //6
                //active = mediaItem.IsActive,
                status = obj.MediaStatusCode,
                datePublished = obj.PublishedDateTime.FormatAsUtcString(),
                //dateLastReviewed = mediaItem.LastReviewedDate.FormatAsUtcString(),
                //rowVersion = mediaItem.RowVersion.ToBase64String(),      //[admin only],

                //7
                tags = MediaTransformationHelper.BuildSerialHashTags(obj.AttributeValues),
                geoTags = MediaTransformationHelper.BuildSerialGeoTags(obj.MediaGeoData),

                //8
                //canManage = mediaItem.CanManage,    //[admin only]
                //canShare = mediaItem.CanShare,      //[admin only]

                //21    setBySystemFields
                domainName = obj.DomainName,
                dateModified = obj.ModifiedDateTime.FormatAsUtcString(),
                //dateCreated = mediaItem.CreatedDate.FormatAsUtcString(),    //[admin only]

                //31    derivedFields
                owningOrgName = obj.OwningBusinessUnitName,
                maintainingOrgName = obj.MaintainingBusinessUnitName,
                embedCode = obj.Embedcode,  //PreferenceTransformation.CreateNewSerialPreference(mediaItem.EffectiveEmbedParameters),
                attribution = obj.Attribution,

                //rating = mediaItem.Rating,
                //ratingCount = mediaItem.RatingCount,
                //ratingCommentCount = mediaItem.RatingCommentCount,

                dateContentAuthored = obj.DateContentAuthored.FormatAsUtcString(),
                dateContentPublished = obj.DateContentPublished.FormatAsUtcString(),
                dateContentReviewed = obj.DateContentReviewed.FormatAsUtcString(),
                dateContentUpdated = obj.DateContentUpdated.FormatAsUtcString(),
                dateSyndicationCaptured = obj.DateSyndicationCaptured.FormatAsUtcString(),
                dateSyndicationUpdated = obj.DateSyndicationUpdated.FormatAsUtcString(),
                dateSyndicationVisible = obj.DateSyndicationVisible.FormatAsUtcString(),

                alternateImages = MediaTransformationHelper.BuildSerialAlternateImage(obj.AlternateImages, forExport),

                extendedAttributes = MediaTransformationHelper.BuildExtendedAttributes(obj.ExtendedAttributes),

                extension = BuildMediaExtension(obj),

                enclosures = MediaTransformationHelper.BuildSerialEnclosures(obj.Enclosures)
            };

            UpdateDynamicUrls(media);
            return media;
        }

        protected static object BuildMediaExtension(MediaObject mediaItem)
        {
            if (string.IsNullOrEmpty(mediaItem.MediaTypeCode))
            {
                return new object();
            }

            switch (mediaItem.MediaTypeCode.ToLower())
            {
                case "ecard":
                    return ECardDetailTransformation.GetSerialECardDetail(
                        (ECardDetail)mediaItem.MediaTypeSpecificDetail);
                default:
                    return new object();
            }
        }

    }
}
