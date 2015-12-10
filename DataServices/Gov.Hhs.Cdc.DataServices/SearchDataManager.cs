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
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.DataServices
{
    public abstract class SearchDataManager<t> : ISearchDataManager
    {
        public abstract string ApplicationCode { get; }

        public virtual string FilterCode { get { return typeof(t).Name.TrimEnd("Object").TrimEnd("Item"); } }

        public abstract List<ListItem> GetListItems(FilterCriterion parameter, bool getOnlySelectedItems);
        public abstract List<HierarchicalItem> GetHierarchicalItems(FilterCriterion parameter, bool getRoot,
            FilterCriterion currentlySelectedItems, string parentKey, FilterCriterion filter);


        //Static variables are unique by generic type.  This is static so that we only have to pull the criteria for a class once
        public static CriteriaDefinitionsForFilter _criteriaForFilter;
        /// <summary>
        /// Defines all the filter criteria that can be used for a media item.
        /// </summary>
        public virtual CriteriaDefinitionsForFilter CriteriaDefinitionsForFilter
        {
            get
            {
                return _criteriaForFilter ??
                    (_criteriaForFilter = SearchAttributeResolver.GetFilterCriteriaDefinitionsForABusinessObject(typeof(t)));
            }
        }

        protected IObjectContextFactory ObjectContextFactory { get; set; }


        public SearchDataManager(IObjectContextFactory objectContextFactory)
        {
            ObjectContextFactory = objectContextFactory;
        }

        public virtual DataSetResult GetData(SearchParameters searchParameters)
        {
            return GetData(searchParameters, null);
        }

        public virtual DataSetResult GetData(SearchParameters searchParameters, ResultSetParms resultSetParms)
        {
            var plan = SearchDataManagerHelper.GetSearchPlan(searchParameters, ApplicationCode, FilterCode, CriteriaDefinitionsForFilter);
            return GetData(plan, resultSetParms ?? new ResultSetParms());
        }

        public virtual DataSetResult GetData(SearchParameters searchParameters, ResultSetParms resultSetParms, IList<int> allowableMediaIds)
        {
            var plan = SearchDataManagerHelper.GetSearchPlan(searchParameters, ApplicationCode, FilterCode, CriteriaDefinitionsForFilter);
            return GetData(plan, resultSetParms ?? new ResultSetParms(), allowableMediaIds);
        }

        public virtual DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms)
        {
            throw new NotImplementedException();
        }

        public virtual DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms, IList<int> allowableMediaIds)
        {
            throw new NotImplementedException();
        }

        public static DataSetResult CreateDataSetResults(IQueryable<t> query, SearchPlan searchPlan, Guid resultSetId, bool logTime = false)
        {

            ReflectionQualifiedNames qualifiedNames = ReflectionQualifiedNames.Get(typeof(t));

            foreach (string key in searchPlan.Plan.Criteria.CriteriaDictionary.Keys)
            {
                FilterCriterion criterion = searchPlan.Plan.Criteria.CriteriaDictionary[key];
                if ((!criterion.HasBeenUsed) && criterion.IsFiltered)
                {
                    query = query.Where(criterion, qualifiedNames);
                }
            }
            
            query = query.OrderBy(searchPlan.Sorting, qualifiedNames);
            bool resultsArePaged = searchPlan.Paging != null && searchPlan.Paging.IsPaged && !searchPlan.Paging.ReturnAllPages;

            int totalRecordCount = query.Count();

            if (resultsArePaged)
            {
                query = query.PageBy(searchPlan.Paging);
            }

            //For now, return everything as a list
            //string sql3 = string.Empty;
            //var converted = query as ObjectQuery;
            //if (converted != null)
            //{
            //    sql3 = converted.ToTraceString();
            //}
            //else
            //{
            //    sql3 = query.ToString();
            //}
            //AuditLogger.LogAuditEvent(sql3);

            //if (logTime)
            //{
            //    var start2 = DateTime.Now;
            //    var executionTime = DateTime.Now.Subtract(start2);
            //    AuditLogger.LogAuditEvent(string.Format("To Trace String took {0} milliseconds", executionTime.TotalMilliseconds), sql3);
            //}

            var start = DateTime.Now;
            IList theRecords = query.ToList();
            if (logTime)
            {
                var executionTime = DateTime.Now.Subtract(start);
                AuditLogger.LogAuditEvent(string.Format("Query took {0} milliseconds", executionTime.TotalMilliseconds), query.ToString());
            }

            return new DataSetResult(
                resultSetId: resultSetId,
                records: theRecords,
                resultsAreSorted: searchPlan.Sorting.IsSorted,
                resultsArePaged: resultsArePaged,
                listIsComplete: true,
                totalRecordCount: resultsArePaged ? totalRecordCount : theRecords.Count,
                recordCount: theRecords.Count,
                firstRecord: searchPlan.Paging.FirstRecordNumber,
                pageSize: searchPlan.Paging.PageSize);

        }

        /// <summary>
        /// Returns a typed set of records, with no requiremeents for setting Media type or Data Source Code
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <returns></returns>
        public List<t> GetRecords(SearchParameters searchParameters)
        {
            searchParameters.ApplicationCode = ApplicationCode;
            searchParameters.FilterCode = FilterCode;
            searchParameters.DataSetCode = FilterCode;
            return ((IEnumerable<t>)GetData(searchParameters).Records).ToList();
        }
    }


}
