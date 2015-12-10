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
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;
using System.Data.Objects;

namespace Gov.Hhs.Cdc.Authorization
{
    public class AuthorizationManager
    {
        public const string MediaAdminRole = "Media Admin";
        public const string VocabAdminRole = "Vocabulary Admin";
        public const string SystemAdminRole = "System Admin";
        public const string FeedsSchedulerRole = "Feeds Scheduler Admin";

        public static AdminUser GetUser(string user)
        {
            //user = user.Replace('|', '\\');
            if (!user.StartsWith("CDC\\"))
            {
                user = "CDC\\" + user;
            }
            var users = GetUsers();

            var currentUser = users.Where(au => au.UserName.ToLower() == user.ToLower()).FirstOrDefault();
            if (currentUser == null)
            {
                Logger.LogError("admin auth failure for " + user);
                return null;
            }
            return currentUser;
        }

        public static IList<AdminUser> GetUsers()
        {
            using (var objectContext = AuthorizationObjectContextFactory.Create() as AuthorizationObjectContext)
            {
                return AdminUserCtl.GetUsers(objectContext).ToList();
            }
        }

        public static List<SerialAdminRole> Roles()
        {
            using (var objectContext = AuthorizationObjectContextFactory.Create() as AuthorizationObjectContext)
            {
                var roles = AdminUserCtl.Roles(objectContext);
                var query = roles as ObjectQuery;
                Console.WriteLine(query.ToTraceString());
                return RemoveNullRoleMembers(roles.ToList()).ToList();
            }
        }

        private static IList<SerialAdminRole> RemoveNullRoleMembers(IList<SerialAdminRole> roles){
            foreach (var role in roles)
            {
                if (role.members.Count() == 1)
                {
                    var member = role.members.First();
                    if (member.UserGuid == Guid.Empty && member.UserName == null)
                    {
                        role.members = null;
                    }
                }
            }

            return roles;
        }

        public static IList<int> AllowedMediaIds(AdminUser user)
        {
            using (var objectContext = AuthorizationObjectContextFactory.Create() as AuthorizationObjectContext)
            {
                return AdminUserCtl.AllowedMediaIds(objectContext, user);
            }
        }

        public static void SetRoles(AdminUser user, AdminUser addedBy)
        {
            using (var context = AuthorizationObjectContextFactory.Create() as AuthorizationObjectContext)
            {
                AdminUserCtl.SetUserRoles(context, user, user.Roles.ToList(), addedBy);
            }
        }

        public static void AddUser(AdminUser user, AdminUser addedBy)
        {
            using (var context = AuthorizationObjectContextFactory.Create() as AuthorizationObjectContext)
            {
                AdminUserCtl.AddUser(context, user, addedBy);
            }
        }

        public static void RemoveUser(AdminUser user)
        {
            using (var context = AuthorizationObjectContextFactory.Create() as AuthorizationObjectContext)
            {
                AdminUserCtl.RemoveUser(context, user);
            }
        }
    }
}
