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
using System.Data;
using System.Data.Objects;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{

    public class MediaUpdateMgr : IUpdateMgr
    {
        public static string TheObjectName { get { return "Media"; } }
        public string ObjectName { get { return MediaUpdateMgr.TheObjectName; } }
        public bool RequiresLogicalCommit { get { return true; } }

        private MediaObjectValidator Validator { get; set; }

        private IList<MediaObjectWithRelationships> Items;

        private IList<MediaObject> ValidationItems;


        private List<IDataCtl> _dataControls;
        /// <summary>
        /// This is used by the UpdateIdsAfterInsert method to find the persisted items and update the new ids into the business objects
        /// </summary>
        public List<IDataCtl> InsertedDataControls
        {
            get
            {
                if (_dataControls == null)
                {
                    _dataControls = new List<IDataCtl>();
                }
                return _dataControls;
            }
        }
        public MediaUpdateMgr MediaObjectUpdateMgr
        {
            get { return this; }
        }

        public MediaUpdateMgr(IList<MediaObject> items)
        {
            Initialize(ValidatorHelper.GetWithValidationKey(items, ObjectName));
        }

        public MediaUpdateMgr(MediaObject item)
        {
            Initialize(ValidatorHelper.GetWithValidationKey(item, ObjectName));
        }

        private void Initialize(IList<MediaObject> medias)
        {
            ValidationItems = medias;
            Items = medias.Select(m => new MediaObjectWithRelationships(m)).ToList();
            Validator = new MediaObjectValidator();
        }


        public void BuildValidationObjects(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaObjectWithRelationships item in Items)
            {
                //We should always have a RelationshipUpdateMgr until we determine that there we don't need it
                item.RelationshipUpdateMgr.BuildValidationObjects(media, validationMessages);

                // geoData
                item.MediaGeoDataMgr.BuildValidationObjects(media, validationMessages);

                // enclosure
                item.EnclosureMgr.BuildValidationObjects(media, validationMessages);

                if (item.AggregateMgr != null)
                {
                    // aggregate
                    item.AggregateMgr.BuildValidationObjects(media, validationMessages);
                }
            }
        }

        public void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, ValidationItems);
            foreach (MediaObjectWithRelationships item in Items)
            {
                item.RelationshipUpdateMgr.PreSaveValidate(ref validationMessages);

                // geoData
                item.MediaGeoDataMgr.PreSaveValidate(ref validationMessages);

                // enclosure
                item.EnclosureMgr.PreSaveValidate(ref validationMessages);

                if (item.AggregateMgr != null)
                {
                    // aggregate
                    item.AggregateMgr.PreSaveValidate(ref validationMessages);
                }
            }
        }

        public void ValidateSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(media, validationMessages, ValidationItems);
            foreach (MediaObjectWithRelationships item in Items)
            {
                item.RelationshipUpdateMgr.ValidateSave(media, validationMessages);

                // geoData
                item.MediaGeoDataMgr.ValidateSave(media, validationMessages);

                // enclosure
                item.EnclosureMgr.ValidateSave(media, validationMessages);

                if (item.AggregateMgr != null)
                {
                    // aggregate
                    item.AggregateMgr.ValidateSave(media, validationMessages);
                }
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, ValidationItems);
            foreach (MediaObjectWithRelationships item in Items)
            {
                item.RelationshipUpdateMgr.PreDeleteValidate(validationMessages);

                // geoData
                item.MediaGeoDataMgr.PreDeleteValidate(validationMessages);

                // enclosure
                item.EnclosureMgr.PreDeleteValidate(validationMessages);

                if (item.AggregateMgr != null)
                {
                    // aggregate
                    item.AggregateMgr.PreDeleteValidate(validationMessages);
                }
            }
        }

        public void ValidateDelete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(media, validationMessages, ValidationItems);
            foreach (MediaObjectWithRelationships item in Items)
            {
                if (item.RelationshipUpdateMgr != null)
                {
                    item.RelationshipUpdateMgr.ValidateDelete(media, validationMessages);
                }

                // GeoData
                if (item.MediaGeoDataMgr != null)
                {
                    item.MediaGeoDataMgr.ValidateDelete(media, validationMessages);
                }

                // enclosure
                if (item.EnclosureMgr != null)
                {
                    item.EnclosureMgr.ValidateDelete(media, validationMessages);
                }

                // aggregate
                if (item.AggregateMgr != null)
                {
                    item.AggregateMgr.ValidateDelete(media, validationMessages);
                }
            }
        }

        /// <summary>
        /// Framework save method to insert or update media items and relationships as needed
        /// </summary>
        /// <param name="media"></param>
        /// <param name="validationMessages"></param>
        public void Save(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaObjectWithRelationships item in Items)
            {
                if (item.MediaObject.Id == 0)
                {
                    Insert(media, item, validationMessages);
                }
                else
                {
                    Update(media, item, validationMessages);
                    foreach (var message in validationMessages.Errors())
                    {
                        message.Id = item.MediaObject.Id.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Framework Insert method to insert all media objects and relationships
        /// </summary>
        /// <param name="media"></param>
        /// <param name="validationMessages"></param>
        public void Insert(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaObjectWithRelationships mediaObject in Items)
            {
                Insert(media, mediaObject, validationMessages);
            }
        }

        /// <summary>
        /// Insert one media object and its relationships
        /// </summary>
        /// <param name="media"></param>
        /// <param name="item"></param>
        /// <param name="validationMessages"></param>
        /// <returns></returns>
        internal IDataCtl Insert(IDataServicesObjectContext media, MediaObjectWithRelationships item, ValidationMessages validationMessages)
        {
            //Don't do logical commit if no relationships
            if (!item.MediaObject.HasRelationships)
            {
                item.RelationshipUpdateMgr = null;
            }

            ResolveAttributeId(media, item.MediaObject.AttributeValues);
            media.TransactionSettings.OwnerGuid = item.MediaObject.ModifiedByGuid;

            MediaCtl mediaCtl = MediaCtl.Create(media, item.MediaObject);

            if (item.MediaObject.MediaTypeParms.IsFeed)
            {
                var feed = MediaCtl.GetSimple((MediaObjectContext)media)
                    .Where(a => a.Title == item.MediaObject.Title && a.MediaTypeCode == "Feed").FirstOrDefault();
                if (feed != null)
                {
                    validationMessages.AddError(item.MediaObject.ValidationKey + ".Title", feed.Id.ToString(), "Title already exists", "");
                }

                var detail = item.MediaObject.MediaTypeSpecificDetail as FeedDetailObject;
                if (detail != null)
                {
                    mediaCtl.AddFeedImage(detail.AssociatedImage);
                }
            }

            if (item.MediaObject.MediaTypeParms.IsFeedItem)
            {
                var detail = item.MediaObject.MediaTypeSpecificDetail as FeedDetailObject;
                if (detail != null)
                {
                    if (item.ImageMgr != null)
                    {
                        var img = item.ImageMgr.Create(media).FirstOrDefault();
                        mediaCtl.AddFeedImage2(img);
                    }
                }
            }

            var newAttributes = item.MediaObject.SafeGetAttributes();

            mediaCtl.AddToDb();
            if (newAttributes.Count() > 0)
            {
                List<AttributeValueObjectCtl> insertedAttributes = CreateDbObject(newAttributes, media).ToList();
                mediaCtl.AddReference(insertedAttributes);
            }

            InsertedDataControls.Add(mediaCtl);
            if (item.RelationshipUpdateMgr != null)
            {
                List<MediaRelationshipCtl> newRelationships = item.RelationshipUpdateMgr.Create(media);
                mediaCtl.AddRelationships(newRelationships);
            }

            if (item.MediaTypeSpecificUpdateMgr != null)
            {
                item.MediaTypeSpecificUpdateMgr.Save(media, item.MediaObject, mediaCtl);
            }

            if (item.MediaGeoDataMgr != null)
            {
                List<GeoDataCtl> newRelationships = item.MediaGeoDataMgr.Create(media);
                mediaCtl.AddGeoDatas(newRelationships);
            }

            if (item.ExtendedAttributeDataMgr != null)
            {
                if (item.MediaObject.ExtendedAttributes != null)
                {
                    mediaCtl.AddExtendedAttributes(item.MediaObject.ExtendedAttributes.ToList());
                }
            }

            if (item.EnclosureMgr != null)
            {
                item.EnclosureMgr.Save(media, item.MediaObject, mediaCtl);
            }

            if (item.AggregateMgr != null)
            {
                item.AggregateMgr.Save(media, item.MediaObject, mediaCtl);
            }

            if (item.FeedExportSettingMgr != null)
            {
                item.FeedExportSettingMgr.Save(media, item.MediaObject, mediaCtl);
            }

            return mediaCtl;
        }

        /// <summary>
        /// Framework method to update all media objects
        /// </summary>
        /// <param name="media"></param>
        /// <param name="validationMessages"></param>
        public void Update(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaObjectWithRelationships mediaObject in Items)
            {
                Update(media, mediaObject, validationMessages);
            }
        }

        /// <summary>
        /// Update one media object
        /// </summary>
        /// <param name="media"></param>
        /// <param name="item"></param>
        /// <param name="validationMessages"></param>
        internal void Update(IDataServicesObjectContext media, MediaObjectWithRelationships item, ValidationMessages validationMessages)
        {
            var db = media as MediaObjectContext;
            //Don't do logical commit if no relationships

            ResolveAttributeId(media, item.MediaObject.AttributeValues);

            MediaObject persistedMediaObject = MediaCtl.GetComplete(db, item.MediaObject.Id, forUpdate: true);
            if (persistedMediaObject == null)
            {
                validationMessages.AddError(item.MediaObject.ValidationKey, "Media object was not found to update");
            }

            if (item.MediaObject.MediaTypeParms.IsFeed)
            {
                var feed = MediaCtl.GetSimple((MediaObjectContext)media)
                    .Where(a => item.MediaObject.Id != a.Id && a.Title == item.MediaObject.Title && a.MediaTypeCode == "Feed").FirstOrDefault();
                if (feed != null)
                {
                    validationMessages.AddError(item.MediaObject.ValidationKey + ".Title", feed.Id.ToString(), "Title already exists", "");
                }
            }

            media.TransactionSettings.OwnerGuid = item.MediaObject.ModifiedByGuid;
            MediaCtl mediaCtl = MediaCtl.Update(media, persistedMediaObject, item.MediaObject, enforceConcurrency: true);

            var newAttributeValues = item.MediaObject.SafeGetAttributes().OrderBy(v => v.AttributeId).ThenBy(v => v.ValueKey.Id);
            var persistedAttributeValues = persistedMediaObject.SafeGetAttributes().OrderBy(v => v.AttributeId).ThenBy(v => v.ValueKey.Id);

            List<AttributeValueObjectCtl> newAttributes = CreateNewAttributes(media, mediaCtl, newAttributeValues, persistedAttributeValues);
            mediaCtl.AddReference(newAttributes);

            UpdateAttributes(media, newAttributeValues, persistedAttributeValues);

            //If no relationships to add/delete, then don't force a logical commit
            if (!item.MediaObject.HasRelationships && !persistedMediaObject.HasRelationships)
            {
                item.RelationshipUpdateMgr = null;
            }
            else
            {
                var allRelationships = MediaRelationshipCtl.Get(db, forUpdate: true)
                .Where(mr => mr.MediaId == item.MediaObject.Id || mr.RelatedMediaId == item.MediaObject.Id).ToList();

                var parentRelationships = allRelationships.Where(r => r.RelationshipTypeName == "Is Parent Of").ToList();
                var childRelationships = allRelationships.Where(r => r.RelationshipTypeName == "Is Child Of").ToList();

                List<MediaRelationshipCtl> newRelationships = item.RelationshipUpdateMgr.Update(media, parentRelationships, childRelationships, validationMessages);
                mediaCtl.AddRelationships(newRelationships);
            }

            if (item.MediaTypeSpecificUpdateMgr != null)
            {
                item.MediaTypeSpecificUpdateMgr.Save(media, item.MediaObject, mediaCtl);
            }

            if (item.MediaGeoDataMgr != null)
            {
                List<GeoDataCtl> newRelationships = item.MediaGeoDataMgr.Create(media);
                mediaCtl.AddGeoDatas(newRelationships);

                if (item.MediaObject.MediaGeoData != null)
                {
                    var newGeoLocations = item.MediaObject.MediaGeoData.OrderBy(a => a.MediaId).ThenBy(a => a.GeoNameId);
                    var persistedGeoLocations = persistedMediaObject.MediaGeoData.OrderBy(a => a.MediaId).ThenBy(a => a.GeoNameId);
                    UpdateGeoLocation(media, newGeoLocations, persistedGeoLocations);
                }
            }

            if (item.ExtendedAttributeDataMgr != null && item.MediaObject.ExtendedAttributes != null)
            {
                mediaCtl.AddExtendedAttributes(item.MediaObject.ExtendedAttributes.ToList());
            }

            var detail = item.MediaObject.MediaTypeSpecificDetail as FeedDetailObject;
            if (detail != null)
            {
                mediaCtl.UpdateFeedImage(item.MediaObject);
            }

            if (item.EnclosureMgr != null)
            {
                item.EnclosureMgr.Save(media, item.MediaObject, mediaCtl);
            }

            if (item.AggregateMgr != null)
            {
                item.AggregateMgr.Save(media, item.MediaObject, mediaCtl);
            }

            if (item.FeedExportSettingMgr != null)
            {
                item.FeedExportSettingMgr.Save(media, item.MediaObject, mediaCtl);
            }
        }

        public void AdditionalSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (IDataCtl dataCtl in InsertedDataControls)
            {
                MediaCtl mediaCtl = (MediaCtl)dataCtl;
                if (mediaCtl.NewBusinessObject.NeedsPersistentUrl())
                {
                    UpdatePersistentUrlForMediaItem(media, mediaCtl, validationMessages, performSave: false);
                }
            }
        }

        internal static void UpdatePersistentUrlForMediaItem(IDataServicesObjectContext media, MediaCtl mediaCtl, ValidationMessages validationMessages, bool performSave)
        {
            string persistentUrlToRefresh = null;
            int numberOfTimesToRetryNewPersistentUrl = 10;
            do
            {
                mediaCtl.SetNewPersistentUrl(PersistentUrlGenerator.GenerateNewPersistentUrl());
                //For Debugging--CheckObjectStateEntry(mediaCtl.NewBusinessObject, media);
                try
                {
                    persistentUrlToRefresh = null;
                    if (performSave)
                    {
                        media.SaveChanges(SaveOptions.DetectChangesBeforeSave);
                    }
                }
                catch (OptimisticConcurrencyException ex)
                {
                    validationMessages.Add(
                        new ValidationMessage(
                            ValidationMessage.ValidationSeverity.Error, "", "", "The media has changed since your last request", ex.StackTrace));
                }
                catch (UpdateException uex)
                {
                    persistentUrlToRefresh = GetPersistentUrlToRefreshFromException(uex);
                }
                numberOfTimesToRetryNewPersistentUrl--;
            }
            while (persistentUrlToRefresh != null && numberOfTimesToRetryNewPersistentUrl > 0);
            if (persistentUrlToRefresh != null)
            {
                throw new ApplicationException("PersistentUrl " + persistentUrlToRefresh + " is a duplicate and has been attempted to be refreshed 10 times.");
            }
        }

        private static string GetPersistentUrlToRefreshFromException(UpdateException uex)
        {
            const string duplicatePersistentUrlMessage = "Cannot insert duplicate key row in object 'Media.Media' with unique index 'IX_PersistentURLToken'. The duplicate key value is (";
            if (uex.InnerException != null && uex.InnerException.Message.StartsWith(duplicatePersistentUrlMessage))
            {
                string rest = uex.InnerException.Message.Substring(duplicatePersistentUrlMessage.Length);
                return rest.Substring(0, rest.IndexOf(")"));
            }
            else
            {
                return null;
            }
        }


        private static void UpdateAttributes(IDataServicesObjectContext media, IOrderedEnumerable<AttributeValueObject> newAttributeValues, IOrderedEnumerable<AttributeValueObject> persistedAttributeValues)
        {
            //If active is removed, then that is an update.  Not an insert
            var existingAttributes = (
                    from p in persistedAttributeValues
                    join n in newAttributeValues on new { p.AttributeId, p.ValueKey.Id } equals new { n.AttributeId, n.ValueKey.Id }
                    into nGroup
                    from n in nGroup.DefaultIfEmpty()
                    select new { PersistedAttribute = p, NewAttribute = n }).ToList();

            foreach (var v in existingAttributes)
            {
                if (v.NewAttribute != null && v.NewAttribute.IsActive)
                {
                    AttributeValueObjectCtl.Update(media, v.PersistedAttribute, v.NewAttribute, enforceConcurrency: false);
                }
                else
                {
                    AttributeValueObjectCtl.Delete(media, v.PersistedAttribute);
                }
            }
        }

        private static void UpdateGeoLocation(IDataServicesObjectContext media, IOrderedEnumerable<MediaGeoDataObject> newObjects, IOrderedEnumerable<MediaGeoDataObject> persistedObjects)
        {
            //If active is removed, then that is an update.  Not an insert
            var existingObjects = (
                    from p in persistedObjects
                    join n in newObjects on new { p.MediaId, p.GeoNameId } equals new { n.MediaId, n.GeoNameId }
                    into nGroup
                    from n in nGroup.DefaultIfEmpty()
                    select new { PersistedObject = p, NewObject = n });

            existingObjects = existingObjects.Where(a => a.NewObject == null).ToList();
            foreach (var item in existingObjects)
                GeoDataCtl.Delete(media, item.PersistedObject);
        }

        public void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaObjectWithRelationships item in Items)
            {
                Delete(media, validationMessages, item);
            }
        }

        private void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages, MediaObjectWithRelationships item)
        {
            MediaObject persistedMediaObject = MediaCtl.GetComplete((MediaObjectContext)media, item.MediaObject.Id, forUpdate: true);
            if (persistedMediaObject == null)
            {
                validationMessages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, item.MediaObject.ValidationKey,
                      "Media object was not found to delete"));
            }

            foreach (AttributeValueObject attributeValue in persistedMediaObject.SafeGetAttributes())
            {
                AttributeValueObjectCtl.Delete(media, attributeValue);
            }

            if (item.MediaTypeSpecificUpdateMgr != null)
            {
                item.MediaTypeSpecificUpdateMgr.Delete(media, item.MediaObject);
            }

            if (item.RelationshipUpdateMgr != null)
            {
                item.RelationshipUpdateMgr.Delete(media, validationMessages);
            }

            if (item.StorageMgr != null)
            {
                item.StorageMgr.Delete(media, validationMessages);
            }

            if (persistedMediaObject.AssociatedImage != null)
            {
                var db = media as MediaObjectContext;
                var image = db.MediaDbEntities.Images.FirstOrDefault(i => i.MediaID == persistedMediaObject.MediaId);
                if (image != null)
                {
                    db.MediaDbEntities.Images.DeleteObject(image);
                }
            }

            if (item.EnclosureMgr!= null)
            {
                item.EnclosureMgr.Delete(media, validationMessages);
            }

            if (item.AggregateMgr != null)
            {
                item.AggregateMgr.Delete(media, validationMessages);
            }

            MediaCtl.Delete(media, persistedMediaObject);
        }

        public void UpdateIdsAfterInsert()
        {
            foreach (IDataCtl dataCtl in InsertedDataControls)
            {
                dataCtl.UpdateIdsAfterInsert();
            }
            foreach (MediaObjectWithRelationships item in Items)
            {
                if (item.RelationshipUpdateMgr != null)
                {
                    item.RelationshipUpdateMgr.UpdateIdsAfterInsert();
                }
            }
        }

        public void ResolveAttributeId(IDataServicesObjectContext media, IEnumerable<AttributeValueObject> attributeValues)
        {
            if (attributeValues == null)
            {
                return;
            }
            foreach (AttributeValueObject attributeValue in attributeValues)
            {
                if (attributeValue.AttributeId == 0 && !string.IsNullOrEmpty(attributeValue.AttributeName))
                {
                    attributeValue.AttributeId = GetAttributeId(media, attributeValue);
                }
                //TODO: figure out how to save an attribute value with the correct language
            }

            //Update the language for all values
            var x = (from av in attributeValues
                     join v in ValueCtl.Get((MediaObjectContext)media)
                         on av.ValueKey.Id equals v.ValueId
                     select UpdateLanguage(av, v)).ToList();
        }

        public string UpdateLanguage(AttributeValueObject attributeValue, ValueObject valueObject)
        {
            attributeValue.ValueKey.LanguageCode = valueObject.LanguageCode;
            return valueObject.LanguageCode;
        }

        private int GetAttributeId(IDataServicesObjectContext media, AttributeValueObject attributeValue)
        {
            return GetAllAttributes(media).Where(a => string.Equals(a.Name, attributeValue.AttributeName, StringComparison.OrdinalIgnoreCase)).Select(a => a.AttributeId).FirstOrDefault();
        }

        private static List<AttributeValueObjectCtl> CreateNewAttributes(IDataServicesObjectContext media, MediaCtl mediaCtl, IOrderedEnumerable<AttributeValueObject> newAttributeValues,
            IOrderedEnumerable<AttributeValueObject> persistedAttributeValues)
        {
            //If active is removed, then that is an update.  Not an insert
            List<AttributeValueObject> newAttributes = (
                from n in newAttributeValues
                join _p in persistedAttributeValues on new { n.AttributeId, n.ValueKey.Id } equals new { _p.AttributeId, _p.ValueKey.Id }
                into p
                from _p in p.DefaultIfEmpty()
                where p == null || !p.Any()
                select n).ToList();

            return CreateDbObject(newAttributes, media).ToList();
        }

        private static IEnumerable<AttributeValueObjectCtl> CreateDbObject(IEnumerable<AttributeValueObject> values, IDataServicesObjectContext media)
        {
            if (values == null)
            {
                return new List<AttributeValueObjectCtl>();
            }
            else
            {
                return values.Select(v => AttributeValueObjectCtl.Create(media, v));
            }
        }

        public void PostSaveValidate(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaObjectWithRelationships item in Items)
            {
                if (item.RelationshipUpdateMgr != null)
                {
                    item.RelationshipUpdateMgr.PostSaveValidate(media, validationMessages);
                }
            }
        }

        public void LogicalCommit(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaObjectWithRelationships item in Items)
            {
                if (item.RelationshipUpdateMgr != null)
                {
                    item.RelationshipUpdateMgr.LogicalCommit(media, validationMessages);
                }
            }
        }

        public void LogicalRollback(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaObjectWithRelationships item in Items)
            {
                if (item.RelationshipUpdateMgr != null)
                {
                    item.RelationshipUpdateMgr.LogicalRollback(media, validationMessages);
                }
            }
        }

        private List<AttributeObject> _allAttributes = null;
        private List<AttributeObject> GetAllAttributes(IDataServicesObjectContext media)
        {
            //TODO: Add this to the cache to improve performance
            return _allAttributes ?? (_allAttributes = AttributeCtl.Get((MediaObjectContext)media, forUpdate: false).ToList());
        }

    }
}
