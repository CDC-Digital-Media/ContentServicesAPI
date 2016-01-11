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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class MediaRelationshipUpdateMgr : IUpdateMgr
    {
        protected MediaRelationshipValidator Validator { get; set; }
        private IList<MediaRelationshipObject> Items;
        protected List<MediaRelationshipValidationObject> RelationshipSets;

        public List<MediaRelationshipObject> AllRelationships
        {
            get
            {
                return RelationshipSets.SelectMany(vi => new List<MediaRelationshipObject>() { vi.Relationship, vi.Inverse }).ToList();
            }
        }

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
        public MediaRelationshipValidationObject GetMediaRelationshipValidationObject(IDataServicesObjectContext media, MediaRelationshipObject r, IEnumerable<MediaRelationshipObject> persistedInverseRelationships, ValidationMessages messages)
        {
            var whatAnInverseRelationshipWouldLookLike = MediaRelationshipValidator.GetInverseRelationship(media, r, messages);

            return new MediaRelationshipValidationObject()
            {
                OriginalRelationship = r,
                OriginalInverse = persistedInverseRelationships.Where(pir => pir.MediaId == whatAnInverseRelationshipWouldLookLike.MediaId && pir.RelationshipTypeName == whatAnInverseRelationshipWouldLookLike.RelationshipTypeName).FirstOrDefault()
            };
        }

        public bool RequiresLogicalCommit { get { return true; } }

        public string ObjectName { get { return "MediaRelationship"; } }
        public MediaRelationshipUpdateMgr(IList<MediaRelationshipObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new MediaRelationshipValidator();
        }

        public MediaRelationshipUpdateMgr(MediaRelationshipObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new MediaRelationshipValidator();
        }

        public void BuildValidationObjects(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            RelationshipSets =
                (List<MediaRelationshipValidationObject>)Items.Select(i => (MediaRelationshipValidationObject)
                    Validator.GetValidationObject(media, validationMessages, i)).ToList();
        }

        internal List<MediaRelationshipCtl> Create(IDataServicesObjectContext media)
        {
            List<MediaRelationshipCtl> insertedRelationships = RelationshipSets.SelectMany(vi => Create(media, vi)).ToList();
            InsertedDataControls.AddRange(insertedRelationships);
            return insertedRelationships;
        }

        private static List<MediaRelationshipCtl> Create(IDataServicesObjectContext media, MediaRelationshipValidationObject vi)
        {
            MediaRelationshipCtl InsertedRelationship = MediaRelationshipCtl.Create(media, vi.Relationship);
            MediaRelationshipCtl InsertedInverse = MediaRelationshipCtl.Create(media, vi.Inverse);
            return new List<MediaRelationshipCtl>() { InsertedRelationship, InsertedInverse };
        }

        public List<MediaRelationshipCtl> Update(IDataServicesObjectContext media, List<MediaRelationshipObject> persistedRelationships, List<MediaRelationshipObject> persistedInverseRelationships, ValidationMessages messages)
        {
            foreach (MediaRelationshipValidationObject validationObject in RelationshipSets)
            {
                if (validationObject.Relationship != null)
                {
                    validationObject.OriginalRelationship = persistedRelationships.Where(oldMr =>
                                  validationObject.Relationship.RelatedMediaId == oldMr.RelatedMediaId
                               && validationObject.Relationship.RelationshipTypeName == oldMr.RelationshipTypeName).FirstOrDefault();
                }
                if (validationObject.Inverse != null)
                {
                    validationObject.OriginalInverse = persistedInverseRelationships.Where(oldMr =>
                                 validationObject.Inverse.MediaId == oldMr.MediaId
                              && validationObject.Inverse.RelationshipTypeName == oldMr.RelationshipTypeName).FirstOrDefault();
                }
            };
            //It wants to delete both parent relationships, which is not right
            var relationshipsToDelete = persistedRelationships.Where(oldMr => !MatchesAnyNewRelationships(oldMr));

            var deletedValidationObjects = relationshipsToDelete.Select(r => GetMediaRelationshipValidationObject(media, r, persistedInverseRelationships, messages)).ToList();
            RelationshipSets.AddRange(deletedValidationObjects);

            List<MediaRelationshipCtl> insertedRelationships = new List<MediaRelationshipCtl>();
            foreach (MediaRelationshipValidationObject item in RelationshipSets)
            {
                UpdateRelationship(media, insertedRelationships, item.Relationship, item.OriginalRelationship);
                UpdateRelationship(media, insertedRelationships, item.Inverse, item.OriginalInverse);
            }
            InsertedDataControls.AddRange(insertedRelationships);
            return insertedRelationships;
        }

        private bool MatchesAnyNewRelationships(MediaRelationshipObject oldRelationship)
        {
            return Items.Where(newMr =>
                                   newMr.RelatedMediaId == oldRelationship.RelatedMediaId
                                && newMr.RelationshipTypeName == oldRelationship.RelationshipTypeName).Any();
        }

        private static void UpdateRelationship(IDataServicesObjectContext media, ICollection<MediaRelationshipCtl> insertedRelationships, MediaRelationshipObject relationship, MediaRelationshipObject originalRelationship)
        {
            if (relationship != null && originalRelationship == null)
            {
                insertedRelationships.Add(MediaRelationshipCtl.Create(media, relationship));
            }
            else if (relationship != null && originalRelationship != null)
            {
                MediaRelationshipCtl.Update(media, originalRelationship, relationship, enforceConcurrency: false);
            }
            else if (relationship == null && originalRelationship != null)
            {
                MediaRelationshipCtl.Delete(media, originalRelationship);
            }
        }

        /// <summary>
        /// Save is not implemented.  A relationshipset should be saved by the media directly using Insert/Update
        /// </summary>
        /// <param name="media"></param>
        /// <param name="validationMessages"></param>
        public void Save(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Insert is not implemented as relationships will always be created by the parent
        /// media item
        /// </summary>
        /// <param name="media"></param>
        /// <param name="validationMessages"></param>
        public void Insert(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();

        }


        /// <summary>
        /// Update is not implemented as relationships will always be updated by the parent
        /// media item
        /// </summary>
        /// <param name="media"></param>
        /// <param name="validationMessages"></param>
        public void Update(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {

        }

        public void Delete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            if (RelationshipSets == null)
            {
                return;
            }

            foreach (MediaRelationshipValidationObject set in RelationshipSets)
            {
                Delete(media, set.Relationship, "Relationship", validationMessages);
                Delete(media, set.Inverse, "Inverse relationship", validationMessages);

                MediaUpdateMgr mgr = new MediaUpdateMgr(
                    MediaCtl.Get((MediaObjectContext)media)
                    .Where(a => a.MediaId == set.Relationship.RelatedMediaId && a.MediaTypeCode == MediaTypeParms.FeedImageMediaType)
                        .ToList());

                mgr.Delete(media, validationMessages);
            }
        }

        private void Delete(IDataServicesObjectContext media, MediaRelationshipObject relationship, string description, ValidationMessages validationMessages)
        {
            MediaRelationshipObject persistedInverse = GetForUpdate(media, relationship);
            if (persistedInverse == null)
            {
                if (validationMessages != null)
                {
                    validationMessages.AddWarning(relationship.ValidationKey, description + " not found to delete");
                }
            }
            else
            {
                MediaRelationshipCtl.Delete(media, persistedInverse);
            }
        }

        public void PostSaveValidate(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.PostSaveValidate(media, validationMessages, RelationshipSets);
            SetCommitted(RelationshipSets, true);
        }

        public void LogicalRollback(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaRelationshipValidationObject set in RelationshipSets)
            {
                RollBackRelationship(media, set.Relationship, set.OriginalRelationship, validationMessages, "Relationship");
                RollBackRelationship(media, set.Inverse, set.OriginalInverse, validationMessages, "Inverse Relationship");
            };
        }

        private void RollBackRelationship(IDataServicesObjectContext media, MediaRelationshipObject newRelationship, MediaRelationshipObject originalRelationship, ValidationMessages validationMessages, string description)
        {
            MediaRelationshipObject persistedRelationship = GetForUpdate(media, newRelationship ?? originalRelationship);
            if (originalRelationship != null)
            {
                //We had a relationship before the save, make sure it gets reverted back
                if (newRelationship == null)
                {
                    MediaRelationshipCtl.Create(media, originalRelationship);
                }
                else
                {
                    MediaRelationshipCtl.Update(media, persistedRelationship, originalRelationship, enforceConcurrency: true);
                }
            }
            else if (newRelationship != null)
            {
                MediaRelationshipCtl.Delete(media, persistedRelationship);
            }
        }

        public void LogicalCommit(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            foreach (MediaRelationshipValidationObject relationshipSet in RelationshipSets)
            {
                CommitRelationship(media, relationshipSet.Relationship, validationMessages, "Relationship");
                CommitRelationship(media, relationshipSet.Inverse, validationMessages, "Inverse Relationship");
            }
        }

        private void CommitRelationship(IDataServicesObjectContext media, MediaRelationshipObject relationship, ValidationMessages validationMessages, string description)
        {
            if (relationship == null)
            {
                return;
            }
            MediaRelationshipObject persistedRelationship = GetForUpdate(media, relationship);
            if (persistedRelationship == null)
            {
                LogRelationshipError(description + " not found to commit", "SaveRelationship", relationship);
                validationMessages.AddError(relationship.ValidationKey, "Media Relationship was not found after update");
            }
            else
            {
                MediaRelationshipCtl.Update(media, persistedRelationship, relationship, enforceConcurrency: false);
            }
        }

        private static void LogRelationshipError(string message, string methodName, MediaRelationshipObject relationship)
        {
            Logger.LogError(
                    string.Format("MediaRelationshipUpdateMgr: {0}  MediaId={1}, RelatedMediaId={2}, RelationshipType={3}",
                    message, relationship.MediaId, relationship.RelatedMediaId, relationship.RelationshipTypeName),
                    methodName);
        }

        private MediaRelationshipObject GetForUpdate(IDataServicesObjectContext media, MediaRelationshipObject relationship)
        {
            return MediaRelationshipCtl.Get((MediaObjectContext)media, forUpdate: true)
                .Where(m =>
                    m.RelationshipTypeName == relationship.RelationshipTypeName &&
                    m.MediaId == relationship.MediaId && m.RelatedMediaId == relationship.RelatedMediaId
                ).FirstOrDefault();
        }

        private static void SetCommitted(IList<MediaRelationshipValidationObject> relationshipSets, bool committed)
        {
            foreach (MediaRelationshipValidationObject set in relationshipSets)
            {
                if (set.Relationship != null)
                {
                    set.Relationship.IsCommitted = committed;
                }
                if (set.Inverse != null)
                {
                    set.Inverse.IsCommitted = committed;
                }
            }
        }

        public void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
        }

        public void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
        }

        public void ValidateSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(media, validationMessages, RelationshipSets);
        }

        public void ValidateDelete(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(media, validationMessages, RelationshipSets);
        }

        public void AdditionalSave(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
        }

        public void UpdateIdsAfterInsert()
        {
            foreach (MediaRelationshipCtl dataCtl in InsertedDataControls)
            {
                dataCtl.UpdateIdsAfterInsert();
            }
        }

    }
}
