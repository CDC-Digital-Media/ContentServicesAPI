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
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class AdminUserInfoTests
    {
        static AdminUserInfoTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        private AdminUser authorizedUser = new AdminUser
        {
            UserName = "",
            UserGuid = Guid.Parse("c5423f7c-0ca4-4e63-8167-658701caeccf")
        };

        [TestMethod]
        public void CanRetrieveName()
        {
            var user = AuthorizationManager.GetUser(authorizedUser.UserName);

            string name = user.Name;
            Console.WriteLine(name);
            Assert.IsFalse(string.IsNullOrEmpty(name));
        }

        [TestMethod]
        public void CanRetrieveGuid()
        {
            var guid = AuthorizationManager.GetUser(authorizedUser.UserName).UserGuid;
            Assert.IsNotNull(guid);
            Assert.AreEqual(authorizedUser.UserGuid, guid);
        }

        [TestMethod]
        public void CanRetrieveRoles()
        {
            var user = AuthorizationManager.GetUser(authorizedUser.UserName);
            Assert.IsTrue(user.Roles.ToList().Count() > 0);
        }

        [TestMethod]
        public void TestRetrieveAdminUsers()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/adminusers", TestUrl.Protocol, TestUrl.AdminApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialAdminUserResults>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestAdminUsersExist()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/adminusers", TestUrl.Protocol, TestUrl.AdminApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialAdminUserResults>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }
            Assert.IsTrue(obj.results.Count() > 0);
        }

        [TestMethod]
        public void TestRetrieveNameViaApi()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/adminusers", TestUrl.Protocol, TestUrl.AdminApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialAdminUserResults>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }

            Assert.IsFalse(string.IsNullOrEmpty(obj.results.First().name));
        }

        [TestMethod]
        public void TestRetrieveSingleUserViaApi()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/adminusers/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, authorizedUser.UserName);
            Console.WriteLine(url);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialAdminUserResults>(callResults);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }
            Assert.AreEqual(1, obj.results.Count());
            Assert.IsFalse(string.IsNullOrEmpty(obj.results.First().email));
        }

        [TestMethod]
        public void TestRetrieveRolesViaApi()
        {
            string url = String.Format("{0}://{1}/adminapi/v1/resources/adminusers", TestUrl.Protocol, TestUrl.AdminApiServer);
            string callResults = TestApiUtility.Get(url);
            Console.WriteLine(callResults);
            var obj = new JavaScriptSerializer().Deserialize<SerialAdminUserResults>(callResults);
            Assert.IsNotNull(obj);
            if (obj.meta.status != 200)
            {
                Console.WriteLine(obj.meta.message[0].userMessage);
                Console.WriteLine(obj.meta.message[0].developerMessage);
                Assert.Fail();
            }
            Assert.IsTrue(obj.results.Where(u => u.firstName == "Nina").Count() > 0);
            Assert.IsTrue(obj.results.Where(u => u.firstName == "Nina").First().roles.Count > 0);
        }

        [TestMethod]
        public void TestUserWithoutMediaAdminRoleCannotEditMedia()
        {
            var auth = new AuthorizationManager();
            var user = new AdminUser { Roles = new List<string> { "test role" }.AsQueryable() };
            int mediaId = -1;
            Assert.IsFalse(user.CanEditMedia(mediaId));
        }

        [TestMethod]
        public void TestUserWithMediaAdminRoleCanEditMedia()
        {
            var auth = new AuthorizationManager();
            var user = new AdminUser { Roles = new List<string> { "Media Admin" }.AsQueryable() };
            int mediaId = -1;
            Assert.IsTrue(user.CanEditMedia(mediaId));
        }

        [TestMethod]
        public void TestSpikeJoin()
        {
            var users = new List<user> { new user { id = 1, name = "user 1" }, new user { id = 2, name = "user 2" } };
            var roles = new List<role> { new role { id = 1, name = "media admin" }, new role { id = 2, name = "media admin" } };

            //returns 2 copies of each role instead of 1
            //var combined = users
            //    .Join(roles, au => au.id, r => r.id, (au, r) => new { user = au, rs = roles })
            //    .Select(o => new
            //    {
            //        id = o.user.id,
            //        name = o.user.name,
            //        roles = o.rs.Select(r => r.name)
            //    });

            var combined = users.Select(o => new
                {
                    id = o.id,
                    name = o.name,
                    roles = roles.Where(r => r.id == o.id).Select(r => r.name)
                });

            Assert.AreEqual(1, combined.First().roles.Count());
        }

        private class role
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        private class user
        {
            public int id { get; set; }
            public string name { get; set; }
        }

    }
}
