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
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.CdcMediaProvider;

namespace SearchUnitTests
{
    [TestClass]
    public class BugTests
    {
        private string validTopicIds = "[25272, 25329, 25651]";
        private string authorizedUser = "";
        int mediaId;

        static BugTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (mediaId > 0)
            {
                new CsMediaProvider().DeleteMedia(mediaId);
            }
        }

        [TestMethod]
        public void TestBug1()
        {
            IApiServiceFactory adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> mediaAdmins;
            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media"),
                @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'TestBug1','sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf',
                'targetUrl':'http://www......[domain]...../flu','rowVersion':'AAAAAAAMoT4=','width':'800','height':'600',
                'eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard','html5Source':'http://.....[devServer]...../eCard_resources/STD01.html',
                'mobileTargetUrl':'http://www......[domain]...../flu',
                'caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
                ,'cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
                ,'cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
                ,'cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
                ,'imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
                ,'imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg','imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg','isMobile':true,'isActive':false}
                ,'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published'
                ,'datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'11','maintainingOrgId':'11'}",
                out mediaAdmins, authorizedUser);
            if (messages.Errors().Count() > 0)
            {
                Assert.Fail(messages.Errors().First().Message);
            }
            mediaId = int.Parse(mediaAdmins.FirstOrDefault().mediaId);

        }

        [TestMethod]
        public void TestBug2()
        {
            var adminService = new AdminApiServiceFactory();
            List<SerialMediaAdmin> mediaAdmins;
            var url = adminService.CreateTestUrl("media", 121812, "update");
            Console.WriteLine(url);
            ValidationMessages messages = TestApiUtility.ApiPostSerializedData<SerialMediaAdmin>(
                adminService,
                url,
                @"{'mediatype':'HTML','mimetype':'.html','encoding':'utf-8','title':'Isn&#39;t this the HTML &lt;data&gt; Object &amp; for Testing Media Preferences','description':'Encode special characters &amp;&lt;&gt;&#39;','sourceUrl':'http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm','targetUrl':'http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm','rowVersion':'AAAAAAAM4Yo=','preferences':[{'type':'WebPage','isDefault':true,'htmlPreferences':{'includedElements':{'xPath':null,'elementIds':null,'classNames':['syndicate']},'excludedElements':null,'stripAnchor':null,'stripComment':null,'stripImage':null,'stripScript':true,'stripStyle':null,'newWindow':null,'imageAlign':'right','outputEncoding':null,'outputFormat':null,'contentNamespace':null},'ecardPreferences':null},{'type':'Mobile','isDefault':true,'htmlPreferences':{'includedElements':{'xPath':null,'elementIds':null,'classNames':['msyndicate']},'excludedElements':null,'stripAnchor':null,'stripComment':null,'stripImage':null,'stripScript':true,'stripStyle':null,'newWindow':null,'imageAlign':'right','outputEncoding':null,'outputFormat':null,'contentNamespace':null},'ecardPreferences':null}],'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published','datepublished':'2014-03-19T15:38:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'','maintainingOrgId':''}",
                out mediaAdmins, authorizedUser);
            Assert.AreEqual(2, messages.Errors().Count());

        }
    }
}
