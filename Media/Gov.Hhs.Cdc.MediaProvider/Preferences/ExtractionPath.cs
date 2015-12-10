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
    public class ExtractionPath
    {
        public List<string> ClassNames { get; set; }
        public List<string> ElementIds { get; set; }
        public string XPath { get; set; }

        public bool HasExtractionCriteria
        {
            get
            {
                return (!string.IsNullOrEmpty(XPath) ||
                    (ElementIds != null && ElementIds.Count > 0) ||
                    (ClassNames != null && ClassNames.Count > 0));
            }
        }

        public ExtractionPath()
        {
        }

        public ExtractionPath(List<string> classes)
        {
            ClassNames = classes != null && classes.Count() > 0 ? classes : null;
        }

        public ExtractionPath(List<string> classes, List<string> elementIds, string xPath)
        {
            Initialize(classes, elementIds, xPath);
        }

        public ExtractionPath(MediaObject media)
        {
            var pref = media.Preferences;
            if (pref != null)
            {
                Initialize(pref.ClassList(), pref.ElementIdList(), pref.XPath());
            }
            else
            {
                XPath = (!string.IsNullOrEmpty(media.ExtractionXpath)) ? media.ExtractionXpath : null;

                if (!string.IsNullOrEmpty(media.ExtractionClasses))
                {
                    var classes = media.ExtractionClasses.Split(',').ToList();
                    ClassNames = classes != null && classes.Count() > 0 ? classes : null;
                }
                if (!string.IsNullOrEmpty(media.ExtractionElementIds))
                {
                    var elementIds = media.ExtractionElementIds.Split(',').ToList();
                    ElementIds = elementIds != null && elementIds.Count() > 0 ? elementIds : null;
                }

            }
        }

        private void Initialize(List<string> classes, List<string> elementIds, string xPath)
        {
            XPath = (!string.IsNullOrEmpty(xPath)) ? xPath : null;
            ElementIds = elementIds != null && elementIds.Count() > 0 ? elementIds : null;
            ClassNames = classes != null && classes.Count() > 0 ? classes : null;
        }

        public bool UseXPath { get { return !string.IsNullOrEmpty(XPath); } }
        public bool UseElementIds { get { return (!UseXPath) && ElementIds != null && ElementIds.Count > 0; } }
        public bool UseClassNames { get { return (!UseXPath) && (!UseElementIds) && ClassNames != null && ClassNames.Count > 0; } }
        //public bool IsInUse { get { return UseXPath || UseElementIds || UseClassNames; } }

        //public override bool Equals(object obj)
        //{
        //    if (obj.GetType() != typeof(ExtractionPath))
        //    {
        //        return false;
        //    }

        //    var path = (ExtractionPath)obj;

        //    return
        //        SafeEquals(XPath, path.XPath)
        //        && SafeEquals(ElementIds, path.ElementIds)
        //        && SafeEquals(ClassNames, path.ClassNames);
        //}

        public static bool SafeEquals(List<string> list1, List<string> list2)
        {
            if (list1 == null)
                return list2 == null || list2.Count == 0;
            if (list2 == null)
                return list1 == null || list1.Count == 0;
            return list1.SequenceEqual(list2, StringComparer.OrdinalIgnoreCase);
        }

        public static bool SafeEquals(string string1, string string2)
        {
            if (string1 == null)
                return string.IsNullOrEmpty(string2);
            if (string2 == null)
                return string.IsNullOrEmpty(string1);
            return string1.Equals(string2, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            if (UseXPath)
                return "XPath: " + XPath;
            else if (UseElementIds)
                return ToString("Element", ElementIds);
            else if (UseClassNames)
                return ToString("Class Name", ClassNames);
            return "";
        }
        public static string ToString(string name, List<string> values)
        {
            return name + (values.Count > 1 ? "s" : "") +
                (string.Join(",", values.Select(v => "'" + v + "'").ToArray()));
        }
    }
}
