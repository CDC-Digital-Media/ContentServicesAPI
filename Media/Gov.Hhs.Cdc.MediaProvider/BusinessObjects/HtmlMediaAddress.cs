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
using System.Text.RegularExpressions;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.MediaProvider
{
    public class HtmlMediaAddress : MediaAddress
    {
        public string Url { get; set; }

        public Uri Uri { get { return new Uri(Url); } }

        public bool IsMobile { get; set; }
        // Valid rendering types are: html32, chtml10, html32, wml11, wml12, xhtml-basic
        public string PreferredRenderingType { get; set; }

        public override string GetUniqueKey() { 
            return Url; 
        }

        public HtmlMediaAddress()
        {
        }

        public HtmlMediaAddress(int mediaId, string url, string sourceCode, string domainName)
        {
            Url = url;
            MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType;
            IsArchived = false;
            IsMobile = false;
            PreferredRenderingType = "Html32";
            SourceId = mediaId;
            SourceTable = "Media";
            SourceCode = sourceCode;
            DomainName = domainName;
        }

        public HtmlMediaAddress(int mediaId, string url, string sourceCode)
        {
            Url = url;
            MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType;
            IsArchived = false;
            IsMobile = false;
            PreferredRenderingType = "Html32";
            SourceId = mediaId;
            SourceTable = "Media";
            SourceCode = sourceCode;
        }


        public EffectiveHtmlExtractionCriteria GetExtractionCriteriaWithDefaults()
        {
            HtmlPreferences mediaPref = null;
            if (MediaObject.Preferences != null && MediaObject.Preferences.Effective != null)
            {
                var pref = MediaObject.Preferences.Effective.PreferencesSets.GetSetContainingWeb();
                if (pref != null)
                {
                    mediaPref = pref.MediaPreferences;
                    mediaPref.IncludedElements = new ExtractionPath(MediaObject);
                }
            }
 
            if (mediaPref == null)
            {
                mediaPref = new HtmlPreferences { IncludedElements = new ExtractionPath(MediaObject) };
            }

            return new EffectiveHtmlExtractionCriteria(mediaPref, IsMobile);
        }
    }
}
