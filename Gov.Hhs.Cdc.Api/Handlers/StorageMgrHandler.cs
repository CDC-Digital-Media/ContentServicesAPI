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
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class StorageMgrHandler : MediaHandlerBase
    {
        
        public static void Insert(IOutputWriter writer, string stream, string apiKey, AdminUser adminUser)
        {
            Save(writer, stream, apiKey, 0, adminUser);
        }

        private static void Save(IOutputWriter writer, string stream, string appKey, int mediaId, AdminUser adminUser)
        {
            if (!adminUser.CanEditMedia(mediaId))
            {
                writer.Write(ValidationMessages.CreateError("auth", "User is not authorized to administer media."));
                return;
            }

            SerialStorageAdmin serialStorage = null;
            try
            {                                
                serialStorage = BaseJsSerializer.Deserialize<SerialStorageAdmin>(stream);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Logger.LogError("Invalid format of serialized storage object: " + stream, "Save");
                writer.Write(ValidationMessages.CreateError("storage", "Invalid format of serialized storage object"));
                return;
            }

            try
            {
                if (mediaId > 0)
                {
                    serialStorage.mediaId = mediaId;
                }

                ActionResults result = StorageHandler.Create(serialStorage, appKey);

                //TODO: the initial design did not use the writer in a MgrHandler.
                // this will only work for version 1 of admin
                writer.Write(new StorageAdminTransformation().CreateSerialResponse((List<StorageObject>)result.Results),
                    result.ValidationMessages);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                writer.Write(ValidationMessages.CreateError("", "Exception has been logged saving the storage object"));
            }
        }
    }
}
