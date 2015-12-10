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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;

namespace Gov.Hhs.Cdc.DataServices.Bo
{   
    public interface ICallParser
    {
        int Version { get; set; }

        QueryParams Query { get; set; }
        Criteria Criteria { get; set; }
        SearchCriteria Criteria2 { get; set; }
        bool CanUseXmlSearch { get; set; }
        IRequestOptions ReqOptions { get; set; }

        bool IsPublicFacing { get; }
        int SecondsToLive { get; }

        List<SortColumn> SortColumns { get; set; }
        IDictionary<string, string> ParamDictionary { get; set; }

        void Parse();
        //void InitParser();
        string QueryString { get; }
        Criterion GetCriterion(ApiParam uriCode, string criteriaCode, string defaultValue = null);
        Criterion GetListCriterion(ApiParam uriCode, string criteriaCode, string defaultValue = null);
        List<string> GetStringListParm(ApiParam uriCode, List<string> defaultValue = null);
        string GetStringParm(ApiParam uriCode, string defaultValue = null);
        //string GetGuidParm(ValidationMessages messages, ApiParm uriCode);

        int? GetIntParm(ValidationMessages messages, ApiParam uriCode, int? defaultValue = null);
        List<int> GetIntListParm(ValidationMessages messages, ApiParam uriCode, List<int> defaultValue = null);
        Criterion GetIntListCriterion(ApiParam uriCode, List<int> defaultValue = null);
        bool? GetBoolParm(ValidationMessages messages, ApiParam uriCode, bool? defaultValue = null);
        FilterCriterionDateRange GetDateRangeParm(ApiParam fromUriCode, ApiParam toUriCode);
        //Criterion GetDateCriterion(ApiParm uriCode, string criteriaCode, CriteriaDateOperator dateOperator, DateTime? defaultValue = null);
        Criterion GetDateRangeCriterion(ApiParam fromUriCode, ApiParam toUriCode, string criteriaCode,
            DateTime? fromDefaultValue = null, DateTime? toDefaultValue = null);
        Criterion GetDateRangeCriterion(ValidationMessages messages, ApiParam singleUriCode, ApiParam rangeUriCode, ApiParam fromUriCode, ApiParam toUriCode, string criteriaCode);
        Criterion GetDateRangeParamCriterion(ApiParam uriCode, string criteriaCode,
            DateTime? fromDefaultValue = null, DateTime? toDefaultValue = null);
        int? ParseInt(string parmName);
        int ReplaceIntParmInCriteria(string parmName, string id);
        void ReplaceIntParmInCriteria(string parmName, int id);
        //void RemoveParmInCriteria(string parmName);

        void UpdateOrAddParmInDictionaryAsInt(string key, int value);
        void UpdateOrAddParmInDictionaryAsString(string key, string value);
        void RemoveParmInDictionaryAsString(string key);

        IDictionary<string, string> RemoveDynamicParamsFromDictionary();
        IDictionary<string, string> RemoveDynamicParamsFromDictionaryForPaging();
    }

}
