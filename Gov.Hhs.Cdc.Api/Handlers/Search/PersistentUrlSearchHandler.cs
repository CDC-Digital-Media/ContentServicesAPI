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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public static class PersistentUrlSearchHandler
    {
        private static IEnumerable<PersistentUrlObject> Get(List<Criterion> criteria)
        {
            ValidationMessages messages = new ValidationMessages();

            SearchParameters searchParameters = new SearchParameters("Media", "PersistentUrl", criteria.ToArray());

            DataSetResult dataSet = ServiceUtility.GetDataSet(searchParameters, out messages);

            return dataSet.Records.Cast<PersistentUrlObject>();
        }

        public static string RedirectUrl(string token)
        {
            string url = "";
            if (!string.IsNullOrEmpty(token))
            {
                List<Criterion> criteria = new List<Criterion>() { new Criterion("Token", token) };

                var item = Get(criteria).FirstOrDefault();
                if (item != null)
                {
                    url = item.Url;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    ServiceUtility.AddBasicApplicationCache(token, new Uri(url));
                }
            }

            return url;
        }

    }
}
