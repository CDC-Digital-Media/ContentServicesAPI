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
using System.Security.Principal;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace MediaCrudTests
{
    [TestClass]
    public class MediaCollectionTests
    {
        int collectionMediaId = 138478;

        static MediaCollectionTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void TestSaveCollection()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            var media = AdminApiCalls.SingleMedia(collectionMediaId);

            var child = TestApiUtility.SinglePublishedHtml();
            int[] testChildMediaIdsToInsert = { int.Parse(child.mediaId) };

            TestMediaItems media1 = new TestMediaItems(media, adminService, testChildMediaIdsToInsert);
            
            SerialMediaAdmin mediaFromGet = media1.Get();
            Assert.AreEqual(1, media1.Media.childRelationships.Count);

            media1.UpdateChildren(testChildMediaIdsToInsert);

            mediaFromGet = media1.Get();
            Assert.AreEqual(1, mediaFromGet.childRelationshipCount);
        }


        /// <summary>
        // If we add 3 medias where 1 => 2, 2 => 3, 3=> 1, we get a recursive loop error
        /// </summary>
        [TestMethod]
        public void TestRecursiveLoopError()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> sampleMedias;

            int[] testMediaIds = { 1, 2 };

            TestApiUtility.ApiGet<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", collectionMediaId.ToString(), "", ""), out sampleMedias);

            TestMediaItems media1 = new TestMediaItems(sampleMedias[0], adminService, testMediaIds);
            media1.Insert();

            TestMediaItems media2 = new TestMediaItems(sampleMedias[0], adminService, media1.MediaId);
            media2.Media.sourceUrl = null;
            media2.Media.mediaType = "Collection";
            media2.Insert();

            TestMediaItems media3 = new TestMediaItems(sampleMedias[0], adminService, media2.MediaId);
            media3.Insert();

            media1.UpdateChildren(media3.MediaId);
            Console.WriteLine(media1.UpdateValidationMessages.Errors().First().Message);
            Assert.AreEqual(4, media1.UpdateValidationMessages.Errors().Count());
            string errorMessage = "Relationship already has an inherited";
            Assert.IsTrue(string.Equals(errorMessage,
                media1.UpdateValidationMessages.Errors().ToList()[0].Message.Substring(0, errorMessage.Length)
                ));

            media1.Delete();
            media2.Delete();
            media3.Delete();

        }


        private class TestMediaItems
        {
            public SerialMediaAdmin Media { get; set; }
            IApiServiceFactory AdminService { get; set; }
            public int MediaId { get { return int.Parse(Media.id); } }
            public ValidationMessages InsertValidationMessages { get; set; }
            public ValidationMessages UpdateValidationMessages { get; set; }
            private string authorizedUser = "";


            public TestMediaItems(SerialMediaAdmin mediaObject, IApiServiceFactory adminService, params int[] children)
            {
                Media = mediaObject;
                AdminService = adminService;
                Media.id = mediaObject.id;
                Media.mediaType = "Collection";
                Media.childRelationships = children
                    .Select(i => new SerialMediaRelationship() { relatedMediaId = i, displayOrdinal = 20 - i }).ToList();
            }

            public void Insert()
            {
                List<SerialMediaAdmin> updatedAdmins;
                TestUrl insertUrl = AdminService.CreateTestUrl("media", "", "", "");
                InsertValidationMessages = TestApiUtility.ApiPost<SerialMediaAdmin>(AdminService, insertUrl, Media, out updatedAdmins, authorizedUser);
                //not getting a media object back when inserting a child
                //maybe because it doesn't exist?
                Assert.AreEqual(1, updatedAdmins.Count, "media " + Media.mediaId + " did not insert as child");
                Media = updatedAdmins[0];
            }

            public TestUrl UrlWithId
            {
                get { return AdminService.CreateTestUrl("media", Media.id, "", ""); }
            }

            public SerialMediaAdmin Get()
            {
                List<SerialMediaAdmin> mediasFromGet;
                TestApiUtility.ApiGet<SerialMediaAdmin>(AdminService, UrlWithId, out mediasFromGet);
                Assert.IsTrue(mediasFromGet.Count() > 0);
                return mediasFromGet[0];
            }

            public void UpdateChildren(params int[] children)
            {
                Media.childRelationships = children
                    .Select(i => new SerialMediaRelationship() { relatedMediaId = i, displayOrdinal = 20 - i }).ToList();
                List<SerialMediaAdmin> updatedAdmins;
                UpdateValidationMessages = TestApiUtility.ApiPut<SerialMediaAdmin>(AdminService, UrlWithId, Media, out updatedAdmins, authorizedUser);
                Assert.IsNotNull(updatedAdmins);
                if (UpdateValidationMessages.Errors().Count() == 0)
                {
                    Assert.AreEqual(1, updatedAdmins.Count);
                    Media = updatedAdmins[0];
                }
                else
                {
                    Assert.Fail(UpdateValidationMessages.Errors().First().Message);
                }
            }

            public void Delete()
            {
                ValidationMessages deleteMessages = TestApiUtility.ApiDelete(AdminService, UrlWithId, "", authorizedUser);
                if (deleteMessages.Errors().Count() > 0)
                {
                    Assert.Fail(deleteMessages.Errors().First().Message);
                }
            }
        }

    }
}
