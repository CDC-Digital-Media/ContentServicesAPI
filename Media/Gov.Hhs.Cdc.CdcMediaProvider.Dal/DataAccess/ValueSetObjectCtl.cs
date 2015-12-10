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
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    //Leave the name as is for right now until the Cache layer is rewritten
    public class ValueSetObjectCtl : BaseCtl<ValueSetObject, ValueSet, ValueSetObjectCtl, MediaObjectContext>, ICachedDataControl
    {
        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.Id = PersistedDbObject.ValueSetID;
        }

        public override string ToString()
        {
            return PersistedBusinessObject.Name + ", Id=" + PersistedBusinessObject.Id.ToString();
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return (PersistedDbObject.ValueSetID == NewBusinessObject.Id
                && PersistedDbObject.LanguageCode == NewBusinessObject.LanguageCode
                && PersistedDbObject.ValueSetName == NewBusinessObject.Name
                && PersistedDbObject.Description == NewBusinessObject.Description
                && PersistedDbObject.DisplayOrdinal == NewBusinessObject.DisplayOrdinal
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive
                && AsBool(PersistedDbObject.IsDefaultable) == NewBusinessObject.IsDefaultable
                && AsBool(PersistedDbObject.IsOrderable) == NewBusinessObject.IsOrderable
                && AsBool(PersistedDbObject.IsHierachical) == NewBusinessObject.IsHierachical
                );
        }

        public override bool VersionMatches()
        {
            return true;
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<ValueSetObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
             IQueryable<ValueSetObject> mediaItems = from v in media.MediaDbEntities.ValueSets
                                                      select new ValueSetObject()
                                                      {
                                                          Id = v.ValueSetID,
                                                          LanguageCode = v.LanguageCode,
                                                          //AttributeId = v.AttributeID,
                                                          Name = v.ValueSetName,
                                                          Description = v.Description,
                                                          DisplayOrdinal = v.DisplayOrdinal,
                                                          IsActive = v.Active == "Yes",
                                                          IsDefaultable = v.IsDefaultable == "Yes",
                                                          IsOrderable = v.IsOrderable == "Yes",
                                                          IsHierachical = v.IsHierachical == "Yes",
                                                          AttributeId = v.AttributeID,
                                                          DbObject = forUpdate ? v : null,

                                                          AttributeName = v.Attribute == null ? "" : v.Attribute.AttributeName,
                                                          AttributeDescription = v.Attribute == null ? "" : v.Attribute.Description
                                                      };
            return mediaItems;
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new ValueSet();

            PersistedDbObject.ValueSetID = NewBusinessObject.Id;
            PersistedDbObject.LanguageCode = NewBusinessObject.LanguageCode;

            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.ValueSetName = NewBusinessObject.Name;
            PersistedDbObject.Description = NewBusinessObject.Description;
            PersistedDbObject.DisplayOrdinal = NewBusinessObject.DisplayOrdinal;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.IsDefaultable = NewBusinessObject.IsDefaultable ? "Yes" : "No";
            PersistedDbObject.IsOrderable = NewBusinessObject.IsOrderable ? "Yes" : "No";
            PersistedDbObject.IsHierachical = NewBusinessObject.IsHierachical ? "Yes" : "No";

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.ValueSets.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.ValueSets.DeleteObject(PersistedDbObject);
        }

        public void Delete(DataSourceBusinessObject persistedBusinessObject)
        {
            TheObjectContext.MediaDbEntities.ValueSets.DeleteObject((ValueSet)persistedBusinessObject.DbObject);
        }

        public IQueryable Get(IDataServicesObjectContext dataEntities)
        {
            return Get((MediaObjectContext)dataEntities);
        }

        public object GetDictionary(IDataServicesObjectContext dataEntities)
        {
            throw new NotImplementedException();
        }
    }
}
