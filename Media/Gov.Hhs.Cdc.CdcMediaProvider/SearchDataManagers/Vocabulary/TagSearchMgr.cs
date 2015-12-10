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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using System.Data.Objects;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class TagSearchMgr : MediaBaseSearchMgr<TagObject>
    {
        public override DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms)
        {
            using (MediaObjectContext mediaDb = (MediaObjectContext)ObjectContextFactory.Create())
            {
                IQueryable<TagObject> tags = TagCtl.Get(mediaDb);

                FilterCriterionSingleSelect mediaId = (FilterCriterionSingleSelect)searchPlan.Plan.Criteria.UseCriterion("MediaId");
                if (mediaId.IsFiltered)
                {
                    int theMediaId = mediaId.GetIntKey();
                    var valueIds = TagCtl.GetTagIdsForMedia(mediaDb, theMediaId);
                    tags = tags.Where(t => valueIds.Contains(t.TagId));
                }

                //string sql2 = ((ObjectQuery)tags).ToTraceString();

                FilterCriterionSingleSelect relatedValueId = (FilterCriterionSingleSelect)searchPlan.Plan.Criteria.UseCriterion("RelatedTagId");
                if (relatedValueId.IsFiltered)
                {
                    int relatedId = relatedValueId.GetIntKey();
                    var valueIds = TagCtl.GetRelatedTagDtos(mediaDb).Where(t => t.RelatedValueId == relatedId);
                    tags = from v in tags
                           join ids in valueIds on v.TagId equals ids.ValueId
                           select v;
                }

                return CreateDataSetResults(tags, searchPlan, resultSetParms.ResultSetId);
            }


        }
    }
}
