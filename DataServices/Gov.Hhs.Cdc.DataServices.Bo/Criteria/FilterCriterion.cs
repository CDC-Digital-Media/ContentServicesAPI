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
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    [Serializable]
    public abstract class FilterCriterion 
    {
        #region dynamicProperties
        private bool isFiltered;
        public bool IsFiltered {
            get
            {
                return Required || isFiltered;
            }
            set
            {
                isFiltered = Required || value;
            }
        }

        public bool HasBeenUsed { get; set; }

        #endregion

        #region configurationProperties
        public abstract string ParameterType { get; }
        public string ApplicationCode { get; set; }
        public string FilterCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DbColumnName { get; set; }
        public string DisplayNote { get; set; }
        public bool Required { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        //public string DisplayGroupName
        //{
        //    get
        //    {
        //        return string.IsNullOrEmpty(GroupName) ? Name : GroupName;
        //    }
        //}
        #endregion
        
        public FilterCriterion()
        {
            HasBeenUsed = false;
        }

        public abstract string ValueToString();

        public virtual void SetValues(string value)
        {
            IsFiltered = true;
        }

        //[DebuggerStepThrough]
        //public void SetValues(XElement values)
        //{
        //    XElement criteriaValues = null;
        //    try
        //    {              
        //        criteriaValues = values.Element("Criteria").Element("TheCriteria")
        //            .Elements(ParameterType)
        //            .Single(x => (string)x.Attribute("Code") == Code);
        //    }
        //    catch
        //    {
        //        //Ignore errors if can't find it.
        //    }
        //    if (criteriaValues == null)
        //    {
        //        IsFiltered = false;
        //    }
        //    else
        //    {
        //        IsFiltered = true;
        //        SetCriteriaValues(criteriaValues);
        //    }
        //}


        protected abstract void SetCriteriaValues(XElement values);

        public virtual void SetCriteriaValues(FilterCriterion criterion)
        {
            if (criterion == null)
            {
                IsFiltered = false;
            }
            else
            {
                IsFiltered = criterion.IsFiltered;
            }
        }

        public virtual void SetCriteriaValues(Criterion criterion)
        {
            if (criterion == null)
            {
                IsFiltered = false;
            }
            else
            {
                IsFiltered = criterion.IsFiltered;
            }
        }

        public override string ToString()
        {
            return Code + ": " + ValueToString();
        }

        public virtual ListItem.KeyType GetKeyType()
        {
            return ListItem.KeyType.StringKey;    //Default
        }

        public virtual List<int> GetIntKeys()
        {
            throw new NotImplementedException();
        }

        public virtual List<string> GetStringKeys()
        {
            throw new NotImplementedException();
        }

        public virtual int GetIntKey()
        {
            throw new NotImplementedException();
        }

        public virtual string GetStringKey()
        {
            throw new NotImplementedException();
        }

        public virtual bool GetBoolValue()
        {
            throw new NotImplementedException();
        }

        public virtual ValidationMessage Validate()
        {
            return null;
        }

        public IQueryable<ListItem> Filter(IQueryable<ListItem> fullList)
        {

            if (GetKeyType() == ListItem.KeyType.IntKey)
            {
                List<int> keys = GetIntKeys();
                return fullList.Where(i => keys.Contains(i.IntKey));
            }
            else
            {
                List<string> keys = GetStringKeys();
                return fullList.Where(i => keys.Contains(i.Value));
            }
            //GetStringKeys cannot be used in LINQ to Entities
        }

    }
}
