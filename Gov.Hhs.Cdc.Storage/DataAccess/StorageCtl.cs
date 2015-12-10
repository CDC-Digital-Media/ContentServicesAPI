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

using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

namespace Gov.Hhs.Cdc.Storage
{
    public class StorageCtl
    {
        public static IQueryable<StorageObject> Get(Model.StorageObjectContext media, bool forUpdate = false)
        {
            IQueryable<StorageObject> storage = from a in media.StorageDbEntities.Storages
                                                //join ms in media.MediaDbEntities.MediaStorages on a.StorageID equals ms.StorageID
                                                select new StorageObject()
                                                {
                                                    StorageId = a.StorageID,
                                                    //MediaId = ms.MediaID,
                                                    //MediaIds = a.MediaStorages.Where(b => b.StorageID == a.StorageID).Select(,
                                                    Name = a.Name,
                                                    ByteStream = a.ByteStream,
                                                    FileExtension = a.FileExtension,
                                                    Width = a.Width,
                                                    Height = a.Height,
                                                    DbObject = forUpdate ? a : null
                                                };
            return storage;
        }
    }
}
