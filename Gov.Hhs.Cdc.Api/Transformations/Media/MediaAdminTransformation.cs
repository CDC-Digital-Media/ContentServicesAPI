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

using System.IO;
using System.Text;

using System.Xml.Linq;
using System.Xml.Serialization;

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaValidatonProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.Logging;


namespace Gov.Hhs.Cdc.Api
{
    public class MediaAdminTransformation : MediaTransformationBase, IMediaTransformation
    {
        public MediaAdminTransformation()
        {
        }

        public MediaObject CreateNewMediaAndDetail(SerialMediaAdmin serialMedia, Guid currentUser)
        {
            MediaObject mediaObj = CreateMediaObject(serialMedia, currentUser);
            switch ((serialMedia.mediaType ?? "").ToLower())
            {
                case "ecard":
                    mediaObj.MediaTypeSpecificDetail = ECardDetailTransformation.CreateNewECardObject((SerialECardDetail)serialMedia.eCard);
                    break;
                case "feed":
                case "feed item":
                case "feed - proxy":
                case "feed - import":
                case "feed - aggregate":
                    if (serialMedia.feed != null)
                    {
                        var feed = serialMedia.feed;

                        var detail = new FeedDetailObject()
                        {
                            EditorialManager = feed.editorialManager,
                            WebMasterEmail = feed.webMasterEmail,
                            Copyright = feed.copyright
                        };
                        
                        if (!string.IsNullOrEmpty(feed.imageTitle + feed.imageDescription + feed.imageSource + feed.imageLink + feed.imageHeight + feed.imageWidth))
                        {
                            var image = new MediaImage
                            {
                                Title = feed.imageTitle,
                                Description = feed.imageDescription,
                                SourceUrl = feed.imageSource,
                                TargetUrl = feed.imageLink,
                                Height = feed.imageHeight.ToNullableInt32(),
                                Width = feed.imageWidth.ToNullableInt32()
                            };

                            detail.AssociatedImage = image;
                        }

                        var export = new List<FeedExportObject>();
                        if (feed.exportSettings != null && feed.exportSettings.Count > 0)
                        {
                            export = feed.exportSettings.Select(a => new FeedExportObject()
                            {
                                Id = a.feedExportId,
                                FilePath = a.filePath,
                                FeedFormatName = a.feedFormat
                            }).ToList();
                        }
                        detail.ExportSettings = export;
                        mediaObj.ExportSettings = detail.ExportSettings;

                        mediaObj.MediaTypeSpecificDetail = detail;
                        mediaObj.AssociatedImage = detail.AssociatedImage;
                    }
                    break;
                default:
                    break;
            }
            return mediaObj;
        }

