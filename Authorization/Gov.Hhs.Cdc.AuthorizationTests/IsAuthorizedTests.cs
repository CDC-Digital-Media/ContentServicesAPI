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
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class IsAuthorizedTests
    {
        private string authorizedUser = "";
        private string unauthorizedUser = "ely9";
        private string vocabOnlyUser = "";
        private string validTopicIds = "[25221, 25222, 25223]";
        private string validOrgId = "14";

        static IsAuthorizedTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
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
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> mediaAdmins;
            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", "", "", ""),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'ApiCallRequiresAdminUserHeader','sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu','rowVersion':'AAAAAAAMoT4=','width':'800','height':'600','eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html','mobileTargetUrl':'http://www......[domain]...../flu','caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false},'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published','datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'" + validOrgId + "','maintainingOrgId':'" + validOrgId + "'}",
                out mediaAdmins);

            Assert.AreEqual(1, messages.Errors().Count());
            Console.WriteLine(messages.Errors().First().Message);
            Assert.IsTrue(messages.Errors().First().Message.Contains("User header not provided"));
        }

        [TestMethod]
        public void CdcUserNotInUsersTableGivesError()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> mediaAdmins;
            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", "", "", ""),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'CdcUserNotInUsersTableGivesError','sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu','rowVersion':'AAAAAAAMoT4=','width':'800','height':'600','eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html','mobileTargetUrl':'http://www......[domain]...../flu','caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false},'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published','datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'" + validOrgId + "','maintainingOrgId':'" + validOrgId + "'}",
                out mediaAdmins, unauthorizedUser);

            Assert.AreEqual(1, messages.Errors().Count());
            Assert.AreEqual("User not authorized for the admin system.", messages.Errors().First().Message);
        }

        [TestMethod]
        public void VocabOnlyUserGivesError()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> mediaAdmins;
            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media", "", "", ""),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'VocabOnlyUserGivesError','sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu','rowVersion':'AAAAAAAMoT4=','width':'800','height':'600','eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html','mobileTargetUrl':'http://www......[domain]...../flu','caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false},'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published','datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'" + validOrgId + "','maintainingOrgId':'" + validOrgId + "'}",
                out mediaAdmins, vocabOnlyUser);

            Assert.AreEqual(1, messages.Errors().Count());
            Assert.AreEqual("User is not authorized to administer media.", messages.Errors().First().Message);
        }

        [TestMethod]
        public void CdcUserInUsersTableSucceeds()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> mediaAdmins;
            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
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
                out mediaAdmins, authorizedUser);

            Console.WriteLine(mediaAdmins[0].id);
            if (messages.Errors().Count() > 0)
            {
                Assert.Fail(messages.Errors().First().Message);
            }
        }

        [TestMethod]
        public void CdcUserInUppercaseSucceeds()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> mediaAdmins;
            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
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
                out mediaAdmins, authorizedUser.ToUpper());

            if (messages.Errors().Count() > 0)
            {
                Assert.Fail(messages.Errors().First().Message);
            }
        }

    }
}
