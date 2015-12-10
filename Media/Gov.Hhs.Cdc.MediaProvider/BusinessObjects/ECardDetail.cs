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



namespace Gov.Hhs.Cdc.MediaProvider
{
    public class ECardDetail : DataSourceBusinessObject, IValidationObject, IMediaTypeSpecificDetail
    {
        public MediaObject Media { get; set; }
        public int MediaId { get; set; }
        public string MobileCardName { get; set; }
        public string Html5Source { get; set; }
        public string Caption { get; set; }
        public string CardText { get; set; }
        public string CardTextOutside { get; set; }
        public string CardTextInside { get; set; }
        public string ImageSourceInsideLarge { get; set; }
        public string ImageSourceInsideSmall { get; set; }
        public string ImageSourceOutsideLarge { get; set; }
        public string ImageSourceOutsideSmall { get; set; }
        public string MobileTargetUrl { get; set; }
        public bool IsMobile { get; set; }
        public string Mobile
        {
            get
            {
                if (IsMobile)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
            set
            {
                IsMobile = (value == "Yes");
            }
        }
        public int DisplayOrdinal { get; set; }
        public bool IsActive { get; set; }
    }
}
