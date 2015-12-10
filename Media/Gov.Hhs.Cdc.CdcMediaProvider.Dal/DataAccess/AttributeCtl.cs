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

using Gov.Hhs.Cdc.MediaProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class AttributeCtl
    {
        public static IQueryable<AttributeObject> Get(MediaObjectContext media, bool forUpdate)
        {
            IQueryable<AttributeObject> mediaItems = from a in media.MediaDbEntities.Attributes
                                                     select new AttributeObject
                                                          {
                                                              AttributeId = a.AttributeID,
                                                              AttributeSetId = a.AttributeSetID,
                                                              Name = a.AttributeName,
                                                              Description = a.Description,
                                                              DisplayOrdinal = a.DisplayOrdinal,
                                                              IsActive = a.Active == "Yes",
                                                              DbObject = forUpdate ? a : null
                                                          };
            return mediaItems.AsQueryable();
        }
    }
}
