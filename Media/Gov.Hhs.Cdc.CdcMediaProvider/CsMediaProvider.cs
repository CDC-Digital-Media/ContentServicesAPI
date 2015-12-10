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
using System.Transactions;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.MediaProvider;


namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class CsMediaProvider : MediaMgrBase, IMediaProvider
    {
        public CompositeMedia GetMediaCollection<t>(int mediaId, bool onlyDisplayable)
        {
            return GetMediaCollection<t>(mediaId, null, 2, 1, false, onlyDisplayable);
        }

        public CompositeMedia GetMediaCollection<t>(int mediaId, Sorting sorting, int childLevel, int parentLevel,
            bool onlyIsPublishedHidden, bool onlyDisplayable)
        {
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                MediaObject mediaItem = MediaCtl.GetComplete(media, mediaId);

                if (mediaItem == null)
                {
                    return null;
                }

                IEnumerable<MediaObject> relatedMedia = MediaCtl.GetRelatedMedia(media, mediaId, childLevel, parentLevel);
                if (onlyIsPublishedHidden)
                {
                    //Is this a feed
                    if (mediaItem.MediaTypeCode == "Feed")
                    {
                        relatedMedia = relatedMedia
                            .Where(i => (i.EffectiveStatusCode == "Published") && i.PublishedDateTime <= DateTime.UtcNow);
                    }
                    else
                    {
                        relatedMedia = relatedMedia
                            .Where(i => (i.EffectiveStatusCode == "Published" || i.EffectiveStatusCode == "Hidden") && i.PublishedDateTime <= DateTime.UtcNow);
                    }
                }
                if (onlyDisplayable)
                {
                    relatedMedia = relatedMedia.Where(i => i.DisplayOnSearch);
                }

                var children = new ListDictionary<int, MediaObject>();
                var parents = new ListDictionary<int, MediaObject>();

                if (sorting == null || !sorting.IsSorted || sorting.SortColumns.Count == 0)
                {
                    sorting = new Sorting(new SortColumn() { Column = "DisplayOrdinal", SortOrder = SortOrderType.Asc });
                }

                var listOfRelatedMedia = SortParentChildren<t>(relatedMedia.AsQueryable(), sorting).Cast<MediaObject>();
                foreach (MediaObject item in listOfRelatedMedia)
                {
                    if (item.Level < 0 && item.RelatedMediaId != null)
                    {
                        parents.Add((int)item.RelatedMediaId, item);
                    }
                    else if (item.Level > 0)
                    {
                        children.Add((int)item.RelatedMediaId, item);
                    }
                }

                CompositeMedia theObject = new CompositeMedia()
                {
                    TheMediaObject = mediaItem,
                    Parents = GetHierarchicalRelations(parents, mediaId, true, 0),
                    Children = GetHierarchicalRelations(children, mediaId, false, 0)
                };

                return theObject;
            }
        }

        private static IEnumerable<t> SortParentChildren<t>(IQueryable query, Sorting sorting)
        {
            ReflectionQualifiedNames qualifiedNames = ReflectionQualifiedNames.Get(typeof(t));
            return query.Cast<t>().OrderBy(sorting, qualifiedNames).ToList();
        }

        public List<MediaObject> GetHierarchicalRelations(ListDictionary<int, MediaObject> dictionary, int mediaId, bool isParent, int count)
        {
            //Make sure we don't get into a loop
            if (count > 20)
            {
                throw new ApplicationException("CsMediaProvider.GetHierarchicalRelations loop occurred");
            }

            if (!dictionary.ContainsKey(mediaId))
            {
                return null;
            }

            List<MediaObject> relations = dictionary[mediaId];
            foreach (MediaObject item in relations)
            {
                if (isParent)
                {
                    item.Parents = GetHierarchicalRelations(dictionary, item.Id, isParent, count + 1);
                }
                else
                {
                    item.Children = GetHierarchicalRelations(dictionary, item.Id, isParent, count + 1);
                }
            }

            return relations;
        }

        public ValidationMessages SaveMedia(MediaObject mediaObject)
        {
            //Logging.Logger.LogInfo("Test 7/2");
            if (mediaObject == null)
            {
                return ValidationMessages.CreateError("Media", "Media object is missing");
            }

            ValidationMessages messages = new ValidationMessages();
            bool imageExists = false;
            var mediaList = new List<MediaObject>() { mediaObject };
            MediaImage image = null;
            if (mediaObject.HasImage())
            {
                image = GetFeedImage(mediaObject.Id);
                imageExists = (image != null);
                if (!imageExists)
                {
                    image = CreateFeedImage(mediaObject);
                }
                else
                {
                    image = UpdateFeedImage(mediaObject, image);
                }
            }

            MediaUpdateMgr updateMgr = new MediaUpdateMgr(mediaObject);
            messages = MediaDataManager.Save(updateMgr);
            if (messages.Errors().Any())
            {
                return messages;
            }

            if (mediaObject == null || mediaObject.Id == 0)
            {
                return messages.AddError("MediaId", "MediaId missing after save");
            }

            //Add or update the enclosures

            

            return messages;
        }

        public ValidationMessages SaveMedia(MediaObject mediaObject, out MediaObject newObject)
        {
            //Logging.Logger.LogInfo("Test 7/2");
            newObject = null;
            if (mediaObject == null)
            {
                return ValidationMessages.CreateError("Media", "Media object is missing");
            }

            ValidationMessages messages = new ValidationMessages();
            bool imageExists = false;
            MediaImage image = null;
            if (mediaObject.HasImage())
            {
                image = GetFeedImage(mediaObject.Id);
                imageExists = (image != null);
                if (!imageExists)
                {
                    image = CreateFeedImage(mediaObject);
                }
                else
                {
                    image = UpdateFeedImage(mediaObject, image);
                }
            }

            var mediaList = new List<MediaObject>() { mediaObject };

            MediaUpdateMgr updateMgr = new MediaUpdateMgr(mediaList);
            messages = MediaDataManager.Save(updateMgr);
            if (messages.Errors().Any())
            {
                return messages;
            }

            if (mediaObject == null || mediaObject.Id == 0)
            {
                return messages.AddError("MediaId", "MediaId missing after save");
            }

            // Get the object to return
            ValidationMessages newMessages = new ValidationMessages();
            newObject = GetMedia(mediaObject.Id, out newMessages);

            messages.Add(newMessages);
            return messages;
        }

        private static MediaImage GetFeedImage(int mediaId)
        {
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                var fi = FeedObjectCtl.GetFeedImage(media).Where(m => m.MediaId == mediaId).FirstOrDefault();                
                return fi;
            }
        }

        private static MediaImage CreateFeedImage(MediaObject media)
        {
            var feed = media.MediaTypeSpecificDetail as FeedDetailObject;
            var image = feed.AssociatedImage;
            image.MediaId = media.MediaId;
            image.ModifiedBy = media.ModifiedByGuid;
            image.CreatedBy = media.CreatedByGuid;

            return image;
        }

        private static MediaImage UpdateFeedImage(MediaObject media, MediaImage oldObject)
        {
            var feed = media.MediaTypeSpecificDetail as FeedDetailObject;
            var image = feed.AssociatedImage;
            image.MediaId = oldObject.MediaId;
            image.ModifiedBy = media.ModifiedByGuid;
            image.CreatedBy = media.CreatedByGuid;

            return image;
        }

        public ValidationMessages SaveStorage(StorageObject storageObject, out StorageObject newObject)
        {
            newObject = null;
            if (storageObject == null)
            { return ValidationMessages.CreateError("Storage", "Storage object is missing"); }

            ValidationMessages messages = new ValidationMessages();

            StorageUpdateMgr updateMgr = new StorageUpdateMgr(storageObject);
            messages = MediaDataManager.Save(updateMgr);
            if (messages.Errors().Any())
            {
                return messages;
            }
            if (storageObject == null || storageObject.StorageId == 0)
            {
                return messages.AddError("StorageId", "MediaId missing after save");
            }
            ValidationMessages getValidationMessages;
            newObject = GetStorage(storageObject.StorageId, out getValidationMessages);

            messages.Add(getValidationMessages);

            return messages;
        }

        public ValidationMessages DeleteStorage(StorageObject storageObject)
        {
            ValidationMessages messages = new ValidationMessages();

            if (storageObject == null)
            {
                return messages.AddError("Storage", "Storage object is missing");
            }

            if (storageObject.StorageId < 1)
            {
                return messages.AddError("StorageId", "StorageId is invalid");
            }

            StorageUpdateMgr updateMgr = new StorageUpdateMgr(storageObject);
            messages = MediaDataManager.Delete(updateMgr);

            return messages;
        }

        public ValidationMessages UpdateMediaWithNewPersistentUrl(int mediaId, out MediaObject theObject)
        {
            ValidationMessages validationMessages = new ValidationMessages();
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    theObject = MediaCtl.GetComplete((MediaObjectContext)media, mediaId, forUpdate: true);
                    //For Debugging--CheckObjectStateEntry(theObject, media);
                    //.Where(m => m.Id == mediaId).FirstOrDefault();
                    if (theObject == null)
                    {
                        validationMessages.AddError(MediaUpdateMgr.TheObjectName, "Media object was not found");
                    }
                    else if (!theObject.NeedsPersistentUrl())
                    {
                        validationMessages.AddError(MediaUpdateMgr.TheObjectName, "Media object is not of a valid type to need a Persistent Url");
                    }
                    {
                        MediaCtl mediaCtl = new MediaCtl((MediaObjectContext)media, theObject);
                        //MediaObjectCtl mediaCtl = MediaObjectCtl.Create(media, theObject);
                        MediaUpdateMgr.UpdatePersistentUrlForMediaItem(media, mediaCtl, validationMessages, true);

                        //For Debugging--CheckObjectStateEntry(theObject, media);
                    }
                    scope.Complete();
                    media.AcceptAllChanges();
                }
            }

            return validationMessages;
        }

        public ValidationMessages DeleteMedia(int id)
        {
            ValidationMessages messages;
            MediaObject theObject = GetMedia(id, out messages);
            if (messages.Errors().Any())
            { return messages; }

            return MediaDataManager.Delete(new MediaUpdateMgr(theObject));
        }

        public ValidationMessages CreateSyndicationList(SyndicationListObject syndicationList)
        {
            syndicationList.SyndicationListStatusCode = "Creating";
            syndicationList.IsActive = true;
            SyndicationListUpdateMgr mediaObjectUpdateMgr = new SyndicationListUpdateMgr(syndicationList);

            return MediaDataManager.Save(mediaObjectUpdateMgr); //Insert
            //SyndicationListMedias. MediaGuids are null
        }

        public ValidationMessages UpdateSyndicationList(SyndicationListObject syndicationList)
        {
            SyndicationListUpdateMgr mediaObjectUpdateMgr = new SyndicationListUpdateMgr(syndicationList);
            return MediaDataManager.Save(mediaObjectUpdateMgr);
        }

        public ValidationMessages UpdateMediaItemsInSyndicationList(Guid syndicationListGuid, List<SyndicationListMediaObject> mediasToAdd,
            List<SyndicationListMediaObject> mediasToDelete, string emailAddress)
        {
            SyndicationListObject syndicationList = null;
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                Guid userGuid = GetUserGuid(media, emailAddress);
                if (userGuid == null)
                {
                    return ValidationMessages.CreateError("EmailAddress", "User does not exist with this email address");
                }
                ValidationMessages validationMessages = new ValidationMessages();
                //We are going to let the standard update handle getting the database object and updating it to reuse validation, etc.
                syndicationList = SyndicationListCtl.Get((MediaObjectContext)media, forUpdate: false)
                    .Where(m => m.SyndicationListGuid == syndicationListGuid)
                    .FirstOrDefault();
                if (syndicationList == null)
                {
                    return ValidationMessages.CreateError("SyndicationListGuid", "Syndication List does not exist with this Guid");
                }
                List<SyndicationListMediaObject> mediaList = syndicationList.Medias == null ? new List<SyndicationListMediaObject>() :
                    syndicationList.Medias.ToList();
                if (mediasToAdd != null)
                {
                    foreach (SyndicationListMediaObject mediaObject in mediasToAdd)
                    {
                        AddMediaItemToSyndicationList(syndicationListGuid, userGuid, mediaList, mediaObject);
                    }
                }

                if (mediasToDelete != null)
                {
                    foreach (SyndicationListMediaObject mediaObject in mediasToDelete)
                    {
                        DeleteMediaItemFromSyndicationList(validationMessages, mediaList, mediaObject);
                    }
                }

                syndicationList.Medias = mediaList;
                if (validationMessages.Errors().Any())
                {
                    return validationMessages;
                }

            }

            return UpdateSyndicationList(syndicationList);
        }

        private static void DeleteMediaItemFromSyndicationList(ValidationMessages validationMessages, List<SyndicationListMediaObject> persistedMedias,
            SyndicationListMediaObject mediaObject)
        {
            SyndicationListMediaObject persistedMedia = persistedMedias.Where(m => m.MediaId == mediaObject.MediaId).FirstOrDefault();
            if (persistedMedia == null)
            {
                validationMessages.AddError("DeletedMedia.MediaId", "Media was not found in syndication list to delete");
            }
            else
            {
                persistedMedias.Remove(persistedMedia);
            }
        }

        private static void AddMediaItemToSyndicationList(Guid syndicationListGuid, Guid userGuid, List<SyndicationListMediaObject> persistedMedias,
            SyndicationListMediaObject mediaObject)
        {
            SyndicationListMediaObject persistedMedia = persistedMedias.Where(m => m.MediaId == mediaObject.MediaId).FirstOrDefault();
            if (persistedMedia == null)
            {
                mediaObject.Complete(syndicationListGuid, userGuid);
                persistedMedias.Add(mediaObject);
            }
            else
            {
                persistedMedia.Update(userGuid);
            }
        }

        public ValidationMessages DeleteSyndicationList(params Guid[] syndicationListGuids)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return SyndicationListUpdateMgr.Delete(media, syndicationListGuids);
            }
        }

        public SyndicationListObject GetLatestSyndicationList(Guid userGuid)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                SyndicationListObject syndicationList = SyndicationListCtl.Get((MediaObjectContext)media, forUpdate: false)
                    .Where(m => m.UserGuid == userGuid && m.IsActive)
                    .OrderByDescending(m => m.CreatedDateTime)
                    .FirstOrDefault();

                return syndicationList;
            }
        }

        public SyndicationListObject GetSyndicationListByName(string listName)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                SyndicationListObject syndicationList = SyndicationListCtl.Get((MediaObjectContext)media, forUpdate: false)
                    .Where(m => m.ListName == listName)
                    .FirstOrDefault();
                return syndicationList;

            }
        }

        public SyndicationListObject GetSyndicationList(Guid listGuid)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                SyndicationListObject syndicationList = SyndicationListCtl.Get((MediaObjectContext)media, forUpdate: false)
                    .Where(m => m.SyndicationListGuid == listGuid)
                    .FirstOrDefault();
                if (syndicationList == null)
                {
                    Logging.Logger.LogWarning("Syndication List was not found:" + listGuid.ToString());
                }

                return syndicationList;
            }
        }

        public static Guid GetUserGuid(MediaObjectContext media, string emailAddress)
        {
            return media.MediaDbEntities.Users.Where(u => u.EmailAddress == emailAddress).Select(u => u.UserGuid).FirstOrDefault();
        }


        public MediaObject GetMedia(int mediaId, out ValidationMessages validationMessages)
        {
            validationMessages = new ValidationMessages();
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                var criteria = new CsBusinessObjects.Media.SearchCriteria { MediaId = mediaId.ToString() };
                var search = MediaCtl.Search(media, criteria.ToXmlString());
                var mediaObject = search.FirstOrDefault(); // see if switching to this has any side effects

                if (mediaObject == null)
                {
                    mediaObject = MediaCtl.GetComplete(media, mediaId);
                }                 

                if (mediaObject == null)
                {
                    validationMessages.AddError("mediaId", "Media with specified ID not found");
                    return null;
                }

                return mediaObject;
            }
        }

        public static MediaObject GetMediaSimple(int mediaId, out ValidationMessages validationMessages)
        {
            validationMessages = new ValidationMessages();
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                MediaObject mediaObject = MediaCtl.GetSimple(media).Where(a => a.Id == mediaId).FirstOrDefault();

                if (mediaObject == null)
                {
                    validationMessages.AddError("mediaId", "Media with specified ID not found");
                    return null;
                }

                return mediaObject;
            }
        }

        public StorageObject GetStorage(int storageId, out ValidationMessages validationMessages)
        {
            validationMessages = new ValidationMessages();
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                StorageObject storageObject = StorageCtl.GetById(media, storageId);

                if (storageObject == null)
                {
                    validationMessages.AddError("mediaId", "Media with specified ID not found");
                    return null;
                }

                return storageObject;
            }
        }

        public static FeedExportObject GetFeedExport(int exportId, out ValidationMessages validationMessages)
        {
            validationMessages = new ValidationMessages();
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                FeedExportObject item = FeedExportCtl.Get(media).Where(a => a.Id == exportId).FirstOrDefault();
                if (item == null)
                {
                    validationMessages.AddError("exportId", "FeedExport with specified ID not found");
                    return null;
                }

                return item;
            }
        }

        public static IList<LocationItem> GetGeo(int mediaId)
        {
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                return LocationItemCtl.GetGeo(media, mediaId).ToList();
            }
        }

        public static IList<MediaObject> Dupes()
        {
            using (var media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                return MediaCtl.Dupes(media).ToList();
            }
        }

        public static IList<Log> Log()
        {
            using (var db = (MediaObjectContext)ObjectContextFactory.Create())
            {
                return db.MediaDbEntities.Logs.OrderByDescending(l => l.LogDateTime).Take(1000).ToList();
            }
        }

    }
}
