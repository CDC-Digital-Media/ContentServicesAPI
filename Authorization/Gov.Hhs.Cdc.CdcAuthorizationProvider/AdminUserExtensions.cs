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

using System.Linq;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;

namespace Gov.Hhs.Cdc.Authorization
{
    public static class AdminUserExtensions
    {
        public static bool CanEditMedia(this AdminUser user, int mediaId)
        {
            if (user == null || user.Roles == null) { return false; }
            var roles = user.Roles.ToList();
            if (roles.Contains(AuthorizationManager.SystemAdminRole)
                || roles.Contains(AuthorizationManager.FeedsAdminRole)
                || roles.Contains(AuthorizationManager.StorefrontManagerRole)
                || roles.Contains(AuthorizationManager.MediaAdminRole))
            {
                return true;
            }

            //TODO:  Add mediaId check
            return false;
        }

        public static bool CanDeleteMedia(this AdminUser user)
        {
            if (user == null || user.Roles == null) { return false; }
            var roles = user.Roles.ToList();
            return roles.Contains(AuthorizationManager.SystemAdminRole);
        }

        public static bool CanEditVocabulary(this AdminUser user)
        {
            if (user == null || user.Roles == null) { return false; }
            var roles = user.Roles.ToList();
            if (roles.Contains(AuthorizationManager.SystemAdminRole)) { return true; }
            return roles.Contains(AuthorizationManager.VocabAdminRole);
        }

        public static bool CanScheduleFeeds(this AdminUser user)
        {
            var roles = user.Roles.ToList();
            if (roles.Contains(AuthorizationManager.SystemAdminRole)) { return true; }
            return roles.Contains(AuthorizationManager.FeedsSchedulerRole);
        }

        public static bool CanAdminData(this AdminUser user)
        {
            if (user == null || user.Roles == null) { return false; }
            var roles = user.Roles.ToList();
            return roles.Contains(AuthorizationManager.SystemAdminRole);
        }

        public static bool CanAdminAuthorization(this AdminUser user)
        {
            if (user == null || user.Roles == null) { return false; }
            var roles = user.Roles.ToList();
            return roles.Contains(AuthorizationManager.SystemAdminRole);
        }
    }
}
