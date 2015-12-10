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
using Gov.Hhs.Cdc.DataProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class ProxyCacheTransformation
    {
        public static ProxyCacheObject CreateProxyCacheObject(SerialProxyCache serialProxyCache)
        {
            ProxyCacheObject newObject = new ProxyCacheObject();

            newObject.Id = ServiceUtility.ParseInt(serialProxyCache.id);
            newObject.Url = serialProxyCache.url;
            newObject.DatasetId = serialProxyCache.datasetId;
            newObject.Data = serialProxyCache.data;
            newObject.ExpirationInterval = serialProxyCache.expirationInterval;
            newObject.ExpirationDateTime = (DateTime)serialProxyCache.expirationDateTime.ParseUtcDateTime();
            newObject.NeedsRefresh = serialProxyCache.needsRefresh.ToLower() == "true" ? true : false;
            newObject.Failures = ServiceUtility.ParseInt(serialProxyCache.failures);

            return newObject;
        }

        public static SerialProxyCache CreateSerialProxyCache(ProxyCacheObject proxyCacheObject)
        {
            SerialProxyCache newSerialObject = new SerialProxyCache();

            newSerialObject.id = proxyCacheObject.Id.ToString();
            newSerialObject.url = proxyCacheObject.Url;
            newSerialObject.datasetId = proxyCacheObject.DatasetId;
            newSerialObject.data = proxyCacheObject.Data;
            newSerialObject.expirationInterval = proxyCacheObject.ExpirationInterval;
            DateTime? expirationDateTime = proxyCacheObject.ExpirationDateTime;
            newSerialObject.expirationDateTime = expirationDateTime.FormatAsUtcString();
            newSerialObject.needsRefresh = proxyCacheObject.NeedsRefresh.ToString();
            newSerialObject.status = proxyCacheObject.GetDataStatus().ToString();
            newSerialObject.failures = proxyCacheObject.Failures.ToString();

            return newSerialObject;
        }

        public static SerialProxyData CreateSerialProxyData(ProxyCacheObject proxyCacheObject)
        {
            SerialProxyData newSerialObject = new SerialProxyData();

            newSerialObject.status = proxyCacheObject.GetDataStatus().ToString();
            newSerialObject.data = proxyCacheObject.Data;

            return newSerialObject;
        }
    }
}
