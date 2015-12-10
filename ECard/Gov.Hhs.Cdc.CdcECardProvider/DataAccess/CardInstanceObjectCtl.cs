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
using System.Text;
using Gov.Hhs.Cdc.ECardProvider;
using Gov.Hhs.Cdc.DataServices;

namespace Gov.Hhs.Cdc.CdcECardProvider.DataAccess
{
    public class CardInstanceObjectCtl : BaseCtl<CardInstanceObject, Gov.Hhs.Cdc.CdcECardProvider.Model.CardInstance,
        CardInstanceObjectCtl, ECardObjectContext>
    {

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.CardInstanceId = PersistedDbObject.CardInstanceId;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.RecipientEmailAddress;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (
                PersistedDbObject.MediaId == NewBusinessObject.MediaId 
                && PersistedDbObject.CardMessageId == NewBusinessObject.CardMessageId 
                && AsBool(PersistedDbObject.IsSender) == NewBusinessObject.IsSender
                && PersistedDbObject.ViewCount == NewBusinessObject.ViewCount
                && DateTime.Equals(PersistedDbObject.LastViewedDateTime, NewBusinessObject.LastViewedDateTime)
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive
                );
        }

        public override bool VersionMatches()
        {
            return true;
            //return PersistedDbObject.RowVersion.SequenceEqual(NewBusinessObject.RowVersion);
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<CardInstanceObject> Get(ECardObjectContext eCardDb, bool forUpdate)
        {
            //PersistentUrl u = (from url in Media.PersistentUrls select url).ToList()[0];
            IQueryable<CardInstanceObject> items = from s in eCardDb.ECardDbEntities.CardInstances
                                                  //let orgids = s.UserOrganizations.Select(a => a.OrganizationID)
                                                  //let uo = eCardDb.RegistrationDbEntities.Organizations.Where(a => orgids.Contains(a.OrganizationId))
                                                  select new CardInstanceObject()
                                                  {
                                                      CardInstanceId = s.CardInstanceId,
                                                      MediaId = s.MediaId,
                                                      CardMessageId = s.CardMessageId,
                                                      IsSender = s.IsSender == "Yes",
                                                      ViewCount = s.ViewCount,
                                                      LastViewedDateTime = s.LastViewedDateTime,
                                                      IsActive = s.Active == "Yes",
                                                      DbObject = forUpdate ? s : null
                                                  };
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Gov.Hhs.Cdc.CdcECardProvider.Model.CardInstance();
            PersistedDbObject.CardInstanceId = NewBusinessObject.CardInstanceId;

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {

            PersistedDbObject.MediaId = NewBusinessObject.MediaId;
            PersistedDbObject.CardMessageId = NewBusinessObject.CardMessageId;
            PersistedDbObject.IsSender = NewBusinessObject.IsSender ? "Yes" : "No";
            PersistedDbObject.ViewCount = NewBusinessObject.ViewCount;
            PersistedDbObject.LastViewedDateTime = NewBusinessObject.LastViewedDateTime;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.ECardDbEntities.CardInstances.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.ECardDbEntities.CardInstances.DeleteObject(PersistedDbObject);
        }

    }
}
