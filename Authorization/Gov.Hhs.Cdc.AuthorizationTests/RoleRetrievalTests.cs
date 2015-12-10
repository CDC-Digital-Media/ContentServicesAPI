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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.Authorization;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class RoleRetrievalTests
    {
        private List<SerialAdminRole> RolesFromHandler()
        {
            var writer = new TestOutputWriter();
            AuthorizationHandler.WriteRoles(writer);
            var obj = (SerialResponse)writer.TheObject;
            return (List<SerialAdminRole>)obj.results;
        }

        private List<SerialAdminRole> RolesFromApi()
        {
            var adminService = new AdminApiServiceFactory();
            var url = adminService.CreateTestUrl("roles");
            List<SerialAdminRole> roles = new List<SerialAdminRole>();
            TestApiUtility.ApiGet<SerialAdminRole>(adminService, url, out roles);
            return roles;
        }

        [TestMethod]
        public void CanRetrieveRolesViaApi()
        {
            var roles = RolesFromApi();
            Assert.IsTrue(roles.Count > 4);
        }

        [TestMethod]
        public void CanRetrieveRolesDirectly()
        {
            var roles = RolesFromHandler();
            Assert.IsTrue(roles.Count > 4);
        }

        [TestMethod]
        public void RolesFromHandlerMatchRolesFromApi()
        {
            var r1 = RolesFromApi().Select(r => new { r.name }).ToList();
            var r2 = RolesFromHandler().Select(r => new { r.name }).ToList();
            CollectionAssert.AreEqual(r1, r2);
        }

        [TestMethod]
        public void VocabRoleIsRetrieved()
        {
            Assert.IsTrue(RolesFromHandler().Any(r => r.name == AuthorizationManager.VocabAdminRole));
        }

        [TestMethod]
        public void MediaAdminRoleIsRetrieved()
        {
            Assert.IsTrue(RolesFromHandler().Any(r => r.name == AuthorizationManager.MediaAdminRole));
        }

        [TestMethod]
        public void SystemAdminRoleIsRetrieved()
        {
            Assert.IsTrue(RolesFromHandler().Any(r => r.name == AuthorizationManager.SystemAdminRole));
        }

        [TestMethod]
        public void FeedsSchedulerRoleIsRetrieved()
        {
            Assert.IsTrue(RolesFromHandler().Any(r => r.name == AuthorizationManager.FeedsSchedulerRole));
        }

        [TestMethod]
        public void VocabRoleIncludesHasMembers()
        {
            var vocabRole = RolesFromHandler().FirstOrDefault(r => r.name == AuthorizationManager.VocabAdminRole);
            Assert.IsNotNull(vocabRole);
            Assert.IsNotNull(vocabRole.members);
            Assert.IsTrue(vocabRole.members.Count() > 0);
        }
    }
}
