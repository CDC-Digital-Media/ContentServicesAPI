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
    public class FeedFormatObjectCtl : BaseCtl<FeedFormatObject, FeedFormat, FeedFormatObjectCtl, MediaObjectContext>
    {
        public FeedFormatObjectCtl()
        {
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new FeedFormat();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.FeedFormatName = NewBusinessObject.Name;

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
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
            PersistedBusinessObject.Name = PersistedDbObject.FeedFormatName;
        }

        public override void Delete()
        {
        }

        public override void AddToDb()
        {
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<FeedFormatObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<FeedFormatObject> items = from item in media.MediaDbEntities.FeedFormats
                                                 where item.Active == true
                                                 select new FeedFormatObject()
                                                 {
                                                     Name = item.FeedFormatName,
                                                     DbObject = forUpdate ? item : null,
                                                 };
            return items;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name;
        }
    }
}
