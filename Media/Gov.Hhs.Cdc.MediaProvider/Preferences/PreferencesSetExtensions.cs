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
    public static class PreferencesSetExtensions
    {
        public static MediaPreferenceSet GetSetContainingWeb(this List<MediaPreferenceSet> sets)
        {
            return sets.Where(s => s.PreferenceType != null && s.PreferenceType.Equals(PreferenceTypeEnum.WebPage.ToString(),
                StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        public static List<string> ClassList(this PreferencesSet preferences)
        {
            var list = new List<string>();
            if (preferences == null) return list;
            var coll = preferences.PreferencesPersistedForMediaItem ?? preferences.D_ForMediaType;
            if (coll == null) return list;
            var ps = coll.PreferencesSets.FirstOrDefault();
            if (ps == null) return list;

            foreach (var pref in coll.PreferencesSets)
            {
                list = list.Union(pref.Classes()).ToList();
            }

            return list;
        }

        public static string ClassNames(this PreferencesSet preferences)
        {
            var list = preferences.ClassList();
            return string.Join(",", list);
        }

        public static List<string> Classes(this MediaPreferenceSet ps)
        {
            var list = new List<string>();
            if (ps == null) return list;
            if (ps.MediaPreferences.IncludedElements == null) return list;
            if (ps.MediaPreferences.IncludedElements.ClassNames == null) return list;

            foreach (string classValue in ps.MediaPreferences.IncludedElements.ClassNames)
                list.AddRange(classValue.Split(',').ToList());

            //return ps.MediaPreferences.IncludedElements.ClassNames;
            return list;
        }

        public static List<string> ElementIdList(this PreferencesSet preferences)
        {
            var list = new List<string>();
            if (preferences == null) return list;
            var coll = preferences.PreferencesPersistedForMediaItem ?? preferences.D_ForMediaType;
            if (coll == null) return list;
            var ps = coll.PreferencesSets.FirstOrDefault();
            if (ps == null) return list;

            foreach (var pref in coll.PreferencesSets)
            {
                list = list.Union(pref.ElementIds()).ToList();
            }

            return list;
        }

        public static string ElementIds(this PreferencesSet preferences)
        {
            var list = preferences.ElementIdList();
            return string.Join(",", list);
        }

        public static List<string> ElementIds(this MediaPreferenceSet ps)
        {
            var list = new List<string>();
            if (ps == null) return list;
            if (ps.MediaPreferences.IncludedElements == null) return list;
            if (ps.MediaPreferences.IncludedElements.ElementIds == null) return list;

            foreach (string id in ps.MediaPreferences.IncludedElements.ElementIds)
                list.AddRange(id.Split(',').ToList());

            //return ps.MediaPreferences.IncludedElements.ElementIds;
            return list;
        }

        public static string XPath(this PreferencesSet preferences)
        {
            if (preferences == null) return string.Empty;
            var coll = preferences.PreferencesPersistedForMediaItem ?? preferences.D_ForMediaType;
            if (coll == null) return string.Empty;
            var ps = coll.PreferencesSets.FirstOrDefault();
            if (ps == null) return string.Empty;
            var mp = ps.MediaPreferences as HtmlPreferences;

            if (mp.IncludedElements == null) return string.Empty;
            if (mp.IncludedElements.XPath == null) return string.Empty;
            return mp.IncludedElements.XPath;
        }
    }
}
