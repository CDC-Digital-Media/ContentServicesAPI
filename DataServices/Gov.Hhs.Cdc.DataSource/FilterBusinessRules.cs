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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServicesCacheProvider;
using Gov.Hhs.Cdc.DataSource.Dal;
using Gov.Hhs.Cdc.Logging;

namespace Gov.Hhs.Cdc.DataSource
{
    public class FilterBusinessRules
    {
        //public static DataSetDefinition GetDataSetDefinition(string applicationCode, string dataSetCode)
        //{
        //    try
        //    {
        //        return GetSearchProviderInfoMethod(applicationCode, dataSetCode);

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, ex.Message);
        //        throw;
        //    }
        //}

        //private static DataSetDefinition GetSearchProviderInfoMethod(string applicationCode, string dataSetCode)
        //{
        //    using (FrameworkObjectContext csData = new FrameworkObjectContext())
        //    {
        //        return FrameworkCache.GetDataSet(csData, applicationCode, dataSetCode)
        //            ?? new DataSetDefinition(applicationCode, dataSetCode);
        //    }
        //}


    }
}
