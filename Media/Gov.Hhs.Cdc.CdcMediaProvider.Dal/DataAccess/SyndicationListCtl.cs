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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{

    public class SyndicationListCtl : BaseCtl<SyndicationListObject, SyndicationList, SyndicationListCtl, MediaObjectContext>
    {

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.SyndicationListGuid = PersistedDbObject.MyListGuid;
        }

        public override string ToString()
        {
            return string.Format("{0}, [1]", PersistedBusinessObject.ToString(),
                PersistedBusinessObject.Medias == null ? "" :
                   string.Join(",", PersistedBusinessObject.Medias.Select(m => m.MediaId.ToString()).ToArray()));

        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (
                PersistedDbObject.MyListStatusCode == NewBusinessObject.SyndicationListStatusCode
                && PersistedDbObject.DomainName == NewBusinessObject.DomainName
                && PersistedDbObject.ListName == NewBusinessObject.ListName
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive);
        }


        public override bool VersionMatches()
        {
            return PersistedDbObject.RowVersion.SequenceEqual(NewBusinessObject.RowVersion);
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<SyndicationListObject> Get(MediaObjectContext mediaDb, bool forUpdate)
        {
            IQueryable<SyndicationListMediaObject> syndicationListMedias = SyndicationListMediaCtl.Get(mediaDb, forUpdate);

            IQueryable<SyndicationListObject> items = from s in mediaDb.MediaDbEntities.SyndicationLists
                                           select new SyndicationListObject()
                                           {
                                               SyndicationListGuid = s.MyListGuid,
                                               DomainName = s.DomainName,
                                               UserGuid = s.UserGuid,
                                               ListName = s.ListName,
                                               SyndicationListStatusCode = s.MyListStatusCode,
                                               IsActive = s.Active == "Yes",
                                               Medias = syndicationListMedias.Where(uo => uo.SyndicationListGuid == s.MyListGuid),
                                               RowVersion = s.RowVersion,
                                               DbObject = forUpdate ? s : null,
                                               CreatedDateTime = s.CreatedDateTime
                                           };
            return items;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new SyndicationList();
            PersistedDbObject.MyListGuid = Guid.NewGuid();
            PersistedDbObject.DomainName = NewBusinessObject.DomainName;
            PersistedDbObject.UserGuid = NewBusinessObject.UserGuid;

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.ListName= NewBusinessObject.ListName;
            PersistedDbObject.MyListStatusCode = NewBusinessObject.SyndicationListStatusCode;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.SyndicationLists.AddObject(PersistedDbObject);

            //We don't have the ID yet, and will have to get it after the commit.
            //NewBusinessObject.Id = PersistedDbObject.MediaId;
            //PersistedBusinessObject.Id = PersistedDbObject.MediaId;
        }

        public void Add(SyndicationListMediaCtl syndicationListMedia)
        {
            syndicationListMedia.PersistedDbObject.MyListGuid = PersistedDbObject.MyListGuid;
            syndicationListMedia.PersistedBusinessObject.SyndicationListGuid = PersistedDbObject.MyListGuid;
            syndicationListMedia.NewBusinessObject.SyndicationListGuid = PersistedDbObject.MyListGuid;
            PersistedDbObject.SyndicationListMedias.Add(syndicationListMedia.PersistedDbObject);
        }

        public override void Delete()
        {
            //Media.RegistrationDbEntities.SyndicationLists.Attach(PersistedDbObject);
            TheObjectContext.MediaDbEntities.SyndicationLists.DeleteObject(PersistedDbObject);
        }
        //public void Delete(SyndicationListObject persistedBusinessObject)
        //{
        //    TheObjectContext.MediaDbEntities.SyndicationLists.DeleteObject((SyndicationList)persistedBusinessObject.DbObject);
        //}

    }
}
