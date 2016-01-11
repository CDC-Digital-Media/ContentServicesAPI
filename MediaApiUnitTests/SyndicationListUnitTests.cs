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
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace MediaApiUnitTests
{
    [TestClass]
    public class SyndicationListUnitTests
    {
        private XNamespace Namespace = "http://schemas.datacontract.org/2004/07/Gov.Hhs.Cdc.Api";
       
        static SyndicationListUnitTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void ApiTestCreateSyndicationList()
        {
            var mediaIdWithTopic = 138587;
            int testMediaId = 138478;
            int testMediaId2 = 291701;

            string listName = "http://betobaccofree.hhs.gov";

            SyndicationListObject oldObject = GetSyndicationListByName(listName);
            
             if( oldObject != null)
                DeleteSyndicationList(oldObject.SyndicationListGuid.ToString() );  //Update this until we get the get by name/delete working from the API
 
            SerialSyndicationList syndicationList = new SerialSyndicationList()
            {
                domainName = "http://betobaccofree.hhs.gov",
                listName = listName,
                media = SerialSyndicationListMedia.CreateList(mediaIdWithTopic)
            };

            string jsonString = new JavaScriptSerializer().Serialize(syndicationList);
            string newGuid;
            ValidationMessages messages = CreateSyndicationList(jsonString, out newGuid);

            if (messages.Errors().Any())
            {
                Console.WriteLine(messages.Errors().First().Message);
                Console.WriteLine(messages.Errors().First().DeveloperMessage);
                Assert.Fail();
            }

            // add media to syndicationlist
            SerialSyndicationList addedItems = new SerialSyndicationList()
            {
                lastUpdatedUserEmailAddress = "sampleEmail5@email",
                media = SerialSyndicationListMedia.CreateList(testMediaId)
            };
            ValidationMessages addItemsMessages = AddMediaItemsToSyndicationList(newGuid, addedItems);
                        
            Assert.AreEqual(0, addItemsMessages.Errors().Count());
            SyndicationListObject listObject1 = GetSyndicationListByName(listName);
            Assert.AreEqual(2, listObject1.Medias.Count());

            // add another media to syndicationlist
            addedItems = new SerialSyndicationList()
            {
                lastUpdatedUserEmailAddress = "sampleEmail5@email",
                media = SerialSyndicationListMedia.CreateList(testMediaId2)
            };
            addItemsMessages = AddMediaItemsToSyndicationList(newGuid, addedItems);

            Assert.AreEqual(0, addItemsMessages.Errors().Count());
            listObject1 = GetSyndicationListByName(listName);
            Assert.AreEqual(3, listObject1.Medias.Count());

            // delete media from syndication list
            SerialSyndicationList deletedItems = new SerialSyndicationList()
            {
                lastUpdatedUserEmailAddress = "sampleEmail5@email",
                media = SerialSyndicationListMedia.CreateList(mediaIdWithTopic)
            };

            ValidationMessages list1Messages = GetSyndicationList(newGuid);
            Assert.AreEqual(0, list1Messages.Errors().Count());

            ValidationMessages deleteItemsMessages = DeleteMediaItemsFromSyndicationList(newGuid, deletedItems);
            Assert.AreEqual(0, deleteItemsMessages.Errors().Count());

            SyndicationListObject listObject2 = GetSyndicationListByName(listName);
            Assert.AreEqual(2, listObject2.Medias.Count());

            ValidationMessages messagesDelete = DeleteSyndicationList(newGuid);
 
            Assert.AreEqual(0, messagesDelete.Errors().Count());

            SyndicationListObject listObject3 = GetSyndicationListByName(listName);
            Assert.IsNull(listObject3);
        }

        private static SyndicationListObject GetSyndicationListByName(string listName)
        {
            TestOutputWriter getObjectWriter = new TestOutputWriter();
            SyndicationListHandler.GetByName(listName, getObjectWriter);
            SyndicationListObject oldObject = (SyndicationListObject)getObjectWriter.TheObject;
            return oldObject;
        }

        private ValidationMessages GetSyndicationList(string id)
        {
            if (TestApiUtility.UseWebService)
            {
                string url = String.Format("{0}://{1}/api/v1/resources/syndicationlists/{2}.xml", TestUrl.Protocol, TestUrl.PublicApiServer, id);
                WebClient serviceRequest = new WebClient();
                string response = serviceRequest.DownloadString(new Uri(url));
                XDocument doc = XDocument.Load(url);
                List<XElement> medias = doc.Descendants(Namespace + "results").ToList();
                 return new ValidationMessages();
            }
            else
            {
                TestOutputWriter getObjectWriter = new TestOutputWriter();
                SyndicationListHandler.Get(id, getObjectWriter);
                return getObjectWriter.ValidationMessages;
            }
        }

        public ValidationMessages CreateSyndicationList(string data, out string newGuid)
        {
            if (TestApiUtility.UseWebService)
            {
                string valueSetsUrl = String.Format("{0}://{1}/api/v1/resources/syndicationlists", TestUrl.Protocol, TestUrl.PublicApiServer);
                string callResults = TestApiUtility.CallAPI(valueSetsUrl, data, "POST");
                SerialResponseWithType<SerialSyndicationList> mediaObj = new JavaScriptSerializer().Deserialize<SerialResponseWithType<SerialSyndicationList>>(callResults);
                newGuid = mediaObj.results.syndicationListId;
                return mediaObj.meta.GetUnserializedMessages();
            }
            else
            {
                SerialResponse response = new SerialResponse();
                TestOutputWriter testWriter = new TestOutputWriter();
                SyndicationListHandler.Create(data, testWriter);
                newGuid = null;
                if (!testWriter.ValidationMessages.Errors().Any())
                {
                    SerialSyndicationList updatedSyndicationList = (SerialSyndicationList)testWriter.TheObject;
                    newGuid = updatedSyndicationList.syndicationListId;
                }
                return testWriter.ValidationMessages;
            }
        }

        public ValidationMessages AddMediaItemsToSyndicationList(string guid, SerialSyndicationList addedItems)
        {
            string jsonString = new JavaScriptSerializer().Serialize(addedItems);
            if (TestApiUtility.UseWebService)
            {
                string url = String.Format("{0}://{1}/api/v1/resources/syndicationlists/{2}/media", TestUrl.Protocol, TestUrl.PublicApiServer, guid);
                string callResults = TestApiUtility.CallAPI(url, jsonString, "POST");
                SerialResponse mediaObj = new JavaScriptSerializer().Deserialize<SerialResponse>(callResults);
                return mediaObj.meta.GetUnserializedMessages();
            }
            else
            {
                TestOutputWriter testWriter = new TestOutputWriter();
                SyndicationListHandler.AddMediaItemsToSyndicationList(jsonString, guid, testWriter);
                return testWriter.ValidationMessages ?? new ValidationMessages();
            }

        }
        public ValidationMessages DeleteMediaItemsFromSyndicationList(string guid, SerialSyndicationList deletedItems)
        {
            string jsonString = new JavaScriptSerializer().Serialize(deletedItems);

            if (TestApiUtility.UseWebService)
            {
                string url = String.Format("{0}://{1}/api/v1/resources/syndicationlists/{2}/media", TestUrl.Protocol, TestUrl.PublicApiServer, guid);
                string callResults = TestApiUtility.CallAPI(url, jsonString, "DELETE");
                SerialResponse mediaObj = new JavaScriptSerializer().Deserialize<SerialResponse>(callResults);
                return mediaObj.meta.GetUnserializedMessages();
            }
            else
            {
                TestOutputWriter testWriter = new TestOutputWriter();
                SyndicationListHandler.DeleteMediaItemsFromSyndicationList(jsonString, guid, testWriter);
                return testWriter.ValidationMessages ?? new ValidationMessages();
            }
        }

        public ValidationMessages DeleteSyndicationList(string id) //, out int id)
        {
            if (TestApiUtility.UseWebService)
            {
                string url = String.Format("{0}://{1}/api/v1/resources/syndicationlists/{2}", TestUrl.Protocol, TestUrl.PublicApiServer, id);
                return TestApiUtility.DeserializeResults(TestApiUtility.CallAPI(url, "", "DELETE"));
            }
            else
            {
                TestOutputWriter testWriter = new TestOutputWriter();
                SyndicationListHandler.Delete(id, testWriter);
                return testWriter.ValidationMessages;
            }
            
        }






        //[TestMethod]  Don't execute for now
        public void ApiTestMarciesMediaUpdate()
        {
            SerialSyndicationList syndicationList = new SerialSyndicationList()
            {
                //userId = "E004D525-1983-4F80-8719-02CF245419E9",
                domainName = "http://betobaccofree.hhs.gov",
                listName = "http://betobaccofree.hhs.gov",
                media = SerialSyndicationListMedia.CreateList(1, 2, 3)
            };
            int id = 118978;

            MediaObject persistedMediaItem = GetValueSetItem(118978);
            Assert.IsNotNull(persistedMediaItem);
            string rowVersion = persistedMediaItem.RowVersion.ToBase64String();

            
            string strDatePublished = DateTime.Now.AddYears(1).ToString("G");
            DateTime datePublished = DateTime.Parse(strDatePublished);
            string jsonContent = "{'source':'Centers for Disease Control and Prevention','mediatype':'HTML','mimetype':'.html','encoding':'utf-8','title':'CDC - Seasonal Influenza (Flu)','description':'','sourceUrl':'http://www......[domain]...../flu','targetUrl':'http://www......[domain]...../flu','language':'English','status':'published','datepublished':'" + strDatePublished + "','topics':[12,15],'rowVersion':'" + rowVersion + "'}";
            string mediaUpdateUrl = String.Format("https://{0}/adminapi/v1/resources/media/{1}/update", TestUrl.AdminApiServer, 118978);
            TestOutputWriter testWriter = new TestOutputWriter();
            var user = new AdminUser { UserGuid = Guid.Parse("ceb728aa-9eaa-44bb-b6bf-8d3f43437edd") };
            MediaMgrHandler.Update(testWriter, jsonContent, TestApiUtility.ConsumerKey, id, user);
            ValidationMessages messages = testWriter.ValidationMessages;

            MediaObject persistedMediaItem2 = GetValueSetItem(118978);
            Assert.AreEqual(datePublished, persistedMediaItem2.PublishedDateTime);
        }

        public static IObjectContextFactory MediaObjectContextFactory = new MediaObjectContextFactory();
        public MediaSearchMgr combinedMediaSearchDataMgr = new MediaSearchMgr();
        private MediaObject GetValueSetItem(int id)
        {
            return combinedMediaSearchDataMgr.GetData(new SearchParameters("Media", "Media", "MediaId".Is(id))).Records.Cast<MediaObject>().FirstOrDefault();
        }



    }
}
