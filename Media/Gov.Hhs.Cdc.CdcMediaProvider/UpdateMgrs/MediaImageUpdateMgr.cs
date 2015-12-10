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
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class MediaImageUpdateMgr : BaseUpdateMgr<MediaImage>
    {
        public MediaImageUpdateMgr(MediaImage image)
        {
            Items = new List<MediaImage>();
            if (image != null)
            {
                Items.Add(image);
            }
        }

        public override string ObjectName
        {
            get { return "MediaImage"; }
        }

        private List<MediaImageCtl> InsertedObjects = new List<MediaImageCtl>();


        internal List<MediaImageCtl> Create(IDataServicesObjectContext media)
        {
            var inserted = InsertedObjects.SelectMany(vi => Create(media, vi)).ToList();
            InsertedDataControls.AddRange(inserted);

            if (InsertedDataControls.Count == 0 && Items.Count > 0)
            {
                Save(media, new ValidationMessages());
            }

            return inserted;
        }

        private static List<MediaImageCtl> Create(IDataServicesObjectContext media, MediaImageCtl ctl)
        {
            var inserted = MediaImageCtl.Create(media, ctl.NewBusinessObject);
            return new List<MediaImageCtl>() { inserted };
        }

        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (var item in Items)
            {
                var persisted = MediaImageCtl.Get((MediaObjectContext)objectContext, forUpdate: true).Where(mi => mi.MediaId == item.MediaId).FirstOrDefault();

                if (persisted == null)
                {
                    var ctl = MediaImageCtl.Create(objectContext, item);
                    ctl.AddToDb();
                    InsertedObjects.Add(ctl);
                }
                else
                {
                    MediaImageCtl.Update(objectContext, persisted, item, enforceConcurrency: true);
                }
            }
        }
    }
}
