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

namespace Gov.Hhs.Cdc.MediaProvider
{
    public class EffectiveHtmlExtractionCriteria
    {
        private HtmlPreferences Criteria { get; set; }
        private bool IsMobile { get; set; }

        public ExtractionPath IncludedElements
        {
            get
            {
                if (Criteria == null) return new ExtractionPath(new List<string>() { "msyndicate", "syndicate" });
                return Criteria.IncludedElements ?? new ExtractionPath(new List<string>() { "msyndicate", "syndicate" });
            }
        }
        public ExtractionPath ExcludedElements
        {
            get
            {
                if (Criteria == null) return new ExtractionPath();
                return Criteria.ExcludedElements;
            }
        }

        public bool StripBreak
        {
            get
            {
                if (Criteria == null) return false;
                return Criteria.StripBreak ?? /*default*/ false;
            }
        }
        public bool StripAnchor
        {
            get
            {
                if (Criteria == null) return false;
                return Criteria.StripAnchor ?? /*default*/ false;
            }
        }
        public bool StripComment
        {
            get
            {
                if (Criteria == null) return true;
                return Criteria.StripComment ?? /*default*/ true;
            }
        }
        public bool StripImage
        {
            get
            {
                if (Criteria == null) return false;
                return Criteria.StripImage ?? /*default*/ false;
            }
        }
        public bool StripScript
        {
            get
            {
                if (Criteria == null) return true;
                return Criteria.StripScript ?? /*default*/
                    true;
            }
        }
        public bool StripStyle
        {
            get
            {
                if (Criteria == null) return true;
                return Criteria.StripStyle ?? /*default*/ true;
            }
        }
        public bool NewWindow
        {
            get
            {
                if (Criteria == null) return true;
                return Criteria.NewWindow ?? /*default*/ true;
            }
        }
        public bool Iframe
        {
            get
            {
                if (Criteria == null) return true;
                return Criteria.Iframe ?? /*default*/ true;
            }
        }
        public string ImageAlign
        {
            get
            {
                if (Criteria == null) return "";
                return Criteria.ImageAlign ?? "";
            }
        }
        public bool ShallAlignImages
        {
            get
            {
                if (Criteria == null) return false;
                return !string.IsNullOrEmpty(Criteria.ImageAlign);
            }
        }
        public string OutputEncoding
        {
            get
            {
                if (Criteria == null) return "UTF-8";
                return GetDefault(Criteria.OutputEncoding, "UTF-8");
            }
        }
        public string OutputFormat
        {
            get
            {
                if (Criteria == null) return "xhtml";
                return GetDefault(Criteria.OutputFormat, "xhtml");
            }
        }
        public string ContentNamespace
        {
            get
            {
                if (Criteria == null) return "cdc";
                return GetDefault(Criteria.ContentNamespace, "cdc");
            }
        }

        private static string GetDefault(string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public EffectiveHtmlExtractionCriteria(HtmlPreferences criteria, bool isMobile)
        {
            Criteria = criteria;
            IsMobile = isMobile;
        }
    }
}