        public MediaObject CreateMediaObject(SerialMediaAdmin media, Guid currentUser)
        {
            int id = ServiceUtility.ParseInt(media.mediaId);
            int length = ServiceUtility.ParseInt(media.length);

            MediaObject obj = new MediaObject()
            {
                //1
                Id = id,
                Title = media.title,
                SubTitle = media.subTitle,
                Description = media.description,
                LanguageCode = media.language,
                MediaTypeCode = media.mediaType,
                MimeTypeCode = media.mimeType,
                CharacterEncodingCode = media.encoding,
                ModifiedByGuid = currentUser,
                MediaGuid = string.IsNullOrEmpty(media.mediaGuid) ? Guid.Parse("00000000-0000-0000-0000-000000000000") : Guid.Parse(media.mediaGuid),

                //2
                SourceUrlForApi = media.sourceUrl,
                TargetUrl = media.targetUrl,
                PersistentUrlToken = media.persistentUrlToken,

                //3
                SourceCode = media.sourceCode,
                OwningBusinessUnitId = media.owningOrgId,
                MaintainingBusinessUnitId = media.maintainingOrgId,

                //4
                AlternateText = media.alternateText,
                NoScriptText = media.noScriptText,
                FeaturedText = media.featuredText,
                Author = media.author,
                Length = media.length.ToNullableInt32(),
                Size = media.size,
                Height = media.height,
                Width = media.width,

                //5
                RatingOverride = media.ratingOverride,
                RatingMinimum = media.ratingMinimum,
                RatingMaximum = media.ratingMaximum,
                Comments = media.comments,

                //6
                MediaStatusCode = media.status,
                PublishedDateTime = media.datePublished.ParseUtcDateTime(),
                LastReviewedDateTime = media.dateLastReviewed.ParseUtcDateTime(),
                RowVersion = media.rowVersion.ToBytes(),

                //7
                AttributeValues = MediaTransformationHelper.CreateAttributeValues(media.language, media.topics, media.audiences, media.tags),

                //8
                Preferences = new PreferencesSet()
                {
                    PreferencesPersistedForMediaItem = PreferenceTransformation.CreateMediaPreferences(media.preferences)
                },
                ExtractionClasses = string.IsNullOrEmpty(media.classNames) ? PreferenceTransformation.ClassNames(media.preferences) : media.classNames,
                ExtractionElementIds = string.IsNullOrEmpty(media.elementIds) ? PreferenceTransformation.ElementIds(media.preferences) : media.elementIds,
                ExtractionXpath = string.IsNullOrEmpty(media.xPath) ? PreferenceTransformation.XPath(media.preferences) : media.xPath,
                Embedcode = media.embedcode,
                MediaRelationships = CreateRelationships(id, media.childRelationships, media.parentRelationships).AsEnumerable(),
                MediaGeoData = CreateGeoDatas(id, media.geoTags).AsEnumerable(),
                ExtendedAttributes = CreateExtendedAttributes(id, media.extendedAttributes),

                DocumentPageCount = ServiceUtility.ParseInt(media.pageCount),
                DocumentDataSize = media.dataSize,
                OmnitureChannel = media.omnitureChannel,

                DateContentAuthored = media.dateContentAuthored.ParseUtcDateTime(),
                DateContentUpdated = media.dateContentUpdated.ParseUtcDateTime(),
                DateContentPublished = media.dateContentPublished.ParseUtcDateTime(),
                DateContentReviewed = media.dateContentReviewed.ParseUtcDateTime(),

                Enclosures = CreateEnclosures(media.enclosures),
                Aggregates = CreateAggregates(media.feedAggregates)
            };

            if (id == 0)
            {
                obj.CreatedByGuid = currentUser;
            }

            return obj;
        }

        private IEnumerable<EnclosureObject> CreateEnclosures(List<SerialEnclosure> list)
        {
            var obj = new List<EnclosureObject>();
            if (list != null)
            {
                obj.AddRange(list.Select(a =>
                    new EnclosureObject()
                    {
                        Id = a.id,
                        ResourceUrl = a.resourceUrl,
                        ContentType = a.contentType,
                        Size = a.size
                    }).ToList());
            }

            return obj.AsEnumerable();
        }

        private IEnumerable<AggregateObject> CreateAggregates(List<SerialAggregate> list)
        {
            var obj = new List<AggregateObject>();
            if (list != null)
            {
                obj.AddRange(list.Select(a =>
                    new AggregateObject()
                    {
                        Id = a.id,
                        SearchXML = TransformNameValuePairToSearchCriteria(a.queryString)
                    }).ToList());
            }

            return obj.AsEnumerable();
        }

        private string TransformNameValuePairToSearchCriteria(string nameValuePair)
        {
            var xmlSearch = new CsBusinessObjects.Media.SearchCriteria();

            var dict = RestHelper.CreateDictionary(nameValuePair);
            foreach (var entry in dict)
            {
                switch (entry.Key)
                {
                    case Param.CONTENT_GROUP:
                        xmlSearch.ContentGroup = entry.Value;
                        break;
                    case Param.PAGE_RECORD_MAX:
                        xmlSearch.RowsPerPage = entry.Value;
                        break;
                    case Param.TOPIC_ID_V2:
                        xmlSearch.TopicId = entry.Value;
                        break;
                    default: break;
                }
            }

            return xmlSearch.ToXmlString();
        }

        private IList<MediaRelationshipObject> CreateRelationships(int mediaId, IList<SerialMediaRelationship> childRelationships, IList<SerialMediaRelationship> parentRelationships)
        {
            var relationships = CreateRelationships(mediaId, childRelationships, "Is Parent Of");

            var rel2 = CreateRelationships(mediaId, parentRelationships, "Is Child Of");

            if (relationships == null)
            {
                return rel2;
            }
            if (rel2 == null)
            {
                return relationships;
            }
            relationships.AddRange(rel2);

            return relationships;
        }

