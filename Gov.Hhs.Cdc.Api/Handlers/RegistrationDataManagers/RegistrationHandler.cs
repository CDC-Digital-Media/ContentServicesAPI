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
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.Api
{
    public static class RegistrationHandler
    {
        private static IRegistrationProvider _RegistrationProvider = null;
        public static IRegistrationProvider RegistrationProvider
        {
            get
            {
                return _RegistrationProvider ?? (_RegistrationProvider = new CsRegistrationProvider());
            }
        }


        public static ActionResults CreateUser(string stream, string appKey)
        {
            SerialUser user = new JavaScriptSerializer().Deserialize<SerialUser>(stream);

            user.active = true;

            UserObject userObj = UserTransformation.CreateUserObject(user);
            userObj.ApiKey = appKey;

            LoginResults loginResults = RegistrationProvider.RegisterUser(userObj, sendEmail: true);

            if (loginResults.ValidationMessages.Errors().Any())
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new ActionResults(
                    new SerialLoginResults(loginResults),
                    loginResults.ValidationMessages
                    );
            }

            return new ActionResults(
                new SerialLoginResults(loginResults),
                loginResults.ValidationMessages,
                GetUserHeaderValues(loginResults.UserToken)
                );
        }

        public static void UpdateUserPassword(string stream, string apiKey, IOutputWriter writer)
        {
            ValidationMessages messages = new ValidationMessages();
            SerialPasswordReset passwordObj = new JavaScriptSerializer().Deserialize<SerialPasswordReset>(stream);

            messages = RegistrationProvider.UpdateUserPassword(passwordObj.email, passwordObj.passwordToken, passwordObj.newPassword, passwordObj.newPasswordRepeat, apiKey);

            writer.Write(messages);
        }

        public static void ResetUserPassword(string stream, string appKey, IOutputWriter writer)
        {
            ValidationMessages messages = new ValidationMessages();
            SerialPasswordReset passObj = new JavaScriptSerializer().Deserialize<SerialPasswordReset>(stream);
            if (passObj == null)
            {
                throw new InvalidOperationException(stream + " is not valid.");
            }

            Uri secureUrl;
            bool isvalid = Uri.TryCreate(passObj.passwordResetUrl, UriKind.Absolute, out secureUrl);

            if (isvalid)
            {
                if (secureUrl.Scheme != Uri.UriSchemeHttps)
                {
                    messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "ResetPassword", "A secure passwordResetUrl is required"));
                }
            }
            else
            {
                messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "ResetPassword", "Invalid passwordResetUrl"));
            }

            if (messages.Messages.Count == 0 ||
                !messages.Messages.Where(a => a.Severity == ValidationMessage.ValidationSeverity.Error).Any())
            {
                messages.Add(RegistrationProvider.ResetUserPassword(passObj.email, secureUrl, appKey));
            }

            writer.Write(messages);
        }


        public static void ValidateUsers(string stream, string appKey, IOutputWriter writer)
        {
            var data = new JavaScriptSerializer().Deserialize<SerialPasswordReset>(stream);
            bool result = RegistrationProvider.ValidateUsers(data.email, appKey);

            writer.Write(new SerialResponse(new SerialValidUsers() { valid = result }));
        }

        private static Guid GetUserGuid(string userGuidString, ValidationMessages messages)
        {
            Guid userGuid;
            if (!Guid.TryParse(userGuidString, out userGuid))
            {
                messages.AddError("Id", "Invalid User Guid");
            }
            return userGuid;
        }

        public static ValidationMessages Create(SerialServiceUser userObj)
        {
            return RegistrationProvider.SaveUser(ServiceUserTransformation.CreateServiceUserObject(userObj));
        }

        public static ValidationMessages DeleteServiceUserByEmail(string emailAddress)
        {
            return RegistrationProvider.DeleteServiceUserByEmail(emailAddress);
        }

        public static ActionResults Update(SerialUser userObj)
        {
            LoginResults loginResults = RegistrationProvider.SaveUser(UserTransformation.CreateUserObject(userObj));

            var results = new ActionResults(
                new SerialLoginResults(loginResults),
                loginResults.ValidationMessages,
                GetUserHeaderValues(loginResults.UserToken)
                );

            return results;
        }

        public static ValidationMessages Delete(string id)
        {
            return RegistrationProvider.DeleteUser(UserObject.CreateByGuid(id));
        }

        public static ActionResults CreateServiceUser(string stream)
        {
            SerialServiceUser user = new JavaScriptSerializer().Deserialize<SerialServiceUser>(stream);
            ValidationMessages messages = RegistrationHandler.Create(user);
            return new ActionResults(
                messages.Errors().Any() ? null : new SerialServiceUser(user), messages);
        }

        public static ActionResults Login(string stream, string appKey)
        {
            SerialUser serialUser = new JavaScriptSerializer().Deserialize<SerialUser>(stream);
            UserObject user = new UserObject(serialUser.email, serialUser.password);
            LoginResults loginResults = RegistrationProvider.Login(user, appKey);

            if (loginResults.ValidationMessages.Errors().Any())
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new ActionResults(null, loginResults.ValidationMessages);
            }
            var results = new ActionResults(
                new SerialLoginResults(loginResults),
                loginResults.ValidationMessages,
                GetUserHeaderValues(loginResults.UserToken)
                );

            return results;
        }

        public static ActionResults SaveApiClient(SerialApiClient theObject, string apiKey)
        {
            ValidationMessages msgs = new ValidationMessages();

            var obj = ApiClientTransformation.CreateApiClientObject(theObject);
            msgs.Add(RegistrationProvider.SaveApiClient(obj, apiKey));

            //update apiClientGuid
            theObject.apiClientGuid = obj.ApiClientGuid.ToString();

            return new ActionResults(
                new SerialApiClientCredential()
                {
                    appKey = obj.ApplicationKey,
                    secret = obj.Secret,
                    //token = obj.Token,
                    apiKey = obj.ApiKey
                }, msgs);
        }

        public static ActionResults DeleteApiClient(SerialApiClient theObject, string apiKey)
        {
            ValidationMessages msgs = new ValidationMessages();

            var obj = ApiClientTransformation.CreateApiClientObject(theObject);
            msgs.Add(RegistrationProvider.DeleteApiClient(obj, apiKey));

            return new ActionResults(
                new SerialApiClientCredential()
                {
                    appKey = obj.ApplicationKey,
                    secret = obj.Secret,
                    //token = obj.Token
                }, msgs);
        }

        public static ActionResults UpdateApiClientApiKey(SerialApiClientCredential theObject)
        {
            ValidationMessages msgs = new ValidationMessages();

            var obj = ApiClientTransformation.CreateApiClientObject(theObject);
            msgs.Add(RegistrationProvider.UpdateApiClientApiKey(obj));

            //update apiClientGuid
            theObject.apiKey = obj.ApiKey;

            return new ActionResults(
                new SerialApiClientCredential()
                {
                    appKey = obj.ApplicationKey,
                    secret = obj.Secret,
                    //token = obj.Token,
                    apiKey = obj.ApiKey
                }, msgs);
        }

        public static ActionResults SetUserAgreement(string stream, string userGuidstring)
        {
            SerialUserAgreement serialUser = new JavaScriptSerializer().Deserialize<SerialUserAgreement>(stream);
            ValidationMessages messages = new ValidationMessages();
            Guid userGuid = GetUserGuid(userGuidstring, messages);

            LoginResults loginResults = RegistrationProvider.SetUserAgreement(userGuid, serialUser.agreedToUsageGuidelines);

            var results = new ActionResults(
                new SerialLoginResults(loginResults),
                loginResults.ValidationMessages,
                GetUserHeaderValues(loginResults.UserToken)
                );

            return results;
        }

        public static NameValueCollection GetUserHeaderValues(UserTokenObject user)
        {
            NameValueCollection headerValues = new NameValueCollection();
            //return user info in response header
            headerValues.Add("user_token", user.UserToken == null ? "" : user.UserToken);
            headerValues.Add("expires_at", user.ExpirationUtcSeconds.ToString());
            return headerValues;
        }

        public static NameValueCollection GetUserHeaderValues(ISerialUserObject user)
        {
            NameValueCollection headerValues = new NameValueCollection();
            //return user info in response header
            headerValues.Add("user_token", user.userToken == null ? "" : user.userToken);
            headerValues.Add("expires_at", user.expiresAt.ToString());
            return headerValues;
        }

    }
}
