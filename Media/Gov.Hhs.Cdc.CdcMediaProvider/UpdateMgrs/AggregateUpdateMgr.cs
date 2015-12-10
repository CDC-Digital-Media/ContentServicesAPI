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
    public class AggregateUpdateMgr : BaseUpdateMgr<AggregateObject>
    {
        public AggregateUpdateMgr(IList<AggregateObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new AggregateObjectValidator();
        }

        public override string ObjectName
        {
            get { return "Aggregate"; }
        }

        public void Save(IDataServicesObjectContext media, MediaObject mediaObject, MediaCtl mediaCtl)
        {
            if (mediaObject.Aggregates != null)
            {
                // list of ids to keep
                var idsOfItemsToKeep = mediaObject.Aggregates.Where(a => a.Id > 0).Select(a => a.Id).ToList();

                // remove the items from the database
                var itemsToDelete = AggregateObjectCtl.Get((MediaObjectContext)media, forUpdate: true)
                    .Where(a => a.MediaId == mediaObject.Id && !idsOfItemsToKeep.Contains(a.Id)).ToList();
                foreach (var item in itemsToDelete)
                {
                    AggregateObjectCtl.Delete(media, item);
                }

                var newItems = mediaObject.Aggregates.ToList();
                foreach (var newItem in newItems)
                {
                    // set mediaId
                    newItem.MediaId = mediaObject.Id;

                    var persistedItem = (newItem.Id == 0) ? null :
                        AggregateObjectCtl.Get((MediaObjectContext)media, forUpdate: true)
                        .Where(a => a.Id == newItem.Id && a.MediaId == mediaObject.Id).FirstOrDefault();

                    if (persistedItem == null)
                    {
                        if (newItem != null)
                        {
                            if (newItem.SearchXML != null)
                            {
                                AggregateObjectCtl.Create(media, newItem).AddToMedia(mediaCtl);
                            }
                        }
                    }
                    else
                    {
                        if (newItem == null)
                            AggregateObjectCtl.Delete(media, persistedItem);
                        else
                            AggregateObjectCtl.Update(media, persistedItem, newItem, enforceConcurrency: true);
                    }
                }
            }
        }

        public void Delete(IDataServicesObjectContext media, MediaObject mediaObject)
        {
            if (mediaObject.Aggregates != null)
            {
                var items = mediaObject.Aggregates.ToList();
                foreach (var item in items)
                {
                    var persistedObj = (item.Id == 0) ? null :
                        AggregateObjectCtl.Get((MediaObjectContext)media, forUpdate: true)
                        .Where(a => a.Id == item.Id && a.MediaId == mediaObject.Id).FirstOrDefault();

                    if (persistedObj != null)
                    {
                        AggregateObjectCtl.Delete(media, persistedObj);
                    }
                }
            }
        }


    }
}
