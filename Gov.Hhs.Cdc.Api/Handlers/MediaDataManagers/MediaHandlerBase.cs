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
using System.Web.Script.Serialization;

using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidatonProvider;
using Gov.Hhs.Cdc.ECardProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class MediaHandlerBase
    {
        protected static IMediaProvider MediaProvider { get; set; }
        protected static IECardProvider ECardProvider { get; set; }

        protected static JavaScriptSerializer BaseJsSerializer = new JavaScriptSerializer();

        public MediaHandlerBase()
        {
            BaseJsSerializer = new JavaScriptSerializer();
            BaseJsSerializer.MaxJsonLength = int.MaxValue;
        }

        public static void Inject(IMediaProvider mediaProvider, IECardProvider eCardProvider)
        {
            MediaProvider = mediaProvider;
            ECardProvider = eCardProvider;
        }

        protected static Guid? ParseGuid(string stringGuid)
        {
            Guid theGuid;
            if (Guid.TryParse(stringGuid, out theGuid))
                return theGuid;
            return null;
        }

        protected static Guid ParseGuid(string guid, string guidName)
        {
            Guid? parsedGuid = ParseGuid(guid);
            if (parsedGuid == null)
                throw new ApplicationException(string.Format("Invalid {0} Guid: {1}", guidName, guid));
            return (Guid)parsedGuid;
        }

        protected static bool UserCanEdit(IOutputWriter writer, AdminUser adminUser)
        {
            if (!adminUser.CanEditMedia(0))
            {
                writer.Write(ValidationMessages.CreateError("auth", "User is not authorized to administer media."));
                return false;
            }
            return true;
        }

    }
}
