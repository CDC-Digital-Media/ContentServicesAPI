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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class SyndicationListMediaCtl : BaseCtl<SyndicationListMediaObject, SyndicationListMedia, SyndicationListMediaCtl, MediaObjectContext>
    {
        public override void UpdateIdsAfterInsert()
        {
        }

        public override string ToString()
        {
            return PersistedBusinessObject.MediaId.ToString();
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (
                   AsBool(PersistedDbObject.HasPulledCode) == NewBusinessObject.HasPulledCode
                && PersistedDbObject.LastPulledCodeDateTime == NewBusinessObject.LastPulledCodeDateTime
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive
                );
        }

        public override bool VersionMatches()
        {
            return PersistedDbObject.RowVersion.SequenceEqual(NewBusinessObject.RowVersion);
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<SyndicationListMediaObject> Get(MediaObjectContext mediaDb, bool forUpdate)
        {
            IQueryable<SyndicationListMediaObject> items = from s in mediaDb.MediaDbEntities.SyndicationListMedias
                                                           join _u in mediaDb.MediaDbEntities.Users on s.ModifiedByGuid equals _u.UserGuid into theUsers
                                                           from u in theUsers.DefaultIfEmpty()
                                             select new SyndicationListMediaObject()
                                             {
                                                 SyndicationListGuid = s.MyListGuid,
                                                 MediaId = s.MediaId,
                                                 HasPulledCode = s.HasPulledCode == "Yes",
                                                 LastPulledCodeDateTime = s.LastPulledCodeDateTime,
                                                 IsActive = s.Active == "Yes",
                                                 RowVersion = s.RowVersion,
                                                 DbObject = forUpdate ? s : null,
                                                 ModifiedByGuid = s.ModifiedByGuid,
                                                 ModifiedByEmailAddress = u == null ? null : u.EmailAddress,
                                                 ModifiedDateTime = u.ModifiedDateTime
                                             };
            
            items = items.OrderByDescending(a => a.ModifiedDateTime);
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new SyndicationListMedia();
            if( PersistedBusinessObject.SyndicationListGuid != null)
                PersistedDbObject.MyListGuid = (Guid)PersistedBusinessObject.SyndicationListGuid;
            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;

            PersistedBusinessObject.SyndicationListGuid = PersistedDbObject.MyListGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.HasPulledCode = NewBusinessObject.HasPulledCode ? "Yes" : "No";
            PersistedDbObject.LastPulledCodeDateTime = NewBusinessObject.LastPulledCodeDateTime;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";
            PersistedDbObject.MediaId = NewBusinessObject.MediaId;
            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = NewBusinessObject.ModifiedByGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.SyndicationListMedias.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.SyndicationListMedias.DeleteObject(PersistedDbObject);
        }

        public void Delete(DataSourceBusinessObject persistedBusinessObject)
        {
            TheObjectContext.MediaDbEntities.SyndicationListMedias.DeleteObject((SyndicationListMedia)persistedBusinessObject.DbObject);
        }
    }
}
