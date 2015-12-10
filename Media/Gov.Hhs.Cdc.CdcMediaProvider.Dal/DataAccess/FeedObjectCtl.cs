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
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class FeedObjectCtl : BaseCtl<FeedDetailObject, Feed, FeedObjectCtl, MediaObjectContext>
    {
        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            //DB Objects are not inherited, so treat them separately
            PersistedDbObject = new Feed();

            PersistedDbObject.Generator = modifiedGuid.ToString();

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGUID = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.Copyright = NewBusinessObject.Copyright;
            PersistedDbObject.ManagingEditor = NewBusinessObject.EditorialManager;
            PersistedDbObject.WebMaster = NewBusinessObject.WebMasterEmail;

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
            PersistedBusinessObject.MediaId = PersistedDbObject.MediaID;
            //This is handled by the CombinedMediaItem
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.Feeds.DeleteObject(PersistedDbObject);
        }

        public override void AddToDb()
        {
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<FeedDetailObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<FeedDetailObject> feed = from c in media.MediaDbEntities.Feeds
                                          select new FeedDetailObject()
                                          {
                                              MediaId = c.MediaID,
                                              EditorialManager = c.ManagingEditor,
                                              WebMasterEmail = c.WebMaster,
                                              Copyright = c.Copyright,

                                              DbObject = forUpdate ? c : null,
                                          };
            return feed;
        }

        public static IQueryable<MediaImage> GetFeedImage(MediaObjectContext media)
        {
            var image = media.MediaDbEntities.Images
                .Select(i => new MediaImage {
                    MediaId = i.MediaID,
                    Title = i.Title,
                    Description = i.Description,
                    Height = i.Height,
                    Width = i.Width,
                    SourceUrl = i.SourceURL,
                    TargetUrl = i.LinkURL,
                    CreatedBy = i.CreatedByGUID,
                    ModifiedBy = i.ModifiedByGUID,
                    CreatedDateTime = i.CreatedDateTime,
                    ModifiedDateTime = i.ModifiedDateTime
                });
                return image;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name;
        }

        public void AddToMedia(MediaCtl reference)
        {
            reference.PersistedDbObject.Feed = PersistedDbObject;
        }
    }
}
