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
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.Logging;

namespace Gov.Hhs.Cdc.Api
{
    public static class AuthorizationHandler
    {

        public static AdminUser Authorize(ref ValidationMessages messages, HttpContext httpContext)
        {
            var adminUser = httpContext.Request.Headers["admin_user"];

            if (string.IsNullOrEmpty(adminUser))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                messages.AddError("auth", "User header not provided.");
                return null;
            }

            var user = AuthorizationManager.GetUser(adminUser);
            if (user == null)
            {
                messages.AddError("auth", "User not authorized for the admin system.");
                return null;
            }
            return user;
        }

        public static void Get(IOutputWriter writer, string id)
        {
            IList<AdminUser> users = null;
            if (!string.IsNullOrEmpty(id))
            {
                users = new List<AdminUser> { AuthorizationManager.GetUser(id) };
            }
            else
            {
                users = AuthorizationManager.GetUsers();
            }
            var response = new SerialResponse { results = users };
            writer.Write(response);
        }

        public static void UpdateUser(IOutputWriter writer, string stream, AdminUser updatedBy)
        {
            ValidationMessages messages = new ValidationMessages();
            SerialAdminUser user = null;
            try
            {
                user = new JavaScriptSerializer().Deserialize<SerialAdminUser>(stream);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                writer.Write(ValidationMessages.CreateError("auth", "Invalid Admin User"));
                return;
            }
            var temp = new AdminUser
            {
                Roles = user.roles,
                UserName = "CDC\\" + user.userName,
                Email = user.email,
                FirstName = user.firstName,
                LastName = user.lastName
            };
            if (user.roles == null)
            {
                AuthorizationManager.AddUser(temp, updatedBy);
            }
            else
            {
                AuthorizationManager.SetRoles(temp, updatedBy);
            }

            var response = new SerialResponse(user);
            writer.Write(response, messages);

        }

        public static void WriteRoles(IOutputWriter writer)
        {
            var roles = AuthorizationManager.Roles();
            var response = new SerialResponse(roles);
            writer.Write(response);
        }

    }
}
