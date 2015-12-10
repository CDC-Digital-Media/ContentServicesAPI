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

using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.DataServices;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class AggregateObjectCtl : BaseCtl<AggregateObject, FeedAggregate, AggregateObjectCtl, MediaObjectContext>
    {
        public AggregateObjectCtl()
        {
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new FeedAggregate();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGUID = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.MediaID = NewBusinessObject.MediaId;
            PersistedDbObject.SearchXML = NewBusinessObject.SearchXML;            

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGUID = modifiedGuid;
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return false;
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.Id = PersistedDbObject.FeedAggregateID;
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.FeedAggregates.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<AggregateObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<AggregateObject> items = from item in media.MediaDbEntities.FeedAggregates
                                                    select new AggregateObject()
                                                    {
                                                        Id = item.FeedAggregateID,
                                                        MediaId = item.MediaID,
                                                        SearchXML = item.SearchXML,
                                                        DbObject = forUpdate ? item : null,
                                                    };
            return items;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name;
        }

        public void AddToMedia(MediaCtl reference)
        {
            reference.PersistedDbObject.FeedAggregates.Add(PersistedDbObject);
        }
    }
}
