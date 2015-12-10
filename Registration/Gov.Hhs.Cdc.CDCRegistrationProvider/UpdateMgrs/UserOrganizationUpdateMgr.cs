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

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public class UserOrganizationUpdateMgr : BaseUpdateMgr<UserOrganizationObject>
    {
        private class CombinedUserOrgObject
        {
            public UserOrganizationObject UserOrganization { get; set; }
            public UserOrganizationCtl UserOrganizationInsertObjectCtl { get; set; }
            public List<UserOrganizationCtl> InsertedUserOrgs = new List<UserOrganizationCtl>();
            public bool HasNewOrganization { get; set; }
            public OrganizationObject Organization { get; set; }
            public OrganizationUpdateMgr OrganizationUpdateMgr { get; set; }
        }

        public override string ObjectName { get { return "UserOrganization"; } }
        List<CombinedUserOrgObject> theObjects;
        List<OrganizationUpdateMgr> organizationUpdateMgrs;

        public UserOrganizationUpdateMgr(IList<UserOrganizationObject> items)
        {
            Items = ValidatorHelper.GetWithValidationKey(items, ObjectName);
            Validator = new UserOrganizationObjectValidator();
            CreateChildUpdateMgrs(Items);

        }

        public UserOrganizationUpdateMgr(UserOrganizationObject item)
        {
            Items = ValidatorHelper.GetWithValidationKey(item, ObjectName);
            Validator = new UserOrganizationObjectValidator();
            CreateChildUpdateMgrs(Items);
        }


        private void CreateChildUpdateMgrs(IList<UserOrganizationObject> items)
        {
            theObjects = (from u in items
                          let hasNewOrganization = u.OrganizationId == 0 && u.Organization != null
                         select new CombinedUserOrgObject()
                         {
                             UserOrganization = u,
                             HasNewOrganization = hasNewOrganization,
                             Organization = u.Organization,
                             OrganizationUpdateMgr =  hasNewOrganization ? new OrganizationUpdateMgr(u.Organization) : null
                         }).ToList();

            organizationUpdateMgrs = (from u in theObjects
                                      where u.HasNewOrganization
                                      select u.OrganizationUpdateMgr).ToList();

        }


        public override void Save(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }




        private static void CreateNewUserOrg(IDataServicesObjectContext objectContext, CombinedUserOrgObject combinedObject, UserCtl user)
        {
            UserOrganizationCtl userOrg = UserOrganizationCtl.Create(objectContext, combinedObject.UserOrganization);
            combinedObject.InsertedUserOrgs.Add(userOrg);

            if (combinedObject.HasNewOrganization)
            {
                //We just have one Organization 
                List<OrganizationCtl> newOrgs = combinedObject.OrganizationUpdateMgr.Insert(objectContext, user);
                foreach (var newOrg in newOrgs)
                {
                    //newOrg.Add(userOrg);
                    newOrg.PersistedDbObject.UserOrganizations.Add(userOrg.PersistedDbObject);
                }
            }
            user.PersistedDbObject.UserOrganizations.Add(userOrg.PersistedDbObject);
            //user.Add(userOrg);
            
        }

        public override void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

        public static void Delete(IDataServicesObjectContext objectContext, IEnumerable<UserOrganizationObject> userOrgs)
        {
            //List<UserOrganizationObjectCtl> objectCtls = GetDataCtls(objectContext);
            foreach (UserOrganizationObject persistedUserOrg in userOrgs)
            {
                UserOrganizationCtl.Delete(objectContext, persistedUserOrg);
                //UserOrganizationObject persistedUserOrg = UserOrganizationObjectCtl.Get((RegistrationObjectContext)objectContext, forUpdate: true)
                //    .Where(uo => uo.UserGuid == userOrg.UserGuid && uo.OrganizationId == userOrg.OrganizationId)
                //    .FirstOrDefault();

                //if (userOrg != null)
                //{
                
                    //UserOrganizationObjectCtl userOrgCtl = new UserOrganizationObjectCtl((RegistrationObjectContext)objectContext);
                    //userOrgCtl.D(persistedUserOrg);
                //}
                //objectContext.DeleteObject<UserOrganizationObject>(userOrganization);
            }
        }


        //private static List<UserOrganizationObjectCtl> GetDataCtls(IBaseObjectContext objectContext)
        //{
        //    List<UserOrganizationObjectCtl> objectCtls =
        //        Items.Select(i => new UserOrganizationObjectCtl((RegistrationObjectContext)objectContext, i)).ToList();
        //    return objectCtls;

        //}

        #region CallsUsedByUserUpdateManager
        public override void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            Validator.PreSaveValidate(ref validationMessages, Items);
            foreach (OrganizationUpdateMgr organizationUpdateMgr in organizationUpdateMgrs)
            {
                organizationUpdateMgr.PreSaveValidate(ref validationMessages);
            }
        }

        public override void PreDeleteValidate(ValidationMessages validationMessages)
        {
            Validator.PreDeleteValidate(validationMessages, Items);
            foreach (OrganizationUpdateMgr organizationUpdateMgr in organizationUpdateMgrs)
            {
                organizationUpdateMgr.PreDeleteValidate(validationMessages);
            }
        }

        public override void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateSave(objectContext, validationMessages, Items);
            foreach (OrganizationUpdateMgr organizationUpdateMgr in organizationUpdateMgrs)
            {
                organizationUpdateMgr.ValidateSave(objectContext, validationMessages);
            }
        }

        public override void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            Validator.ValidateDelete(objectContext, validationMessages, Items);
            foreach (OrganizationUpdateMgr organizationUpdateMgr in organizationUpdateMgrs)
            {
                organizationUpdateMgr.ValidateDelete(objectContext, validationMessages);
            }
        }

        /// <summary>
        /// Pass in the saved UserObjectCtl, which contains the db objects to update
        /// </summary>
        /// <param name="objectContext"></param>
        /// <param name="user"></param>
        public void Update(IDataServicesObjectContext objectContext, UserCtl user)
        {
            foreach (CombinedUserOrgObject combinedObject in theObjects)
            {
                UserOrganizationObject persistedObject = user.PersistedBusinessObject.Organizations
                    .Where(uo => uo.OrganizationId == combinedObject.UserOrganization.OrganizationId).FirstOrDefault();
                //UserOrganizationObjectCtl userOrg = new UserOrganizationObjectCtl(objectContext, combinedObject.UserOrganization);
                //Need to add a new item
                if (persistedObject == null)
                    CreateNewUserOrg(objectContext, combinedObject, user);
                else
                    UserOrganizationCtl.Update(objectContext, persistedObject, combinedObject.UserOrganization, enforceConcurrency: true);
            }
        }
        public void Insert(IDataServicesObjectContext objectContext, UserCtl user)
        {
            foreach (CombinedUserOrgObject combinedObject in theObjects)
            {
                //UserOrganizationObjectCtl userOrg = new UserOrganizationObjectCtl(objectContext, combinedObject.UserOrganization);

                CreateNewUserOrg(objectContext, combinedObject, user);
            }
        }

        public override void UpdateIdsAfterInsert()
        {
            foreach (CombinedUserOrgObject combinedObject in theObjects)
            {
                if (combinedObject.OrganizationUpdateMgr != null)
                    combinedObject.OrganizationUpdateMgr.UpdateIdsAfterInsert();
                foreach (UserOrganizationCtl userOrg in combinedObject.InsertedUserOrgs)
                {
                    userOrg.UpdateIdsAfterInsert();
                }
            }
        }

        #endregion
    }
}
