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
using System.Web;

using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class AttributeValueObjectCtl : BaseCtl<AttributeValueObject, MediaValue, AttributeValueObjectCtl, MediaObjectContext>
    {

        public override void UpdateIdsAfterInsert()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return PersistedBusinessObject.ToString();

        }

        public override bool DbObjectEqualsBusinessObject()
        {
            if (NewBusinessObject == null)
            {
                return (!PersistedBusinessObject.IsActive);
            }

            return (AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive
                    && PersistedDbObject.DisplayOrdinal == NewBusinessObject.DisplayOrdinal);
        }

        public override bool VersionMatches()
        {
            return true;    //We are not checking for versions at attribute value
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<AttributeValueObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<AttributeValueObject> mediaItems = from mv in media.MediaDbEntities.MediaValues
                                                          orderby mv.Attribute.AttributeName
                                                          select new AttributeValueObject
                                                          {
                                                              MediaId = mv.MediaId,
                                                              AttributeName = mv.Attribute.AttributeName,
                                                              ValueName = mv.Value.ValueName,
                                                              ValueKey = new ValueKey() { Id = mv.ValueId, LanguageCode = mv.ValueLanguageCode },
                                                              AttributeId = mv.AttributeID,
                                                              DisplayOrdinal = mv.DisplayOrdinal,
                                                              IsActive = mv.Active == "Yes",
                                                              DbObject = forUpdate ? mv : null
                                                          };
            return mediaItems;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new MediaValue();
            PersistedDbObject.ValueId = NewBusinessObject.ValueKey.Id;
            PersistedDbObject.MediaId = NewBusinessObject.MediaId;
            PersistedDbObject.ValueLanguageCode = HttpUtility.HtmlDecode(NewBusinessObject.ValueKey.LanguageCode ?? "");
            PersistedDbObject.AttributeID = NewBusinessObject.AttributeId;

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            if (NewBusinessObject == null)
            {
                PersistedDbObject.Active = "No";
                PersistedDbObject.DisplayOrdinal = PersistedBusinessObject.DisplayOrdinal;
            }
            else
            {
                PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";
                PersistedDbObject.DisplayOrdinal = NewBusinessObject.DisplayOrdinal;
            }
            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }


        public override void AddToDb()
        {
            throw new NotImplementedException();
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.MediaValues.DeleteObject(PersistedDbObject);
        }

    }
}
