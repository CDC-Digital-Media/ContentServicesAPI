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
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcEmailProvider;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.EmailProvider;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.RegistrationProvider;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class CsRegistrationProvider : IRegistrationProvider
    {
        private static IObjectContextFactory ObjectContextFactory { get; set; }

        private static IEmailProvider _emailProvider = null;
        private static IEmailProvider EmailProvider
        {
            get
            {
                return _emailProvider ?? (_emailProvider = new CsEmailProvider());
            }
            set
            {
                _emailProvider = value;
            }
        }


        public CsRegistrationProvider()
        {
            ObjectContextFactory = new RegistrationObjectContextFactory();
        }

        //Used for testing
        public static void Inject(IEmailProvider emailProvider)
        {
            EmailProvider = emailProvider;
        }

        private DataManager RegistrationDataManager { get { return new DataManager(ObjectContextFactory); } }

        public LoginResults RegisterUser(UserObject userObject, bool sendEmail, string overriddenEmailAddress = null)
        {
            ValidationMessages messages = RegistrationDataManager.Save(new UserUpdateMgr(userObject));

            if (messages.Errors().Any())
            {
                return new LoginResults(messages);
            }

            if (sendEmail)
            {
                SendEmailToRegisteredUser(userObject, "Registered Account", overriddenEmailAddress);
            }

            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                CreateMyListIfDoesNotExist(userObject.Organizations.FirstOrDefault().OrganizationId, registrationDb);
                registrationDb.SaveChanges();
            }

            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                var db = registrationDb as RegistrationObjectContext;
                UserObject userFromDb = UserCtl.GetUsers(db, forUpdate: true)
                    .Where(u => u.EmailAddress == userObject.EmailAddress).FirstOrDefault();

                //update user password
                userFromDb.Password = userObject.Password;
                userFromDb.PasswordSalt = CredentialManager.GenerateApiKeyTokenSalt();
                UserUpdateMgr.EncryptPassword(userFromDb);
                UserCtl userObjectCtl = UserCtl.Update(registrationDb, userFromDb, userFromDb, enforceConcurrency: true);
                userObjectCtl.SetPassword();

                UserTokenObject userToken = CreateUserToken(userFromDb);
                ValidationMessages validationMessages = RegistrationDataManager.Save(new UserTokenUpdateMgr(userToken));
                LoginResults loginResults = CreateLoginResults(userFromDb, db, userToken, validationMessages);
                registrationDb.SaveChanges();

                return loginResults;
            }
        }

        private void CreateMyListIfDoesNotExist(int organizationId, IDataServicesObjectContext registrationDb)
        {
            RegistrationObjectContext regDb = (RegistrationObjectContext)registrationDb;
            var orgs = OrganizationDomainCtl.Get(regDb);
            var domains = orgs.Where(od => od.OrganizationId == organizationId);
            foreach (OrganizationDomainObject orgDomain in domains)
            {
                Model.SyndicationList syndicationList = regDb.RegistrationDbEntities.SyndicationLists.Where(m => m.DomainName == orgDomain.DomainName).FirstOrDefault();
                if (syndicationList == null)
                {
                    regDb.RegistrationDbEntities.SyndicationLists.AddObject(
                        new Model.SyndicationList()
                        {
                            MyListGuid = Guid.NewGuid(),
                            DomainName = orgDomain.DomainName,
                            ListName = orgDomain.DomainName,
                            MyListStatusCode = "Creating",
                            Active = "Yes",
                            CreatedDateTime = DateTime.UtcNow,
                            ModifiedDateTime = DateTime.UtcNow
                        });
                }
            }
        }

        private void SendEmailToRegisteredUser(UserObject user, string emailType, string overriddenEmailAddress)
        {
            string toEmailAddress = string.IsNullOrEmpty(overriddenEmailAddress)
                ? user.EmailAddress : overriddenEmailAddress;

            var newKeys = new object();
            newKeys = new
            {
                User = new UserTemplateObject(user),
                Organization = new OrganizationTemplateObject(user.Organizations.Select(uo => uo.Organization).FirstOrDefault())
            };

            if (EmailProvider == null)
            {
                return;
            }
            EmailProvider.Send(emailType, newKeys, new EmailRouting(toEmailAddress));
        }

        public LoginResults SaveUser(UserObject theObject)
        {
            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                ValidationMessages validationMessages = RegistrationDataManager.Save(new UserUpdateMgr(theObject));

                var db = registrationDb as RegistrationObjectContext;

                if (theObject.Organizations == null || theObject.Organizations.Count() == 0)
                {
                    return new LoginResults(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "User",
                        "User " + theObject.EmailAddress + " does not have any associated organizations."));
                }

                CreateMyListIfDoesNotExist(theObject.Organizations.FirstOrDefault().OrganizationId, registrationDb);
                registrationDb.SaveChanges();

                UserObject userFromDb = UserCtl.GetUsers(db, forUpdate: true)
                    .Where(u => u.UserGuid == theObject.UserGuid).FirstOrDefault();

                UserTokenObject userToken = CreateUserToken(userFromDb);
                LoginResults loginResults = CreateLoginResults(userFromDb, db, userToken, validationMessages);
                registrationDb.SaveChanges();

                return loginResults;
            }
        }

        public ValidationMessages SaveUser(ServiceUserObject theObject)
        {
            return RegistrationDataManager.Save(new ServiceUserUpdateMgr(theObject));
        }

        public ValidationMessages SaveApiClient(ApiClientObject theObject, string apiKey)
        {
            ValidationMessages messages = new ValidationMessages();

            if (GetApiClientByAppKey(apiKey) != null)
            {
                messages.Add(RegistrationDataManager.Save(new ApiClientUpdateMgr(theObject)));
            }
            else
            {
                messages.AddError("Registration", "Invalid Request");
            }

            return messages;
        }

        public ValidationMessages DeleteApiClient(ApiClientObject theObject, string apiKey)
        {
            ValidationMessages messages = new ValidationMessages();

            if (GetApiClientByAppKey(apiKey) != null)
            {
                messages.Add(RegistrationDataManager.Delete(new ApiClientUpdateMgr(theObject)));
            }
            else
            {
                messages.AddError("Registration", "Invalid Request");
            }

            return messages;
        }

        public ValidationMessages DeleteUser(UserObject theObject)
        {
            return RegistrationDataManager.Delete(new UserUpdateMgr(theObject));
        }

        public ValidationMessages DeleteServiceUserByEmail(string emailAddress)
        {
            return RegistrationDataManager.Delete(new ServiceUserUpdateMgr(new ServiceUserObject() { EmailAddress = emailAddress }));
        }

        //public ValidationMessages UpdateApiClientToken(ApiClientObject theObject)
        //{
        //    ValidationMessages msg = new ValidationMessages();

        //    ApiClientObject apiClient;
        //    if (!VerifyApiClientCredentials(theObject, out apiClient))
        //        return new ValidationMessages(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "Authentication",
        //                "Failed due to invalid credentials"));

        //    //credentials are valid, so generate new token
        //    using (RegistrationObjectContext db = (RegistrationObjectContext)ObjectContextFactory.Create())
        //    {
        //        var ctl = ApiClientCtl.Update(db, apiClient, theObject, true);
        //        ctl.UpdateToken();

        //        db.SaveChanges();
        //    }

        //    return msg;
        //}

        public ValidationMessages UpdateApiClientApiKey(ApiClientObject theObject)
        {
            ValidationMessages messages = new ValidationMessages();

            ApiClientObject apiClient;
            if (!VerifyApiClientCredentials(theObject, out apiClient))
            {
                return new ValidationMessages(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "Authentication",
                         "Failed due to invalid credentials"));
            }

            //credentials are valid, so generate new token
            using (RegistrationObjectContext db = (RegistrationObjectContext)ObjectContextFactory.Create())
            {
                var ctl = ApiClientCtl.Update(db, apiClient, theObject, true);
                ctl.UpdateApiKey();

                db.SaveChanges();
            }

            return messages;
        }

        private bool VerifyApiClientCredentials(ApiClientObject theObject, out ApiClientObject persistentObject)
        {
            persistentObject = GetApiClientByAppKey(theObject.ApplicationKey, forUpdate: true);
            if (persistentObject == null)
            { 
                return false; 
            }

            //compare hash to validate credentials
            bool isValid = CredentialManager.CompareHash(ConfigurationManager.AppSettings["ApiClientPassword"], persistentObject.Salt, theObject.Secret, passwordFormat: 0);

            if (isValid)
            {
                theObject.ServerUserGuid = persistentObject.ServerUserGuid;
                theObject.ApiTypeName = persistentObject.ApiTypeName;
                theObject.Name = persistentObject.Name;
                theObject.IsActive = persistentObject.IsActive;
                theObject.ApiKey = persistentObject.ApiKey;
            }

            return isValid;
        }

        public LoginResults SetUserAgreement(Guid userGuid, bool agreedToUsageGuidelines)
        {
            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                var db = registrationDb as RegistrationObjectContext;
                ValidationMessages validationMessages = new ValidationMessages();

                UserObject userFromDb = UserCtl.GetUsers((RegistrationObjectContext)registrationDb, forUpdate: true)
                    .Where(u => u.UserGuid == userGuid).FirstOrDefault();
                userFromDb.AgreedToUsageGuidelines = agreedToUsageGuidelines;

                UserCtl.Update(registrationDb, userFromDb, userFromDb, enforceConcurrency: true);
                UserCtl userObjectCtl = UserCtl.Create(registrationDb, userFromDb);

                UserTokenObject userToken = CreateUserToken(userFromDb);
                registrationDb.SaveChanges();
                LoginResults loginResults = CreateLoginResults(userFromDb, db, userToken, validationMessages);

                return loginResults;
            }
        }

        public ValidationMessages UpdateUserPassword(string userEmail, string currentPassword, string newPassword, string newPasswordRepeat, string apiKey)
        {
            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                var db = registrationDb as RegistrationObjectContext;

                //validate API client
                ApiClientObject apiClientItem = ApiClientCtl.Get(db).Where(a => a.ApplicationKey == apiKey).FirstOrDefault();
                if (apiClientItem == null)
                {
                    return new ValidationMessages(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "ApiKey",
                         "Failed due to invalid ApiKey"));
                }

                //verify user
                UserObject user = UserCtl.GetUsers(db, forUpdate: true)
                    .Where(u => u.EmailAddress == userEmail).FirstOrDefault();

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.TempPassword))
                    {
                        // check if the password reset session has expires
                        if (CredentialManager.TokenExpirationInSeconds() > user.ExpirationSeconds)
                        {
                            return new ValidationMessages(
                                  new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "Reset Password",
                                 "Session Expired"));
                        }
                    }

                    //check the current password
                    user.Password = currentPassword;

                    //verify user credentials
                    if (!VerifyUserCredential(registrationDb, user, apiClientItem.ApiClientGuid))
                    {
                        return new ValidationMessages(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "User",
                         "The email address entered doesn't match the originating password reset request"));
                    }

                    //set the new password
                    user.Password = newPassword;
                    user.PasswordRepeat = newPasswordRepeat;

                    ValidationMessages messages = new ValidationMessages();

                    //needed in order to validate password without also validating other things, such as
                    //   whether user already exists in the system (UserObjectValidator.ValidateSave)
                    //   whether user already belongs to org (UserOrganizationObjectValidator.ValidateSave
                    //so we can't actually use RegistrationDataManager.Save as the validations applied only apply to Inserts

                    var validator = new UserObjectValidator();
                    validator.PreSaveValidate(ref messages, new List<UserObject> { user });

                    if (!messages.Errors().Any())
                    {
                        user.PasswordSalt = CredentialManager.GenerateApiKeyTokenSalt();
                        UserUpdateMgr.EncryptPassword(user);

                        //update user token
                        user.UserToken = CredentialManager.GenerateApiKeyTokenSalt();
                        user.ExpirationSeconds = CredentialManager.TokenExpirationInSeconds(VALID_REQUEST_WINDOW);

                        UserCtl userObjectCtl = UserCtl.Update(registrationDb, user, user, enforceConcurrency: true);
                        userObjectCtl.SetPassword();
                        registrationDb.SaveChanges();

                        try
                        {
                            EmailProvider.Send("Changed Password", null, new EmailRouting(user.EmailAddress));
                        }
                        catch (SmtpException ex)
                        {
                            Logger.LogError(ex);
                        }
                    }

                    return messages;
                }
                else
                {
                    return new ValidationMessages(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "User",
                          "The email address entered doesn't match the originating password reset request"));
                }
            }
        }

        public ValidationMessages ResetUserPassword(string userEmail, Uri passwordResetUrl, string apiKey)
        {
            ValidationMessages validationMessages = new ValidationMessages();

            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                //validate API client
                ApiClientObject apiClientItem = ApiClientCtl.Get((RegistrationObjectContext)registrationDb)
                    .Where(a => a.ApplicationKey == apiKey).FirstOrDefault();
                if (apiClientItem == null)
                {
                    return new ValidationMessages(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "ApiKey",
                         "Failed due to invalid ApiKey"));
                }

                //verify user
                UserObject user = UserCtl.GetUsers((RegistrationObjectContext)registrationDb, forUpdate: true)
                    .Where(u => u.EmailAddress == userEmail).FirstOrDefault();

                if (user != null)
                {
                    if (!validationMessages.Errors().Any())
                    {
                        string passwordresettoken = CredentialManager.GenerateApiKeyTokenSalt();
                        string tokenParamName = ConfigurationManager.AppSettings["PasswordResetParam"];

                        long timeout = 3600;
                        long.TryParse(ConfigurationManager.AppSettings["PasswordResetTimeoutSeconds"], out timeout);

                        //set the temporary password
                        user.TempPassword = passwordresettoken;
                        user.PasswordSalt = CredentialManager.GenerateApiKeyTokenSalt();
                        user.ExpirationSeconds = CredentialManager.TokenExpirationInSeconds(timeout);

                        UserUpdateMgr.EncryptTempPassword(user);
                        UserCtl userObjectCtl = UserCtl.Update(registrationDb, user, user, enforceConcurrency: true);
                        userObjectCtl.SetTempPassword();
                        userObjectCtl.SetToken();
                        registrationDb.SaveChanges();

                        object key = ResetKey(passwordResetUrl, passwordresettoken, tokenParamName);

                        int retryAttempts = 0;
                        int.TryParse(ConfigurationManager.AppSettings["SmtpRetryAttempts"], out retryAttempts);

                        try
                        {
                            if (EmailProvider == null)
                            {
                                return validationMessages;
                            }
                            EmailProvider.Send("Forgot Password", key, new EmailRouting(user.EmailAddress), retryAttempts);
                        }
                        catch (SmtpException ex)
                        {
                            Logger.LogError(ex);
                            return ValidationMessages.CreateError("Reset", "Unable to reset password at this time.  Please try again.");
                        }
                    }
                }
                else
                {
                    return new ValidationMessages(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "User",
                           "Invalid User"));
                }
            }

            return validationMessages;
        }

        public static object ResetKey(Uri passwordResetUrl, string passwordresettoken, string tokenParamName)
        {
            if (string.IsNullOrEmpty(passwordResetUrl.Query))
            {
                return new
                {
                    url = passwordResetUrl.AbsoluteUri + "?" + tokenParamName + "=" + passwordresettoken
                };
            }

            if (passwordResetUrl.Query.Contains(tokenParamName))
            {
                var existingToken = passwordResetUrl.Query.Split('=')[1];
                return new
                {
                    url = passwordResetUrl.AbsoluteUri.Replace(existingToken, passwordresettoken)
                };
            }

            return new
            {
                url = passwordResetUrl.AbsoluteUri + "&" + tokenParamName + "=" + passwordresettoken
            };

        }

        public UserObject GetUser(Guid userGuid)
        {
            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                UserObject user = UserCtl.GetUsers((RegistrationObjectContext)registrationDb, forUpdate: false)
                    .Where(u => u.UserGuid == userGuid).FirstOrDefault();

                return user;
            }
        }

        public ServiceUserObject GetServiceUserByEmail(string emailAddress)
        {
            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                ServiceUserObject serviceUser = ServiceUserCtl.Get((RegistrationObjectContext)registrationDb, returnPassword: false, forUpdate: false)
                    .Where(u => u.EmailAddress.ToLower() == emailAddress.ToLower()).FirstOrDefault();

                return serviceUser;
            }
        }

        public ValidationMessages SaveOrganizations(IList<OrganizationObject> objects)
        {
            return RegistrationDataManager.Save(new OrganizationUpdateMgr(objects));
        }

        public ValidationMessages SaveOrganization(OrganizationObject theObject)
        {
            return RegistrationDataManager.Save(new OrganizationUpdateMgr(theObject));
        }

        public ValidationMessages DeleteOrganizations(IList<OrganizationObject> objects)
        {
            return RegistrationDataManager.Delete(new OrganizationUpdateMgr(objects));
        }

        public ValidationMessages DeleteOrganization(OrganizationObject theObject)
        {
            return RegistrationDataManager.Delete(new OrganizationUpdateMgr(theObject));
        }

        public OrganizationObject GetOrganization(int organizationId)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return OrganizationCtl.Get((RegistrationObjectContext)media, forUpdate: false)
                    .Where(u => u.Id == organizationId).FirstOrDefault();
            }
        }

        //TODO: Put this in config (it is currenty in 3 places)
        public const int VALID_REQUEST_WINDOW = 900;                             // 900 seconds - 15 minutes

        public LoginResults Login(UserObject userFromClient, string appKey)
        {
            using (RegistrationObjectContext objectContext = (RegistrationObjectContext)ObjectContextFactory.Create())
            {
                ApiClientObject apiClientItem = ApiClientCtl.Get((RegistrationObjectContext)objectContext)
                    .Where(a => a.ApplicationKey == appKey).FirstOrDefault();
                if (apiClientItem == null)
                {
                    return new LoginResults(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "ApiKey",
                          "Login failed due to missing ApiKey"));
                }

                if (!VerifyUserCredential(objectContext, userFromClient, apiClientItem.ApiClientGuid))
                {
                    return new LoginResults(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "User",
                          "Invalid Credentials"));
                }

                //get user from DB
                UserObject userFromDb = UserCtl.GetUsers((RegistrationObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.EmailAddress == userFromClient.EmailAddress).FirstOrDefault();

                if (userFromDb != null)
                {
                    userFromDb.TempPassword = null;
                    UserCtl userObjectCtl = UserCtl.Update(objectContext, userFromDb, userFromDb, enforceConcurrency: true);
                    userObjectCtl.SetTempPassword();

                    if ((userFromDb.IsMigrated && userFromDb.PasswordSalt.Length == 24) || string.IsNullOrEmpty(userFromDb.PasswordSalt))
                    {
                        userFromDb.Password = userFromClient.Password;
                        userFromDb.PasswordSalt = CredentialManager.GenerateApiKeyTokenSalt();
                        UserUpdateMgr.EncryptPassword(userFromDb);
                        userObjectCtl.SetPassword();
                    }
                    objectContext.SaveChanges();
                }

                //generate user tokens
                UserTokenObject userToken = CreateUserToken(userFromClient);
                ValidationMessages validationMessages = RegistrationDataManager.Save(new UserTokenUpdateMgr(userToken));
                LoginResults loginResults = CreateLoginResults(userFromDb, objectContext, userToken, validationMessages);

                return loginResults;
            }
        }

        private static LoginResults CreateLoginResults(UserObject userObj, RegistrationObjectContext objectContext, UserTokenObject userToken, ValidationMessages validationMessages)
        {
            UserObject user = UserCtl.GetUsers(objectContext).Where(u => u.EmailAddress == userObj.EmailAddress).FirstOrDefault();
            List<DomainSyndicationListObject> syndicationLists = user == null ? null : GetSyndicationListsForUser(objectContext, user.UserGuid);

            LoginResults loginResults = new LoginResults()
            {
                UserToken = userToken,
                ValidationMessages = validationMessages,
                User = user,
                SyndicationLists = syndicationLists
            };

            return loginResults;
        }

        private static UserTokenObject CreateUserToken(UserObject userObj)
        {
            UserTokenObject userToken = new UserTokenObject()
            {
                EmailAddress = userObj.EmailAddress,
                UserToken = CredentialManager.GenerateApiKeyTokenSalt(),
                ExpirationUtcSeconds = CredentialManager.TokenExpirationInSeconds(VALID_REQUEST_WINDOW)
            };

            return userToken;
        }

        private static List<DomainSyndicationListObject> GetSyndicationListsForUser(RegistrationObjectContext objectContext, Guid userGuid)
        {
            IQueryable<string> domains = from uo in UserOrganizationCtl.Get(objectContext)
                                         join od in OrganizationDomainCtl.Get(objectContext)
                                             on uo.OrganizationId equals od.OrganizationId
                                         where uo.UserGuid == userGuid
                                         select od.DomainName;

            List<DomainSyndicationListObject> syndicationLists = DomainSyndicationListItemCtl.GetDomainSyndicationList(objectContext)
                .Where(l => domains.Contains(l.DomainName) && l.IsActive).ToList();

            return syndicationLists;
        }


        private bool VerifyUserCredential(IDataServicesObjectContext objectContext, UserObject userFromClient, Guid apiClientGuid)
        {
            UserObject userFromDB = UserCtl.GetUsers((RegistrationObjectContext)objectContext)
                .Where(u => u.EmailAddress == userFromClient.EmailAddress)
                .FirstOrDefault();

            if (userFromDB == null)
            {
                return false;
            }
            else
            {
                string salt = UserUpdateMgr.GetSalt(userFromDB);
                if (!CredentialManager.CompareHash(userFromClient.Password, salt, userFromDB.Password, userFromDB.PasswordFormat))
                {
                    if (!string.IsNullOrEmpty(userFromDB.TempPassword))
                    {
                        if (!CredentialManager.CompareHash(userFromClient.Password, salt, userFromDB.TempPassword, userFromDB.PasswordFormat))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            userFromClient.UserGuid = userFromDB.UserGuid;
            userFromClient.FirstName = userFromDB.FirstName;
            userFromClient.MiddleName = userFromDB.MiddleName;
            userFromClient.LastName = userFromDB.LastName;
            userFromClient._usageGuidelinesAgreementDateTime = userFromDB._usageGuidelinesAgreementDateTime;
            userFromClient.IsActive = userFromDB.IsActive;

            return true;
        }

        public bool ValidateUsers(string email, string appKey)
        {
            bool test = false;

            using (RegistrationObjectContext objectContext = (RegistrationObjectContext)ObjectContextFactory.Create())
            {
                ApiClientObject apiClientItem = ApiClientCtl.Get(objectContext).Where(a => a.ApplicationKey == appKey).FirstOrDefault();
                if (apiClientItem == null)
                {
                    return false;
                }

                UserObject userFromDB = UserCtl.GetUsers((RegistrationObjectContext)objectContext)
                    .Where(u => u.EmailAddress == email)
                    .FirstOrDefault();
                if (userFromDB != null)
                {
                    return true;
                }
            }

            return test;
        }

        public ApiClientObject GetApiClientByAppKey(string appKey, bool forUpdate = false)
        {
            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                return ApiClientCtl.Get((RegistrationObjectContext)registrationDb, forUpdate)
                    .Where(a => a.ApplicationKey == appKey).FirstOrDefault();
            }
        }

        public ApiClientObject GetApiClientByApiKey(string apiKey, bool forUpdate = false)
        {
            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                return ApiClientCtl.Get((RegistrationObjectContext)registrationDb, forUpdate)
                    .Where(a => a.ApiKey == apiKey).FirstOrDefault();
            }
        }

        public List<ApiClientObject> GetApiClientList()
        {
            using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            {
                return ApiClientCtl.Get((RegistrationObjectContext)registrationDb)
                    .Where(a => a.IsActive == true && a.ApiKey != null && a.ConnectionStringName != null).ToList();
            }
        }

        //public bool IsClientApplicationKeyValid(string appKey)
        //{
        //    using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
        //    {
        //        ApiClientObject apiClientItem = ApiClientCtl.Get((RegistrationObjectContext)registrationDb)
        //             .Where(a => a.ApplicationKey == appKey).FirstOrDefault();

        //        return (apiClientItem != null);
        //    }
        //}


        public static int StorefrontUserCount()
        {
            using (var db = ObjectContextFactory.Create() as RegistrationObjectContext)
            {
                //TODO:  Refactor to use below
                return UserCtl.GetUsers(db).Count(u => u.AgreedToUsageGuidelines);
            }
        }

        public IEnumerable<UserObject> GetUsers()
        {
            using (var db = ObjectContextFactory.Create() as RegistrationObjectContext)
            {
                return UserCtl.GetUsers(db).ToList();
            }
        }
    }
}
