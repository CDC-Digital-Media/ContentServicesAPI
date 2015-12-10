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
using System.Text.RegularExpressions;

namespace Gov.Hhs.Cdc.MediaProvider
{
    [Serializable]
    public class HtmlExtractionCriteria : ExtractionCriteria
    {
        public ExtractionPath IncludedElements { get; set; }
        public ExtractionPath ExcludedElements { get; set; }

        public bool? StripAnchor { get; set; }    // = false;
        public bool? StripComment { get; set; }    // = true;
        public bool? StripImage { get; set; }    // = false;
        public bool? StripScript { get; set; }    // = true
        public bool? StripStyle { get; set; }    // = true;
        public bool? NewWindow { get; set; }    // = true;
        public bool? Iframe { get; set; }    // = true;
        public string ImageAlign { get; set; }
        public string OutputEncoding { get; set; }
        public string OutputFormat { get; set; }
        public string ContentNamespace { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }

        public override ExtractionCriteria Merge(ExtractionCriteria subordinateCriteria)
        {
            HtmlExtractionCriteria subordinateHtmlCriteria = (HtmlExtractionCriteria)subordinateCriteria;
            HtmlExtractionCriteria mergedCriteria = new HtmlExtractionCriteria();

            mergedCriteria.IncludedElements = Merge(IncludedElements, subordinateHtmlCriteria.IncludedElements);
            mergedCriteria.ExcludedElements = Merge(ExcludedElements, subordinateHtmlCriteria.ExcludedElements);

            mergedCriteria.StripAnchor = Merge(StripAnchor, subordinateHtmlCriteria.StripAnchor);
            mergedCriteria.StripComment = Merge(StripComment, subordinateHtmlCriteria.StripComment);
            mergedCriteria.StripImage = Merge(StripImage, subordinateHtmlCriteria.StripImage);
            mergedCriteria.StripScript = Merge(StripScript, subordinateHtmlCriteria.StripScript);
            mergedCriteria.StripStyle = Merge(StripStyle, subordinateHtmlCriteria.StripStyle);
            mergedCriteria.NewWindow = Merge(NewWindow, subordinateHtmlCriteria.NewWindow);

            mergedCriteria.ImageAlign = Merge(ImageAlign, subordinateHtmlCriteria.ImageAlign);
            mergedCriteria.OutputEncoding = Merge(OutputEncoding, subordinateHtmlCriteria.OutputEncoding);
            mergedCriteria.OutputFormat = Merge(OutputFormat, subordinateHtmlCriteria.OutputFormat);
            return mergedCriteria;
        }
    }
}
