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
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.Connection;

namespace RegistrationApiUnitTests
{
    public class RegistrationHelper
    {
        public static IObjectContextFactory RegistrationObjectContextFactory = new RegistrationObjectContextFactory();
        public UserSearchDataMgr userSearchDataMgr = new UserSearchDataMgr();
        IRegistrationProvider RegistrationProvider = new CsRegistrationProvider();
        private bool UseAPI = true;

        public RegistrationHelper()
        {
            CurrentConnection.Name = "ContentServicesDb";
        }

        static ISearchControllerFactory _searchControllerFactory;
        static ISearchControllerFactory SearchControllerFactory
        {
            get
            {
                return _searchControllerFactory ?? (_searchControllerFactory = ContentServicesDependencyBuilder.SearchControllerFactory);
            }
        }

        public void DeleteAUserIfItExists(string emailAddress)
        {
            var parm = new SearchParameters("Registration", "User", "EmailAddress".Is(emailAddress));
            DataSetResult EmailAddresses = userSearchDataMgr.GetData(parm);
            if (EmailAddresses != null && EmailAddresses.RecordCount > 0)
            {
                List<UserObject> accounts = (List<UserObject>)EmailAddresses.Records;
                //Get and delete are not implemented in the API, and will be implemented in the Admin, not Storefront
                UserObject savedAccount = RegistrationProvider.GetUser(accounts[0].UserGuid);
                ValidationMessages messages = RegistrationProvider.DeleteUser(savedAccount);
            }
        }

        public ValidationMessages DeleteOrgIfItExists(string orgName)
        {
            var parm = new SearchParameters("Registration", "Organization", "Name".Is(orgName));
            var results = SearchControllerFactory.GetSearchController(parm).Get();
            var orgs = (List<OrganizationObject>)results.Records;
            var org = orgs.Where(o => o.Name == orgName).FirstOrDefault();
            if (org == null) return null;

            var orgObj = new OrganizationObject { Id = org.Id };
            return RegistrationProvider.DeleteOrganization(orgObj);
        }

        public bool OrgExists(string name)
        {
            var parm = new SearchParameters("Registration", "Organization", "Name".Is(name));
            var results = SearchControllerFactory.GetSearchController(parm).Get();
            return results.RecordCount > 0;
        }

        public ActionResultsWithType<SerialLoginResults> CreateUser(TestSerialUser user) //, out int id)
        {
            string userUrl = String.Format("{0}://{1}/api/v2/resources/users", TestUrl.Protocol, TestUrl.PublicApiServer);
            string data = new JavaScriptSerializer().Serialize(user);
            string callResults = TestApiUtility.CallAPI(userUrl, data, "POST");
            Console.WriteLine(callResults);
            return new ActionResultsWithType<SerialLoginResults>(callResults);
        }

        public SerialValidUsers DoesEmailExist(string email)
        {
            Console.WriteLine(email);
            var url = string.Format("{0}://{1}/api/v2/resources/user_email_exists", TestUrl.Protocol, TestUrl.PublicApiServer);
            var data = new JavaScriptSerializer().Serialize(new SerialPasswordReset { email = email });
            var results = TestApiUtility.CallAPI(url, data, "POST");
            return new ActionResultsWithType<SerialValidUsers>(results).ResultObject;
        }

        public TestSerialUser MigratedUser()
        {
            var provider = new CsRegistrationProvider();
            var user = provider.GetUsers().Where(u => u.IsMigrated && u.Organizations.Count() == 0).FirstOrDefault();
            if (user == null) return null;
            return new TestSerialUser()
            {
                email = user.EmailAddress,
                firstName = user.FirstName,
                lastName = user.LastName,
                password = user.Password,
                passwordRepeat = user.Password,
                agreedToUsageGuidelines = user.AgreedToUsageGuidelines,
                id = user.UserGuid.ToString(),
                organizations = user.Organizations.ToList().Select(u => UserOrganizationTransformation.CreateSerialUserOrgItem(u)).ToList()
            };
        }

        public ActionResultsWithType<SerialLoginResults> UpdateUser(SerialUser user)
        {
            var url = string.Format("{0}://{1}/api/v2/resources/users/{2}", TestUrl.Protocol, TestUrl.PublicApiServer, user.id);
            var data = new JavaScriptSerializer().Serialize(user);
            var results = TestApiUtility.CallAPI(url, data, "PUT");
            return new ActionResultsWithType<SerialLoginResults>(results);
        }

        public ActionResults Login(TestSerialUser user)
        {
            string jsonString = new JavaScriptSerializer().Serialize(user);
            if (UseAPI)
            {
                string url = String.Format("{0}://{1}/api/v2/resources/logins", TestUrl.Protocol, TestUrl.PublicApiServer);
                string callResults = TestApiUtility.CallAPI(url, jsonString, "POST");
                ActionResults results = new ActionResults(callResults);

                Dictionary<string, object> resultUser = (Dictionary<string, object>)results.Results;
                return results;
            }
            else
                return RegistrationHandler.Login(jsonString, TestApiUtility.ConsumerKey);
        }

        public ActionResultsWithType<SerialLoginResults> Login2(TestSerialUser user)
        {

            string jsonString = new JavaScriptSerializer().Serialize(user);

            string url = String.Format("{0}://{1}/api/v2/resources/logins", TestUrl.Protocol, TestUrl.PublicApiServer);
            string callResults = TestApiUtility.CallAPI(url, jsonString, "POST");
            return new ActionResultsWithType<SerialLoginResults>(callResults);
        }

        public ActionResultsWithType<SerialLoginResults> UpdateUser(TestSerialUser user)
        {
            string jsonString = new JavaScriptSerializer().Serialize(user);
            Console.WriteLine(jsonString);
            string url = String.Format("{0}://{1}/api/v2/resources/users/{2}", TestUrl.Protocol, TestUrl.PublicApiServer, user.id);
            Console.WriteLine(url);
            string callResults = TestApiUtility.CallAPI(url, jsonString, "PUT");
            Console.WriteLine(callResults);
            return new ActionResultsWithType<SerialLoginResults>(callResults);
        }

        public ValidationMessages UpdateUsageAgreements(string id, SerialUserAgreement userAgreements)
        {

            string jsonString = new JavaScriptSerializer().Serialize(userAgreements);

            string url = String.Format("{0}://{1}/api/v2/resources/users/{2}/usageagreements", TestUrl.Protocol, TestUrl.PublicApiServer, id);
            string callResults = TestApiUtility.CallAPI(url, jsonString, "PUT");
            return new ActionResults(callResults).ValidationMessages;
        }

        public static ActionResults GetActionResults(string jsonString)
        {
            return new ActionResults(jsonString);
        }

        public class SerialLoginResponse
        {
            public SerialLoginResponse() { }

            public SerialMeta meta { get; set; }
            public SerialLoginResults results { get; set; }
        }
    }
}
