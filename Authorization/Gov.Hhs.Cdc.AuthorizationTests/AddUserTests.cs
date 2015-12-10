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
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class AddUserTests
    {
        AdminUser unauthorizedUser = new AdminUser { UserName = "vne1", FirstName = "Sheila", LastName = "Brewton", Email = "vne1@email" };
        public const string addedBy = "";

        [TestInitialize]
        public void Init()
        {
            var userBeforeAdd = AuthorizationManager.GetUser(unauthorizedUser.UserName);

            if (userBeforeAdd != null)
            {
                Console.WriteLine("removing " + userBeforeAdd.UserGuid);
                AuthorizationManager.RemoveUser(userBeforeAdd);
            }
        }

        [TestMethod]
        public void CanAddNewAdminUser()
        {
            var userBeforeAdd = AuthorizationManager.GetUser(unauthorizedUser.UserName);

            if (userBeforeAdd != null)
            {
                Assert.Fail("User " + userBeforeAdd.UserGuid + " already exists.");
            }

            AuthorizationManager.AddUser(unauthorizedUser, AuthorizationManager.GetUser(addedBy));

            var userAfterAdd = AuthorizationManager.GetUser(unauthorizedUser.UserName);
            Assert.IsNotNull(userAfterAdd);
            Assert.IsNotNull(userAfterAdd.UserGuid);
            Assert.AreEqual(unauthorizedUser.Email, userAfterAdd.Email);
            Assert.AreEqual(unauthorizedUser.FirstName, userAfterAdd.FirstName);
        }


        [TestMethod]
        public void CanAddNewAdminUserViaApi()
        {
            var userBeforeAdd = AuthorizationManager.GetUser(unauthorizedUser.UserName);

            Assert.IsNull(userBeforeAdd);

            string url = String.Format("{0}://{1}/adminapi/v1/resources/adminusers", TestUrl.Protocol, TestUrl.AdminApiServer);
            //var serial = new SerialAdminUser
            //{
            //    userName = unauthorizedUser.UserName,
            //    firstName = unauthorizedUser.FirstName,
            //    lastName = unauthorizedUser.LastName,
            //    email = unauthorizedUser.Email
            //};
            //string json = new JavaScriptSerializer().Serialize(serial);
            string json = "{ \"userName\" : \"vne1\", \"email\" : \"vne1@email\", \"firstName\" : \"test\", \"lastName\" : \"test\" }";
            Console.WriteLine(url);
            Console.WriteLine(json);
            var response = TestApiUtility.CallAPIPost(url, json, addedBy);
            Console.WriteLine(response);
            Assert.IsFalse(string.IsNullOrEmpty(response), response);

            var userAfterAdd = AuthorizationManager.GetUser(unauthorizedUser.UserName);
            Assert.IsNotNull(userAfterAdd, response);
        }
    }
}
