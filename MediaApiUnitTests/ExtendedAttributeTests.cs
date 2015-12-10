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
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaApiUnitTests
{
    [TestClass]
    public class ExtendedAttributeTests
    {
        int mediaIdWithExtendedAttributes = 281371;

        [TestMethod]
        public void CanRetrieveExtendedAttributesViaPublicApi()
        {
            var url = string.Format("{0}://{1}/api/{2}/resources/{3}/{4}", TestUrl.Protocol,
                TestUrl.PublicApiServer, "v2", "media", mediaIdWithExtendedAttributes);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(url);
            Console.WriteLine(result);

            var action = new ActionResultsWithType<List<SerialMediaV2>>(result);

            var obj = action.ResultObject[0];

            var ser = new JavaScriptSerializer().Serialize(obj.extendedAttributes);
            Console.WriteLine(ser);

            Assert.IsTrue(obj.extendedAttributes.Count > 0);
        }

        [TestMethod]
        public void CanRetrieveExtendedAttributesViaAdminApi()
        {
            var url = string.Format("{0}://{1}/adminapi/{2}/resources/{3}/{4}", TestUrl.Protocol,
                TestUrl.AdminApiServer, "v2", "media", mediaIdWithExtendedAttributes);
            string result = TestApiUtility.Get(url);
            Console.WriteLine(url);
            Console.WriteLine(result);

            var action = new ActionResultsWithType<List<SerialMediaAdmin>>(result);
            Assert.IsTrue(action.ResultObject.Count > 0);
            var obj = action.ResultObject[0];

            var ser = new JavaScriptSerializer().Serialize(obj.extendedAttributes);
            Console.WriteLine(ser);

            Assert.IsTrue(obj.extendedAttributes.Count > 0);
        }

        [TestMethod]
        public void CanUpdateAttribute()
        {
            SerialMediaAdmin data = TestApiUtility.SinglePublishedHtml();
            var mediaId = data.mediaId;
            data.extendedAttributes["Other"] = "z";

            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new KeyValuePairJsonConverter() });

            var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, mediaId);
            Console.WriteLine(url);

            var callResults = TestApiUtility.CallAPIPut(url, jss.Serialize(data), "");
            Console.WriteLine(callResults);

            var result = new JavaScriptSerializer().Deserialize<SerialResponseWithType<List<SerialMediaAdmin>>>(callResults);
            if (result.meta.status != 200)
            {
                Console.WriteLine(result.meta.message[0].userMessage);
                Console.WriteLine(result.meta.message[0].developerMessage);
                Assert.Fail(result.meta.message[0].userMessage);
            }

            Assert.IsNotNull(result.results);
            Assert.AreEqual(1, result.results.Count);

            Assert.AreEqual(mediaId.ToString(), result.results[0].mediaId);

            Assert.IsNotNull(result.results[0].extendedAttributes);
            Assert.IsTrue(result.results[0].extendedAttributes.Count > 0);
            Assert.IsNotNull(result.results[0].extendedAttributes["Other"]);

            Assert.AreEqual("z", result.results[0].extendedAttributes["Other"]);
        }

        [TestMethod]
        public void CanDeleteAttributes()
        {
            SerialMediaAdmin data = TestApiUtility.SinglePublishedHtml();
            var mediaId = data.mediaId;
            data.extendedAttributes = null;

            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new KeyValuePairJsonConverter() });
            var ser = jss.Serialize(data);
            Console.WriteLine(ser);

            var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, mediaId);

            var callResults = TestApiUtility.CallAPIPut(url, ser, "");

            var result = new JavaScriptSerializer().Deserialize<SerialResponseWithType<List<SerialMediaAdmin>>>(callResults);
            if (result.meta.status != 200)
            {
                Console.WriteLine(result.meta.message[0].userMessage);
                Console.WriteLine(result.meta.message[0].developerMessage);
                Assert.Fail(result.meta.message[0].userMessage);
            }

            Assert.IsNotNull(result.results);
            Assert.AreEqual(1, result.results.Count);

            Assert.AreEqual(mediaId.ToString(), result.results[0].mediaId);

            Assert.AreEqual(0, result.results[0].extendedAttributes.Count);
        }

        [TestMethod]
        public void CannotUpdateDescriptionWithMSWordQuote()
        {
            SerialMediaAdmin data = TestApiUtility.SinglePublishedHtml();
            var mediaId = data.mediaId;
            var msWord = "Hello, I have MS Word “double quote” and it’s got a special apostrophe.";

            data.description = data.description + msWord;

            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new KeyValuePairJsonConverter() });

            var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, mediaId);
            Console.WriteLine(url);

            var callResults = TestApiUtility.CallAPIPut(url, jss.Serialize(data), "");
            Console.WriteLine(callResults);

            var result = new JavaScriptSerializer().Deserialize<SerialResponseWithType<List<SerialMediaAdmin>>>(callResults);
            Assert.AreNotEqual(200, result.meta.status);
        }

        [TestMethod]
        public void CanAddAttribute()
        {
            SerialMediaAdmin data = TestApiUtility.SinglePublishedHtml();
            var mediaId = data.mediaId;
            data.extendedAttributes["CanAddAttribute"] = "true";

            var jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new KeyValuePairJsonConverter() });

            var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, mediaId);
            Console.WriteLine(url);

            var callResults = TestApiUtility.CallAPIPut(url, jss.Serialize(data), "");
            Console.WriteLine(callResults);

            var result = new JavaScriptSerializer().Deserialize<SerialResponseWithType<List<SerialMediaAdmin>>>(callResults);
            if (result.meta.status != 200)
            {
                Console.WriteLine(result.meta.message[0].userMessage);
                Console.WriteLine(result.meta.message[0].developerMessage);
                Assert.Fail(result.meta.message[0].userMessage);
            }

            Assert.IsNotNull(result.results);
            Assert.AreEqual(1, result.results.Count);

            Assert.AreEqual(mediaId.ToString(), result.results[0].mediaId);

            Assert.IsNotNull(result.results[0].extendedAttributes);
            Assert.IsTrue(result.results[0].extendedAttributes.Count > 0);
            Assert.IsTrue(result.results[0].extendedAttributes.ContainsKey("CanAddAttribute"));

            Assert.AreEqual("true", result.results[0].extendedAttributes["CanAddAttribute"]);
        }



        //https://.....[devReportingApplicationServer2]...../adminapi/v1/resources/media/?mediatype=HTML&status=published&max=10&pagenum=1&callback=?

    }
}
