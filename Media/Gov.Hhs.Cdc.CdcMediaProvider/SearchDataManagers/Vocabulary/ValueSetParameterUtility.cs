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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class ValueSetParameterUtility
    {
        private MediaObjectContext Media { get; set; }

        private List<int> _valueSetIds = null;
        public List<int> ValueSetIds
        {
            get
            {
                return _valueSetIds ?? (_valueSetIds = GetValueSetIds());
            }
        }

        public List<int> ChildValueSetIds
        {
            get
            {
                return ValueSetIds.Union(InheritedValueSetIds).ToList();
            }
        }

        private List<int> InheritedValueSetIds = new List<int>();
        public bool FilteredByValueSet
        {
            get
            {
                return ValueSetIdCriterion.IsFiltered || ValueSetNameCriterion.IsFiltered;
            }
        }

        private FilterCriterion ValueSetIdCriterion;
        private FilterCriterion ValueSetNameCriterion;
        private FilterCriterionMultiSelect LanguageCriterion;

        public ValueSetParameterUtility(MediaObjectContext media, FilterCriteria criteria, FilterCriterionMultiSelect languageCriterion)
        {
            Media = media;
            ValueSetIdCriterion = criteria.UseCriterion("ValueSet");
            ValueSetNameCriterion = criteria.UseCriterion("ValueSetName");
            LanguageCriterion = languageCriterion;
        }

        private List<int> GetValueSetIds()
        {
            if (FilteredByValueSet)
            {

                var ids = ValueSetIdCriterion.IsFiltered ?
                    ValueSetIdCriterion.GetIntKeys() :
                    ValueSetNameCriterion.GetStringKeys().SelectMany(k => GetValueSetIdForEachLanguage(k));
                return ids.ToList();
            }
            else
            {
                return new List<int>();
            }
        }


        private List<int> GetValueSetIdForEachLanguage(string key)
        {
            if (key.Contains("|"))
                return new List<int>() { GetValueSetId(key) };
            //Join languages if we haven't already
            if (LanguageCriterion == null)
            {
                throw new ApplicationException("HierVocabularyKey(" + key + ") is not of the format 'Name|LanguageCode' and language code was not set");
            }
            return LanguageCriterion.GetStringKeys().Select(l => GetValueSetId(key + "|" + l)).ToList();

        }

        private int GetValueSetId(string key)
        {
            string[] keys = key.Split('|');
            if (keys.Count() != 2)
            {
                throw new ApplicationException("HierVocabularyKey(" + key + ") is not of the format 'Name|LanguageCode'");
            }

            if (string.Equals(keys[0].ToLower(), "Topics", StringComparison.OrdinalIgnoreCase))
            {
                ValueSetObject topicsValueSet = MediaCacheController.GetValueSetItem(Media, "Topics", keys[1]);
                if (topicsValueSet != null)
                {
                    InheritedValueSetIds.Add(topicsValueSet.Id);
                }

                return topicsValueSet != null ? topicsValueSet.Id : 0;
            }
            else
            {
                ValueSetObject valueSetItem = MediaCacheController.GetValueSetItem(Media, keys[0], keys[1]);
                return valueSetItem != null ? valueSetItem.Id : 0;
            }
        }


    }
}
