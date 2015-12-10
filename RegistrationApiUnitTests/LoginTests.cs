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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationApiUnitTests
{
    [TestClass]
    public class LoginTests
    {
        RegistrationHelper helper = new RegistrationHelper();

        static LoginTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void ExistingLoginIncludesWebsite()
        {
            TestSerialUser testUser = CreateUser();

            helper.DeleteAUserIfItExists(testUser.email);
            helper.CreateUser(testUser);

            var loginResults = helper.Login2(testUser);

            Assert.IsNotNull(loginResults.ResultObject.User.organizations.First().website);

            helper.DeleteAUserIfItExists(testUser.email);
        }

        [TestMethod]
        public void ExistingLoginIncludesUsageGuidelines()
        {
            TestSerialUser testUser = CreateUser();

            helper.DeleteAUserIfItExists(testUser.email);
            helper.CreateUser(testUser);

            var loginResults = helper.Login2(testUser);

            Assert.IsNotNull(loginResults.ResultObject);
            Assert.IsTrue(loginResults.ResultObject.User.agreedToUsageGuidelines);
            helper.DeleteAUserIfItExists(testUser.email);
        }

        [TestMethod]
        public void MigratedUserWithNewOrgCreatesSyndicationList()
        {
            var user = helper.MigratedUser();

            //TODO:  Create migrated user
            /*
             * 
             */

            if (user == null) return;

            Assert.IsNotNull(user, "No unassigned migrated users");
            Assert.IsNotNull(user.id);
            Console.WriteLine(user.id);
            Assert.AreEqual(0, user.organizations.Count);
            var randomOrgName = new Random().Next().ToString();
            var ticks = DateTime.Now.Ticks;
            user.organizations.Add(new SerialUserOrgItem
            {
                name = randomOrgName,
                type = "Application Developer",
                country = "United States",
                stateProvince = "Florida",
                county = "Duval County",
                geoNameId = 4153840,
                website = new List<SerialUserOrgDomain> { new SerialUserOrgDomain 
                                                           {  url = "http://www." + randomOrgName + ticks + ".com"
                                                               , isDefault = true } }
            });
            //PUT with brand new org
            //{"url":"https://.....[devApiServer]...../api/v2/resources/users/8a927627-55b8-e411-a324-0017a477681a",
            //"data":"{\"id\":\"8a927627-55b8-e411-a324-0017a477681a\",\"firstName\":\"Clayton\",\"middleName\":\"H\",\"lastName\":\"Davis\",\
            //"email\":\"senderEmail3@email\",\"agreedToUsageGuidelines\":false,\"organizations\":[{\"name\":\"2191135\",\"type\":\"Application Developer\"
            //,\"country\":\"United States\",\"stateProvince\":\"Florida\",\"county\":\"Duval County\",\"geoNameId\":4153840,
            //\"website\":[{\"url\":\"http://www.2191135.com\",\"isDefault\":true}]}],\"password\":null,\"passwordRepeat\":null,\"active\":false}",
            //"httpMethod":"PUT"}
            //syndicationLists is [] in the response.
            var result = helper.UpdateUser(user);
            Assert.IsNotNull(result.ResultObject.syndicationLists);
            Assert.AreEqual(1, result.ResultObject.syndicationLists.Count);
        }

        public TestSerialUser CreateUser()
        {
            int orgId = 2;
            return new TestSerialUser()
            {
                email = "test@email",
                firstName = "New",
                lastName = "Testuser",
                password = "Password1",
                passwordRepeat = "Password1",
                organizations = new List<SerialUserOrgItem>() { new SerialUserOrgItem(orgId) },
                agreedToUsageGuidelines = true
            };
        }

    }
}
