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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gov.Hhs.Cdc.ThumbnailProvider
{
    public class ThumbnailObject : DataSourceBusinessObject, IValidationObject
    {
        public int MediaId { get; set; }
        public byte[] Thumbnail { get; set; }

        public string Name { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }
        public bool MediaThumbnailIsNull { get; set; }
        public ThumbnailObject()
        {
            Thumbnail = new byte[0];
        }
    }
}
