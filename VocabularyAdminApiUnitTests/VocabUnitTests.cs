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

using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace VocabularyAdminApiUnitTests
{
    [TestClass]
    public class VocabUnitTests
    {      
        private XNamespace Namespace = "http://schemas.datacontract.org/2004/07/Gov.Hhs.Cdc.Api";
        private const int audienceValueSetId = 3;
        private const int topicId = 25222;
        private string authorizedUser = "";

        static VocabUnitTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        //for testing purpose only, accept any dodgy certificate... 
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        [TestMethod]
        public void ApiGetMediasByTopicV1()
        {
            string url = String.Format("{0}://{1}/api/v1/resources/media/?format=xml&topicid=" + topicId.ToString() + "&max=50", TestUrl.Protocol, TestUrl.PublicApiServer);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));
            XDocument doc = XDocument.Load(url);
            List<XElement> medias = doc.Descendants(Namespace + "mediaItem").ToList();
            Assert.IsTrue(medias.Count() > 1 && medias.Count() < 60, medias.Count().ToString());
        }

        [TestMethod]
        public void ApiGetMediasByTopicV2()
        {
            string url = String.Format("{0}://{1}/api/v2/resources/media/?format=xml&topicids=" + topicId.ToString() + "&max=50", TestUrl.Protocol, TestUrl.PublicApiServer);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));
            
            XDocument doc = XDocument.Load(url);
            List<XElement> medias = doc.Descendants(Namespace + "mediaItem2").ToList();
            Assert.IsTrue(medias.Count() > 1 && medias.Count() < 60, medias.Count().ToString());
        }

        [TestMethod]
        public void ApiGetAllValueSets()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/valuesets?format=xml&max=50", TestUrl.Protocol, TestUrl.AdminApiServer);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));

            XDocument doc = XDocument.Load(url);
            List<XElement> valueSets = doc.Descendants(Namespace + "valueSet").ToList();
            Assert.AreEqual(50, valueSets.Count());
        }

        [TestMethod, Ignore] //audiences not returned -- for now
        public void ApiGetAllValuesetsById()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/valuesets?format=xml&id=" + audienceValueSetId.ToString() + "&max=50", TestUrl.Protocol, TestUrl.AdminApiServer);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));

            XDocument doc = XDocument.Load(url);
            List<XElement> valueSets = doc.Descendants(Namespace + "vocabValueItem").ToList();
            Assert.IsTrue(10 < valueSets.Count && valueSets.Count < 30);
        }

        [TestMethod, Ignore] //audiences not returned -- for now
        public void ApiGetAllValuesetsById2()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/valuesets/" + audienceValueSetId.ToString() + ".xml?language=english&sort=displayOrdinal&max=50", TestUrl.Protocol, TestUrl.AdminApiServer);


            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));
            XDocument doc = XDocument.Load(url);
            List<XElement> values = doc.Descendants(Namespace + "vocabValueItem").ToList();
            Assert.IsTrue(10 < values.Count && values.Count < 20);
        }


        //http://localhost:11111/adminapi/v1/resources/valuesets.xml?max=10&page=1&id=3&q=herder
        //http://.....[devApiServer]...../adminapi/v1/resources/valuesets.xml?max=10&page=1&id=3&q=herder
        [TestMethod, Ignore] //audiences not returned -- for now
        public void ApiGetAllValuesetsByIdAndQ()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/valuesets.xml?max=10&page=1&id=" + audienceValueSetId + "&q=partner", TestUrl.Protocol, TestUrl.AdminApiServer);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));
            XDocument doc = XDocument.Load(url);
            List<XElement> valueSets = doc.Descendants(Namespace + "vocabValueItem").ToList();
            Assert.IsTrue(valueSets.Count() == 1);
        }
        ///adminapi/v1/resources/valuesets.json	max=10&page=1&q=audi
        ///
        //http://.....[devApiServer]...../adminapi/v1/resources/valuesets?format=xml&q=audi
        [TestMethod]
        public void ApiGetAllValueSetsWithQ()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/valuesets?format=xml&q=audi", TestUrl.Protocol, TestUrl.AdminApiServer);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));
            XDocument doc = XDocument.Load(url);
            List<XElement> valueSets = doc.Descendants(Namespace + "valueSet").ToList();
            Assert.IsTrue(valueSets.Count() > 0);
        }

        [TestMethod]
        public void ApiGetValuesByValueSetName()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/values?format=xml&valueset=topics&language=english", TestUrl.Protocol, TestUrl.AdminApiServer);
            Console.WriteLine(url);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));
            XDocument doc = XDocument.Load(url);
            List<XElement> valueSets = doc.Descendants(Namespace + "vocabValueItem").ToList();
            Assert.AreEqual(100, valueSets.Count());
        }

        [TestMethod]
        public void ApiGetValuesByValueSetId()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/values?format=xml&valuesetid=1,2", TestUrl.Protocol, TestUrl.AdminApiServer);

            ////Used for SSL
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            WebClient serviceRequest = new WebClient();
            string response = serviceRequest.DownloadString(new Uri(url));
            XDocument doc = XDocument.Load(url);
            List<XElement> valueSets = doc.Descendants(Namespace + "vocabValueItem").ToList();
            Assert.IsTrue(50 < valueSets.Count() && valueSets.Count() < 500);
        }


        //https://.....[devApiServer]...../adminapi/v1/resources/valuesets
        //data: '{"name":"I have no values","languageCode":"English","description":"no, really","displayOrdinal":"-1","isActive":"true","isDefaultable":"true","isOrderable":"true","isHierachical":"true"}'
        //data: "{'name':'I have no values','languageCode':'English','description':'no, really','displayOrdinal':'-1','isActive':'true','isDefaultable':'true','isOrderable':'true','isHierachical':'true'}"
        ///


        public static IObjectContextFactory MediaObjectContextFactory = new MediaObjectContextFactory();
        public BusinessUnitSearchDataMgr businessUnitSearchDataMgr = new BusinessUnitSearchDataMgr();
        public ValueSetSearchDataMgr valueSetSearchDataMgr = new ValueSetSearchDataMgr();
        
        private ValueSetObject GetValueSetObject(string name)
        {
            return valueSetSearchDataMgr.GetData(new SearchParameters("Media", "ValueSet", "Name".Is(name))).Records.Cast<ValueSetObject>().FirstOrDefault();
        }

        [TestMethod]
        public void ApiTestCreateValueSet()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();

            var valueProvider = new CsValueProvider();

            string name = "API Test 1";
            //string valueSetJson =        "{'name':'" + name + "','language':'English','description':'no, really','displayOrdinal':'-1','isActive':'true','isDefaultable':'true','isOrderable':'true','isHierachical':'true'}";
            //SerialValueSetObj valueSet = new JavaScriptSerializer().Deserialize<SerialValueSetObj>(valueSetJson);
            SerialValueSetObj valueSet = new SerialValueSetObj()
            {
                name = name,
                language = "English",
                description = "no, really",
                displayOrdinal = "-1",
                isActive = "true",
                isDefaultable = "true",
                isOrderable = "true",
                isHierachical = "true"
            };

            //string updatedValueSetJson = "{'name':'" + name + "','language':'English','description':'no, really','displayOrdinal':'-1','isActive':'true','isDefaultable':'false','isOrderable':'true','isHierachical':'true'}";
            //SerialValueSetObj updatedValueSet = new JavaScriptSerializer().Deserialize<SerialValueSetObj>(updatedValueSetJson);
            SerialValueSetObj updatedValueSet = new SerialValueSetObj()
            {
                name = name,
                language = "English",
                description = "no, really",
                displayOrdinal = "-1",
                isActive = "true",
                isDefaultable = "false",
                isOrderable = "true",
                isHierachical = "true"
            };

            var theRecord = GetValueSetObject(name);
            if (theRecord != null)
                TestApiUtility.ApiDelete(adminService, adminService.CreateTestUrl("valuesets", theRecord.Id, "", ""), null, authorizedUser);


            var urlWithoutId = adminService.CreateTestUrl("valuesets", "", "", "");
            List<SerialValueSetObj> createdValueSets;
            var messages = TestApiUtility.ApiPost<SerialValueSetObj>(adminService, 
                urlWithoutId, valueSet, out createdValueSets, authorizedUser);

            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().Count() + " errors.");
            }
            int id = GetValueSetObject(name).Id;
            var urlWithId = adminService.CreateTestUrl("valuesets", id, "", "");

            var duplicateMessages = TestApiUtility.ApiPost<SerialValueSetObj>(adminService,
                urlWithoutId, valueSet, out createdValueSets, authorizedUser);

            Assert.AreEqual(1, duplicateMessages.Errors().Count());
            Assert.AreEqual("Value set with this name already exists", duplicateMessages.Errors().ToList()[0].Message);


            var messagesPut = TestApiUtility.ApiPut<SerialValueSetObj>(adminService,
                urlWithId, updatedValueSet, out createdValueSets, authorizedUser);

            Assert.AreEqual(0, messagesPut.Errors().Count());

            var theRecord2 = GetValueSetObject(name);
            Assert.AreEqual(false, theRecord2.IsDefaultable);


            var messagesDelete = TestApiUtility.ApiDelete(adminService, urlWithId, null, authorizedUser); 

            var theRecord3 = GetValueSetObject(name);
            Assert.IsNull(theRecord3);

        }

    }
}
