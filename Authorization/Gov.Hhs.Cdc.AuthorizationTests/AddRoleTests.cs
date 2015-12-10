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
using Gov.Hhs.Cdc.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class AddRoleTests
    {
        public const string userWithoutFeedsRole = "";

        [TestInitialize]
        public void TestSetup()
        {
            var user = AuthorizationManager.GetUser(userWithoutFeedsRole);
            var roles = new List<string> { AuthorizationManager.MediaAdminRole, AuthorizationManager.SystemAdminRole };
            user.Roles = roles;
            AuthorizationManager.SetRoles(user, user);
        }

        [TestMethod]
        public void CanAddRole()
        {
            var user = AuthorizationManager.GetUser(userWithoutFeedsRole);

            Assert.IsFalse(user.Roles.Contains(AuthorizationManager.FeedsSchedulerRole));

            var roles = user.Roles.ToList();
            roles.Add(AuthorizationManager.FeedsSchedulerRole);
            Assert.AreEqual(3, roles.Count);

            user.Roles = roles;
            AuthorizationManager.SetRoles(user, user);
            user = AuthorizationManager.GetUser(userWithoutFeedsRole);
            Assert.IsTrue(user.Roles.Contains(AuthorizationManager.FeedsSchedulerRole));
        }

        [TestMethod]
        public void CanAddRoleViaApi()
        {
            var user = AuthorizationManager.GetUser(userWithoutFeedsRole);
            Assert.IsFalse(user.Roles.Contains(AuthorizationManager.FeedsSchedulerRole));

            var adminService = new AdminApiServiceFactory();
            var url = adminService.CreateTestUrl("adminusers");
            Console.WriteLine(url);

            var data = "{\"userName\":\"\",\"roles\":[\"Media Admin\",\"System Admin\", \"Feeds Scheduler Admin\"]}";
            var result = TestApiUtility.CallAPIPost(url.ToString(), data, user.UserName);
            Console.WriteLine(result);

            user = AuthorizationManager.GetUser(userWithoutFeedsRole);
            Assert.IsTrue(user.Roles.Contains(AuthorizationManager.FeedsSchedulerRole));
 
        }
    }
}
