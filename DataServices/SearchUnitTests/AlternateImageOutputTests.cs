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
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SearchUnitTests
{
    [TestClass]
    public class AlternateImageOutputTests
    {
        public AlternateImageOutputTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }
        private static int mediaIdWithAlternateImage = 356165;

        private static SerialAlternateImage store = null;
        private static StorageObject search = null;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var criteria = new SearchCriteria { MediaId = mediaIdWithAlternateImage.ToString() };
            var searchResult = CsMediaSearchProvider.Search(criteria);
            Assert.IsNotNull(searchResult.First().AlternateImages);
            search = searchResult.First().AlternateImages.First();

            var url = string.Format("{0}://{1}/api/{2}/resources/{3}/{4}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "media", mediaIdWithAlternateImage);
            Console.WriteLine(url);
            var result = TestApiUtility.Get(url);
            Console.WriteLine(result);
            var action = new ActionResultsWithType<List<SerialMediaV2>>(result);
            Assert.IsNotNull(action.ResultObject);
            Assert.IsTrue(action.ResultObject.Count > 0);
            var media = action.ResultObject[0];
            Assert.IsNotNull(media.alternateImages);
            store = media.alternateImages.First();
        }

        [TestMethod]
        public void AlternateImageIsNotNull()
        {
            Assert.IsNotNull(search);
            Assert.IsNotNull(store);
        }

        [TestMethod]
        public void StorageIdsMatch()
        {
            Assert.AreEqual(store.id, search.StorageId);
        }

        [TestMethod]
        public void NamesMatch()
        {
            Assert.AreEqual(store.name, search.Name);
        }

        [TestMethod]
        public void HeightsMatch()
        {
            Assert.AreEqual(store.height, search.Height);
        }

        [TestMethod]
        public void WidthsMatch()
        {
            Assert.AreEqual(store.width, search.Width);
        }
    }
}
