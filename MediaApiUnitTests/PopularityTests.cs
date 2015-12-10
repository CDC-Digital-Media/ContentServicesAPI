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
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaApiUnitTests
{
    [TestClass]
    public class PopularityTests
    {
        [TestMethod]
        public void CanRetrievePopularityReport()
        {
            var url = "https://.....[Data Server]...../resource/rppv-wbiv.json";
            using (var webClient = new WebClient())
            {
                var results = webClient.DownloadString(url);

                var pop = new JavaScriptSerializer().Deserialize<List<ContentPopularity>>(results);
                var sorted = pop.OrderByDescending(a => a.page_views);

                var urls = sorted.Select(i => i.content_source_urls).Distinct();

                var media = TestApiUtility.PublicApiV2MediaSearch("sort=-popularity");
                var top = media.First().sourceUrl;
                Console.WriteLine("top: " + top);

                var mediaUrls = media.Select(ro => ro.sourceUrl).Distinct();
                var inBoth = new Dictionary<string, int>();
                foreach (var item in mediaUrls)
                {
                    if (urls.Any(u => u == item))
                    {
                        Console.WriteLine(pop.Where(p => p.content_source_urls == item).Sum(p => p.page_views) + item);
                        inBoth.Add(item, pop.Where(p => p.content_source_urls == item).Sum(p => p.page_views));
                    }
                }
                Assert.IsTrue(inBoth.Count() > 10);
                var views = pop.Where(p => p.content_source_urls == top).First().page_views;
                Assert.IsTrue(views > 1000, views.ToString());
            }
        }

        private class ContentPopularity
        {
            public string content_source_urls { get; set; }
            public int page_views { get; set; }
            public string date { get; set; }
        }
    }


}
