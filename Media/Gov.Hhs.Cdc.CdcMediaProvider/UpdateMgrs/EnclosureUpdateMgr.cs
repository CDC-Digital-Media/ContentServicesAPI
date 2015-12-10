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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class EnclosureUpdateMgr : BaseUpdateMgr<EnclosureObject>
    {
        private IEnumerable<EnclosureObject> enumerable;


        public EnclosureUpdateMgr(IList<EnclosureObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new EnclosureObjectValidator();
        }

        public EnclosureUpdateMgr(IEnumerable<EnclosureObject> enumerable)
        {
            // TODO: Complete member initialization
            this.enumerable = enumerable;
        }

        public override string ObjectName
        {
            get { return "Enclosure"; }
        }

        public void Save(IDataServicesObjectContext media, MediaObject mediaObject, MediaCtl mediaCtl)
        {
            if (mediaObject.Enclosures != null)
            {
                // list of ids to keep
                var idsOfItemsToKeep = mediaObject.Enclosures.Where(a => a.Id > 0).Select(a => a.Id).ToList();

                // remove the enclosures from the database
                var enclosuresToDelete = EnclosureObjectCtl.Get((MediaObjectContext)media, forUpdate: true)
                    .Where(a => a.MediaId == mediaObject.Id && !idsOfItemsToKeep.Contains(a.Id)).ToList();
                foreach (var item in enclosuresToDelete)
                {
                    EnclosureObjectCtl.Delete(media, item);
                }

                var newEnclosures = mediaObject.Enclosures.ToList();
                foreach (var newItem in newEnclosures)
                {
                    // set mediaId
                    newItem.MediaId = mediaObject.Id;

                    var persistedEnclosure = (newItem.Id == 0) ? null :
                        EnclosureObjectCtl.Get((MediaObjectContext)media, forUpdate: true)
                        .Where(a => a.Id == newItem.Id && a.MediaId == mediaObject.Id).FirstOrDefault();

                    if (persistedEnclosure == null)
                    {
                        if (newItem != null)
                        {
                            if (/*newItem.MediaId > 0 &&*/ newItem.ResourceUrl != null && newItem.ContentType != null && newItem.Size > 0)
                            {
                                EnclosureObjectCtl.Create(media, newItem).AddToMedia(mediaCtl);
                            }
                        }
                    }
                    else
                    {
                        if (newItem == null)
                            EnclosureObjectCtl.Delete(media, persistedEnclosure);
                        else
                            EnclosureObjectCtl.Update(media, persistedEnclosure, newItem, enforceConcurrency: true);
                    }
                }
            }
        }

        public void Delete(IDataServicesObjectContext media, MediaObject mediaObject)
        {
            if (mediaObject.Enclosures != null)
            {
                var newEnclosures = mediaObject.Enclosures.ToList();
                foreach (var newItem in newEnclosures)
                {
                    var persistedEnclosure = (newItem.Id == 0) ? null :
                        EnclosureObjectCtl.Get((MediaObjectContext)media, forUpdate: true)
                        .Where(a => a.Id == newItem.Id && a.MediaId == mediaObject.Id).FirstOrDefault();

                    if (persistedEnclosure != null)
                    {
                        EnclosureObjectCtl.Delete(media, persistedEnclosure);
                    }
                }
            }
        }

    }
}
