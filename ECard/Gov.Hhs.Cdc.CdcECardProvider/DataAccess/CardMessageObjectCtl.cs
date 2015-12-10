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
    public class CardMessageObjectCtl : BaseCtl<CardMessageObject, Gov.Hhs.Cdc.CdcECardProvider.Model.CardMessage,
        CardMessageObjectCtl, ECardObjectContext>
    {

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.CardMessageId = PersistedDbObject.CardMessageId;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.SenderEmail;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (
                string.Equals(PersistedDbObject.PersonalMessage, NewBusinessObject.PersonalMessage)
                && string.Equals(PersistedDbObject.StyleSheet, NewBusinessObject.StyleSheet)
                && string.Equals(PersistedDbObject.SenderUserAgent, NewBusinessObject.SenderUserAgent)
                && string.Equals(AsBool(PersistedDbObject.FromMobile), NewBusinessObject.IsFromMobile)
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

        public static IQueryable<CardMessageObject> Get(ECardObjectContext eCardDb, bool forUpdate)
        {
            //PersistentUrl u = (from url in Media.PersistentUrls select url).ToList()[0];
            IQueryable<CardMessageObject> items = from s in eCardDb.ECardDbEntities.CardMessages
                                           //let orgids = s.UserOrganizations.Select(a => a.OrganizationID)
                                           //let uo = eCardDb.RegistrationDbEntities.Organizations.Where(a => orgids.Contains(a.OrganizationId))
                                           select new CardMessageObject()
                                           {
                                               CardMessageId = s.CardMessageId,
                                               PersonalMessage = s.PersonalMessage,
                                               StyleSheet = s.StyleSheet,
                                               SenderUserAgent = s.SenderUserAgent,
                                               IsFromMobile = s.FromMobile == "Yes",
                                               DbObject = forUpdate ? s : null
                                           };
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Gov.Hhs.Cdc.CdcECardProvider.Model.CardMessage();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {

            PersistedDbObject.PersonalMessage = NewBusinessObject.PersonalMessage;
            PersistedDbObject.StyleSheet = NewBusinessObject.StyleSheet;
            PersistedDbObject.SenderUserAgent = NewBusinessObject.SenderUserAgent;
            PersistedDbObject.FromMobile = NewBusinessObject.IsFromMobile ? "Yes" : "No";
          
            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.ECardDbEntities.CardMessages.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.ECardDbEntities.CardMessages.DeleteObject(PersistedDbObject);
        }

    }
}
