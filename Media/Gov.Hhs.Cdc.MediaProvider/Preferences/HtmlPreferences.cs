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
using System.Xml.Serialization;

namespace Gov.Hhs.Cdc.MediaProvider
{
    [Serializable]
    public class HtmlPreferences //: IMediaPreferences
    {
        public ExtractionPath IncludedElements { get; set; }
        public ExtractionPath ExcludedElements { get; set; }

        public bool? StripBreak { get; set; }    // = false;
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
        public string PostProcess { get; set; }

        [XmlIgnore]
        public string XPathAsString { get; set; }

        [XmlIgnore]
        public string ElementsAsString { get; set; }

        [XmlIgnore]
        public string ClassesAsString { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }

        public bool HasExtractionCriteria
        {
            get
            {
                return XPathAsString != null || ElementsAsString != null || ClassesAsString != null ||
                    (IncludedElements != null && IncludedElements.HasExtractionCriteria) ||
                    (ExcludedElements != null && ExcludedElements.HasExtractionCriteria);
            }
        }

        public bool IsCustom
        {
            get
            {
                return HasExtractionCriteria || StripAnchor != null || StripBreak != null || StripComment != null
                    || StripImage != null || StripScript != null || StripStyle != null || NewWindow != null
                    || Iframe != null || ImageAlign != null || OutputEncoding != null || OutputFormat != null
                    || ContentNamespace != null;
            }

        }

        public static HtmlPreferences GetDefaultPreferences()
        {
            return new HtmlPreferences()
            {
                IncludedElements = new ExtractionPath(new List<string> { "syndicate" }, null, null),
                ExcludedElements = null,

                StripBreak = false,
                StripAnchor = false,
                StripComment = true,
                StripImage = false,
                StripScript = true,
                StripStyle = true,

                NewWindow = true,
                Iframe = true,
                ImageAlign = "",
                OutputEncoding = "utf-8",
                OutputFormat = "xhtml",
                ContentNamespace = "cdc"
            };
        }

        public HtmlPreferences Merge(HtmlPreferences subordinatePreferences)
        {
            if (subordinatePreferences == null)
                return this;
            HtmlPreferences subordinateHtmlPreferences = subordinatePreferences;
            HtmlPreferences mergedCriteria = new HtmlPreferences()
            {
                IncludedElements = subordinateHtmlPreferences.IncludedElements ?? IncludedElements,
                ExcludedElements = subordinateHtmlPreferences.ExcludedElements ?? ExcludedElements,

                StripBreak = subordinateHtmlPreferences.StripBreak ?? StripBreak,
                StripAnchor = subordinateHtmlPreferences.StripAnchor ?? StripAnchor,
                StripComment = subordinateHtmlPreferences.StripComment ?? StripComment,
                StripImage = subordinateHtmlPreferences.StripImage ?? StripImage,
                StripScript = subordinateHtmlPreferences.StripScript ?? StripScript,
                StripStyle =  subordinateHtmlPreferences.StripStyle ?? StripStyle,

                NewWindow =  subordinateHtmlPreferences.NewWindow ?? NewWindow,
                Iframe = subordinateHtmlPreferences.Iframe ?? Iframe,
                ImageAlign = subordinateHtmlPreferences.ImageAlign ?? ImageAlign,
                OutputEncoding = subordinateHtmlPreferences.OutputEncoding ?? OutputEncoding,
                OutputFormat =  subordinateHtmlPreferences.OutputFormat ?? OutputFormat,
                ContentNamespace =  subordinateHtmlPreferences.ContentNamespace ?? ContentNamespace
            };
            return mergedCriteria;
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj.GetType() != typeof(HtmlPreferences))
        //        throw new ApplicationException("Invalid type for HtmlPreferences.Matches of " + obj.GetType());
        //    var preferences = (HtmlPreferences)obj;
        //    return SafeEquals(IncludedElements, preferences.IncludedElements)
        //        && SafeEquals(ExcludedElements, preferences.ExcludedElements);
        //}
        //private static bool SafeEquals(ExtractionPath extractionPath1, ExtractionPath extractionPath2)
        //{
        //    var path1 = extractionPath1 ?? new ExtractionPath();
        //    var path2 = extractionPath2 ?? new ExtractionPath();
        //    return path1.Equals(path2);
        //}

        //public HtmlPreferences MergeOnlyExtractionCriteria(HtmlPreferences subordinatePreferences)
        //{
        //    HtmlPreferences otherPreferences = (HtmlPreferences)subordinatePreferences;
        //    IncludedElements = otherPreferences.IncludedElements;
        //    ExcludedElements = otherPreferences.ExcludedElements;
        //    return this;
        //}
    }
}
