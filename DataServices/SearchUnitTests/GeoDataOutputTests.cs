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
    public class GeoDataOutputTests
    {
        private static int mediaIdWithGeoData = 348973;//TODO:  Change to insert media with geo
        private static SerialMediaV2 media = null;
        private static SerialGeoTag geo;
        private static MediaObject searchResult = null;
        private static MediaGeoDataObject searchGeo;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var criteria = new SearchCriteria { MediaId = mediaIdWithGeoData.ToString() };
            var result = CsMediaSearchProvider.Search(criteria);

            Assert.AreEqual(1, result.Count());

            searchResult = result.First();
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}/{4}", TestUrl.Protocol, TestUrl.PublicApiServer, "v2", "media", mediaIdWithGeoData);
            var item = TestApiUtility.Get(url);
            Console.WriteLine(url);
            Console.WriteLine(item);

            var x = new ActionResultsWithType<List<SerialMediaV2>>(item);
            Assert.AreEqual(1, x.ResultObject.Count());
            media = x.ResultObject[0];
            geo = media.geoTags[0];
            searchGeo = searchResult.MediaGeoData.First();
        }

        public GeoDataOutputTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        //        "ASCIIName": "Georgia",
        //"CountryCode": "US",
        //"GeoNameID": 4197000,
        //"ParentID": 6252001,
        //"Latitude": 32.75042,
        //"Longitude": -83.50018,
        //"Timezone": "America/New_York",
        //"Admin1Code": "GA"

        [TestMethod]
        public void GeoDataIsReturned()
        {
            Assert.IsNotNull(searchResult.MediaGeoData);
        }
        [TestMethod]
        public void NamesMatch()
        {
            Assert.AreEqual(geo.name, searchGeo.Name);
        }

        [TestMethod]
        public void CountryCodesMatch()
        {
            Assert.AreEqual(geo.countryCode, searchGeo.CountryCode);   
        }

        [TestMethod]
        public void GeoNameIdsMatch()
        {
            Assert.AreEqual(geo.geoNameId, searchGeo.GeoNameId);
        }

        [TestMethod]
        public void ParentIdsMatch()
        {
            Assert.AreEqual(geo.parentId, searchGeo.ParentId);
        }

        [TestMethod]
        public void LatitudesMatch()
        {
            Assert.AreEqual(geo.latitude, searchGeo.Latitude);
        }

        [TestMethod]
        public void LongitudesMatch()
        {
            Assert.AreEqual(geo.longitude, searchGeo.Longitude);
        }

        [TestMethod]
        public void TimeZonesMatch()
        {
            Assert.AreEqual(geo.timezone, searchGeo.Timezone);
        }

        [TestMethod]
        public void StateCodesMatch()
        {
            Assert.AreEqual(geo.admin1Code, searchGeo.Admin1Code);
        }

    }
}
