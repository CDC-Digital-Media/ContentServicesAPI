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
using System.Xml.Linq;
using Gov.Hhs.Cdc.MediaValidatonProvider;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{

    public class MediaValidatorConfig 
    {
        public string XMLCatalogPath { get; set; }

        public MediaValidatorConfig()
        {
            XMLCatalogPath = System.Configuration.ConfigurationManager.AppSettings["XMLCatalogPath"];
        }

        public MediaValidatorConfig(XElement appSettings)
        {
            if (appSettings != null)
            {
                if (appSettings.Element("XMLCatalogPath") != null)
                {
                    XMLCatalogPath = appSettings.Element("XMLCatalogPath").Value;
                }
            }
        }
    }
}
