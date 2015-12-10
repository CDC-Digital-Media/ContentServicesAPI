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
using System.Data.Objects.SqlClient;
using Gov.Hhs.Cdc.DataServicesCacheProvider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal.Dal
{

    public class RelationshipTypeItemCtl : ICachedDataControl
    {
        public IQueryable Get(IDataServicesObjectContext dataEntities)
        //public static IQueryable<RelationshipTypeItem> Get(MediaObjectContext media)
        {
            return GetRelationshipTypeItem((MediaObjectContext)dataEntities);
        }

        public static IQueryable<RelationshipTypeItem> GetRelationshipTypeItem(MediaObjectContext media)
        {
            IQueryable<RelationshipTypeItem> mediaItems =
                                              from v in media.MediaDbEntities.RelationshipTypes
                                              select new RelationshipTypeItem()
                                              {

                                                  RelationshipTypeName = v.RelationshipTypeName,
                                                  ShortType = v.ShortType,
                                                  Description = v.Description,
                                                  InverseRelationshipTypeName = v.InverseRelationshipTypeName,
                                                  DisplayOrdinal = v.DisplayOrdinal,
                                                  IsActive = (v.Active == "Yes")
                                              };
            return mediaItems;
        }



        public object GetDictionary(IDataServicesObjectContext dataEntities)
        {
            throw new NotImplementedException();
        }
    }
}
