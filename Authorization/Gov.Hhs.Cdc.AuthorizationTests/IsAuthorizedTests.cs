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
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class IsAuthorizedTests
    {
        private string authorizedUser = "";
        private string unauthorizedUser = "ely9";
        private string vocabOnlyUser = "";
        private string mediaOnlyUser = "AUE3";
        private string validTopicIds = "[25221, 25222, 25223]";
        private string validOrgId = "14";
        IApiServiceFactory adminService = new AdminApiServiceFactory();
        JavaScriptSerializer jss = new JavaScriptSerializer();

        static IsAuthorizedTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestCleanup]
        public void Cleanup()
        {
            DeleteMediaByTitle("CdcUserInUppercaseSucceeds");
            DeleteMediaByTitle("CdcUserInUsersTableSucceeds");
            DeleteMediaByTitle("CdcUserNotInUsersTableGivesError");
            DeleteMediaByTitle("CannotPostStorageWithoutAppropriateRole");
        }

        public void DeleteMediaByTitle(string title)
        {
            var criteria = "?name=" + title;
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            if (results.Count() > 100)
            {
                throw new ApplicationException("More than 100 results to delete.");
            }

            var mediaMgr = new CsMediaProvider();

            foreach (SerialMediaAdmin item in results)
            {
                var id = int.Parse(item.mediaId);
                mediaMgr.DeleteMedia(id);
            }
        }

        [TestMethod]
        public void UserNotInUserTableReturnsNull()
        {
            var user = "junk";
            var retrieved = AuthorizationManager.GetUser(user);
            Assert.IsNull(retrieved);
        }

        [TestMethod]
        public void ApiCallRequiresAdminUserHeader()
        {
            var sw = Stopwatch.StartNew();
            var response = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media"),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'ApiCallRequiresAdminUserHeader','sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu','rowVersion':'AAAAAAAMoT4=','width':'800','height':'600','eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html','mobileTargetUrl':'http://www......[domain]...../flu','caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false},'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published','datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'" + validOrgId + "','maintainingOrgId':'" + validOrgId + "'}"
                );
            Console.WriteLine(sw.Elapsed.Seconds + " seconds for API post");

            Assert.AreEqual(1, response.meta.message.Count());
            Console.WriteLine(response.meta.message.First().userMessage);
            Assert.IsTrue(response.meta.message.First().userMessage.Contains("User header not provided"));
            Console.WriteLine(sw.Elapsed.Seconds + " seconds for end of test");
        }

        [TestMethod]
        public void CdcUserNotInUsersTableGivesError()
        {
            var response = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media"),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'CdcUserNotInUsersTableGivesError','sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu','rowVersion':'AAAAAAAMoT4=','width':'800','height':'600','eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html','mobileTargetUrl':'http://www......[domain]...../flu','caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false},'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published','datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'" + validOrgId + "','maintainingOrgId':'" + validOrgId + "'}",
                unauthorizedUser);

            Assert.AreEqual(1, response.meta.message.Count());
            Assert.AreEqual("User not authorized for the admin system.", response.meta.message.First().userMessage);
        }

        [TestMethod]
        public void CannotPostMediaWithoutAppropriateRole()
        {
            var response = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media"),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'VocabOnlyUserGivesError','sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu','rowVersion':'AAAAAAAMoT4=','width':'800','height':'600','eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html','mobileTargetUrl':'http://www......[domain]...../flu','caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false},'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published','datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'" + validOrgId + "','maintainingOrgId':'" + validOrgId + "'}",
                vocabOnlyUser);
            Assert.AreEqual(1, response.meta.message.Count);
            Assert.AreEqual("User is not authorized to administer media.", response.meta.message.First().userMessage);
        }

        [TestMethod]
        public void CannotPostStorageWithoutAppropriateRole()
        {
            var response = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media"),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'CannotPostStorageWithoutAppropriateRole','sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu','rowVersion':'AAAAAAAMoT4=','width':'800','height':'600','eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html','mobileTargetUrl':'http://www......[domain]...../flu','caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false},'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published','datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'" + validOrgId + "','maintainingOrgId':'" + validOrgId + "'}",
                authorizedUser);

            var mediaAdmins = response.results as List<SerialMediaAdmin>;

            Assert.IsNotNull(mediaAdmins);
            Assert.IsNotNull(mediaAdmins.First());

            var mediaId = mediaAdmins.First().mediaId;

            var jss = new JavaScriptSerializer();
            string imageString = "[255,216,255,225,0,24,69,120,105,102,0,0,73,73,42,0,8,0,0,0,0,0,0,0,0,0,0,0,255,236,0,17,68,117,99,107,121,0,1,0,4,0,0,0,60,0,0,255,225,3,43,104,116,116,112,58,47,47,110,115,46,97,100,111,98,101,46,99,111,109,47,120,97,112,47,49,46,48,47,0,60,63,120,112,97,99,107,101,116,32,98,101,103,105,110,61,34,239,187,191,34,32,105,100,61,34,87,53,77,48,77,112,67,101,104,105,72,122,114,101,83,122,78,84,99,122,107,99,57,100,34,63,62,32,60,120,58,120,109,112,109,101,116,97,32,120,109,108,110,115,58,120,61,34,97,100,111,98,101,58,110,115,58,109,101,116,97,47,34,32,120,58,120,109,112,116,107,61,34,65,100,111,98,101,32,88,77,80,32,67,111,114,101,32,53,46,51,45,99,48,49,49,32,54,54,46,49,52,53,54,54,49,44,32,50,48,49,50,47,48,50,47,48,54,45,49,52,58,53,54,58,50,55,32,32,32,32,32,32,32,32,34,62,32,60,114,100,102,58,82,68,70,32,120,109,108,110,115,58,114,100,102,61,34,104,116,116,112,58,47,47,119,119,119,46,119,51,46,111,114,103,47,49,57,57,57,47,48,50,47,50,50,45,114,100,102,45,115,121,110,116,97,120,45,110,115,35,34,62,32,60,114,100,102,58,68,101,115,99,114,105,112,116,105,111,110,32,114,100,102,58,97,98,111,117,116,61,34,34,32,120,109,108,110,115,58,120,109,112,61,34,104,116,116,112,58,47,47,110,115,46,97,100,111,98,101,46,99,111,109,47,120,97,112,47,49,46,48,47,34,32,120,109,108,110,115,58,120,109,112,77,77,61,34,104,116,116,112,58,47,47,110,115,46,97,100,111,98,101,46,99,111,109,47,120,97,112,47,49,46,48,47,109,109,47,34,32,120,109,108,110,115,58,115,116,82,101,102,61,34,104,116,116,112,58,47,47,110,115,46,97,100,111,98,101,46,99,111,109,47,120,97,112,47,49,46,48,47,115,84,121,112,101,47,82,101,115,111,117,114,99,101,82,101,102,35,34,32,120,109,112,58,67,114,101,97,116,111,114,84,111,111,108,61,34,65,100,111,98,101,32,80,104,111,116,111,115,104,111,112,32,67,83,54,32,40,87,105,110,100,111,119,115,41,34,32,120,109,112,77,77,58,73,110,115,116,97,110,99,101,73,68,61,34,120,109,112,46,105,105,100,58,69,68,51,65,51,57,50,49,51,51,57,67,49,49,69,52,57,51,52,48,68,55,55,65,51,55,68,67,66,66,54,57,34,32,120,109,112,77,77,58,68,111,99,117,109,101,110,116,73,68,61,34,120,109,112,46,100,105,100,58,69,68,51,65,51,57,50,50,51,51,57,67,49,49,69,52,57,51,52,48,68,55,55,65,51,55,68,67,66,66,54,57,34,62,32,60,120,109,112,77,77,58,68,101,114,105,118,101,100,70,114,111,109,32,115,116,82,101,102,58,105,110,115,116,97,110,99,101,73,68,61,34,120,109,112,46,105,105,100,58,69,68,51,65,51,57,49,70,51,51,57,67,49,49,69,52,57,51,52,48,68,55,55,65,51,55,68,67,66,66,54,57,34,32,115,116,82,101,102,58,100,111,99,117,109,101,110,116,73,68,61,34,120,109,112,46,100,105,100,58,69,68,51,65,51,57,50,48,51,51,57,67,49,49,69,52,57,51,52,48,68,55,55,65,51,55,68,67,66,66,54,57,34,47,62,32,60,47,114,100,102,58,68,101,115,99,114,105,112,116,105,111,110,62,32,60,47,114,100,102,58,82,68,70,62,32,60,47,120,58,120,109,112,109,101,116,97,62,32,60,63,120,112,97,99,107,101,116,32,101,110,100,61,34,114,34,63,62,255,238,0,38,65,100,111,98,101,0,100,192,0,0,0,1,3,0,21,4,3,6,10,13,0,0,8,143,0,0,11,37,0,0,16,95,0,0,23,221,255,219,0,132,0,6,4,4,4,5,4,6,5,5,6,9,6,5,6,9,11,8,6,6,8,11,12,10,10,11,10,10,12,16,12,12,12,12,12,12,16,12,14,15,16,15,14,12,19,19,20,20,19,19,28,27,27,27,28,31,31,31,31,31,31,31,31,31,31,1,7,7,7,13,12,13,24,16,16,24,26,21,17,21,26,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,31,255,194,0,17,8,0,84,0,155,3,1,17,0,2,17,1,3,17,1,255,196,0,205,0,0,2,3,1,1,1,0,0,0,0,0,0,0,0,0,0,0,3,1,2,4,5,6,7,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,2,3,5,4,16,0,1,3,3,3,3,4,3,1,0,0,0,0,0,0,0,0,1,17,2,18,3,4,33,19,20,16,32,35,48,64,34,5,49,50,21,80,17,0,1,2,1,7,9,6,3,7,5,0,0,0,0,0,0,1,0,2,17,33,49,225,50,146,3,51,65,81,97,113,145,161,209,18,34,16,129,66,130,162,52,114,19,67,48,177,193,241,82,98,35,32,64,240,226,20,18,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,112,80,49,19,1,0,2,1,2,4,6,3,1,1,1,0,0,0,0,0,1,0,17,33,49,81,16,65,97,145,240,113,129,193,209,241,32,161,225,177,48,64,255,218,0,12,3,1,0,2,17,3,17,0,0,1,250,70,241,84,169,38,252,116,0,0,0,0,0,0,0,15,148,122,222,6,66,194,165,70,122,50,225,182,51,89,116,74,197,147,16,21,50,197,147,23,154,166,58,178,225,59,230,46,108,233,182,36,82,186,85,154,183,202,203,76,234,165,76,250,197,236,101,146,51,58,11,203,244,143,51,219,165,147,97,0,80,68,179,96,68,182,176,2,164,146,73,3,179,170,115,235,146,203,244,226,91,16,249,64,36,128,36,104,0,0,17,79,198,215,115,91,47,45,13,83,115,0,21,179,62,178,68,154,115,176,12,91,231,5,108,209,157,58,109,57,210,22,70,92,131,37,113,100,227,206,222,107,159,217,235,58,249,238,102,203,38,139,51,92,208,96,22,42,39,81,178,128,0,104,206,179,205,241,39,94,150,190,112,129,150,111,151,53,137,52,202,193,87,43,149,21,4,128,200,208,174,51,156,227,169,100,151,149,26,198,140,239,53,202,233,208,217,165,92,208,205,100,150,150,108,104,15,206,184,171,192,61,40,192,24,111,51,162,149,131,197,92,167,61,19,40,63,124,224,144,53,75,96,0,3,145,215,135,91,151,108,150,38,180,12,150,19,255,218,0,8,1,1,0,1,5,2,146,228,238,165,220,135,222,202,18,246,79,177,150,70,73,119,43,41,18,230,94,84,82,121,89,11,8,229,228,213,115,47,45,37,202,201,216,134,93,250,83,39,32,228,228,28,156,131,145,144,46,85,243,147,144,114,114,5,201,200,19,39,32,228,100,28,140,134,228,100,137,42,101,115,34,82,138,102,205,4,204,248,238,184,185,55,18,119,114,174,55,50,82,78,68,149,121,42,146,151,216,41,253,5,117,205,188,166,250,190,244,20,116,94,219,150,113,98,70,214,36,147,141,138,113,177,91,141,138,113,241,142,54,41,197,196,56,248,167,27,20,226,226,28,124,99,99,28,216,198,56,216,167,31,20,216,199,54,49,205,140,115,143,138,100,69,100,177,176,180,236,35,37,132,54,16,107,135,148,242,158,67,202,121,79,39,163,114,245,168,93,223,176,37,203,82,55,236,8,144,84,166,37,49,41,137,42,34,155,214,27,122,195,70,237,133,90,98,83,18,152,147,191,110,51,228,91,57,86,74,144,168,184,159,38,65,181,170,101,82,43,65,147,167,218,100,113,241,113,254,214,115,183,21,18,225,90,21,167,73,14,61,162,228,81,32,92,119,212,162,101,19,40,153,68,202,46,8,230,69,136,223,183,252,28,19,105,98,109,43,165,169,33,111,109,20,185,250,197,39,83,94,37,250,151,145,22,84,196,166,37,48,41,137,110,220,20,75,86,209,75,249,22,108,67,251,31,90,37,219,106,155,150,196,87,54,33,210,106,168,149,169,27,142,178,253,75,178,77,202,237,27,150,133,88,34,70,236,17,121,16,167,147,2,18,169,62,226,204,239,89,200,250,116,132,34,173,5,85,28,183,90,244,156,101,36,219,184,66,19,73,75,245,47,53,90,26,30,83,204,121,79,41,31,199,110,174,159,139,148,154,55,153,190,84,31,255,218,0,8,1,2,0,1,5,2,24,100,27,216,176,195,12,48,195,12,48,195,12,48,195,12,48,195,13,209,134,41,41,24,97,134,24,97,134,244,28,168,113,199,28,126,142,56,227,142,56,227,142,56,227,168,168,69,27,217,176,221,27,185,134,237,97,134,236,100,25,59,151,163,127,150,253,91,181,134,232,195,13,235,104,124,79,137,161,161,161,167,173,255,218,0,8,1,3,0,1,5,2,212,113,212,117,246,53,40,146,81,212,73,41,82,137,37,42,82,165,42,82,165,42,82,165,42,82,165,42,82,165,42,81,212,113,250,84,57,81,88,226,72,85,42,42,29,71,81,253,10,74,70,65,144,100,25,6,65,134,65,144,97,144,100,25,6,65,144,100,25,6,66,148,17,89,102,174,48,195,123,5,81,208,113,251,157,7,65,251,28,114,174,174,58,142,189,208,41,21,87,217,34,177,185,238,27,189,199,28,113,199,31,214,212,249,31,35,83,83,83,95,91,255,218,0,8,1,2,2,6,63,2,92,255,218,0,8,1,3,2,6,63,2,187,131,255,0,255,218,0,8,1,1,1,6,63,2,233,104,55,89,115,197,15,224,134,126,160,189,191,168,44,8,121,135,246,56,175,180,84,151,207,180,86,43,237,21,204,47,175,1,248,138,17,191,188,135,196,86,51,225,147,168,168,252,231,218,43,25,246,138,198,125,162,177,95,104,172,103,218,43,25,246,138,198,125,162,177,95,104,172,87,218,43,21,246,138,197,125,162,177,95,104,172,107,216,255,0,154,86,43,230,253,69,69,17,6,108,92,175,107,117,242,133,18,214,22,234,92,205,229,150,113,9,51,32,94,214,192,104,145,8,6,114,234,82,134,141,32,40,158,89,116,39,59,167,170,121,23,130,76,144,81,1,177,208,21,65,101,18,230,194,63,214,99,115,118,0,202,64,81,109,205,209,25,192,11,219,221,89,11,219,221,67,225,11,219,221,89,11,219,221,89,10,31,243,221,89,11,219,221,89,11,219,221,89,11,219,221,89,11,219,221,89,11,2,238,200,88,23,118,66,192,187,178,23,183,186,178,23,183,186,178,22,5,221,144,176,46,236,133,129,119,100,44,22,77,26,161,73,59,101,129,213,5,215,202,93,38,153,132,52,102,80,232,209,33,226,188,59,41,83,51,97,226,171,55,101,42,179,118,82,171,55,101,42,176,216,171,55,101,42,179,118,82,171,13,159,97,229,76,187,112,234,124,210,45,57,160,140,4,97,161,77,175,165,68,9,10,153,76,166,81,34,69,16,38,253,165,77,185,1,9,78,133,50,153,76,139,57,101,2,51,100,85,119,80,170,238,60,21,64,166,201,20,58,99,166,17,85,61,20,170,158,154,84,222,154,85,83,102,149,81,219,59,126,100,35,44,130,17,200,154,111,26,1,241,76,2,136,111,120,109,42,86,157,138,163,149,71,118,30,144,101,85,70,195,193,77,184,162,90,216,156,203,202,191,63,193,101,245,42,74,164,170,74,164,170,74,149,114,58,101,55,122,233,100,211,24,53,85,147,63,43,21,77,204,82,178,7,245,73,248,118,119,235,89,180,242,210,171,13,148,162,188,168,68,129,174,52,42,205,143,127,21,89,187,248,170,205,223,197,86,110,153,248,169,193,248,99,197,70,18,246,115,222,187,149,177,132,86,48,216,120,40,243,5,88,41,20,123,9,140,37,85,247,142,10,18,109,69,121,83,91,7,202,12,173,154,76,234,83,121,189,125,77,232,31,228,151,90,250,157,241,92,192,18,38,153,76,235,37,70,109,106,234,237,179,186,245,189,211,202,175,139,99,210,216,220,145,47,54,180,217,76,96,58,98,70,77,74,125,82,158,10,177,218,120,45,95,184,240,236,147,62,114,62,229,254,206,81,63,121,40,175,42,240,249,160,190,158,229,39,203,220,188,43,194,178,44,139,78,95,176,57,209,155,114,252,145,230,157,121,87,255,218,0,8,1,1,3,1,63,33,49,166,154,154,116,116,229,70,34,193,97,75,104,31,83,37,122,146,72,115,87,63,252,38,71,139,245,152,75,110,188,108,23,153,241,243,148,122,53,251,224,211,49,159,190,33,215,120,121,139,89,127,39,251,199,155,241,253,103,130,127,236,220,241,253,103,132,253,231,137,253,224,75,240,254,112,83,21,248,249,207,7,251,193,248,255,0,220,79,143,253,207,12,251,207,45,52,205,93,123,230,15,232,123,204,62,172,189,122,227,222,88,26,250,110,247,189,225,182,225,143,113,40,121,42,135,31,74,210,9,170,53,41,111,92,147,42,138,0,112,74,210,29,80,245,199,62,113,165,105,74,9,129,229,20,180,0,197,55,207,238,91,22,182,13,141,58,198,128,148,209,77,74,116,193,40,107,45,233,4,172,126,78,241,108,208,182,138,33,205,70,181,35,10,25,102,242,205,229,149,175,47,120,154,201,72,122,250,65,12,218,39,237,20,213,60,254,57,160,242,24,191,200,6,129,229,241,197,202,80,241,114,152,52,27,127,41,224,255,0,105,226,47,104,174,190,47,164,240,95,180,7,67,240,109,60,41,237,60,89,237,23,213,188,123,64,180,15,22,211,194,158,211,194,158,209,167,194,118,158,46,63,83,70,238,6,1,203,87,42,187,58,196,141,192,34,48,90,249,156,153,97,70,29,5,121,64,107,24,233,75,14,253,132,176,165,79,170,1,0,160,197,90,58,123,136,172,61,76,161,195,22,249,190,103,128,163,196,80,101,170,121,190,101,146,201,100,178,89,44,138,83,194,240,157,121,99,27,176,124,85,117,62,195,53,230,202,174,127,181,180,223,69,110,98,189,33,34,81,102,55,157,47,105,210,246,157,47,105,132,30,136,34,66,106,250,58,205,97,108,93,95,145,123,65,110,69,2,245,237,58,94,211,165,237,58,94,209,253,123,90,78,170,228,59,193,116,253,219,47,62,142,24,62,3,226,83,192,244,132,243,189,125,213,147,39,60,41,238,201,163,95,83,45,96,192,224,197,132,48,72,49,117,0,52,151,153,134,6,70,207,93,162,69,248,72,78,249,253,74,131,84,89,98,241,188,96,46,219,66,127,76,199,127,163,251,5,248,63,176,208,152,91,35,154,238,253,7,130,66,115,248,186,66,114,38,48,224,197,171,211,148,175,83,79,47,230,9,77,245,158,61,115,50,154,107,247,247,133,117,126,179,1,51,97,191,137,152,55,115,169,120,145,185,175,42,231,231,22,41,187,208,95,121,118,146,176,45,16,163,86,225,141,164,227,65,215,88,5,235,245,171,188,204,128,90,84,250,106,224,27,80,188,224,124,137,164,157,9,109,37,217,175,207,31,169,195,108,21,114,123,24,31,55,91,12,238,76,190,156,249,113,53,15,149,167,166,115,187,53,132,239,132,126,78,190,168,32,96,211,47,3,214,170,104,185,115,88,189,166,161,165,172,65,5,8,58,242,103,220,64,22,172,116,102,125,87,188,10,42,0,51,83,144,126,198,96,126,132,109,230,114,91,78,149,63,83,132,22,5,25,3,78,107,50,242,137,100,52,231,219,180,161,61,119,55,82,187,27,250,243,137,208,78,193,157,153,129,98,104,92,175,226,106,242,245,250,32,214,28,170,133,48,15,80,20,46,136,21,208,129,147,87,48,6,82,166,185,196,169,213,91,98,203,154,91,75,139,94,79,89,191,60,19,217,17,81,23,240,167,130,19,95,136,99,11,166,115,214,151,199,25,176,54,122,56,159,169,195,135,207,89,212,233,172,115,198,18,248,158,153,226,196,60,21,58,63,204,117,215,62,52,210,105,233,208,223,242,115,107,235,167,204,210,242,152,111,89,102,235,76,214,190,176,255,0,39,175,234,60,141,99,235,30,220,63,255,218,0,8,1,2,3,1,63,33,42,186,202,111,58,146,155,255,0,225,6,209,59,68,109,19,180,4,78,210,181,164,174,210,187,74,237,43,180,166,210,187,74,236,74,237,43,180,174,210,155,79,33,41,43,141,73,73,132,100,226,235,41,61,95,240,190,240,79,54,91,118,91,118,91,118,91,118,91,118,91,187,45,187,45,187,45,187,45,187,45,187,45,187,45,187,45,187,45,187,45,187,45,187,58,210,194,52,91,45,150,255,0,224,21,75,74,101,191,43,75,74,127,3,129,121,92,40,157,9,208,152,152,149,43,128,184,133,192,74,149,43,142,102,101,240,56,92,185,114,229,240,73,95,133,113,191,194,165,74,149,42,87,227,82,191,33,56,215,18,191,0,255,0,215,170,122,39,163,254,193,137,136,240,255,218,0,8,1,3,3,1,63,33,114,233,45,180,232,78,135,254,23,113,155,134,27,204,181,171,29,201,184,102,77,88,238,51,168,206,163,58,140,234,51,168,206,163,58,140,234,51,168,206,179,60,204,190,239,121,116,240,221,218,14,249,75,19,61,56,43,193,110,49,233,140,175,202,151,84,68,154,142,211,164,78,145,58,68,233,19,164,74,108,78,145,58,68,166,196,233,19,164,78,145,58,4,233,19,164,78,145,58,68,232,19,92,128,165,37,37,63,240,8,215,8,18,188,42,84,168,209,198,129,42,84,168,145,149,148,151,46,8,210,117,167,90,102,102,92,190,7,62,147,213,55,18,217,114,248,226,98,81,193,225,82,165,74,227,122,229,238,232,143,27,227,95,133,203,151,46,92,191,198,229,254,59,151,197,252,223,143,252,15,196,219,115,215,61,127,246,12,204,241,255,218,0,12,3,1,0,2,17,3,17,0,0,16,135,97,36,146,73,36,146,10,183,77,223,35,227,6,35,123,191,50,231,47,250,155,61,175,112,170,112,146,216,34,117,204,255,0,125,182,219,110,103,74,31,243,19,223,180,124,32,64,3,138,248,134,90,167,0,2,65,232,130,10,6,98,72,228,26,136,71,28,99,150,20,36,128,146,120,6,166,154,73,146,73,190,16,111,255,218,0,8,1,1,3,1,63,16,199,190,114,81,47,148,128,206,253,33,210,33,94,196,183,26,214,85,43,10,230,195,28,238,23,202,4,18,194,235,33,96,208,25,222,25,6,170,249,58,146,137,68,162,81,40,148,74,37,18,137,68,162,81,40,148,74,207,175,180,19,67,165,170,183,93,160,101,200,90,173,88,89,24,194,225,49,3,104,141,3,19,153,53,193,197,231,102,211,212,87,85,53,75,169,169,221,212,188,177,172,176,171,77,17,228,92,213,162,121,144,182,99,79,30,100,103,188,4,54,5,29,33,185,117,72,41,36,20,208,184,64,46,237,161,197,133,121,92,141,132,195,85,180,5,47,151,51,77,51,59,165,231,116,148,213,208,53,20,21,183,40,197,225,45,112,26,100,191,52,24,108,9,200,24,48,219,164,13,36,15,6,110,176,106,9,98,139,185,102,190,136,68,146,1,166,65,134,28,111,57,172,84,166,138,6,192,81,2,221,82,64,73,110,84,144,64,10,56,152,86,3,171,124,203,199,154,26,236,48,39,104,179,91,80,200,227,56,210,52,40,168,147,3,67,106,175,40,148,184,200,182,11,86,47,155,82,153,26,186,149,228,6,132,120,42,46,240,46,132,164,134,192,149,231,10,180,95,156,233,166,197,205,172,1,171,7,135,82,210,91,220,232,70,176,228,229,152,158,152,143,246,124,76,45,167,23,125,200,23,218,122,16,5,73,84,166,58,58,79,73,209,229,105,40,90,4,98,82,77,148,144,116,63,41,77,163,182,15,135,186,133,52,109,63,43,173,45,39,244,248,60,125,235,129,58,66,195,200,251,198,166,226,239,122,171,84,5,89,74,192,37,197,103,69,6,31,70,52,57,92,81,54,96,40,172,211,204,187,205,204,128,203,166,178,211,75,92,251,17,214,16,243,43,253,33,38,144,52,97,183,202,37,10,45,23,122,252,252,145,208,95,90,179,189,32,93,87,150,208,208,215,94,176,48,183,206,80,241,238,129,204,180,237,217,254,24,8,187,82,240,41,207,110,103,80,239,58,135,121,212,59,206,161,222,117,14,243,168,119,154,200,209,231,210,89,7,69,28,12,127,161,141,9,161,118,102,170,239,1,198,204,174,48,173,112,235,165,68,86,162,1,109,79,50,221,149,229,22,204,15,66,193,103,41,244,169,244,169,244,168,94,120,65,66,213,181,200,128,165,46,155,129,69,155,96,68,212,38,73,171,26,131,247,41,108,194,2,150,210,225,202,125,42,125,42,125,42,80,220,16,11,11,94,85,232,148,245,54,214,142,118,39,39,50,85,59,231,114,68,46,145,122,147,224,90,69,245,96,12,8,91,138,81,40,54,209,178,192,227,95,36,72,10,65,10,36,187,171,57,35,13,231,194,44,48,205,119,193,140,246,10,13,85,222,170,214,53,155,111,150,162,183,204,9,134,203,167,82,247,154,106,174,87,10,186,210,20,233,1,214,233,29,102,57,69,85,150,46,109,10,139,142,128,8,112,0,142,186,67,1,92,67,54,201,251,230,70,137,10,173,230,242,108,196,18,232,40,75,54,223,66,209,219,82,88,52,225,60,224,89,164,3,34,70,111,144,163,51,90,38,154,108,85,235,21,129,115,202,142,117,206,52,98,226,23,109,115,148,72,103,206,45,137,250,167,121,109,49,93,100,255,0,80,30,1,72,160,85,173,71,151,72,215,41,147,111,95,25,214,62,232,205,216,128,197,154,37,138,14,137,145,139,231,203,88,142,4,217,103,90,177,43,102,46,96,225,231,43,210,226,235,232,168,84,178,48,121,69,149,82,139,179,120,110,150,192,218,140,103,65,80,216,192,107,4,77,83,128,17,202,101,154,57,64,5,95,90,49,179,20,142,198,196,43,95,37,214,7,88,34,9,145,209,137,192,183,69,85,108,127,172,162,189,186,20,243,182,58,100,133,23,121,128,29,124,220,184,225,228,71,52,83,55,85,160,185,38,5,196,117,193,213,53,38,23,32,181,96,166,168,92,132,64,32,129,150,217,115,35,121,81,77,91,21,145,209,146,30,82,49,106,121,174,239,184,237,197,181,125,58,104,181,192,85,140,171,68,138,21,162,153,201,157,61,56,164,8,54,6,129,26,105,200,196,245,47,49,231,2,147,40,27,25,151,129,235,98,246,214,160,0,208,40,244,137,141,136,20,13,54,222,174,90,165,169,17,214,200,189,153,73,70,4,132,88,62,131,142,16,5,218,1,101,0,22,84,50,161,29,155,212,195,71,148,115,109,72,202,60,155,190,81,72,140,26,5,94,133,166,146,142,43,8,67,90,88,70,200,215,150,69,6,205,113,120,133,176,82,219,186,69,51,214,178,196,214,140,70,115,120,210,131,64,110,216,80,251,139,8,119,35,76,5,229,117,22,132,173,50,130,148,85,65,222,5,231,1,46,90,234,88,123,193,196,5,85,171,209,235,94,154,64,179,82,249,7,91,181,211,78,8,125,11,55,131,94,100,188,220,216,13,1,200,107,5,60,161,115,97,152,133,138,30,44,107,158,246,122,181,61,223,202,105,234,91,254,32,203,56,89,117,149,115,154,55,135,62,105,205,251,123,234,166,79,240,143,116,230,52,13,156,51,249,106,248,102,176,229,235,224,207,211,127,147,165,127,144,125,4,213,57,15,157,203,155,89,133,158,175,243,80,245,117,95,37,28,15,255,218,0,8,1,2,3,1,63,16,26,203,254,35,79,194,120,3,31,165,255,0,135,102,236,74,186,59,16,102,142,209,229,211,180,183,161,218,21,209,218,60,147,180,20,209,216,142,203,177,58,110,196,54,93,137,211,118,33,182,236,79,164,39,77,216,134,219,177,29,151,98,59,110,196,240,15,228,233,29,137,145,80,61,96,76,55,222,52,106,203,140,92,52,155,166,24,98,235,206,24,106,192,7,56,138,12,226,83,205,239,48,213,178,191,104,96,166,224,155,192,143,10,225,81,107,15,56,5,140,121,199,251,147,239,32,63,36,251,201,246,147,239,39,222,79,180,159,105,62,242,125,228,251,200,175,201,62,210,125,228,251,201,246,147,168,238,198,58,212,129,54,229,248,169,214,157,105,214,153,149,41,148,240,166,83,255,0,15,152,196,154,28,52,181,224,54,75,101,178,216,91,164,67,128,150,101,178,217,108,68,187,139,223,129,230,149,23,212,190,50,86,200,155,37,37,32,84,192,49,175,56,36,20,250,66,242,136,148,148,225,153,230,253,202,240,101,183,159,51,199,73,94,49,41,226,165,60,4,167,138,148,148,225,100,181,85,181,248,91,133,164,83,199,230,9,210,95,105,125,165,246,151,218,40,215,240,169,109,165,246,137,47,192,47,130,147,135,204,46,45,146,253,59,203,244,239,6,237,47,210,90,234,95,167,120,42,16,127,39,240,249,143,78,231,131,16,240,87,224,8,199,47,203,148,97,215,78,37,47,19,230,127,255,218,0,8,1,3,3,1,63,16,90,1,254,161,246,167,138,147,192,127,240,255,0,72,203,94,227,28,251,204,13,63,100,171,133,222,40,247,33,243,198,13,247,24,127,73,159,112,207,186,103,217,49,254,195,15,234,51,236,24,255,0,113,131,124,140,63,176,202,255,0,103,230,125,226,106,8,174,157,163,152,29,147,4,137,242,151,178,17,194,208,72,202,216,95,148,83,128,237,20,185,168,88,184,111,164,187,145,218,23,93,7,164,31,63,210,11,108,76,197,109,22,18,229,240,190,134,124,191,145,170,3,225,180,3,226,38,75,253,81,79,136,128,124,68,230,126,169,245,147,232,200,7,197,62,186,63,203,39,215,159,19,235,207,136,23,197,20,248,137,245,100,250,178,125,25,59,54,196,36,116,37,123,251,83,210,30,163,6,253,91,237,57,53,13,153,209,133,76,113,199,252,89,241,2,45,88,73,58,79,13,64,18,229,54,148,218,83,105,66,216,39,215,1,109,123,74,109,41,180,166,210,161,80,78,94,59,78,143,142,210,155,112,106,202,227,181,238,151,186,90,90,45,203,30,184,90,107,6,151,135,175,234,57,77,23,13,201,105,110,20,94,73,91,39,146,59,83,227,131,199,57,105,105,105,105,76,35,208,128,106,17,91,124,107,191,0,184,14,12,248,136,37,119,149,149,149,128,127,26,74,111,6,83,133,214,120,75,70,124,74,16,167,50,187,61,165,118,123,68,5,211,43,179,218,82,175,50,189,123,75,15,249,153,241,49,114,186,92,240,230,62,27,252,65,190,127,153,164,222,181,224,96,186,204,248,159,255,217]";

            byte[] image = jss.Deserialize<byte[]>(imageString);

            //create object to serialize
            var storage = new SerialStorageAdmin
            {
                name = "test",
                height = 84,
                width = 155,
                fileExtension = "jpg",
                mediaId = int.Parse(mediaId),
                data = image,
                type = "StorefrontThumbnail"
            };

            var url = string.Format("{0}://{1}/adminapi/v1/resources/links", TestUrl.Protocol, TestUrl.AdminApiServer);
            Console.WriteLine(url);

            //add storage item via Admin API
            var callResults = TestApiUtility.CallAPIPost(url, jss.Serialize(storage), vocabOnlyUser);
            Console.WriteLine(callResults);

            var result = jss.Deserialize<SerialResponseWithType<List<SerialStorageAdmin>>>(callResults);
            Assert.AreNotEqual(200, result.meta.status);
            //if (result.meta.status != 200)
            //{
            Console.WriteLine(result.meta.message[0].userMessage);
            Console.WriteLine(result.meta.message[0].developerMessage);
            //    Assert.Fail(vocabOnlyUser + " " + result.meta.message[0].userMessage);
            //}

            Assert.AreEqual(0, result.results.Count);




            //check mediaItem to verify deletion
            url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol,
                TestUrl.AdminApiServer, mediaId);
            callResults = TestApiUtility.Get(url);
            var mediaResult = jss.Deserialize<SerialResponseWithType<List<SerialMediaAdmin>>>(callResults);
            if (mediaResult.meta.status != 200)
            {
                Console.WriteLine(mediaResult.meta.message[0].userMessage);
                Console.WriteLine(mediaResult.meta.message[0].developerMessage);
                Assert.Fail(mediaResult.meta.message[0].userMessage);
            }
            Assert.AreEqual(0, mediaResult.results[0].alternateImages.Count);

        }

        [TestMethod]
        public void CannotAddValueWithoutRole()
        {
            var valuesetsUrl = adminService.CreateTestUrl("valuesets").ToString();
            var valuesetResults = TestApiUtility.Get(valuesetsUrl);

            var valueset = jss.Deserialize<SerialResponseWithType<List<SerialValueSetItem>>>(valuesetResults)
                .results.Where(vs => vs.id > 1).First();
            Assert.IsNotNull(valueset);

            var value = new SerialValueObj
            {
                valueName = "CannotAddValueWithoutRoleTest" + DateTime.Now.Ticks,
                valueSet = valueset.name
            };

            var valuesUrl = adminService.CreateTestUrl("valuesets").ToString();

            var callResults = TestApiUtility.CallAPIPost(valuesUrl, jss.Serialize(value), mediaOnlyUser);

            var result = jss.Deserialize<SerialResponse>(callResults);
            Assert.AreNotEqual(200, result.meta.status);
            Console.WriteLine(result.meta.message.First().userMessage);
        }

        [TestMethod]
        public void CannotAddValuesetWithoutRole()
        {
            var valuesetsUrl = adminService.CreateTestUrl("valuesets").ToString();
            var set = new SerialValueSetObj { name = "Valueset" + DateTime.Now.Ticks };
            var callResults = TestApiUtility.CallAPIPost(valuesetsUrl, jss.Serialize(set), mediaOnlyUser);
            var result = jss.Deserialize<SerialResponse>(callResults);
            Assert.AreNotEqual(200, result.meta.status);
            Console.WriteLine(result.meta.message.First().userMessage);
        }

        [TestMethod]
        public void CannotAssignRoleWithoutSystemAdminRole()
        {
                        var user = AuthorizationManager.GetUser(mediaOnlyUser);
            Assert.IsFalse(user.Roles.Contains(AuthorizationManager.SystemAdminRole));

            var url = adminService.CreateTestUrl("adminusers");

            var data = "{\"userName\":\"\",\"roles\":[\"Media Admin\",\"System Admin\", \"Feeds Scheduler Admin\"]}";
            var result = TestApiUtility.CallAPIPost(url.ToString(), data, mediaOnlyUser);
            Console.WriteLine(result);

            Assert.AreNotEqual(200, jss.Deserialize<SerialResponse>(result).meta.status);
        }

        [TestMethod]
        public void CdcUserInUsersTableSucceeds()
        {
            var result = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media"),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'CdcUserInUsersTableSucceeds',
'sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu',
'rowVersion':'AAAAAAAMoT4=','width':'800','height':'600',
'eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html'
,'mobileTargetUrl':'http://www......[domain]...../flu'
,'caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false}
,'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published'
,'datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'11','maintainingOrgId':'1'}",
                authorizedUser);

            if (result.meta.message.Count() > 0)
            {
                Assert.Fail(result.meta.message.First().userMessage);
            }
        }

        [TestMethod]
        public void CdcUserInUppercaseSucceeds()
        {
            var response = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media"),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'CdcUserInUppercaseSucceeds',
'sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu',
'rowVersion':'AAAAAAAMoT4=','width':'800','height':'600',
'eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html'
,'mobileTargetUrl':'http://www......[domain]...../flu'
,'caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false}
,'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published'
,'datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'11','maintainingOrgId':'1'}",
                authorizedUser.ToUpper());

            if (response.meta.message.Count() > 0)
            {
                Assert.Fail(response.meta.message.First().userMessage);
            }
        }

    }
}
