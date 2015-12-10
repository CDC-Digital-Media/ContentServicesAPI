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
    public class ValueRelationshipCtl : BaseCtl<ValueRelationshipObject, ValueRelationship, ValueRelationshipCtl, MediaObjectContext>
    {
        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new ValueRelationship()
            {
                RelationshipTypeName = NewBusinessObject.RelationshipTypeName,
                ValueID = NewBusinessObject.ValueId,
                ValueLanguageCode = NewBusinessObject.ValueLanguageCode,
                RelatedValueID = NewBusinessObject.RelatedValueId,
                RelatedValueLanguageCode = NewBusinessObject.RelatedValueLanguageCode,
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
            return PersistedDbObject.RelationshipTypeName == NewBusinessObject.RelationshipTypeName
                && PersistedDbObject.ValueID == NewBusinessObject.ValueId
                && PersistedDbObject.ValueLanguageCode == NewBusinessObject.ValueLanguageCode
                && PersistedDbObject.RelatedValueID == NewBusinessObject.RelatedValueId
                && PersistedDbObject.RelatedValueLanguageCode == NewBusinessObject.RelatedValueLanguageCode
                && PersistedDbObject.DisplayOrdinal == NewBusinessObject.DisplayOrdinal
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive;
        }
        

        public override bool VersionMatches()
        {
            //TODO: Add Row Version matching
            //return PersistedDbObject.RowVersion.SequenceEqual(NewBusinessObject.RowVersion);
            return true;
        }


        public override void UpdateIdsAfterInsert()
        {
            //PersistedBusinessObject.ValueID = PersistedDbObject.ValueID;
            throw new NotImplementedException("Not implemented on purpose");
        }


        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.ValueRelationships.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.ValueRelationships.AddObject(PersistedDbObject);

            //We don't have the ID yet, and will have to get it after the commit.
            //NewBusinessObject.Id = PersistedDbObject.MediaId;
            //PersistedBusinessObject.Id = PersistedDbObject.MediaId;
        }


        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<ValueRelationshipObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<ValueRelationshipObject> mediaItems = from vr in media.MediaDbEntities.ValueRelationships
                                                             select new ValueRelationshipObject()
                                                             {
                                                                 RelationshipTypeName = vr.RelationshipTypeName,
                                                                 ValueId = vr.ValueID,
                                                                 ValueLanguageCode = vr.ValueLanguageCode,
                                                                 TypeName = vr.RelationshipType.ShortType,
                                                                 Description = vr.RelationshipTypeName,

                                                                 RelatedValueId = vr.RelatedValueID,
                                                                 RelatedValueLanguageCode = vr.RelatedValueLanguageCode,
                                                                 RelatedValueName = vr.RelatedValue.ValueName,
                                                                 DisplayOrdinal = vr.DisplayOrdinal,
                                                                 IsActive = vr.Active == "Yes",
                                                                 IsCommitted = vr.Active != "Unc",
                                                                 IsBeingDeleted = false,
                                                                 DbObject = forUpdate ? vr : null
                                                             };
            return mediaItems;
        }

    }



}
