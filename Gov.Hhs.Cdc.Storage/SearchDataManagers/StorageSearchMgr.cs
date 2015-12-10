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

using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

namespace Gov.Hhs.Cdc.Storage
{
    public class StorageSearchMgr : MediaBaseSearchMgr<StorageObject>
    {
        public override DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms)
        {
            using (Model.StorageObjectContext mediaDb = (Model.StorageObjectContext)ObjectContextFactory.Create())
            {
                IQueryable<StorageObject> storage = StorageCtl.Get(mediaDb);

                if (searchPlan.Plan.Criteria.IsFilteredBy("StorageId"))
                {
                    List<int> storageIds = searchPlan.Plan.Criteria.UseCriterion("StorageId").GetIntKeys();
                    storage = storage.Where(i => storageIds.Contains(i.StorageId));
                }

                return CreateDataSetResults(storage, searchPlan, resultSetParms.ResultSetId);
            }
        }

    }
}
