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
using System.Data.Objects;
using System.Linq;
using System.Transactions;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using System.Collections;

namespace Gov.Hhs.Cdc.Authorization
{
    public static class AdminUserCtl
    {
        public static IList<int> AllowedMediaIds(AuthorizationObjectContext db, AdminUser user)
        {
            var result = db.AuthorizationDbEntities.EvaluateUserMediaSets1(user.UserGuid);
            return result.Where(m => m != null).Select(m => m.Value).ToList();
        }

        public static IQueryable<SerialAdminRole> Roles(AuthorizationObjectContext db)
        {


            var categorized = from r in db.AuthorizationDbEntities.Roles
                              join ur in db.AuthorizationDbEntities.UserRoles on r.RoleCode equals ur.RoleCode into uroj
                              from ur2 in uroj.DefaultIfEmpty()
                              join u in db.AuthorizationDbEntities.Users on ur2.UserGUID equals u.UserGuid into uoj
                              from u2 in uoj.DefaultIfEmpty()
                              select new
                              {
                                  role = r.RoleCode,
                                  member = u2
                              };


            var roles = from r in categorized
                        group r.member by r.role into m
                        select new SerialAdminRole
                        {
                            name = m.Key,
                            members = (m.Select(m1 => new AdminUser
                                {
                                    UserGuid = (m1.UserGuid == null ? Guid.Empty : m1.UserGuid),
                                    LastName = m1.LastName,
                                    FirstName = m1.FirstName,
                                    Email = m1.EmailAddress,
                                    UserName = m1.NetworkID
                                }))
                        };

            return roles;

        }

        public static IQueryable<AdminUser> GetUsers(AuthorizationObjectContext db)
        {
            var roles = db.AuthorizationDbEntities.UserRoles;
            return db.AuthorizationDbEntities.AdministrativeUsers
                .Select(u => new AdminUser
            {
                UserGuid = u.UserGuid,
                UserName = u.NetworkID,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Roles = roles.Where(r => r.UserGUID == u.UserGuid).Select(r => r.RoleCode),
                Email = u.EmailAddress,
                MediaSets = db.AuthorizationDbEntities.MediaSet_Combined
                    .Where(msc => msc.UserGUID == u.UserGuid)
                    .Select(m => new CsBusinessObjects.Admin.MediaSet
                   {
                       Name = m.MediaSetName,
                       Criteria = m.DynamicSearchCriteria
                   })
            });
        }

        public static void SetUserRoles(AuthorizationObjectContext db, AdminUser user, IList<string> roles, AdminUser updater)
        {
            var dbUser = db.AuthorizationDbEntities.Users.Where(u => u.NetworkID == user.UserName).FirstOrDefault();
            if (dbUser == null) { throw new ApplicationException("User " + user.UserName + " not found."); }
            var existing = db.AuthorizationDbEntities.UserRoles.Where(u => u.User.NetworkID == user.UserName);
            using (var scope = new TransactionScope())
            {
                foreach (var item in existing)
                {
                    db.AuthorizationDbEntities.UserRoles.DeleteObject(item);
                }
                foreach (var item in roles)
                {
                    var role = db.AuthorizationDbEntities.Roles.Where(r => r.RoleCode == item).FirstOrDefault();
                    db.AuthorizationDbEntities.UserRoles.AddObject(new UserRole
                    {
                        User = dbUser,
                        Active = "Yes",
                        CreatedByGuid = updater.UserGuid,
                        ModifiedByGuid = updater.UserGuid,
                        RoleCode = item,
                        CreatedDateTime = DateTime.UtcNow,
                        ModifiedDateTime = DateTime.UtcNow
                    });
                }
                db.SaveChanges(SaveOptions.DetectChangesBeforeSave);
                db.AcceptAllChanges();
                scope.Complete();
            }
        }

        public static void AddUser(AuthorizationObjectContext db, AdminUser user, AdminUser addedBy)
        {
            db.AuthorizationDbEntities.CreateUser(user.FirstName, null, user.LastName, user.UserName, user.Email, null, null, null);
        }

        public static void RemoveUser(AuthorizationObjectContext db, AdminUser user)
        {
            using (var scope = new TransactionScope())
            {

                var userOrg = db.AuthorizationDbEntities.UserOrganizations.FirstOrDefault(uo => uo.UserGuid == user.UserGuid);
                if (userOrg != null) { db.AuthorizationDbEntities.UserOrganizations.DeleteObject(userOrg); }

                var userRoles = db.AuthorizationDbEntities.UserRoles.Where(ur => ur.UserGUID == user.UserGuid);
                foreach (var userRole in userRoles)
                {
                    db.AuthorizationDbEntities.UserRoles.DeleteObject(userRole);
                }

                var dbUser = db.AuthorizationDbEntities.Users.FirstOrDefault(u => u.UserGuid == user.UserGuid);
                if (dbUser != null) { db.AuthorizationDbEntities.Users.DeleteObject(dbUser); }

                db.SaveChanges(SaveOptions.DetectChangesBeforeSave);
                db.AcceptAllChanges();
                scope.Complete();

            }
        }

        public static IQueryable<CsBusinessObjects.Admin.MediaSet> MediaSets(AuthorizationObjectContext db)
        {
            return db.AuthorizationDbEntities.MediaSets
                .Select(ms => new CsBusinessObjects.Admin.MediaSet
                {
                    Name = ms.MediaSetName,
                    Criteria = ms.MediaSetDynamic.SearchCriteria
                });
        }

        internal static void AddMediaSet(AuthorizationObjectContext db, CsBusinessObjects.Admin.MediaSet set, AdminUser addedBy)
        {
            db.AuthorizationDbEntities.MediaSets.AddObject(new MediaSet { MediaSetName = set.Name });
            db.AuthorizationDbEntities.MediaSetDynamics.AddObject(new MediaSetDynamic { MediaSetName = set.Name, SearchCriteria = set.Criteria });
            db.SaveChanges();
        }

        internal static void DeleteMediaSet(AuthorizationObjectContext db, string id)
        {
            var dyn = db.AuthorizationDbEntities.MediaSetDynamics.FirstOrDefault(msd => msd.MediaSetName == id);
            if (dyn == null)
            {
                throw new InvalidOperationException("Media set " + id + " not found.");
            }
            db.AuthorizationDbEntities.MediaSetDynamics.DeleteObject(dyn);


            var set = db.AuthorizationDbEntities.MediaSets.FirstOrDefault(ms => ms.MediaSetName == id);
            if (set == null)
            {
                throw new InvalidOperationException("Media set " + id + " not found.");
            }
            db.AuthorizationDbEntities.MediaSets.DeleteObject(set);
            db.SaveChanges();
        }
    }
}
