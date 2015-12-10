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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.MediaProvider;

using Gov.Hhs.Cdc.CsCaching;
using System.Configuration;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class MediaTransformationBase
    {
        protected virtual List<SerialValueList> GetValueItemLists(IEnumerable<AttributeValueObject> attributes)
        {
            List<SerialValueList> valueItemList = (from a in attributes
                                                   group a by a.AttributeName into g
                                                   select new SerialValueList()
                                                   {
                                                       attributeName = g.Key,
                                                       valueItems = g.Select(av => MediaTransformationHelper.GetAttribute(av)).ToList()
                                                   }
                            ).ToList();
            return valueItemList;
        }


        protected void UpdateDynamicUrls(IMediaDynamicUrl media)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultApiKey"]))
            {
                media.url = AppendApiKeyToUrlQueryString(media.url);
                media.embedUrl = AppendApiKeyToUrlQueryString(media.embedUrl);
                media.syndicateUrl = AppendApiKeyToUrlQueryString(media.syndicateUrl);
                media.contentUrl = AppendApiKeyToUrlQueryString(media.contentUrl);
                media.thumbnailUrl = AppendApiKeyToUrlQueryString(media.thumbnailUrl);
            }
        }

        protected static string AppendApiKeyToUrlQueryString(string url)
        {
            if (!string.IsNullOrEmpty(url) && string.IsNullOrEmpty(ConfigurationManager.AppSettings["DefaultApiKey"]))
            {
                Dictionary<string, string> dict = CacheManager.CachedValue<Dictionary<string, string>>(Param.API_KEY);
                string apiKey = dict.FirstOrDefault(x => x.Value == Connection.CurrentConnection.Name).Key;

                url += "?apikey=" + apiKey;
            }

            return url;
        }

    }
}
