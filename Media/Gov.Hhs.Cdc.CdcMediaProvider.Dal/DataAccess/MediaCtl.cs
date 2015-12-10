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
using System.Data.Entity;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

using Gov.Hhs.Cdc;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.MediaProvider;
using Newtonsoft.Json;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class MediaCtl
         : BaseCtl<MediaObject, Medias, MediaCtl, MediaObjectContext>
    {
        public MediaCtl()
        {
        }

        public MediaCtl(MediaObjectContext media, MediaObject newBusinessObject)
        {
            DataEntities = media;
            SetBusinessObjects(newBusinessObject, newBusinessObject);
        }

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.Id = PersistedDbObject.MediaId;
            PersistedBusinessObject.RowVersion = PersistedDbObject.RowVersion;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name + ", MediaId=" + PersistedBusinessObject.Id.ToString();
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return false;
        }

        public override bool VersionMatches()
        {
            if (NewBusinessObject.RowVersion == null)
            {
                throw new ApplicationException("RowVersion is missing.");
            }
            if (PersistedDbObject == null)
            {
                return false;
            }
            return PersistedDbObject.RowVersion.SequenceEqual(NewBusinessObject.RowVersion);
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Medias();
            PersistedDbObject.MediaGuid = Guid.NewGuid();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
            PersistedDbObject.DateSyndicationCaptured = modifiedDateTime;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.SourceCode = HttpUtility.HtmlDecode(NewBusinessObject.SourceCode ?? "");
            PersistedDbObject.OwningBusinessUnitId = NewBusinessObject.OwningBusinessUnitId;
            PersistedDbObject.MaintainingBusinessUnitId = NewBusinessObject.MaintainingBusinessUnitId;

            PersistedDbObject.LanguageCode = HttpUtility.HtmlDecode(NewBusinessObject.LanguageCode ?? "");
            PersistedDbObject.MediaTypeCode = HttpUtility.HtmlDecode(NewBusinessObject.MediaTypeCode ?? "");
            PersistedDbObject.MimeTypeCode = HttpUtility.HtmlDecode(NewBusinessObject.MimeTypeCode ?? "");
            PersistedDbObject.CharacterEncodingCode = HttpUtility.HtmlDecode(NewBusinessObject.CharacterEncodingCode ?? "");

            PersistedDbObject.Title = HttpUtility.HtmlDecode(NewBusinessObject.Title ?? "");
            PersistedDbObject.SubTitle = HttpUtility.HtmlDecode(NewBusinessObject.SubTitle ?? "");
            PersistedDbObject.Description = HttpUtility.HtmlDecode(NewBusinessObject.Description ?? "");
            PersistedDbObject.SourceUrl = NewBusinessObject.SourceUrlForSave;
            PersistedDbObject.TargetUrl = NewBusinessObject.TargetUrl;
            PersistedDbObject.AlternateText = HttpUtility.HtmlDecode(NewBusinessObject.AlternateText ?? "");
            PersistedDbObject.NoScriptText = HttpUtility.HtmlDecode(NewBusinessObject.NoScriptText ?? "");
            PersistedDbObject.FeaturedText = HttpUtility.HtmlDecode(NewBusinessObject.FeaturedText ?? "");

            PersistedDbObject.Author = HttpUtility.HtmlDecode(NewBusinessObject.Author ?? "");
            PersistedDbObject.Length = NewBusinessObject.Length;
            PersistedDbObject.Height = NewBusinessObject.Height;
            PersistedDbObject.Width = NewBusinessObject.Width;
            PersistedDbObject.RatingOverride = NewBusinessObject.RatingOverride;
            PersistedDbObject.RatingMinimum = NewBusinessObject.RatingMinimum;
            PersistedDbObject.RatingMaximum = NewBusinessObject.RatingMaximum;

            PersistedDbObject.Comments = HttpUtility.HtmlDecode(NewBusinessObject.Comments ?? "");

            if (NewBusinessObject.MediaStatusCodeValue == MediaStatusCodeValue.Published)
            {
                PersistedDbObject.PublishedDateTime = NewBusinessObject.PublishedDateTime;
                PersistedDbObject.DateSyndicationVisible = NewBusinessObject.PublishedDateTime;
            }

            PersistedDbObject.LastReviewedDateTime = NewBusinessObject.LastReviewedDateTime;

            PersistedDbObject.SourceCode = HttpUtility.HtmlDecode(NewBusinessObject.SourceCode ?? "");
            PersistedDbObject.DomainName = HttpUtility.HtmlDecode(NewBusinessObject.DomainName ?? "");
            PersistedDbObject.OwningBusinessUnitId = NewBusinessObject.OwningBusinessUnitId;
            PersistedDbObject.MaintainingBusinessUnitId = NewBusinessObject.MaintainingBusinessUnitId;

            PersistedDbObject.EmbedParameters = SerializePreferences(NewBusinessObject.Preferences);
            PersistedDbObject.EmbedCode = HttpUtility.HtmlDecode(NewBusinessObject.Embedcode ?? "");
            PersistedDbObject.PageCount = NewBusinessObject.DocumentPageCount;
            PersistedDbObject.DataSize = NewBusinessObject.DocumentDataSize;

            PersistedDbObject.MediaStatusCode = NewBusinessObject.MediaStatusCodeValue.ToString();
            PersistedDbObject.OmnitureChannel = NewBusinessObject.OmnitureChannel;


            PersistedDbObject.RowVersion = NewBusinessObject.RowVersion;
            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;

            PersistedDbObject.DateSyndicationUpdated = modifiedDateTime;

            PersistedDbObject.DateContentAuthored = NewBusinessObject.DateContentAuthored;
            PersistedDbObject.DateContentPublished = NewBusinessObject.DateContentPublished;
            PersistedDbObject.DateContentReviewed = NewBusinessObject.DateContentReviewed;
            PersistedDbObject.DateContentUpdated = NewBusinessObject.DateContentUpdated;
        }


        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.Medias.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            if (PersistedDbObject == null)
            {
                Logger.LogInfo("MediaCtl.Delete, PersistedDbObject is null");
                return;
            }
            TheObjectContext.MediaDbEntities.Medias.DeleteObject(PersistedDbObject);
        }

        public void SetNewPersistentUrl(string newPersistentUrlToken)
        {
            NewBusinessObject.PersistentUrlToken = newPersistentUrlToken;
            PersistedBusinessObject.PersistentUrlToken = newPersistentUrlToken;
            if (PersistedDbObject == null)
            {
                return;
            }
            PersistedDbObject.PersistentURLToken = newPersistentUrlToken;
        }

        private string SerializePreferences(PreferencesSet preferences)
        {
            if (preferences == null || preferences.PreferencesPersistedForMediaItem == null)
            {
                return null;
            }
            return preferences.PreferencesPersistedForMediaItem.ToXElement<MediaPreferenceSetCollection>().ToString();
        }

        public void AddReference(IEnumerable<AttributeValueObjectCtl> reference)
        {
            foreach (AttributeValueObjectCtl topicCtl in reference)
            {
                PersistedDbObject.MediaValues.Add(topicCtl.PersistedDbObject);
            }
        }

        public void AddEnclosures(IEnumerable<EnclosureObjectCtl> reference)
        {
            foreach (var item in reference)
            {
                PersistedDbObject.Enclosures.Add(item.PersistedDbObject);
            }
        }

        public void AddRelationships(IEnumerable<MediaRelationshipCtl> relationships)
        {
            foreach (var i in relationships)
            {
                if (i.PersistedDbObject.MediaId == 0 || i.PersistedDbObject.MediaId == PersistedDbObject.MediaId)
                {
                    var existing = PersistedDbObject.MediaRelationships.FirstOrDefault(mr => mr.RelatedMediaId == i.PersistedDbObject.MediaId);
                    if (existing == null)
                    {
                        PersistedDbObject.MediaRelationships.Add(i.PersistedDbObject);
                    }
                }
                else if (i.PersistedDbObject.RelatedMediaId == 0 || i.PersistedDbObject.RelatedMediaId == PersistedDbObject.MediaId)
                {
                    var existing = PersistedDbObject.MediaRelationships1.FirstOrDefault(mr => mr.MediaId == i.PersistedDbObject.MediaId);
                    if (existing == null)
                    {
                        PersistedDbObject.MediaRelationships1.Add(i.PersistedDbObject);
                    }
                }
            }
            var reg = PersistedDbObject.MediaRelationships;
            var flip = PersistedDbObject.MediaRelationships1;
        }

        public void AddGeoDatas(IEnumerable<GeoDataCtl> ctls)
        {
            foreach (var ctl in ctls)
            {
                if (ctl.PersistedDbObject.MediaId == 0 || ctl.PersistedDbObject.MediaId == PersistedDbObject.MediaId)
                {
                    PersistedDbObject.GeoDatas.Add(ctl.PersistedDbObject);
                }
            }
        }

        public void AddFeedImage2(MediaImageCtl image)
        {
            if (image == null)
            {
                return;
            }

            var existing = PersistedDbObject.Image;
            if (existing == null)
            {
                PersistedDbObject.Image = image.PersistedDbObject;
            }
        }


        public void AddFeedImage(MediaImage image)
        {
            if (image == null)
            {
                return;
            }

            var existing = PersistedDbObject.Image;
            if (existing == null)
            {
                PersistedDbObject.Image = new Image
                {
                    Height = image.Height.Value,
                    Width = image.Width.Value,
                    MediaID = image.MediaId,
                    Title = image.Title,
                    Description = image.Description,
                    LinkURL = image.TargetUrl,
                    SourceURL = image.SourceUrl,
                    CreatedByGUID = image.CreatedBy,
                    ModifiedByGUID = image.ModifiedBy,
                    CreatedDateTime = DateTime.UtcNow,
                    ModifiedDateTime = DateTime.UtcNow
                };
            }
        }

        public void UpdateFeedImage(MediaObject mediaItem)
        {
            if (!mediaItem.HasFeedImage && PersistedDbObject.Image == null)
            {
                return;
            }

            MediaImage image = mediaItem.AssociatedImage;
            if (image == null)
            {
                if (PersistedDbObject.Image != null)
                {
                    DeleteImage();
                }
                return;
            }

            if (PersistedDbObject.Image == null)
            {
                var newImage = new Image
                {
                    Height = image.Height.Value,
                    Width = image.Width.Value,
                    MediaID = PersistedDbObject.MediaId,
                    Title = image.Title,
                    Description = image.Description,
                    LinkURL = image.TargetUrl,
                    SourceURL = image.SourceUrl,
                    ModifiedByGUID = image.ModifiedBy,
                    ModifiedDateTime = DateTime.UtcNow,
                    CreatedByGUID = image.ModifiedBy,
                    CreatedDateTime = DateTime.UtcNow
                };

                TheObjectContext.MediaDbEntities.Images.AddObject(newImage);
            }
            else
            {
                PersistedDbObject.Image.Height = image.Height.Value;
                PersistedDbObject.Image.Width = image.Width.Value;
                PersistedDbObject.Image.MediaID = image.MediaId;
                PersistedDbObject.Image.Title = image.Title;
                PersistedDbObject.Image.Description = image.Description;
                PersistedDbObject.Image.LinkURL = image.TargetUrl;
                PersistedDbObject.Image.SourceURL = image.SourceUrl;
                PersistedDbObject.Image.ModifiedByGUID = image.ModifiedBy;
                PersistedDbObject.Image.ModifiedDateTime = DateTime.UtcNow;
            }
        }

        public void DeleteImage()
        {
            if (PersistedDbObject.Image != null)
            {
                TheObjectContext.MediaDbEntities.Images.DeleteObject(PersistedDbObject.Image);
            }
        }

        public void AddExtendedAttributes(IList<CsBusinessObjects.Media.ExtendedAttribute> items)
        {
            foreach (var item in items) //new or updated items
            {
                var existing = PersistedDbObject.ExtendedAttributes.FirstOrDefault(ea => ea.Name == item.Name);
                if (existing == null)
                {
                    PersistedDbObject.ExtendedAttributes.Add(new ExtendedAttribute { Name = item.Name, Value = item.Value, CreatedDateTime = DateTime.UtcNow, ModifiedDateTime = DateTime.UtcNow });
                }
                else
                {
                    existing.Value = item.Value;
                }
            }
            for (var i = 0; i < PersistedDbObject.ExtendedAttributes.Count; i++) //delete if not in the list passed in
            {
                var item = PersistedDbObject.ExtendedAttributes.ElementAt(i);
                var passed = items.FirstOrDefault(ea => ea.Name == item.Name);
                if (passed == null)
                {
                    TheObjectContext.MediaDbEntities.ExtendedAttributes.DeleteObject(item);
                }
            }
        }

        public override object Get(bool forUpdate)
        {
            throw new NotImplementedException();
        }

        public static IQueryable<MediaObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            var data = media.MediaDbEntities.CombinedMediaList1;
            var obj = TransformMediaToMediaObject(media, data, forUpdate);
            return obj;
        }

        public static IQueryable<MediaObject> GetTopMedia(MediaObjectContext media, bool forUpdate = false)
        {
            var relatedMedia = from m in media.MediaDbEntities.CombinedMediaList1
                               join mObject in media.MediaDbEntities.TopMedias on m.MediaId equals mObject.MediaID
                               select m;

            return TransformMediaToMediaObject(media, relatedMedia, forUpdate);
        }

        public static IQueryable<MediaObject> GetRelatedMedia(MediaObjectContext media, int mediaId, int toChildLevel, int toParentLevel, bool forUpdate = false)
        {
            var mediaRelationList = media.MediaDbEntities.GetMediaRelationships(mediaId, toChildLevel, toParentLevel).ToList();
            var ids = mediaRelationList.Select(a => a.MediaId).ToList();

            var data = media.MediaDbEntities.CombinedMediaList1.Where(m => ids.Contains(m.MediaId));
            var mediaList = TransformMediaToMediaObject(media, data, forUpdate).ToDictionary(a => a.MediaId.ToString(), a => a);

            //foreach (var item in mediaList)
            //    SetRelationshipValues(item, mediaRelationList.Where(a => a.MediaId == item.MediaId).FirstOrDefault());

            var mo = new List<MediaObject>();
            foreach (var item in mediaRelationList)
            {
                mo.Add(SetRelationshipValues(mediaList[item.MediaId.ToString()], item));
            }

            //mediaList = mediaRelationList.ForEach(a => SetRelationshipValues(mediaList.Where(b => b.MediaId == a.MediaId).FirstOrDefault(), a)).ToList();

            AddSubLists(media, mo.AsQueryable());

            return mo.AsQueryable();
        }

        private static MediaObject SetRelationshipValues(MediaObject obj, GetMediaRelationships_Result mr)
        {
            var newMedia = obj.ShallowCopy();

            newMedia.RelatedMediaId = mr.RelatedMediaId;
            newMedia.Level = mr.Lvl;
            newMedia.RelationshipTypeName = mr.RelationshipTypeName;
            newMedia.DisplayOrdinal = mr.DisplayOrdinal;

            return newMedia;
        }

        public static IQueryable<MediaObject> Get(MediaObjectContext media, int topLevelMediaId)
        {
            var relatedMedia = media.MediaDbEntities.CombinedMediaList1.Where(m =>
                media.MediaDbEntities.MediaRelationships
                .Where(a => a.MediaId == topLevelMediaId)
                .Select(b => b.RelatedMediaId).Contains(m.MediaId));
            return TransformMediaToMediaObject(media, relatedMedia, forUpdate: false);
        }

        public static byte[] GetThumbnailForMediaType(MediaObjectContext media, int mediaId)
        {
            var items = media.MediaDbEntities.Medias.Where(m => m.MediaId == mediaId).ToList();
            if (items.Count == 0)
            {
                throw new ApplicationException("media id " + mediaId + " not found.");
            }
            var mediaType = items.First().MediaTypeCode;
            return media.MediaDbEntities.MediaTypes.Where(mt => mt.MediaTypeCode == mediaType).Select(mt => mt.Thumbnail).FirstOrDefault();
        }

        private static IQueryable<MediaObject> TransformMediaToMediaObject(MediaObjectContext media, IQueryable<CombinedMediaList1> sourceListOfMedia, bool forUpdate)
        {
            IQueryable<AttributeValueObject> allValueItems = AttributeValueObjectCtl.Get(media, forUpdate);
            IQueryable<MediaGeoDataObject> geoData = GeoDataCtl.Get(media, forUpdate);
            IQueryable<EnclosureObject> enclosures = EnclosureObjectCtl.Get(media, forUpdate);

            var langItems = LanguageItemCtl.Get(media, forUpdate);
            var sourceItems = SourceItemCtl.Get(media);

            IQueryable<MediaObject> mediaItems = from m in sourceListOfMedia
                                                 join mDbObject in media.MediaDbEntities.Medias on m.MediaId equals mDbObject.MediaId

                                                 let mt = (from mediaType in media.MediaDbEntities.MediaTypes
                                                           where mediaType.MediaTypeCode == m.MediaTypeCode
                                                           select mediaType).FirstOrDefault()

                                                 let extendedAttributes = media.MediaDbEntities.ExtendedAttributes.Where(ea => ea.MediaID == m.MediaId)
                                                 let language = langItems.Where(i => i.Code == m.LanguageCode).FirstOrDefault()
                                                 let source = sourceItems.Where(i => i.Code == m.SourceCode).FirstOrDefault()

                                                 let syndicationList = media.MediaDbEntities.SyndicationListMedias.Where(s => s.MediaId == m.MediaId).FirstOrDefault()

                                                 select new MediaObject()
                                                 {
                                                     SyndicationListModifiedDateTime = syndicationList.ModifiedDateTime,

                                                     Id = m.MediaId,
                                                     MediaTypeCode = m.MediaTypeCode,

                                                     AttributeValues = allValueItems.Where(av => av.MediaId == m.MediaId),
                                                     Popularity = media.MediaDbEntities.TopMedias.FirstOrDefault(tm => tm.MediaID == m.MediaId && tm.DownloadCount > 0) != null ? media.MediaDbEntities.TopMedias.FirstOrDefault(tm => tm.MediaID == m.MediaId).DownloadCount.Value : 0,
                                                     MediaGeoData = geoData.Where(gd => gd.MediaId == m.MediaId),

                                                     MediaId = m.MediaId,

                                                     MediaGuid = m.MediaGuid,

                                                     LanguageCode = m.LanguageCode,

                                                     MimeTypeCode = m.MimeTypeCode,
                                                     CharacterEncodingCode = m.CharacterEncodingCode,

                                                     Title = m.Title,
                                                     Name = m.Title,
                                                     SubTitle = m.SubTitle,
                                                     Description = m.Description,
                                                     SourceUrl = m.SourceUrl,
                                                     TargetUrl = m.TargetUrl,
                                                     AlternateText = m.AlternateText,
                                                     NoScriptText = m.NoScriptText,
                                                     FeaturedText = m.FeaturedText,

                                                     Author = m.Author,
                                                     Length = m.Length,
                                                     Height = m.Height,
                                                     Width = m.Width,
                                                     RatingOverride = m.RatingOverride,
                                                     RatingMinimum = m.RatingMinimum,
                                                     RatingMaximum = m.RatingMaximum,

                                                     Comments = m.Comments,

                                                     LastReviewedDateTime = m.LastReviewedDateTime,

                                                     DomainName = m.DomainName,
                                                     OwningBusinessUnitId = m.OwningBusinessUnitId,
                                                     MaintainingBusinessUnitId = m.MaintainingBusinessUnitId,

                                                     MediaStatusCode = m.MediaStatusCode,
                                                     PersistentUrlToken = m.PersistentURLToken,

                                                     CreatedByGuid = m.CreatedByGuid,
                                                     CreatedDateTime = m.CreatedDateTime,
                                                     ModifiedByGuid = m.ModifiedByGuid,
                                                     ModifiedDateTime = m.ModifiedDateTime,
                                                     RowVersion = m.RowVersion,

                                                     LastReviewedDate = m.LastReviewedDateTime,
                                                     CreatedDate = m.CreatedDateTime,
                                                     SourceCode = m.SourceCode,
                                                     DateContentAuthored = m.DateContentAuthored,
                                                     DateContentPublished = m.DateContentPublished,
                                                     DateContentReviewed = m.DateContentReviewed,
                                                     DateContentUpdated = m.DateContentUpdated,
                                                     DateSyndicationCaptured = m.CreatedDateTime,
                                                     DateSyndicationUpdated = m.ModifiedDateTime,
                                                     DateSyndicationVisible = m.PublishedDateTime,

                                                     OwningBusinessUnitName = m.OwningBusinessUnitName,
                                                     MaintainingBusinessUnitName = m.MaintainingBusinessUnitName,

                                                     Attribution = m.AttributionText,
                                                     EmbedParametersForMediaItem = m.EmbedParameters,
                                                     EmbedParametersForMediaType = m.MediaTypeEmbedParameters,

                                                     PublishedDateTime = m.PublishedDateTime,
                                                     EffectiveStatusCode = m.EffectiveStatus,

                                                     Embedcode = m.EmbedCode,   //mediaTypePreferences.EmbedParameters,
                                                     DocumentPageCount = m.PageCount,
                                                     DocumentDataSize = m.DataSize,

                                                     ExtendedAttributes = extendedAttributes.Select(ea => new Gov.Hhs.Cdc.CsBusinessObjects.Media.ExtendedAttribute { Name = ea.Name, Value = ea.Value }),

                                                     Preferences = new PreferencesSet()
                                                     {
                                                         MediaId = m.MediaId,
                                                         SerializedPreferencesFor_C_MediaItem = m.EmbedParameters,
                                                         MediaTypeCode = mt.MediaTypeCode,
                                                         SerializedPreferencesFor_D_MediaType = mt.EmbedParameters
                                                     },

                                                     LangItem = language,
                                                     Source = source,

                                                     SourceAcronym = source.Acronym,

                                                     DbObject = forUpdate ? mDbObject : null,
                                                     LanguageIsoCode = language.ISOLanguageCode3,
                                                     DisplayOnSearch = m.DisplayOnSearch == "Yes",

                                                     AssociatedImage = new MediaImage
                                                     {
                                                         Height = m.ImageHeight,
                                                         TargetUrl = m.ImageLink,
                                                         SourceUrl = m.ImageURL,
                                                         Title = m.ImageTitle,
                                                         Width = m.ImageWidth,
                                                         Description = m.ImageDescription
                                                     },
                                                     Enclosures = enclosures.Where(a => a.MediaId == m.MediaId),
                                                     OmnitureChannel = m.OmnitureChannel
                                                 };

            return mediaItems;
        }

        public static MediaObject GetComplete(MediaObjectContext media, int mediaId, bool forUpdate = false)
        {
            //var criteria = new CsBusinessObjects.Media.SearchCriteria { MediaId = mediaId.ToString() };
            //var xml = criteria.ToXmlString();
            //var results = Search(media, xml);
            //var single = results.Select(m => m).ToList().FirstOrDefault();
            //if (single == null)
            //{
            //    Logger.LogError("MediaCtl.GetComplete Search returned no results.  Search:  " + xml);
            //    //var singleMedia = MediaCtl.Get(media, forUpdate).Where(m => m.Id == mediaId);
            //    //var complete = AddSubLists(media, singleMedia, forUpdate).FirstOrDefault();
            //    //return complete;
            //}
            var singleMedia = MediaCtl.Get(media, forUpdate).Where(m => m.Id == mediaId);
            var complete = AddSubLists(media, singleMedia, forUpdate).FirstOrDefault();
            return complete;

            //return single;

            //var singleMedia = MediaCtl.Get(media, forUpdate).Where(m => m.Id == mediaId);
            //var complete = AddSubLists(media, singleMedia, forUpdate).FirstOrDefault();
            //return complete;
        }

        public static List<MediaObject> AddSubLists(MediaObjectContext media, IQueryable<MediaObject> selectedMedias, bool forUpdate = false)
        {
            return AddSubLists(media, selectedMedias.ToList(), forUpdate);
        }

        public static List<MediaObject> AddSubLists(MediaObjectContext media, IEnumerable<MediaObject> selectedMedias, bool forUpdate = false, bool getGeoData = true)
        {
            var selectedMediaIds = selectedMedias.Select(m => m.MediaId).ToList();

            //SQL Server has a limit of 2200 items in a contains
            if (selectedMediaIds.Count > 2200)
            {
                return selectedMedias.ToList();
            }

            Guid adminGuid = Guid.Parse("00000000-0000-0000-0000-000000000000");

            var storageItems = StorageCtl.Get(media, forUpdate);
            var eCards = ECardObjectCtl.GetWithoutMedia(media, forUpdate);
            var feeds = FeedObjectCtl.Get(media, forUpdate);
            var enclosures = EnclosureObjectCtl.Get(media, forUpdate);


            var subLists = (from mdb in media.MediaDbEntities.Medias
                            where selectedMediaIds.Contains(mdb.MediaId)
                            let storageIds = mdb.MediaStorages.Select(ms => ms.StorageID)
                            select new
                            {
                                mediaId = mdb.MediaId,
                                alternateImages = storageItems.Where(alt => storageIds.Contains(alt.StorageId)),
                                relationships = mdb.MediaRelationships,
                                //mediaTypeExtension = eCards.Where(ext => ext.MediaId == mdb.MediaId).FirstOrDefault(),
                                ecard = eCards.Where(ext => ext.MediaId == mdb.MediaId).FirstOrDefault(),
                                feed = feeds.Where(ext => ext.MediaId == mdb.MediaId).FirstOrDefault(),
                                enclosures = enclosures.Where(a => a.MediaId == mdb.MediaId),
                                //mediaTypeExtension = mdb.MediaTypeCode.ToLower() == "ecard" ? eCards.Where(ext => ext.MediaId == mdb.MediaId).FirstOrDefault() : feeds.Where(ext => ext.MediaId == mdb.MediaId).FirstOrDefault(),
                                //mediaTypeExtension = ExtensionFromMedia(media, mdb, forUpdate),// eCards.Where(ext => ext.MediaId == mdb.MediaId).FirstOrDefault(),
                                userPreferences = mdb.MediaPreferences.Where(p => p.MediaID == mdb.MediaId && p.UserID == adminGuid).FirstOrDefault(),
                                mediaTypePreferences = mdb.MediaType.MediaTypePreferences.Where(p => p.MediaTypeCode == mdb.MediaTypeCode && p.UserID == adminGuid).FirstOrDefault()
                            }).ToList();

            var mediasWithSubLists = (from m in selectedMedias
                                      join sl in subLists on m.MediaId equals sl.mediaId
                                      select new
                                      {
                                          media = m,
                                          alternateImages = sl.alternateImages,
                                          relationships = sl.relationships,
                                          ecard = sl.ecard,
                                          feed = sl.feed,
                                          enclosures = sl.enclosures,
                                          //mediaTypeExtension = sl.mediaTypeExtension,
                                          userPreferences = sl.userPreferences,
                                          mediaTypePreferences = sl.mediaTypePreferences
                                      }).ToList();

            foreach (var m in mediasWithSubLists)
            {
                m.media.AlternateImages = m.alternateImages;
                m.media.MediaRelationships = MediaRelationshipCtl.CreateMediaRelationships(m.relationships.AsQueryable(), forUpdate);
                m.media.Enclosures = m.enclosures;

                switch (m.media.MediaTypeCode.ToLower())
                {
                    case "ecard":
                        m.media.MediaTypeSpecificDetail = m.ecard;
                        break;
                    case "feed":
                    case "feed - proxy":
                    case "feed - import":
                    case "feed - aggregate":
                        if (m.feed != null)
                        {
                            m.feed.AssociatedImage = m.media.AssociatedImage;
                        }
                        m.media.MediaTypeSpecificDetail = m.feed;
                        break;
                    case "feed item":
                        FeedDetailObject detail = new FeedDetailObject();
                        detail.AssociatedImage = m.media.AssociatedImage;
                        m.media.MediaTypeSpecificDetail = detail;
                        break;
                    default:
                        break;
                }

                m.media.UserOverriddenEmbedParametersForMediaItem = m.userPreferences == null ? null : m.userPreferences.EmbedParameters;
                m.media.UserOverriddenEmbedParametersForMediaType = m.mediaTypePreferences == null ? null : m.mediaTypePreferences.EmbedParameters;
            }

            return selectedMedias.ToList();
        }

        public static IMediaTypeSpecificDetail ExtensionFromMedia(MediaObjectContext db, Medias media, bool forUpdate)
        {
            switch (media.MediaTypeCode.ToLower())
            {
                case "ecard":
                    var eCards = ECardObjectCtl.GetWithoutMedia(db, forUpdate);
                    return eCards.Where(ext => ext.MediaId == media.MediaId).FirstOrDefault();
                case "feed":
                case "feed - proxy":
                case "feed - import":
                case "feed - aggregate":
                    var feeds = FeedObjectCtl.Get(db, forUpdate);
                    return feeds.Where(ext => ext.MediaId == media.MediaId).FirstOrDefault();
                default:
                    return null;
            }
        }

        public static IQueryable<MediaObject> GetSimple(MediaObjectContext media)
        {
            IQueryable<MediaObject> mediaItems = from m in media.MediaDbEntities.CombinedMediaList1
                                                 let mediaType = media.MediaDbEntities.MediaTypes.Where(mt => mt.MediaTypeCode == m.MediaTypeCode).FirstOrDefault()
                                                 select new MediaObject()
                                                 {
                                                     Id = m.MediaId,
                                                     MediaGuid = m.MediaGuid,
                                                     MediaTypeCode = m.MediaTypeCode,
                                                     MimeTypeCode = m.MimeTypeCode,
                                                     Title = m.Title,
                                                     Description = m.Description,
                                                     SourceUrl = m.SourceUrl,
                                                     TargetUrl = m.TargetUrl,
                                                     LanguageCode = m.LanguageCode,
                                                     PublishedDateTime = m.PublishedDateTime,
                                                     LastReviewedDate = m.LastReviewedDateTime,
                                                     MediaStatusCode = m.MediaStatusCode,
                                                     EffectiveStatusCode = m.EffectiveStatus,
                                                     ModifiedDateTime = m.ModifiedDateTime,
                                                     CreatedDate = m.CreatedDateTime,
                                                     EmbedParametersForMediaItem = m.EmbedParameters,
                                                     EmbedParametersForMediaType = mediaType.EmbedParameters,

                                                     PersistentUrlToken = m.PersistentURLToken,
                                                     RowVersion = m.RowVersion,
                                                     DisplayOnSearch = mediaType.Display == "Yes"
                                                 };
            return mediaItems;
        }

        public static IEnumerable<MediaObject> Search(MediaObjectContext media, string criteria)
        {
            //var watch = new Stopwatch();
            //watch.Start();
            var results = media.MediaDbEntities.SearchMediaXML(criteria);
            //Logger.LogInfo("DB query time: " + watch.ElapsedMilliseconds.ToString());

            var obj = results.Select(r => new MediaObject
            {
                MediaId = r.MediaId,
                Id = r.MediaId,
                MediaGuid = r.MediaGuid,
                Title = r.Title,
                Name = r.Title,
                Description = r.Description,
                MediaTypeCode = r.MediaTypeCode,
                LanguageCode = r.LanguageCode,
                LanguageIsoCode = r.languageIsoCode,
                LangItem = new LanguageItem { Code = r.LanguageCode, ISOLanguageCode3 = r.languageIsoCode },
                SourceUrl = r.SourceUrl,
                PersistentUrlToken = r.PersistentURLToken,
                SourceCode = r.SourceCode,
                Attribution = r.AttributionText,
                AlternateText = r.AlternateText,
                NoScriptText = r.NoScriptText,
                FeaturedText = r.FeaturedText,
                Author = r.Author,
                Length = r.Length,
                Height = r.Height,
                Width = r.Width,
                Source = new SourceItem
                {
                    Code = r.SourceCode,
                    Acronym = r.Acronym,
                    LogoLargeUrl = r.LogoLargeUrl,
                    LogoSmallUrl = r.LogoSmallUrl
                },
                SourceAcronym = r.Acronym,
                Children = r.ListChildrenJSON == null ? new List<MediaObject>() :
                    JsonConvert.DeserializeObject<List<MediaObject>>(r.ListChildrenJSON),
                TargetUrl = r.TargetUrl,
                CharacterEncodingCode = r.CharacterEncodingCode,
                MimeTypeCode = r.MimeTypeCode,
                //Parents = r.ListParentsJSON == null ? new List<MediaObject>() :
                //    JsonConvert.DeserializeObject<List<MediaObject>>(r.ListParentsJSON),
                RatingOverride = r.RatingOverride,
                RatingMinimum = r.RatingMinimum,
                Comments = r.Comments,
                RowVersion = r.RowVersion,
                RatingMaximum = r.RatingMaximum,
                MediaStatusCode = r.MediaStatusCode,
                PublishedDateTime = r.PublishedDateTime,
                ModifiedDateTime = r.ModifiedDateTime,
                ModifiedByGuid = r.ModifiedByGUID,
                CreatedByGuid = r.CreatedByGUID,
                CreatedDateTime = r.CreatedDateTime,
                CreatedDate = r.CreatedDateTime,
                DomainName = r.DomainName,
                AttributeValues = r.VocabularyJSON == null ? new List<AttributeValueObject>() :
                JsonConvert.DeserializeObject<List<AttributeValueObject2>>(r.VocabularyJSON)
                .Select(a => new AttributeValueObject
                {
                    MediaId = r.MediaId,
                    ValueKey = new ValueKey { Id = a.Id, LanguageCode = a.language },
                    ValueName = a.Name,
                    AttributeName = a.attributeName,
                    AttributeId = int.Parse(a.attributeID)
                }),
                MediaGeoData = r.GeoJSON == null ? new List<MediaGeoDataObject>() : JsonConvert.DeserializeObject<List<MediaGeoDataObject>>(r.GeoJSON)
                .Select(g => new MediaGeoDataObject
                {
                    Name = g.AsciiName,
                    CountryCode = g.CountryCode,
                    GeoNameId = g.GeoNameId,
                    ParentId = g.ParentId,
                    Longitude = g.Longitude,
                    Latitude = g.Latitude,
                    Timezone = g.Timezone,
                    Admin1Code = g.Admin1Code
                }),
                MediaRelationships = MediaRelationshipCtl.Get(media).Where(mr => mr.MediaId == r.MediaId).ToList(),   //  new List<MediaRelationshipObject>(),
                ExtendedAttributes = r.DictionaryJSON == null ? new List<CsBusinessObjects.Media.ExtendedAttribute>()
                : JsonConvert.DeserializeObject<List<CsBusinessObjects.Media.ExtendedAttribute>>(r.DictionaryJSON),
                AlternateImages = r.StorageJSON == null ? null : JsonConvert.DeserializeObject<List<StorageObject>>(r.StorageJSON),

                DateContentAuthored = r.DateContentAuthored,
                DateContentPublished = r.DateContentPublished,
                DateContentUpdated = r.DateContentUpdated,
                DateContentReviewed = r.DateContentReviewed,
                DateSyndicationCaptured = r.DateSyndicationCaptured,
                DateSyndicationUpdated = r.DateSyndicationUpdated,
                DateSyndicationVisible = r.DateSyndicationVisible,

                DocumentDataSize = r.DataSize,
                DocumentPageCount = r.PageCount,
                Preferences = new PreferencesSet
                {
                    MediaId = r.MediaId,
                    SerializedPreferencesFor_C_MediaItem = r.EmbedParameters,
                    MediaTypeCode = r.MediaTypeCode,
                    SerializedPreferencesFor_D_MediaType = r.MediaTypeEmbedParameters
                },
                ExtractionClasses = r.ExtractionClasses,
                ExtractionElementIds = r.ExtractionElementIds,
                ExtractionXpath = r.ExtractionXpath,

                Popularity = media.MediaDbEntities.TopMedias.FirstOrDefault(tm => tm.MediaID == r.MediaId && tm.DownloadCount > 0) != null ?
                    media.MediaDbEntities.TopMedias.FirstOrDefault(tm => tm.MediaID == r.MediaId).DownloadCount.Value
                    : 0,
                Embedcode = r.EmbedCode,
                PageNumber = r.PageNumber.HasValue ? r.PageNumber.Value : 0,
                TotalPages = r.TotalPageCount.HasValue ? r.TotalPageCount.Value : 0,
                TotalRows = r.TotalRows.HasValue ? r.TotalRows.Value : 0,
                RowsPerPage = r.RowsPerPage.HasValue ? r.RowsPerPage.Value : 0,
                PageOffset = r.PageOffset.HasValue ? r.PageOffset.Value : 0,
                MediaTypeSpecificDetail = DetailForType(r),
                MaintainingBusinessUnitId = r.MaintainingBusinessUnitId,
                MaintainingBusinessUnitName = r.MaintainingOrgName,
                OwningBusinessUnitId = r.OwningBusinessUnitId,
                OwningBusinessUnitName = r.OwningOrgName,
                OmnitureChannel = r.OmnitureChannel,
                Enclosures = r.EnclosureJSON == null ? new List<EnclosureObject>() : JsonConvert.DeserializeObject<List<EnclosureObject>>(r.EnclosureJSON)
                .Select(a => new EnclosureObject
                {
                    Id = a.Id,
                    MediaId = r.MediaId,
                    ResourceUrl = a.ResourceUrl,
                    ContentType = a.ContentType,
                    Size = a.Size
                }),
                Aggregates = r.FeedAggregateJSON == null ? new List<AggregateObject>() : JsonConvert.DeserializeObject<List<AggregateObject>>(r.FeedAggregateJSON)
                .Select(a => new AggregateObject
                {
                    Id = a.Id,
                    MediaId = r.MediaId,
                    SearchXML = a.SearchXML
                })                
            });

            var list = obj.ToList();
            return list;
        }

        private static IMediaTypeSpecificDetail DetailForType(SearchMediaXML_Result r)
        {
            var image = new MediaImage
            {
                Title = r.ImageTitle,
                SourceUrl = r.ImageURL,
                TargetUrl = r.ImageLink,
                Height = r.ImageHeight,
                Width = r.ImageWidth,
                Description = r.ImageDescription
            };

            switch (r.MediaTypeCode.ToLower())
            {
                case "ecard":
                    {
                        if (r.CardJSON == null)
                        {
                            return null;
                        }
                        var detail = JsonConvert.DeserializeObject<List<ECardDetail>>(r.CardJSON);
                        return detail.FirstOrDefault();
                    }
                case "feed":
                case "feed - proxy":
                case "feed - import":
                case "feed - aggregate":
                    return new FeedDetailObject
                    {
                        EditorialManager = r.ManagingEditor,
                        WebMasterEmail = r.WebMaster,
                        Copyright = r.Copyright,
                        AssociatedImage = image,
                        ExportSettings = r.FeedExportJSON == null ? new List<FeedExportObject>() : JsonConvert.DeserializeObject<List<FeedExportObject>>(r.FeedExportJSON)
                        .Select(a => new FeedExportObject
                        {
                            Id = a.Id,
                            MediaId = r.MediaId,
                            FilePath = a.FilePath,
                            FeedFormatName = a.FeedFormatName
                        }).ToList()
                    };
                case "feed item":
                    return new FeedDetailObject
                    {
                        AssociatedImage = image
                    };

                default:
                    return null;
            }
        }

        public static IEnumerable<MediaObject> Dupes(MediaObjectContext media)
        {
            var dupes = media.MediaDbEntities.Medias
                .Where(m => m.MediaTypeCode == "Html")
                .GroupBy(g => g.SourceUrl)
                .Where(grp => grp.Count() > 1)
                .Select(x => new MediaObject { SourceUrl = x.Key });
            return dupes;
        }

    }
}
