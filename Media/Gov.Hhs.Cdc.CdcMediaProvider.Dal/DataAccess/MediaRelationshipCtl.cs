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
using System.Linq;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class MediaRelationshipCtl : BaseCtl<MediaRelationshipObject, MediaRelationship, MediaRelationshipCtl, MediaObjectContext>
    {
        public override string ToString()
        {
            return (PersistedBusinessObject ?? NewBusinessObject ?? new MediaRelationshipObject()).ToString();
        }
        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new MediaRelationship()
            {
                RelationshipTypeName = NewBusinessObject.RelationshipTypeName,
                MediaId = NewBusinessObject.MediaId,
                RelatedMediaId = NewBusinessObject.RelatedMediaId,
                CreatedDateTime = modifiedDateTime,
                CreatedByGuid = modifiedGuid
            };
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.DisplayOrdinal = NewBusinessObject.DisplayOrdinal;
            PersistedDbObject.Active = !NewBusinessObject.IsCommitted ? "Unc" :
                    (NewBusinessObject.IsActive ? "Yes" : "No");

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            if (PersistedDbObject == null)
            {
                return false;
            }

            return PersistedDbObject.RelationshipTypeName == NewBusinessObject.RelationshipTypeName
                && PersistedDbObject.MediaId == NewBusinessObject.MediaId
                && PersistedDbObject.RelatedMediaId == NewBusinessObject.RelatedMediaId
                && PersistedDbObject.DisplayOrdinal == NewBusinessObject.DisplayOrdinal
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive;
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override void UpdateIdsAfterInsert()
        {
            //The relationship may be added as a child or parent of the current media, so either Id may be missing
            //Update both just in case
            PersistedBusinessObject.MediaId = PersistedDbObject.MediaId;
            PersistedBusinessObject.RelatedMediaId = PersistedDbObject.RelatedMediaId;
            PersistedBusinessObject.DisplayOrdinal = PersistedDbObject.DisplayOrdinal;
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.MediaRelationships.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.MediaRelationships.AddObject(PersistedDbObject);
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<MediaRelationshipObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            return CreateMediaRelationships(media.MediaDbEntities.MediaRelationships, forUpdate);
        }

        public static IQueryable<MediaRelationshipObject> CreateMediaRelationships(IQueryable<MediaRelationship> relationships, bool forUpdate = false)
        {
            IQueryable<MediaRelationshipObject> mediaRelationshipItem = from r in relationships
                                                                        select new MediaRelationshipObject()
                                                                        {
                                                                            RelationshipTypeName = r.RelationshipTypeName,
                                                                            MediaId = r.MediaId,
                                                                            RelatedMediaId = r.RelatedMediaId,
                                                                            DisplayOrdinal = r.DisplayOrdinal,
                                                                            IsActive = r.Active == "Yes",
                                                                            DbObject = forUpdate ? r : null
                                                                        };
            return mediaRelationshipItem;
        }
    }
}