        public List<MediaRelationshipObject> CreateRelationships(int mediaId, IList<SerialMediaRelationship> medias, string relationshipTypeName)
        {
            if (medias == null)
            {
                return null;
            }

            List<MediaRelationshipObject> results = (medias.Select(m => new MediaRelationshipObject()
            {
                MediaId = mediaId,
                IsActive = true,
                RelationshipTypeName = relationshipTypeName,
                DisplayOrdinal = m.displayOrdinal,
                RelatedMediaId = m.relatedMediaId
            })).ToList();

            return results;
        }

        public List<MediaGeoDataObject> CreateGeoDatas(int mediaId, List<SerialGeoTag> medias)
        {
            if (medias == null)
            {
                return null;
            }

            List<MediaGeoDataObject> results = (medias.Select(m => new MediaGeoDataObject()
            {
                MediaId = mediaId,
                GeoNameId = (int)m.geoNameId
            })).ToList();

            return results;
        }

        public static List<CsBusinessObjects.Media.ExtendedAttribute> CreateExtendedAttributes(int mediaId, Dictionary<string, string> attr)
        {
            var list = new List<CsBusinessObjects.Media.ExtendedAttribute>();
            if (attr == null) { return list; }

            foreach (var att in attr)
            {
                list.Add(new CsBusinessObjects.Media.ExtendedAttribute { Name = att.Key, Value = att.Value });
            }
            return list;
        }

        public SerialResponse CreateSerialResponse(IEnumerable<MediaObject> mediaObject, bool forExport = false)
        {
            return new SerialResponse(mediaObject.Select(a => CreateSerialObject(a)).ToList());
        }

        public SerialMediaAdmin CreateSerialObject(MediaObject mediaObject, bool forExport = false)
        {
            SerialMediaAdmin serialMedia = BuildSerialMediaAdmin(mediaObject);

            if (!string.IsNullOrEmpty(serialMedia.mediaType))
            {
                switch (serialMedia.mediaType.ToLower())
                {
                    case "ecard":
                        serialMedia.eCard = ECardDetailTransformation.GetSerialECardDetail((ECardDetail)mediaObject.MediaTypeSpecificDetail);
                        break;
                    case "feed":
                    case "feed item":
                    case "feed - proxy":
                    case "feed - import":
                    case "feed - aggregate":
                        var feed = mediaObject.MediaTypeSpecificDetail as FeedDetailObject;
                        if (feed != null)
                        {
                            serialMedia.feed = SerialFeedFromFeed(feed);
                        }
                        break;
                    default:
                        break;
                }
            }

            return serialMedia;
        }

        private static SerialFeedDetail SerialFeedFromFeed(FeedDetailObject feed)
        {
            var image = new MediaImage();
            if (feed.AssociatedImage != null)
            {
                image = feed.AssociatedImage;
            }

            var export = new List<SerialFeedExport>();
            if (feed.ExportSettings != null && feed.ExportSettings.Count > 0)
            {
                export = feed.ExportSettings.Select(a => new SerialFeedExport()
                {
                    feedExportId = a.Id,
                    filePath = a.FilePath,
                    feedFormat = a.FeedFormatName
                }).ToList();
            }

            return new SerialFeedDetail
            {
                editorialManager = feed.EditorialManager,
                webMasterEmail = feed.WebMasterEmail,
                copyright = feed.Copyright,

                imageTitle = image.Title,
                imageDescription = image.Description,
                imageSource = image.SourceUrl,
                imageLink = image.TargetUrl,
                imageHeight = image.Height.HasValue ? image.Height.Value.ToString() : "",
                imageWidth = image.Width.HasValue ? image.Width.Value.ToString() : "",

                exportSettings = export
            };
        }

