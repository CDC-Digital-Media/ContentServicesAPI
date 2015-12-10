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
using System.Data.Objects;
using System.Linq.Dynamic;
using Gov.Hhs.Cdc.UserProvider;

namespace Gov.Hhs.Cdc.DataSource.Media
{
    internal class OrganizationSearchDataMgr : MediaBaseSearchDataMgr
    {
        public OrganizationSearchDataMgr(EntitiesConfigurationItems configItems)
            : base(configItems)
        {

        }


        public override DataSetResult GetData(SearchPlan plan, ResultSetParms resultSetParms)
        {
            using (MediaObjectContext media = new MediaObjectContext(ConfigItems))
            {
                IQueryable<OrganizationItem> orgItems = (IQueryable<OrganizationItem>) new Dal.OrganizationItemCtl().Get(media);

                if (plan.Criteria.IsFilteredBy("Name"))
                {
                    string name = plan.Criteria.UseCriterion("Name").GetStringKey();
                    orgItems = orgItems.Where(o => o.Name.StartsWith(name));
                }

                //if (plan.Criteria.IsFilteredBy("OrgType"))
                //{
                //    List<string> stringKeys = plan.Criteria.GetCriterion("OrgType").GetStringKeys();
                //    orgItems = orgItems.Where("@0.Contains(outerIt.OrganizationTypeCode)", stringKeys);

                //    ////orgItems = orgItems.Where(o => stringKeys.Contains(o.OrganizationTypeCode));
                //    //orgItems = orgItems.WhereIn(c => c.OrganizationTypeCode, stringKeys);
                //    //string[] array = stringKeys.ToArray();
                //    //orgItems = orgItems.Where("OrganizationTypeCode.Contains(@0)", array);
                //}
                //string sql = ((ObjectQuery)orgItems).ToTraceString();

                //Code to execute DynamicLinq 
                //orgItems = orgItems.Where((FilterCriterionMultiSelect) plan.Criteria.UseCriterion("OrgType"));
                //string sql2 = ((ObjectQuery)orgItems).ToTraceString();

                return CreateDataSetResults<OrganizationItem>(orgItems, plan, resultSetParms.ResultSetId);

            }
        }
    }
}
