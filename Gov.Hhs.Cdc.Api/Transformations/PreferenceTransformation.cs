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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class PreferenceTransformation
    {
        public static List<SerialPreferenceSet> CreateSerialMediaPreferences(MediaPreferenceSetCollection preferenceSetCollection)
        {
            if (preferenceSetCollection == null || preferenceSetCollection.PreferencesSets == null)
            {
                return null;
            }
            return preferenceSetCollection.PreferencesSets.Select(s => CreateSerialMediaPreferences(s)).ToList();
        }

        private static SerialPreferenceSet CreateSerialMediaPreferences(MediaPreferenceSet preferenceSet)
        {
            SerialPreferenceSet results = new SerialPreferenceSet
            {
                isDefault = preferenceSet.IsDefault,
                type = preferenceSet.PreferenceType
            };
            results.htmlPreferences = CreateSerialMediaPreference((HtmlPreferences)preferenceSet.MediaPreferences);
            return results;
        }

        public static MediaPreferenceSetCollection CreateMediaPreferences(IEnumerable<SerialPreferenceSet> preferenceSets)
        {
            return preferenceSets == null ? null :
                new MediaPreferenceSetCollection()
                {
                    PreferencesSets = preferenceSets.Select(s => CreateMediaPreferences(s)).ToList()
                };
        }

        public static string ClassNames(IEnumerable<SerialPreferenceSet> preferences)
        {
            if (preferences == null)
            {
                return string.Empty;
            }
            var pref = preferences.FirstOrDefault();
            if (pref == null)
            {
                return string.Empty;
            }
            if (pref.htmlPreferences.includedElements == null)
            {
                return string.Empty;
            }
            if (pref.htmlPreferences.includedElements.classNames == null)
            {
                return string.Empty;
            }
            return string.Join(",", pref.htmlPreferences.includedElements.classNames);
        }

        public static string ElementIds(IEnumerable<SerialPreferenceSet> preferences)
        {
            if (preferences == null)
            {
                return string.Empty;
            }
            var pref = preferences.FirstOrDefault();
            if (pref == null)
            {
                return string.Empty;
            }
            if (pref.htmlPreferences.includedElements == null)
            {
                return string.Empty;
            }
            if (pref.htmlPreferences.includedElements.elementIds == null)
            {
                return string.Empty;
            }
            return string.Join(",", pref.htmlPreferences.includedElements.elementIds);
        }

        public static string XPath(IEnumerable<SerialPreferenceSet> preferences)
        {
            if (preferences == null)
            {
                return string.Empty;
            }
            var pref = preferences.FirstOrDefault();
            if (pref == null)
            {
                return string.Empty;
            }
            if (pref.htmlPreferences.includedElements == null)
            {
                return string.Empty;
            }
            if (pref.htmlPreferences.includedElements.xPath == null)
            {
                return string.Empty;
            }
            return pref.htmlPreferences.includedElements.xPath;
        }

        private static MediaPreferenceSet CreateMediaPreferences(SerialPreferenceSet preferenceSet)
        {
            MediaPreferenceSet results = new MediaPreferenceSet()
            {
                IsDefault = preferenceSet.isDefault,
                PreferenceType = preferenceSet.type
            };
            if (preferenceSet.htmlPreferences != null)
            {
                results.MediaPreferences = CreateMediaPreference(preferenceSet.htmlPreferences);
            }
            return results;
        }

        private static SerialHtmlPreferences CreateSerialMediaPreference(HtmlPreferences preferences)
        {
            return new SerialHtmlPreferences()
            {
                includedElements = CreateSerialExtractionPath(preferences.IncludedElements),
                excludedElements = CreateSerialExtractionPath(preferences.ExcludedElements),
                stripBreak = preferences.StripBreak,
                stripAnchor = preferences.StripAnchor,
                stripComment = preferences.StripComment,
                stripImage = preferences.StripImage,
                stripScript = preferences.StripScript,
                newWindow = preferences.NewWindow,
                iframe = preferences.Iframe,
                imageAlign = preferences.ImageAlign,
                outputEncoding = preferences.OutputEncoding,
                outputFormat = preferences.OutputFormat,
                contentNamespace = preferences.ContentNamespace
            };
        }

        private static SerialExtractionPath CreateSerialExtractionPath(ExtractionPath path)
        {
            return path == null ? null :
                new SerialExtractionPath()
                {
                    xPath = path.XPath,
                    elementIds = path.ElementIds,
                    classNames = path.ClassNames
                };
        }

        private static HtmlPreferences CreateMediaPreference(SerialHtmlPreferences preferences)
        {
            return new HtmlPreferences()
            {
                IncludedElements = CreateExtractionPath(preferences.includedElements),
                ExcludedElements = CreateExtractionPath(preferences.excludedElements),
                StripAnchor = preferences.stripAnchor,
                StripComment = preferences.stripComment,
                StripImage = preferences.stripImage,
                StripScript = preferences.stripScript,
                StripStyle = preferences.stripStyle,
                NewWindow = preferences.newWindow,
                Iframe = preferences.iframe,
                ImageAlign = preferences.imageAlign,
                OutputEncoding = preferences.outputEncoding,
                OutputFormat = preferences.outputFormat,
                ContentNamespace = preferences.contentNamespace
            };
        }

        public static MediaObject GetHtmlExtractionCriteria(IDictionary<string, string> queryParms)
        {
            return new MediaObject
            {
                ExtractionClasses = GetString(queryParms, Param.SYND_CLASS, string.Empty),
                ExtractionElementIds = GetString(queryParms, Param.SYND_ELEMENT, string.Empty),
                ExtractionXpath = GetString(queryParms, Param.SYND_XPATH, string.Empty)
            };
        }

        private static List<string> validImageAlignValues = new List<string>() { "left", "right" };
        public static MediaPreferenceSet GetHtmlExtractionCriteria(IDictionary<string, string> queryParms, int apiVersion)
        {
            HtmlPreferences extractionCriteria = GetExtractionByVersion(queryParms, apiVersion);

            if (extractionCriteria.ImageAlign != null && !validImageAlignValues.Contains(extractionCriteria.ImageAlign, StringComparer.OrdinalIgnoreCase))
            {
                extractionCriteria.ImageAlign = null;
            }

            var includedClasses = GetIncludedItems(extractionCriteria.ClassesAsString);
            var includedElements =  GetIncludedItems(extractionCriteria.ElementsAsString);

            extractionCriteria.IncludedElements = TransformExtractionPath(includedClasses, includedElements, extractionCriteria.XPathAsString);

            extractionCriteria.ExcludedElements = TransformExtractionPath(
                GetExcludedItems(extractionCriteria.ClassesAsString), GetExcludedItems(extractionCriteria.ElementsAsString), null);

            string preferenceType = GetString(queryParms, ApiParam.Type.ToString(), null);
            if (preferenceType == null || string.Equals(preferenceType, "web", StringComparison.OrdinalIgnoreCase))
            {
                preferenceType = "WebPage";
            }

            return new MediaPreferenceSet()
            {
                MediaPreferences = extractionCriteria,
                IsDefault = false,
                PreferenceType = preferenceType
            };
        }

        private static ExtractionPath TransformExtractionPath(List<string> classes, List<string> elements, string xpath)
        {
            if (classes == null && elements == null && string.IsNullOrEmpty(xpath))
            {
                return null;
            }
            return new ExtractionPath(classes, elements, xpath);
        }

        private static HtmlPreferences GetExtractionByVersion(IDictionary<string, string> queryParms, int apiVersion)
        {
            HtmlPreferences extractionCriteria = new HtmlPreferences();

            extractionCriteria.XPathAsString = GetString(queryParms, Param.SYND_XPATH, null);
            extractionCriteria.NewWindow = GetNullableBool(queryParms, Param.NEW_WINDOW, null);
            extractionCriteria.OutputEncoding = GetString(queryParms, Param.OUTPUT_ENCODING, null);
            extractionCriteria.OutputFormat = GetString(queryParms, Param.OUTPUT_FORMAT, null);
            extractionCriteria.ContentNamespace = GetString(queryParms, Param.CONTENT_NAMESPACE, null);

            switch (apiVersion)
            {
                case 1:
                    extractionCriteria.ElementsAsString = GetString(queryParms, Param.SYND_ELEMENT, null);
                    extractionCriteria.ClassesAsString = GetString(queryParms, Param.SYND_CLASS, null);

                    extractionCriteria.StripAnchor = GetNullableBool(queryParms, Param.STRIPANCHOR, null);
                    extractionCriteria.StripComment = GetNullableBool(queryParms, Param.STRIPCOMMENT, null);
                    extractionCriteria.StripImage = GetNullableBool(queryParms, Param.STRIPIMAGE, null);
                    extractionCriteria.StripScript = GetNullableBool(queryParms, Param.STRIPSCRIPT, null);
                    extractionCriteria.StripStyle = GetNullableBool(queryParms, Param.STRIPSTYLE, null);

                    extractionCriteria.ImageAlign = GetString(queryParms, Param.IMAGE_ALIGN, null);
                    break;
                case 2:
                    extractionCriteria.ElementsAsString = GetString(queryParms, Param.SYND_ELEMENT_V2, null);
                    extractionCriteria.ClassesAsString = GetString(queryParms, Param.SYND_CLASS_V2, null);
                    extractionCriteria.StripBreak = GetNullableBool(queryParms, Param.STRIPBREAK_V2, null);
                    extractionCriteria.StripAnchor = GetNullableBool(queryParms, Param.STRIPANCHOR_V2, null);
                    extractionCriteria.StripComment = GetNullableBool(queryParms, Param.STRIPCOMMENT_V2, null);
                    extractionCriteria.StripImage = GetNullableBool(queryParms, Param.STRIPIMAGE_V2, null);
                    extractionCriteria.StripScript = GetNullableBool(queryParms, Param.STRIPSCRIPT_V2, null);
                    extractionCriteria.StripStyle = GetNullableBool(queryParms, Param.STRIPSTYLE_V2, null);
                    extractionCriteria.ImageAlign = GetString(queryParms, Param.IMAGE_ALIGN_V2, null);
                    extractionCriteria.Iframe = GetNullableBool(queryParms, Param.IFRAME_V2, null);
                    break;
            }

            return extractionCriteria;
        }

        private static HtmlPreferences GetExtractionDefaultsByVersion(IDictionary<string, string> queryParms, int apiVersion)
        {
            HtmlPreferences extractionCriteria = new HtmlPreferences();

            extractionCriteria.XPathAsString = GetString(queryParms, Param.SYND_XPATH, null);
            extractionCriteria.NewWindow = GetNullableBool(queryParms, Param.NEW_WINDOW, true);
            extractionCriteria.OutputEncoding = GetString(queryParms, Param.OUTPUT_ENCODING, "utf-8");
            extractionCriteria.OutputFormat = GetString(queryParms, Param.OUTPUT_FORMAT, "xhtml");
            extractionCriteria.ContentNamespace = GetString(queryParms, Param.CONTENT_NAMESPACE, "cdc");

            switch (apiVersion)
            {
                case 1:
                    extractionCriteria.ElementsAsString = GetString(queryParms, Param.SYND_ELEMENT, null);
                    extractionCriteria.ClassesAsString = GetString(queryParms, Param.SYND_CLASS, "syndicate");

                    extractionCriteria.StripAnchor = GetNullableBool(queryParms, Param.STRIPANCHOR, false);
                    extractionCriteria.StripComment = GetNullableBool(queryParms, Param.STRIPCOMMENT, true);
                    extractionCriteria.StripImage = GetNullableBool(queryParms, Param.STRIPIMAGE, false);
                    extractionCriteria.StripScript = GetNullableBool(queryParms, Param.STRIPSCRIPT, true);
                    extractionCriteria.StripStyle = GetNullableBool(queryParms, Param.STRIPSTYLE, true);

                    extractionCriteria.ImageAlign = GetString(queryParms, Param.IMAGE_ALIGN, null);
                    break;
                case 2:
                    extractionCriteria.ElementsAsString = GetString(queryParms, Param.SYND_ELEMENT_V2, null);
                    extractionCriteria.ClassesAsString = GetString(queryParms, Param.SYND_CLASS_V2, "syndicate");
                    extractionCriteria.StripBreak = GetNullableBool(queryParms, Param.STRIPBREAK_V2, false);
                    extractionCriteria.StripAnchor = GetNullableBool(queryParms, Param.STRIPANCHOR_V2, false);
                    extractionCriteria.StripComment = GetNullableBool(queryParms, Param.STRIPCOMMENT_V2, true);
                    extractionCriteria.StripImage = GetNullableBool(queryParms, Param.STRIPIMAGE_V2, false);
                    extractionCriteria.StripScript = GetNullableBool(queryParms, Param.STRIPSCRIPT_V2, true);
                    extractionCriteria.StripStyle = GetNullableBool(queryParms, Param.STRIPSTYLE_V2, true);
                    extractionCriteria.ImageAlign = GetString(queryParms, Param.IMAGE_ALIGN_V2, null);
                    extractionCriteria.Iframe = GetNullableBool(queryParms, Param.IFRAME_V2, true);
                    break;
            }

            return extractionCriteria;
        }

        private static string GetString(IDictionary<string, string> queryParms, string name, string defaultValue)
        {
            return queryParms.ContainsKey(name)
                ? queryParms[name]
                : defaultValue;
        }

        private static bool? GetNullableBool(IDictionary<string, string> queryParms, string name, bool? defaultValue)
        {
            return
                queryParms.ContainsKey(name) ?
                    string.Equals(queryParms[name], "true", StringComparison.OrdinalIgnoreCase)
                    : defaultValue;

        }

        private static List<string> GetIncludedItems(string strFilter)
        {
            if (strFilter == null)
            {
                return null;
            }
            return strFilter.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries).Where(a => !a.StartsWith("!")).ToList();
        }

        private static List<string> GetExcludedItems(string strFilter)
        {
            if (strFilter == null)
            {
                return null;
            }

            return strFilter.Split(new char[] { ',' },
                StringSplitOptions.RemoveEmptyEntries).Where(a => a.StartsWith("!"))
                .Select(b => b.Trim('!')).ToList();
        }

        private static ExtractionPath CreateExtractionPath(SerialExtractionPath path)
        {
            return path == null ? null :
                new ExtractionPath()
                {
                    XPath = path.xPath,
                    ElementIds = path.elementIds,
                    ClassNames = path.classNames
                };
        }


    }
}
