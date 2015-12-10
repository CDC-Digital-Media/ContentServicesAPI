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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;

namespace RegistrationTest
{

    [TestClass]
    public class ServiceUserTests
    {
        public ServiceUserTests()
        {
            DAL.Initialize();
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void AddServiceUser()
        {
            var serviceUser = CreateServiceUser("Password1");
            ValidationMessages messages = RegistrationHandler.Create(serviceUser);

            Assert.IsFalse(messages.Errors().Any());
        }

        [TestMethod]
        public void RemoveServiceUser()
        {
            var serviceUser = CreateServiceUser("Password1");
            ValidationMessages messages = RegistrationHandler.DeleteServiceUserByEmail(serviceUser.email);

            Assert.IsFalse(messages.Errors().Any());
        }

        private SerialServiceUser CreateServiceUser(string password)
        {
            return new SerialServiceUser()
            {
                email = "test@email.com",
                firstName = "Test",
                middleName = "M",
                lastName = "Account",
                password = password,
                passwordRepeat = password,
                organization = CreateSerialUserOrgItem(),
                active = true
            };
        }

        private SerialUserOrgItem CreateSerialUserOrgItem()
        {
            int orgId = 0;
            List<OrganizationObject> organizations = DAL.GetOrganizations();            
            if (organizations != null && organizations.Count > 0)            
                orgId = organizations[0].Id;
            
            return new SerialUserOrgItem()
            {
                id = orgId,
                @default = true                
            };
        }
    }
}
