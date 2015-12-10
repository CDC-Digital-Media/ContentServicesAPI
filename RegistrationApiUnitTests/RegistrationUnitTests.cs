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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.RegistrationProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationApiUnitTests
{
    [TestClass]
    public class RegistrationUnitTests
    {
        private XNamespace Namespace = "http://schemas.datacontract.org/2004/07/Gov.Hhs.Cdc.Api";

        static RegistrationUnitTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        public UserSearchDataMgr userSearchDataMgr = new UserSearchDataMgr();
        public OrganizationTypeSearchDataMgr organizationTypeSearchDataMgr = new OrganizationTypeSearchDataMgr();
        public OrganizationSearchDataMgr organizationSearchDataMgr = new OrganizationSearchDataMgr();

        IRegistrationProvider RegistrationProvider = new CsRegistrationProvider();

        [TestMethod]
        public void TestCreateUser()
        {
            RegistrationHelper registrationHelper = new RegistrationHelper();
            string emailAddress = "senderEmail15@email";

            DeleteAUserIfItExists(emailAddress);

            List<OrgTypeObject> OrganizationTypes = GetOrganizationTypes();
            List<OrganizationObject> organizations = GetOrganizations();

            Guid api_id;
            Guid.TryParse("627159ce-f537-452f-bee1-88dc609c082d", out api_id);

            int orgIdWithOneSyndicationList = 592;

            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                agreedToUsageGuidelines = false,
                password = "P@ssword1",
                passwordRepeat = "P@ssword1",
                organizations = new List<SerialUserOrgItem>() { new SerialUserOrgItem(orgIdWithOneSyndicationList) }
            };
            ActionResultsWithType<SerialLoginResults> createUserResults1 = registrationHelper.CreateUser(newUserObject);

            Assert.IsTrue(!createUserResults1.ValidationMessages.Errors().Any());
            Assert.AreEqual(1, createUserResults1.ResultObject.syndicationLists.Count());
            Guid newUserObjectGuid = Guid.Parse(createUserResults1.ResultObject.User.id);

            //Try logging in
            ActionResultsWithType<SerialLoginResults> loginActionResults = registrationHelper.Login2(newUserObject);
            Assert.IsTrue(!loginActionResults.ValidationMessages.Errors().Any());
            Assert.IsTrue(loginActionResults.ResultObject.syndicationLists.Any());
            Assert.IsFalse(loginActionResults.ResultObject.User.agreedToUsageGuidelines);

            SerialUserAgreement agreements = new SerialUserAgreement()
            {
                agreedToUsageGuidelines = true
            };
            ValidationMessages usageAgreementMessages = registrationHelper.UpdateUsageAgreements(loginActionResults.ResultObject.User.id, agreements);

            var x = usageAgreementMessages.Errors().ToList();
            Assert.IsTrue(!usageAgreementMessages.Errors().Any());


            ActionResultsWithType<SerialLoginResults> loginActionResults2 = registrationHelper.Login2(newUserObject);
            Assert.IsTrue(loginActionResults2.ResultObject.User.agreedToUsageGuidelines);

            //Clean up
            registrationHelper.DeleteAUserIfItExists(emailAddress);
            UserObject deletedObject = RegistrationProvider.GetUser(newUserObjectGuid);
            Assert.IsNull(deletedObject);
        }

        [TestMethod]
        public void CanCreateUserWithoutPassword() //for migration
        {
            RegistrationHelper registrationHelper = new RegistrationHelper();
            string emailAddress = "senderEmail15@email";

            DeleteAUserIfItExists(emailAddress);

            List<OrgTypeObject> OrganizationTypes = GetOrganizationTypes();
            List<OrganizationObject> organizations = GetOrganizations();

            Guid api_id;
            Guid.TryParse("627159ce-f537-452f-bee1-88dc609c082d", out api_id);

            TestSerialUser newUserObject = new TestSerialUser()
            {
                email = emailAddress,
                firstName = "New",
                lastName = "Testuser",
                agreedToUsageGuidelines = false,
                organizations = new List<SerialUserOrgItem>() { new SerialUserOrgItem(2) }
            };
            ActionResultsWithType<SerialLoginResults> createUserResults1 = registrationHelper.CreateUser(newUserObject);

            Assert.IsTrue(!createUserResults1.ValidationMessages.Errors().Any());
            Guid newUserObjectGuid = Guid.Parse(createUserResults1.ResultObject.User.id);

            //Try logging in
            ActionResultsWithType<SerialLoginResults> loginActionResults = registrationHelper.Login2(newUserObject);
            Assert.IsTrue(!loginActionResults.ValidationMessages.Errors().Any());
            Assert.IsTrue(loginActionResults.ResultObject.syndicationLists.Any());
            Assert.IsFalse(loginActionResults.ResultObject.User.agreedToUsageGuidelines);

            SerialUserAgreement agreements = new SerialUserAgreement()
            {
                agreedToUsageGuidelines = true
            };
            ValidationMessages usageAgreementMessages = registrationHelper.UpdateUsageAgreements(loginActionResults.ResultObject.User.id, agreements);

            var x = usageAgreementMessages.Errors().ToList();
            Assert.IsTrue(!usageAgreementMessages.Errors().Any());


            ActionResultsWithType<SerialLoginResults> loginActionResults2 = registrationHelper.Login2(newUserObject);
            Assert.IsTrue(loginActionResults2.ResultObject.User.agreedToUsageGuidelines);

            //Clean up
            registrationHelper.DeleteAUserIfItExists(emailAddress);
            UserObject deletedObject = RegistrationProvider.GetUser(newUserObjectGuid);
            Assert.IsNull(deletedObject);
        }
        private void DeleteAUserIfItExists(string emailAddress)
        {
            DataSetResult EmailAddresses = userSearchDataMgr.GetData(new SearchParameters("Registration", "User", "EmailAddress".Is(emailAddress)));
            if (EmailAddresses.RecordCount > 0)
            {
                List<UserObject> accounts = (List<UserObject>)EmailAddresses.Records;
                //Get and delete are not implemented in the API, and will be implemented in the Admin, not Storefront
                UserObject savedAccount = RegistrationProvider.GetUser(accounts[0].UserGuid);
                RegistrationProvider.DeleteUser(savedAccount);
            }
        }

        public List<OrgTypeObject> GetOrganizationTypes()
        {
            SearchParameters searchParameters = new SearchParameters("Registration", "OrgType", "DisplayOrdinal".Direction(SortOrderType.Asc));
            DataSetResult organizations = organizationTypeSearchDataMgr.GetData(searchParameters);
            return (List<OrgTypeObject>)organizations.Records;
        }

        public List<OrganizationObject> GetOrganizations()
        {
            SearchParameters searchParameters = new SearchParameters("Registration", "Organization", "Name".Direction(SortOrderType.Asc));
            DataSetResult organizations = organizationSearchDataMgr.GetData(searchParameters);
            return (List<OrganizationObject>)organizations.Records;
        }
    }
}
