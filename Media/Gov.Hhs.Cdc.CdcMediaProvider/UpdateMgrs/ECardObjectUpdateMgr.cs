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

    public class ECardObjectUpdateMgr : IMediaTypeSpecificUpdateMgr //, IUpdateMgr
    {
        public void Save(IDataServicesObjectContext media, MediaObject mediaObject, MediaCtl mediaCtl)
        {
            ECardDetail newECardDetail = (ECardDetail)mediaObject.MediaTypeSpecificDetail;
            ECardDetail persistedECardDetail = (mediaObject.Id == 0) ? null : 
                ECardObjectCtl.Get((MediaObjectContext)media, forUpdate: true).Where(m => m.MediaId == mediaObject.Id).FirstOrDefault();

            //return ECardObjectCtl.Create(media, (ECardDetail)detail);
            if (newECardDetail == null && persistedECardDetail == null)
                return;
            
            if (persistedECardDetail == null)
            {
                if( newECardDetail != null)
                    ECardObjectCtl.Create(media, newECardDetail).AddToMedia(mediaCtl);    
            }
            else
            {
                if( newECardDetail == null)
                    ECardObjectCtl.Delete(media, persistedECardDetail);
                else
                    ECardObjectCtl.Update(media, persistedECardDetail, newECardDetail, enforceConcurrency: true);
            }
        }

        public void Delete(IDataServicesObjectContext media, MediaObject mediaObject)
        {
            ECardDetail persistedECardDetail = (mediaObject.Id == 0) ? null :
                ECardObjectCtl.Get((MediaObjectContext)media, forUpdate: true).Where(m => m.MediaId == mediaObject.Id).FirstOrDefault();

            if (persistedECardDetail != null)
            {
                ECardObjectCtl.Delete(media, persistedECardDetail);
            }
        }

    }
}
