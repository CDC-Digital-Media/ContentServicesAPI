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

using System.IO;
using System.Xml;
using System.Xml.Linq;

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using System.ServiceModel.Syndication;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.CdcMediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class FeedManaged : FeedBase
    {
        public FeedManaged(List<MediaObject> mediaList, ICallParser parser)
            : base(mediaList, parser)
        {
        }

        public override string Generate()
        {
            GetManagedFeed();
            return this.strReturn;
        }      

    }
}
