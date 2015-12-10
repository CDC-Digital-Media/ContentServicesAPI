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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Gov.Hhs.Cdc.Media.Bo;
//using Gov.Hhs.Cdc.Media.Bll;
//using Gov.Hhs.Cdc.DataServices.Bo;
//using Gov.Hhs.Cdc.DataSource.Media;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.UserProvider;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace UserTests
{
    [TestClass]
    public class UserTests
    {
        static SearchControllerFactory SearchControllerFactory = new SearchControllerFactory();

        public List<OrganizationItem> GetOrganizations()
        {
            SearchParameters searchParameters = new SearchParameters()
            {
                ApplicationCode = "Media", DataSetCode = "Organization",
                Sorting = new Sorting(new SortColumn("Name", SortOrderType.Asc))
            };

            DataSetResult organizations = SearchControllerFactory.GetSearchController(searchParameters).Get();
            return (List<OrganizationItem>)organizations.Records;
        }

        [TestMethod]
        public void TestCreateUser()
        {
            IUserProvider userProvider = new Gov.Hhs.Cdc.CdcUserProvider.UserProvider();

            List<OrganizationItem> organizations = GetOrganizations();
            UserObject newUserObject = new UserObject()
            {
                EMailAddress = "sampleEmail6@email",
                FirstName = "Test",
                LastName = "1",
                OrganizationId = organizations[0].Id,
                Password = "Password1",
                PasswordRepeat = "Password1",
                PasswordSalt = "Ye Ole Salt",
                IsActive = true
            };
            

            ValidationMessages messages1 = userProvider.SaveUser(newUserObject);
            Assert.IsTrue(!messages1.Errors().Any());
            UserObject savedObject = userProvider.GetUser(newUserObject.UserGuid);
            Assert.IsNotNull(savedObject);
            userProvider.DeleteUser(savedObject);

            UserObject deletedObject = userProvider.GetUser(newUserObject.UserGuid);
            Assert.IsNull(deletedObject);
            Guid a = newUserObject.UserGuid;
        }

        [TestMethod]
        public void TestCreateUserAndOrganization()
        {
            IUserProvider userProvider = new Gov.Hhs.Cdc.CdcUserProvider.UserProvider();
            List<OrganizationItem> organizations = GetOrganizations();
            UserObject newUserObject = new UserObject()
            {
                EMailAddress = "sampleEmail7@email",
                FirstName = "Test",
                LastName = "1",
                Password = "Password1",
                PasswordRepeat = "Password1",
                PasswordSalt = "Ye Ole Salt",
                IsActive = true,
                Organization = new OrganizationObject()
                {
                    ParentId = organizations[0].Id,
                    OrganizationTypeCode = "Commercial Organization",
                    OrganizationTypeOther = "OrganizationTypeOther",
                    Name = "New   Organization  1",
                    Address = "Address",
                    AddressContinued = "AddressContinued",
                    City = "City",
                    PostalCode = "12345",
                    Phone = "123 456 7890",
                    Fax = "123 456 7891",
                    Email = "sampleEmail8@email",
                    WebSite = "www......[domain].....",
                    IsActive = true
                }
            };
            ValidationMessages messages1 = userProvider.SaveUser(newUserObject);
            Assert.IsTrue(messages1.Errors().Any());

            //UserObject savedObject = UserObjectMgr.Get(newUserObject.UserGuid);
            //UserObjectMgr.Delete(savedObject);

            //UserObject deletedObject = UserObjectMgr.Get(newUserObject.UserGuid);
            //Assert.IsNull(deletedObject);
            Guid a = newUserObject.UserGuid;
        }

    }
}
