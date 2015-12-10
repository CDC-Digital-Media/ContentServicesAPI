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
using System.Web;

using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.CsCaching;

namespace Gov.Hhs.Cdc.Api.Public
{
    public class PersistentUrlRedirectModule
    {
        public static string GetUrl(string[] segments)
        {
            string endToken = segments[segments.GetUpperBound(0)];
            string sUrl = string.Empty;

            if (!string.IsNullOrEmpty(endToken) && !endToken.Contains("."))
            {
                endToken = endToken.ToUpper();
                try
                {
                    var cache = CacheManager.CachedValue<Uri>(endToken);
                    if (cache == null)
                    {
                        sUrl = PersistentUrlSearchHandler.RedirectUrl(endToken);
                    }
                    else
                    {
                        sUrl = cache.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("The Persistent URL key (" + endToken + ") was not found in the collection. " + ex.ToString());
                }
            }

            return sUrl;
        }


    }
}
