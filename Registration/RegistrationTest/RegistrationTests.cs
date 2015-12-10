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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcEmailProvider;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Search.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RegistrationTest
{
    [TestClass]
    public class RegistrationTests
    {
        static RegistrationTests()
        {
            DAL.Initialize();
        }

        [TestMethod]
        public void TestCreateUser()
        {
            TestEmailProvider emailProvider = new TestEmailProvider();
            CsRegistrationProvider.Inject(emailProvider);
            string emailAddress = "senderEmail1@email";
            IRegistrationProvider registrationProvider = new Gov.Hhs.Cdc.CdcRegistrationProvider.CsRegistrationProvider();

            SearchParameters accountSearchParms = new SearchParameters("Registration", "User", "EmailAddress".Is(emailAddress));
            DataSetResult EmailAddresses = DAL.SearchControllerFactory.GetSearchController(accountSearchParms).Get();
            if (EmailAddresses.RecordCount > 0)
            {
                List<UserObject> accounts = (List<UserObject>)EmailAddresses.Records;
                UserObject savedAccount = registrationProvider.GetUser(accounts[0].UserGuid);
                registrationProvider.DeleteUser(savedAccount);
            }

            List<OrgTypeObject> OrganizationTypes = DAL.GetOrganizationTypes();

            List<OrganizationObject> organizations = DAL.GetOrganizations();
            List<UserOrganizationObject> userOrgsToAdd = new List<UserOrganizationObject>();

            if (organizations != null && organizations.Count > 0)
            {
                userOrgsToAdd.Add(new UserOrganizationObject()
                {
                    OrganizationId = organizations[0].Id,
                    IsPrimary = true,
                    IsActive = true
                });
            }

            UserObject newUserObject = new UserObject()
            {
                EmailAddress = emailAddress,
                FirstName = "Test",
                LastName = "1",
                Password = "Password1",
                PasswordRepeat = "Password1",
                IsActive = true,
                Organizations = userOrgsToAdd
            };


            Guid api_id;
            Guid.TryParse("627159ce-f537-452f-bee1-88dc609c082d", out api_id);
            newUserObject.ApiClientGuid = api_id;

            GenerateUserToken(newUserObject);

            ValidationMessages messages1 = registrationProvider.SaveUser(newUserObject).ValidationMessages;
            Assert.IsTrue(!messages1.Errors().Any());
            UserObject savedObject = registrationProvider.GetUser(newUserObject.UserGuid);
            Assert.IsNotNull(savedObject);
            registrationProvider.DeleteUser(savedObject);

            UserObject deletedObject = registrationProvider.GetUser(newUserObject.UserGuid);
            Assert.IsNull(deletedObject);
            Guid a = newUserObject.UserGuid;
        }

        [TestMethod]
        public void TestCreateUserWithoutOrgsGivesAppropriateMessage()
        {
            string emailAddress = "senderEmail1@email";
            IRegistrationProvider registrationProvider = new Gov.Hhs.Cdc.CdcRegistrationProvider.CsRegistrationProvider();

            SearchParameters accountSearchParms = new SearchParameters("Registration", "User", "EmailAddress".Is(emailAddress));
            DataSetResult EmailAddresses = DAL.SearchControllerFactory.GetSearchController(accountSearchParms).Get();
            if (EmailAddresses.RecordCount > 0)
            {
                List<UserObject> accounts = (List<UserObject>)EmailAddresses.Records;
                UserObject savedAccount = registrationProvider.GetUser(accounts[0].UserGuid);
                registrationProvider.DeleteUser(savedAccount);
            }

            UserObject newUserObject = new UserObject()
           {
               EmailAddress = emailAddress,
               FirstName = "Test",
               LastName = "1",
               Password = "Password1",
               PasswordRepeat = "Password1",
               IsActive = true,
               Organizations = null
           };

            Guid api_id;
            Guid.TryParse("627159ce-f537-452f-bee1-88dc609c082d", out api_id);
            newUserObject.ApiClientGuid = api_id;

            GenerateUserToken(newUserObject);

            ValidationMessages messages1 = registrationProvider.SaveUser(newUserObject).ValidationMessages;
            Assert.IsTrue(messages1.Errors().Any());
        }


        private static void GenerateUserToken(UserObject userObj)
        {
            userObj.UserToken = CredentialManager.GenerateApiKeyTokenSalt();
            userObj.TempPassword = CredentialManager.GenerateApiKeyTokenSalt();
            userObj.ExpirationSeconds = CredentialManager.TokenExpirationInSeconds(300);
        }

        [TestMethod]
        public void TestCreateUserAndOrganization()
        {
            string emailAddress = "senderEmail1@email";
            string orgName = "New   Organization  5";
            IRegistrationProvider registrationProvider = new Gov.Hhs.Cdc.CdcRegistrationProvider.CsRegistrationProvider();

            //Delete User if it exists
            DeleteUser(emailAddress, registrationProvider);

            //Delete Organization if it exists
            DeleteOrg(orgName, registrationProvider);

            List<OrganizationObject> organizations = DAL.GetOrganizations();
            List<UserOrganizationObject> orgsToAdd = new List<UserOrganizationObject>();

            orgsToAdd.Add(new UserOrganizationObject()
            {
                IsPrimary = true,
                IsActive = true,
                Organization = new OrganizationObject()
                {
                    OrganizationTypeCode = "Commercial Organization",
                    Name = orgName,
                    Address = "Address",
                    AddressContinued = "AddressContinued",
                    City = "The City",
                    CountryId = "US",
                    StateProvinceId = "GA",
                    CountyId = "Fulton",
                    PostalCode = "12345",
                    Phone = "123 456 7890",
                    Fax = "123 456 7891",
                    Email = "sampleEmail8@email",
                    OrganizationDomains = new List<OrganizationDomainObject>(){new OrganizationDomainObject() { DomainName = "http:\\abc.efg.xyz" }, 
                        new OrganizationDomainObject() { DomainName = "http:\\abc.efg.hij" } },
                    IsActive = true,
                    GeoNameId = 4196508
                }
            });

            UserObject newUserObject = new UserObject()
            {
                EmailAddress = emailAddress,
                FirstName = "FirstName",
                LastName = "Lastname",
                Password = "Password1",
                PasswordRepeat = "Password1",
                IsActive = true,
                Organizations = orgsToAdd
            };

            SerialUser serialUser = UserTransformation.CreateSerialUser(newUserObject, null);

            Guid api_id;
            Guid.TryParse("627159ce-f537-452f-bee1-88dc609c082d", out api_id);
            newUserObject.ApiClientGuid = api_id;

            GenerateUserToken(newUserObject);
            LoginResults results1 = registrationProvider.RegisterUser(newUserObject, sendEmail: true, overriddenEmailAddress: "senderEmail1@email");
            List<ValidationMessage> errorMessages = results1.ValidationMessages.Errors().ToList();
            string theMessage = errorMessages.Count > 0 ? errorMessages[0].Message : "";
            Assert.AreEqual(0, errorMessages.Count, theMessage);

            UserObject savedObject = registrationProvider.GetUser(newUserObject.UserGuid);
            registrationProvider.DeleteUser(savedObject);

            UserObject deletedObject = registrationProvider.GetUser(newUserObject.UserGuid);

            if (newUserObject.Organizations != null && newUserObject.Organizations.Count() > 0)
            {
                foreach (UserOrganizationObject obj in newUserObject.Organizations)
                {
                    OrganizationObject orgObject = registrationProvider.GetOrganization(obj.OrganizationId);
                    Assert.IsNotNull(orgObject);
                    registrationProvider.DeleteOrganization(orgObject);
                }
            }

            Assert.IsNull(deletedObject);
            Guid a = newUserObject.UserGuid;
        }

        [TestMethod]
        public void TestRegisterUserMustHaveSamePassword()
        {
            string emailAddress = "senderEmail1@email";
            string orgName = "New   Organization  16";
            IRegistrationProvider registrationProvider = new Gov.Hhs.Cdc.CdcRegistrationProvider.CsRegistrationProvider();

            //Delete User if it exists
            DeleteUser(emailAddress, registrationProvider);

            //Delete Organization if it exists
            DeleteOrg(orgName, registrationProvider);

            List<OrganizationObject> organizations = DAL.GetOrganizations();
            List<UserOrganizationObject> orgsToAdd = new List<UserOrganizationObject>();

            orgsToAdd.Add(new UserOrganizationObject()
            {
                IsPrimary = true,
                IsActive = true,
                Organization = new OrganizationObject()
                {
                    OrganizationTypeCode = "Commercial Organization",
                    OrganizationTypeOther = "OrganizationTypeOther",
                    Name = orgName,
                    Address = "Address",
                    AddressContinued = "AddressContinued",
                    City = "City",
                    PostalCode = "12345",
                    Phone = "123 456 7890",
                    Fax = "123 456 7891",
                    Email = "sampleEmail8@email",
                    OrganizationDomains = new List<OrganizationDomainObject> { new OrganizationDomainObject { DomainName = "test.com" } },
                    IsActive = true,
                    GeoNameId = 4196508
                }
            });

            UserObject newUserObject = new UserObject()
            {
                EmailAddress = emailAddress,
                FirstName = "Test",
                LastName = "1",
                Password = "Password1",
                PasswordRepeat = "Password2",
                IsActive = true,
                Organizations = orgsToAdd
            };

            Guid api_id;
            Guid.TryParse("627159ce-f537-452f-bee1-88dc609c082d", out api_id);
            newUserObject.ApiClientGuid = api_id;

            GenerateUserToken(newUserObject);
            LoginResults results1 = registrationProvider.RegisterUser(newUserObject, sendEmail: true, overriddenEmailAddress: "senderEmail1@email");
            List<ValidationMessage> errorMessages = results1.ValidationMessages.Errors().ToList();
            string theMessage = errorMessages.Count > 0 ? errorMessages[0].Message : "";
            Assert.AreEqual(1, errorMessages.Count());
            Assert.IsTrue(errorMessages.First().Message.Contains("must match"));
        }

        [TestMethod]
        public void ResetPasswordKeyContainsOnlyOneToken()
        {
            var data = new SerialPasswordReset { email = "senderEmail1@email", passwordResetUrl = "https://localhost:56643/index.aspx?prt=yL9MR78vCzkv95p9C9dSoTBBWgl7hi0pfnvsD2SaYk" };

            Uri secureUrl;
            bool isvalid = Uri.TryCreate(data.passwordResetUrl, UriKind.Absolute, out secureUrl);

            var registrationProvider = new CsRegistrationProvider();
            var key = CsRegistrationProvider.ResetKey(secureUrl, "yL9MR78vCzkv95p9C9dSoTBBWgl7hi0pfnvsD2SaYk", "prt");
            Console.WriteLine(key);

            Assert.AreEqual(1, countOccurences("prt", key.ToString()));
        }

        private static int countOccurences(string needle, string haystack)
        {
            return (haystack.Length - haystack.Replace(needle, "").Length) / needle.Length;
        }

        private static void DeleteOrg(string orgName, IRegistrationProvider registrationProvider)
        {
            SearchParameters organizationSearchParms = new SearchParameters("Registration", "Organization", "Name".Is(orgName));
            DataSetResult prexistingOrganizations = DAL.SearchControllerFactory.GetSearchController(organizationSearchParms).Get();
            if (prexistingOrganizations.RecordCount > 0)
            {
                List<OrganizationObject> theOrgs = (List<OrganizationObject>)prexistingOrganizations.Records;
                //The org name search is a "Contains".  We need exact match
                theOrgs = theOrgs.Where(o => o.Name == orgName).ToList();
                OrganizationObject savedOrganization = registrationProvider.GetOrganization(theOrgs[0].Id);
                registrationProvider.DeleteOrganization(savedOrganization);
            }
        }

        private static void DeleteUser(string emailAddress, IRegistrationProvider registrationProvider)
        {
            SearchParameters accountSearchParms = new SearchParameters("Registration", "User", "EmailAddress".Is(emailAddress));
            DataSetResult EmailAddresses = DAL.SearchControllerFactory.GetSearchController(accountSearchParms).Get();
            if (EmailAddresses.RecordCount > 0)
            {
                List<UserObject> accounts = (List<UserObject>)EmailAddresses.Records;
                UserObject savedAccount = registrationProvider.GetUser(accounts[0].UserGuid);
                registrationProvider.DeleteUser(savedAccount);
            }
        }

        [TestMethod]
        public void MoreThan100UsersExist()
        {
            var stats = TestApiUtility.AdminApiStats();
            Assert.IsTrue(stats.storefrontUserCount > 100);
        }

    }
}
