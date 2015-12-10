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

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;


namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class FeedExportObjectUpdateMgr : BaseUpdateMgr<FeedExportObject>
    {

        public FeedExportObjectUpdateMgr(IList<FeedExportObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new FeedExportObjectValidator();
        }

        public override string ObjectName
        {
            get { return "FeedExport"; }
        }

        public void Save(IDataServicesObjectContext media, MediaObject mediaObject, MediaCtl mediaCtl)
        {
            if (mediaObject.ExportSettings != null)
            {
                // list of ids to keep
                var idsOfItemsToKeep = mediaObject.ExportSettings.Where(a => a.Id > 0).Select(a => a.Id).ToList();
                // remove the enclosures from the database
                var itemsToDelete = FeedExportCtl.Get((MediaObjectContext)media, forUpdate: true)
                    .Where(a => a.MediaId == mediaObject.Id && !idsOfItemsToKeep.Contains(a.Id)).ToList();
                foreach (var item in itemsToDelete)
                {
                    FeedExportCtl.Delete(media, item);
                }

                var itemsToSaveOrUpdate = mediaObject.ExportSettings.ToList();
                foreach (var item in itemsToSaveOrUpdate)
                {
                    // set mediaId
                    item.MediaId = mediaObject.Id;

                    FeedExportObject existingObject = (mediaObject.Id == 0) ? null :
                        FeedExportCtl.Get((MediaObjectContext)media, forUpdate: true).Where(m => m.MediaId == mediaObject.Id && item.Id == m.Id).FirstOrDefault();

                    if (item == null && existingObject == null)
                        return;

                    if (existingObject == null)
                    {
                        if (item != null)
                        {
                            if (item.FilePath != null && item.FeedFormatName != null)
                            {
                                FeedExportCtl.Create(media, item).AddToMedia(mediaCtl);
                            }
                        }
                    }
                    else
                    {
                        if (item == null)
                            FeedExportCtl.Delete(media, existingObject);
                        else
                            FeedExportCtl.Update(media, existingObject, item, enforceConcurrency: true);
                    }
                }

            }

        }

    }
}