        private SerialMediaAdmin BuildSerialMediaAdmin(MediaObject mediaItem)
        {
            IEnumerable<MediaGeoDataObject> geoDatas = mediaItem.MediaGeoData;
            List<SerialGeoTag> geoTags = null;
            if (geoDatas != null)
            {
                geoTags = geoDatas
                   .OrderBy(r => r.GeoNameId)
                   .Select(r => CreateNewSerialGeoTag(r)).ToList();
            }
            else
            {
                geoTags = new List<SerialGeoTag>();
            }

            List<SerialMediaRelationship> children = null;
            List<SerialMediaRelationship> parents = null;
            var childRelationshipCount = 0;            
            var parentsCount = 0;
            if (mediaItem.MediaRelationships == null)
            {
                mediaItem.MediaRelationships = new List<MediaRelationshipObject>();
            }
            else
            {
                var relationships = mediaItem.MediaRelationships.Where(r => r.IsActive && r.IsCommitted);

                // children
                children = relationships.Where(r => r.RelationshipTypeName == "Is Parent Of")
                .OrderBy(r => r.DisplayOrdinal)
                .Select(r => CreateNewSerialMediaRelationship(r)).ToList();

                childRelationshipCount = children.Count();

                // parent
                parents = relationships.Where(r => r.RelationshipTypeName == "Is Child Of")
                .OrderBy(r => r.DisplayOrdinal)
                .Select(r => CreateNewSerialMediaRelationship(r)).ToList();

                parentsCount = parents.Count();
            }
          
            List<SerialMediaAdmin> childObjects = null;
            var childrenCount = 0;

            if (mediaItem.Children != null)
            {
                childObjects = mediaItem.Children.Select(c => new SerialMediaAdmin { mediaId = c.MediaId.ToString() }).ToList();
                if (childrenCount == 0)
                {
                    childrenCount = childObjects.Count();
                }
            }

            List<SerialMediaAdmin> parentObjects = null;
            if (mediaItem.Parents != null)
            {
                parentObjects = mediaItem.Parents.Select(c => new SerialMediaAdmin { mediaId = c.MediaId.ToString() }).ToList();
                if (parentsCount == 0)
                {
                    parentsCount = parentObjects.Count(); 
                }
            }            

            if (mediaItem.Preferences == null)
            {
                mediaItem.Preferences = new PreferencesSet();
            }

            SerialMediaAdmin serialMedia = new SerialMediaAdmin()
            {
                //1
                mediaId = mediaItem.Id.ToString(),
                id = mediaItem.Id.ToString(),
                title = mediaItem.Title,
                subTitle = mediaItem.SubTitle,
                description = mediaItem.Description,
                language = mediaItem.LanguageCode,
                mediaType = mediaItem.MediaTypeCode,
                mimeType = mediaItem.MimeTypeCode,    //[admin only]
                encoding = mediaItem.CharacterEncodingCode,
                mediaGuid = mediaItem.MediaGuid.ToString(),

                //2
                sourceUrl = mediaItem.SourceUrlForApi,
                targetUrl = mediaItem.TargetUrl,
                persistentUrlToken = mediaItem.PersistentUrlToken,

                persistentUrl = ServiceUtility.GetPersistentUrl(mediaItem.PersistentUrlToken),

                //3
                sourceCode = mediaItem.SourceCode,
                owningOrgId = mediaItem.OwningBusinessUnitId,
                maintainingOrgId = mediaItem.MaintainingBusinessUnitId,

                //4
                alternateImages = MediaTransformationHelper.BuildSerialAlternateImage(mediaItem.AlternateImages, forExport: false),
                alternateText = mediaItem.AlternateText,
                noScriptText = mediaItem.NoScriptText,
                featuredText = mediaItem.FeaturedText,
                author = mediaItem.Author,
                length = mediaItem.Length.ToString(),
                size = mediaItem.Size,
                height = mediaItem.Height,
                width = mediaItem.Width,

                //41
                childRelationshipCount = childRelationshipCount,
                childRelationships = children,

                childCount = childrenCount,
                children = childObjects,

                parentCount = parents.Count(),
                parentRelationships = parents,
                parents = parentObjects,

                //5
                ratingOverride = mediaItem.RatingOverride,
                ratingMinimum = mediaItem.RatingMinimum,
                ratingMaximum = mediaItem.RatingMaximum,
                comments = mediaItem.Comments,

                //6
                status = mediaItem.MediaStatusCode,
                datePublished = mediaItem.PublishedDateTime.FormatAsUtcString(),
                dateLastReviewed = mediaItem.LastReviewedDate.FormatAsUtcString(),
                rowVersion = mediaItem.RowVersion == null ? new byte[0].ToBase64String() : mediaItem.RowVersion.ToBase64String(),      //[admin only],
                dateModified = mediaItem.ModifiedDateTime.FormatAsUtcString(),

                //7
                tags = MediaTransformationHelper.BuildSerialAttributeDictionary(mediaItem.AttributeValues),

                //8

                //21    setBySystemFields
                domainName = mediaItem.DomainName,
                dateCreated = mediaItem.CreatedDate.FormatAsUtcString(),    //[admin only]
                createdBy = UserIdFromGuid(mediaItem.CreatedByGuid),
                modifiedBy = UserIdFromGuid(mediaItem.ModifiedByGuid),

                //31    derivedFields
                thumbnailUrl = ServiceUtility.GetResourceUrl(mediaItem.Id, Param.MEDIA_THUMBNAIL),
                owningOrgName = mediaItem.OwningBusinessUnitName,
                maintainingOrgName = mediaItem.MaintainingBusinessUnitName,
                effectivePreferences = PreferenceTransformation.CreateSerialMediaPreferences(
                    mediaItem.Preferences.Effective),
                preferences = PreferenceTransformation.CreateSerialMediaPreferences(
                    mediaItem.Preferences.PreferencesPersistedForMediaItem),
                classNames = mediaItem.Preferences.ClassNames(),
                elementIds = mediaItem.Preferences.ElementIds(),
                xPath = mediaItem.Preferences.XPath(),
                attribution = mediaItem.Attribution,

                embedcode = mediaItem.Embedcode,

                geoTags = geoTags,

                extendedAttributes = MediaTransformationHelper.BuildExtendedAttributes(mediaItem.ExtendedAttributes),

                pageCount = mediaItem.DocumentPageCount.ToString(),
                dataSize = mediaItem.DocumentDataSize,
                omnitureChannel = mediaItem.OmnitureChannel,

                //rating = mediaItem.Rating,
                //ratingCount = mediaItem.RatingCount,
                //ratingCommentCount = mediaItem.RatingCommentCount,                

                //valueItemLists = MediaSearchHandler.GetValueItemLists(mediaItem.Attributes),

                dateContentAuthored = mediaItem.DateContentAuthored.FormatAsUtcString(),
                dateContentPublished = mediaItem.DateContentPublished.FormatAsUtcString(),
                dateContentReviewed = mediaItem.DateContentReviewed.FormatAsUtcString(),
                dateContentUpdated = mediaItem.DateContentUpdated.FormatAsUtcString(),

                dateSyndicationCaptured = mediaItem.DateSyndicationCaptured.FormatAsUtcString(),
                dateSyndicationUpdated = mediaItem.DateSyndicationUpdated.FormatAsUtcString(),
                dateSyndicationVisible = mediaItem.DateSyndicationVisible.FormatAsUtcString(),
                enclosures = MediaTransformationHelper.BuildSerialEnclosures(mediaItem.Enclosures),
                feedAggregates = MediaTransformationHelper.BuildSerialAggregates(mediaItem.Aggregates)
            };

            UpdateDynamicUrls(serialMedia);
            return serialMedia;
        }

        private string UserIdFromGuid(Guid guid)
        {
            var users = AuthorizationManager.GetUsers();
            var user = users.FirstOrDefault(u => u.UserGuid == guid);
            if (user == null)
            {
                Logger.LogInfo("Guid " + guid.ToString() + " not found in Admin Users");
                return "Unknown User " + guid.ToString();
            }
            return user.Name + " (" + user.UserName + ")";
        }

        private static SerialMediaRelationship CreateNewSerialMediaRelationship(MediaRelationshipObject r)
        {
            return new SerialMediaRelationship() { relatedMediaId = r.RelatedMediaId, displayOrdinal = r.DisplayOrdinal };
        }

        private static SerialGeoTag CreateNewSerialGeoTag(MediaGeoDataObject r)
        {
            return new SerialGeoTag()
            {
                geoNameId = r.GeoNameId,
                name = r.Name,
                countryCode = r.CountryCode,
                parentId = r.ParentId,
                latitude = r.Latitude,
                longitude = r.Longitude,
                timezone = r.Timezone,
                admin1Code = r.Admin1Code
            };
        }

    }
}
