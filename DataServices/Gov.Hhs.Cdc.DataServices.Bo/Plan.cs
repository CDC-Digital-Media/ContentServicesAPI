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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{

    [Serializable]
    [XmlInclude(typeof(Plan))]
    public class Plan : DataSourceBusinessObject, IValidationObject
    {
        [XmlIgnore]
        public FilterCriteria Criteria { get; set; }

        //This is not referenced, but is serialized
        [XmlElement("Criteria")]
        public FilterCriteriaSerializableList TheCriteria
        {
            get { return new FilterCriteriaSerializableList(Criteria.CriteriaDictionary); }
            set { Criteria = new FilterCriteria(value.TheCriteria); }
        }

        [XmlIgnore]
        public long Id { get; set; }

        public string ApplicationCode { get; set; }
        public string DataSetCode { get; set; }
        public string FilterCode { get; set; }
        public string ReportCode { get; set; }
        public string ControlCode { get; set; }
        public string EnvironmentCode { get; set; }

        public List<string> CriteriaKeys { get; set; }

        public bool IsLinkedPlan { get; set; } 
        public long ParentPlanId {get; set;}
        public string LinkCode { get; set; }

        public string ActionCode { get; set; }

        [XmlIgnore]
        public string PersistedCriteriaXml { get; set; }

        [XmlIgnore]
        public bool _returnAsList = true;
        [XmlIgnore]
        public bool ReturnAsList
        {
            get { return _returnAsList; }
            set { _returnAsList = value; }
        }


        public Plan()
        {
            IsLinkedPlan = false;
        }

        public Plan(SearchParameters searchParameters)
        {
            IsLinkedPlan = true;
            ApplicationCode = searchParameters.ApplicationCode;
            EnvironmentCode = searchParameters.EnvironmentCode;
            DataSetCode = string.IsNullOrEmpty(searchParameters.DataSetCode) ? searchParameters.FilterCode : searchParameters.DataSetCode;
            FilterCode = string.IsNullOrEmpty(searchParameters.FilterCode) ? searchParameters.DataSetCode : searchParameters.FilterCode;
            ControlCode = searchParameters.ControlCode;
            ActionCode = searchParameters.ActionCode;
            ReportCode = null;
            LinkCode = null;
            ParentPlanId = 0;
            CriteriaKeys = null;
        }




        /// <summary>
        /// Create a new plan for a linked report
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <param name="ReportId"></param>
        /// <param name="LinkCode"></param>
        /// <param name="keys"></param>
        public Plan(string applicationCode, string environmentCode, string filterCode, string controlCode, string reportCode, string linkCode, long parentPlanId, List<string> keys)
        {
            IsLinkedPlan = true;
            ApplicationCode = applicationCode;
            EnvironmentCode = environmentCode;
            FilterCode = filterCode;
            ControlCode = controlCode;
            ReportCode = reportCode;
            LinkCode = linkCode;
            ParentPlanId = parentPlanId;
            CriteriaKeys = keys;
        }



        public XElement GetCriteriaXml()
        {
            return XDocument.Parse(PersistedCriteriaXml).Root;
        }


    }
}
