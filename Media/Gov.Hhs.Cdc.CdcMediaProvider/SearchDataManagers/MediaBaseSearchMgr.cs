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
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public abstract class MediaBaseSearchMgr<t> : SearchDataManager<t>
    {
        private DataListManager _dataListManager;
        public DataListManager DataListManager { get { return _dataListManager = new DataListManager(MediaMgrBase.ObjectContextFactory); } }

        public override string ApplicationCode { get { return "Media"; } }


        public MediaBaseSearchMgr()
            : base(MediaMgrBase.ObjectContextFactory)
        {
        }

        public override List<ListItem> GetListItems(FilterCriterion parameter, bool getOnlySelectedItems)
        {
            return DataListManager.GetListItems(parameter, getOnlySelectedItems);
        }

        public override List<HierarchicalItem> GetHierarchicalItems(FilterCriterion parameter, bool getRoot, FilterCriterion currentlySelectedItems, string parentKey, FilterCriterion filter)
        {
            throw new NotImplementedException();
        }

        public override DataSetResult GetData(SearchPlan plan, ResultSetParms resultSetParms)
        {
            throw new NotImplementedException();
        }

        public override DataSetResult GetData(SearchPlan plan, ResultSetParms resultSetParms, IList<int> allowableMediaIds)
        {
            throw new NotImplementedException();
        }
    }
}
