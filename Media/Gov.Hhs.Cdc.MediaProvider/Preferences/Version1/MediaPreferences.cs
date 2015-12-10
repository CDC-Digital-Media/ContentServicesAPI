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
using System.Xml.Serialization;

namespace Gov.Hhs.Cdc.MediaProvider
{

    [Serializable]
    //[XmlInclude(typeof(Plan))]
    //[XmlInclude(typeof(HtmlExtractionCriteria))]
    public class MediaPreferences
    {
        //[XmlIgnore]

        //public bool StripAnchor { get; set; }    // = false;
        //public bool StripComment { get; set; }    // = true;
        //public bool StripImage { get; set; }    // = false;
        //public bool StripScript { get; set; }    // = true;
        //public bool StripStyle { get; set; }    // = true;
        //public string ImageAlign {get; set;}    // = "";

        //public string OutputEncoding { get; set; }    // = "UTF-8";
        //public string OutputFormat { get; set; }    // = "xhtml";
        //public string ContentNamespace { get; set; }    // = "CDC";
        //public bool NewWindow { get; set; }    // = true;


        [System.Xml.Serialization.XmlElementAttribute("HtmlCriterion", typeof(HtmlExtractionCriteria))]
        public List<ExtractionCriteria> ExtractionCriteria { get; set; }

        public MediaPreferences()
        {
        }

        //public MediaPreferences(XElement values)
        //{
        //    string x = values.ToString();
        //    var abc = values.Element("Criteria").Element("TheCriteria")
        //        .Elements(ParameterType)
        //    //    .Single(x => (string)x.Attribute("Code") == Code);
        //}
    }
}
