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
using Gov.Hhs.Cdc.CdcMediaProvider.Dal.Dal;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class MediaTypeSearchDataMgr : MediaBaseSearchMgr<MediaTypeItem>
    {
        public override List<ListItem> GetListItems(FilterCriterion parameter, bool getOnlySelectedItems)
        {
            return new List<ListItem>();
        } 

        public override List<HierarchicalItem> GetHierarchicalItems(FilterCriterion parameter, bool getRoot, FilterCriterion currentlySelectedItems, string parentKey, FilterCriterion filter)
        {
            throw new NotImplementedException();
        }

        public override DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms)
        {
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                FilterCriterionBoolean isPublicFacing = (FilterCriterionBoolean)searchPlan.Plan.Criteria.UseCriterion("IsPublicFacing");
                bool getOnlyPublicItems = isPublicFacing != null && isPublicFacing.IsFiltered && isPublicFacing.Value;
                
                IQueryable<MediaTypeItem> mediaItems = MediaTypeItemCtl.GetMediaTypeItem(media);
                if (getOnlyPublicItems)
                { mediaItems = mediaItems.Where(m => m.IsActive && m.Display); } //&& m.DisplayOrdinal > 0

                return CreateDataSetResults(mediaItems, searchPlan, resultSetParms.ResultSetId);
            }
        }

        
    }
}
