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
    public class FeedObjectUpdateMgr : IMediaTypeSpecificUpdateMgr
    {

        public void Save(IDataServicesObjectContext media, MediaObject mediaObject, MediaCtl mediaCtl)
        {
            FeedDetailObject newFeedDetail = (FeedDetailObject)mediaObject.MediaTypeSpecificDetail;
            FeedDetailObject persistedFeedDetail = (mediaObject.Id == 0) ? null :
                FeedObjectCtl.Get((MediaObjectContext)media, forUpdate: true).Where(m => m.MediaId == mediaObject.Id).FirstOrDefault();
                        
            if (newFeedDetail == null && persistedFeedDetail == null)
                return;            

            if (persistedFeedDetail == null)
            {
                if (newFeedDetail != null)
                {
                    if (newFeedDetail.Copyright != null && newFeedDetail.EditorialManager != null && newFeedDetail.WebMasterEmail != null)
                    {
                        FeedObjectCtl.Create(media, newFeedDetail).AddToMedia(mediaCtl);
                    }
                }
            }
            else
            {
                if (newFeedDetail == null)
                    FeedObjectCtl.Delete(media, persistedFeedDetail);
                else
                    FeedObjectCtl.Update(media, persistedFeedDetail, newFeedDetail, enforceConcurrency: true);
            }
        }

        public void Delete(IDataServicesObjectContext media, MediaObject mediaObject)
        {
            FeedDetailObject persistedFeedDetail = (mediaObject.Id == 0) ? null :
                FeedObjectCtl.Get((MediaObjectContext)media, forUpdate: true).Where(m => m.MediaId == mediaObject.Id).FirstOrDefault();

            if (persistedFeedDetail != null)
            {
                FeedObjectCtl.Delete(media, persistedFeedDetail);
            }
        }

    }
}
    
