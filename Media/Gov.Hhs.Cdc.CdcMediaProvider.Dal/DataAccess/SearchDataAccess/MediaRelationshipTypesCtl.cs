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
namespace Gov.Hhs.Cdc.DataSource.Media
{
    internal class MediaRelationshipTypesCtl
    {
        internal static string Get(MediaObjectContext media, ValueRelationshipObject obj, string inverseRelation)    //, MediaDataContext relationMedia)
        {

            RelationshipType rt = media.MediaDbEntities.RelationshipTypes
                .Where(r => r.ShortType == obj.RelationshipTypeName.ToUpper()).FirstOrDefault();

            inverseRelation = rt.InverseRelationshipTypeName;

            // API passing the shortType as the relationshipName
            // so change to long Name before saving
            obj.RelationshipTypeName = rt.RelationshipTypeName;
            return inverseRelation;
        }
        private static string Get2(ValueRelationshipObject obj, string inverseRelation, MediaObjectContext relationMedia)
        {
            RelationshipType rt = relationMedia.RelationshipTypes
            .Where(r => r.RelationshipTypeName == obj.RelationshipTypeName).FirstOrDefault();

            inverseRelation = rt.InverseRelationshipTypeName;
            return inverseRelation;
        }
        //public static CombinedMediaItem NewMediaItem()
        //{
        //    return new CombinedMediaItem();
        //}

    }
}
