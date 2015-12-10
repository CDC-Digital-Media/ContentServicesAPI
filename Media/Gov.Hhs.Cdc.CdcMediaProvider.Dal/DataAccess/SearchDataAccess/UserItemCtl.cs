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
using Gov.Hhs.Cdc.UserProvider;

namespace Gov.Hhs.Cdc.DataSource.Media
{
    public class UserItemCtl
    {
        public static IQueryable<UserItem> Get(MediaObjectContext media)
        {
            IQueryable<UserItem> sourceItems = from s in media.MediaDbEntities.Users
                                               join o in media.MediaDbEntities.Organizations
                                               on s.OrganizationId equals o.OrganizationId
                                               select new UserItem()
                                               {
                                                   UserGuid = s.UserId,
                                                   OrganizationId = s.OrganizationId,
                                                   OrganizationName = o.Name,
                                                   FirstName = s.FirstName,
                                                   MiddleInitial = s.MiddleInitial,
                                                   EMailAddress = s.EMailAddress,
                                                   IsActive = s.Active == "Yes"
                                               };
            return sourceItems;
        }
    }
}
