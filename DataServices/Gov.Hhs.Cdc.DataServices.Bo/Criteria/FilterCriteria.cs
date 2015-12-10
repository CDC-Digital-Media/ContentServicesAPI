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
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Gov.Hhs.Cdc.DataServices.Bo
{

    [Serializable]
    public class FilterCriteria
    {
        [XmlIgnore]
        private Dictionary<string, FilterCriterion> _theCriteria;

        //This is not referenced, but is serialized
        public FilterCriteriaSerializableList TheCriteria
        {
            get { return new FilterCriteriaSerializableList(CriteriaDictionary); }
            set { CriteriaDictionary = value.TheCriteria.ToDictionary(c => c.Code, c => c); }
        }

        [XmlIgnore]
        public Dictionary<string, FilterCriterion> CriteriaDictionary
        {
            get { return _theCriteria ?? (_theCriteria = new Dictionary<string, FilterCriterion>(StringComparer.OrdinalIgnoreCase)); }
            set { _theCriteria = value; }
        }

        public FilterCriteria()
        {
        }

        public FilterCriteria(FilterCriteria originalCriteria)
        {
            CriteriaDictionary = originalCriteria.CriteriaDictionary.ToDictionary(c => c.Key, c => c.Value);
        }


        public FilterCriteria(List<FilterCriterion> criteria)
        {
            foreach (FilterCriterion criterion in criteria)
            {
                Set(criterion.Code, criterion);
            }
        }

        public FilterCriterion GetCriterion(string key)
        {
            if (CriteriaDictionary == null)
            {
                return null;
            }
            return CriteriaDictionary[key];
        }

        //public t GetCriterion<t>(string key) where t : FilterCriterion
        //{
        //    if (CriteriaDictionary == null)
        //        return (t)null;
        //    return (t) CriteriaDictionary[key];
        //}

        public bool? GetCriterionBoolValue(string criterionCode)
        {
            FilterCriterion criterion = GetCriterion(criterionCode);
            if (criterion != null)
            {
                return criterion.GetBoolValue();
            }
            else
            {
                return null;
            }
        }

        public string GetStringKey(string criterionCode)
        {
            FilterCriterion criterion = GetCriterion(criterionCode);
            if (criterion != null)
            {
                return criterion.GetStringKey();
            }
            else
            {
                return null;
            }
        }

        public List<string> GetStringKeys(string criterionCode)
        {
            FilterCriterion criterion = GetCriterion(criterionCode);
            if (criterion != null)
            {
                return criterion.GetStringKeys();
            }
            else
            {
                return null;
            }
        }

        public List<int> GetIntKeys(string criterionCode)
        {
            FilterCriterion criterion = GetCriterion(criterionCode);
            if (criterion != null)
            {
                return criterion.GetIntKeys();
            }
            else
            {
                return null;
            }
        }

        public int GetIntKey(string criterionCode)
        {
            FilterCriterion criterion = GetCriterion(criterionCode);
            if (criterion != null)
            {
                return criterion.GetIntKey();
            }
            else
            {
                return 0;
            }
        }


        public FilterCriterion UseCriterion(string criterionCode)
        {
            FilterCriterion criterion = GetCriterion(criterionCode);
            if (criterion == null)
            {
                throw new ApplicationException("Criterion " + criterionCode + " is missing");
            }
            criterion.HasBeenUsed = true;
            return criterion;
        }

        public bool IsFilteredBy(string key)
        {
            if (CriteriaDictionary == null)
            {
                return false;
            }
            FilterCriterion filterCriterion = UseCriterion(key);
            return filterCriterion != null && filterCriterion.IsFiltered;
        }

        public bool IsFilteredByAndChecked(string key)
        {
            if (CriteriaDictionary == null)
            {
                return false;
            }
            FilterCriterion filterCriterion = UseCriterion(key);
            if (filterCriterion != null && filterCriterion.IsFiltered
              && filterCriterion.GetType() == typeof(FilterCriterionBoolean))
            {
                return ((FilterCriterionBoolean)filterCriterion).Value;
            }
            return false;
        }


        //public void SetSelectedValues(XElement updatedCriteria)
        //{
        //    foreach (string key in CriteriaDictionary.Keys)
        //    {
        //        FilterCriterion criterion = CriteriaDictionary[key];
        //        criterion.SetValues(updatedCriteria);
        //    }
        //}


        public void Set(string code, FilterCriterion filterCriterion)
        {
            if( filterCriterion == null)
            {
                //Don't add anything if filterCriterion is null.  Set to not filtered if it already has the criterion specified
                if (CriteriaDictionary.ContainsKey(code))
                {
                    CriteriaDictionary[code].IsFiltered = false;
                }
            }
            else
            {
                //Set the criterion
                filterCriterion.Code = code;
                if (CriteriaDictionary.ContainsKey(code))
                {
                    CriteriaDictionary[code] = filterCriterion;
                }
                else
                {
                    CriteriaDictionary.Add(code, filterCriterion);
                }
            }
        }

        //public void SetIfFiltered(string code, FilterCriterion filterCriterion)
        //{

        //    if (filterCriterion.IsFiltered)
        //    {
        //        filterCriterion.Code = code;
        //        if (CriteriaDictionary.ContainsKey(code))
        //        {
        //            CriteriaDictionary[code] = filterCriterion;
        //        }
        //        else
        //        {
        //            CriteriaDictionary.Add(code, filterCriterion);
        //        }
        //    }
        //    else
        //    {
        //        if (CriteriaDictionary.ContainsKey(code))
        //            CriteriaDictionary.Remove(code);
        //    }
        //}

    }

}
