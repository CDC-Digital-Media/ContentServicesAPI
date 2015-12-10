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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServicesCacheProvider;

namespace Gov.Hhs.Cdc.DataSource.Dal
{

    public class FilterCriteriaDTOCtl : ICachedDataControl
    {


        public static CacheDataSetDictionary<List<FilterCriteriaDefinition>> GetFilterCriteriaDTODictionary(FrameworkObjectContext theEntities)
        {
            List<CriteriaForApplication> filterAttributes = SearchAttributeResolver.GetFilterCriteriaDtos();
            if (theEntities != null && theEntities.GetEfObjectContext() != null)
            {
                FrameworkObjectContext csData = theEntities;
                IQueryable<FilterCriteriaDefinition> dtosFromDatabase = GetFilterCriteriaDtos(csData);

                filterAttributes = filterAttributes
                    .Select(a => new CriteriaForApplication(a.ApplicationCode,
                        Merge(a.ApplicationCode, a.Criteria, dtosFromDatabase.Where(p => p.ApplicationCode == a.ApplicationCode)).ToList()
                        )).ToList();
            }

            filterAttributes = filterAttributes
                .Select(a => new CriteriaForApplication(a.ApplicationCode, a.Criteria
                        )).ToList();
            CacheDataSetDictionary<List<FilterCriteriaDefinition>> dictionary = new CacheDataSetDictionary<List<FilterCriteriaDefinition>>();
            foreach (CriteriaForApplication a in filterAttributes)
            {
                foreach (CriteriaDefinitionsForFilter f in a.Criteria)
                {

                    dictionary.Add(a.ApplicationCode + "|" + f.FilterCode, f.Criteria);

                }
            }
            return dictionary;
        }

        private static List<FilterCriteriaDefinition> Merge(IEnumerable<FilterCriteriaDefinition> attributes, IEnumerable<FilterCriteriaDefinition> databaseCriteria)
        {
            List<FilterCriteriaDefinition> test = databaseCriteria.ToList();

            //Merge database criteria into ours
            IEnumerable<FilterCriteriaDefinition> leftOuterJoin = from a in attributes
                                                                  join _d in databaseCriteria on a.Code equals _d.Code into _db
                                                                  from d in _db.DefaultIfEmpty()
                                                                  select d == null ? a : d;

            IEnumerable<FilterCriteriaDefinition> rightOuterJoin = from d in databaseCriteria
                                                                   join _a in attributes on d.Code equals _a.Code into _at
                                                                   from a in _at.DefaultIfEmpty()
                                                                   where a == null
                                                                   select d;
            List<FilterCriteriaDefinition> results = leftOuterJoin.Union(rightOuterJoin).ToList();
            return results;

        }

        private static List<CriteriaDefinitionsForFilter> Merge(string applicationCode, IEnumerable<CriteriaDefinitionsForFilter> attributes,
            IQueryable<FilterCriteriaDefinition> databaseCriteria)
        {
            List<FilterCriteriaDefinition> test = databaseCriteria.ToList();
            List<CriteriaDefinitionsForFilter> results = attributes
                    .Select(a => new CriteriaDefinitionsForFilter(
                        a.FilterCode,
                        a.NameSpace,
                        Merge(a.Criteria, databaseCriteria.Where(p => p.FilterCode == a.FilterCode).ToList())
                      )).ToList();
            return results;

        }

        private static IQueryable<FilterCriteriaDefinition> GetFilterCriteriaDtos(FrameworkObjectContext csData)
        {
            return csData.Entities.FilterCriterions
                //.Where(f => f.CriterionCode== "SelectNoneForTesting")
                .Select(criterion =>
                new FilterCriteriaDefinition()
                {
                    ApplicationCode = criterion.ApplicationCode,
                    FilterCode = criterion.FilterCode,
                    Code = criterion.CriterionCode,

                    //GroupOrder = g != null ? g.GroupOrder : criterion.CriterionOrder,
                    //GroupName = g.GroupName,

                    DisplayOrder = criterion.CriterionOrder,
                    DisplayName = criterion.CriterionName,
                    Type = criterion.CriterionType,
                    ListKeyType = criterion.ListKeyType,
                    DbColumnName = criterion.DbColumnName,
                    IsRequired = criterion.IsRequired == "Yes",
                    DisplayNote = criterion.DisplayNote,
                    GroupCode = criterion.FilterGroupCode,

                    AllowDateInFuture = criterion.FilterCriterionDate != null ?
                        criterion.FilterCriterionDate.AllowDateInFuture == "Yes" : true,
                    AllowDateInPast = criterion.FilterCriterionDate != null ?
                        criterion.FilterCriterionDate.AllowDateInPast == "Yes" : true,
                    IsIncludedByDefault = criterion.IsIncludedByDefault == "yes",

                    //ControlCriterionEnumerable = controlCriterion.Where(
                    //    cc => new {cc.ApplicationCode, cc.FilterCode, cc.CriterionCode} == 
                    //            new {criterion.ApplicationCode,criterion.FilterCode, criterion.CriterionCode})

                }
            );
        }


        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            throw new NotImplementedException();
        }
    }
}
