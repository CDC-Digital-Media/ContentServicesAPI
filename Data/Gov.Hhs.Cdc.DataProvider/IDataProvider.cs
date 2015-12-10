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
using Gov.Hhs.Cdc.DataProvider;

namespace Gov.Hhs.Cdc.DataProvider
{
    public interface IDataProvider
    {
        ProxyCacheObject ProxyRequest(string url, string datasetId, out ValidationMessages messages);

        ProxyCacheObject GetProxyData(int id, out ValidationMessages messages);

        //ProxyCacheObject GetProxyDataByUrl(string url, out ValidationMessages messages);

        //ValidationMessages ExpireProxyDataById(int id);

        //ValidationMessages ExpireProxyDataByUrl(string url);

        //ValidationMessages ExpireProxyDataByDatasetId(string datasetId);

        //ValidationMessages ExpireProxyDataByDate(DateTime expiredBeforeDate);

        //ValidationMessages UpdateProxyDataExpirationIntervalById(int id, TimeSpan expirationIntervalTimeSpan);

        //ValidationMessages UpdateProxyDataExpirationIntervalByUrl(string url, TimeSpan expirationIntervalTimeSpan);

        ValidationMessages SaveProxyData(ProxyCacheObject proxyCacheObject);

        //ValidationMessages UpdateProxyDataByUrl(string url, string data);

        ValidationMessages DeleteProxyData(int id);

        //ValidationMessages DeleteProxyDataByUrl(string url);

        //ValidationMessages DeleteProxyDataByDate(DateTime expiredBeforeDate);

        //ValidationMessages DeleteProxyDataByDatasetId(string datasetId);

        Boolean ValidateAppKey(string key);

        ProxyCacheAppKeyObject GetProxyCacheAppKey(string key, out ValidationMessages messages);

        //ValidationMessages ToggleProxyCacheAppKey(string key, bool isActive);

        ValidationMessages SaveProxyCacheAppKey(ProxyCacheAppKeyObject newProxyCacheAppKeyObject);

        ValidationMessages DeleteProxyCacheAppKey(string key);

    }
}
