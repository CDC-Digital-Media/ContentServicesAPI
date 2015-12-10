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
using System.Web;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class MediaTransformationV1 : MediaTransformationBase, IMediaTransformation
    {
        public MediaTransformationV1()
        {
        }

        public SerialResponse CreateSerialResponse(IEnumerable<MediaObject> mediaObjects, bool forExport = false)
        {
            return new SerialResponse(mediaObjects.Select(a => CreateSerialObject(a)).ToList());
        }

        public SerialMediaV1 CreateSerialObject(MediaObject obj)
        {
            var media = new SerialMediaV1()
            {
                //1
                mediaId = obj.Id.ToString(),
                title = obj.Title,
                description = obj.Description,
                language = obj.LanguageCode,
                mediaType = obj.MediaTypeCode,

                //2
                sourceUrl = obj.SourceUrlForApi,
                targetUrl = obj.TargetUrl,
                persistentUrlToken = obj.PersistentUrlToken,

                persistentUrl = ServiceUtility.GetPersistentUrl(obj.PersistentUrlToken),
                url = ServiceUtility.GetResourceUrl(obj.Id, null),
                embedUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_EMBED),
                syndicateUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_SYNDICATE),
                contentUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_CONTENT),
                thumbnailUrl = ServiceUtility.GetResourceUrl(obj.Id, Param.MEDIA_THUMBNAIL),

                //3
                sourceCode = obj.SourceCode,  //renamed from sourceCode to source
                owningOrgId = obj.OwningBusinessUnitId,
                maintainingOrgId = obj.MaintainingBusinessUnitId,

                status = obj.MediaStatusCode,
                datePublished = SafeToString(obj.PublishedDateTime),
                //dateLastReviewed = mediaItem.LastReviewedDate.FormatAsUtcString(),
                //rowVersion = mediaItem.RowVersion.ToBase64String(),      //[admin only],

                //7
                tags = MediaTransformationHelper.BuildSerialTags(obj.AttributeValues),


                //8
                //canManage = mediaItem.CanManage,    //[admin only]
                //canShare = mediaItem.CanShare,      //[admin only]

                //21    setBySystemFields
                domainName = obj.DomainName,
                dateModified = SafeToString(obj.ModifiedDateTime),
                //dateCreated = mediaItem.CreatedDate.FormatAsUtcString(),    //[admin only]

                //31    derivedFields
                owningOrgName = obj.OwningBusinessUnitName,
                maintainingOrgName = obj.MaintainingBusinessUnitName,
                embedCode = obj.Embedcode,
                //PreferenceTransformation.CreateNewSerialPreference(obj.EmbedParametersForMediaItem),
                attribution = obj.Attribution,
                enclosures = MediaTransformationHelper.BuildSerialEnclosures(obj.Enclosures)

                //rating = mediaItem.Rating,
                //ratingCount = mediaItem.RatingCount,
                //ratingCommentCount = mediaItem.RatingCommentCount,

                //version2
                //extension = BuildMediaExtension(mediaItem)
            };

            UpdateDynamicUrls(media);
            return media;
        }

        protected static string SafeToString(DateTime? date)
        {
            return date == null ? "" : ((DateTime)date).ToString("yyyy-MM-dd hh:mm:ss");
        }
    }
}
