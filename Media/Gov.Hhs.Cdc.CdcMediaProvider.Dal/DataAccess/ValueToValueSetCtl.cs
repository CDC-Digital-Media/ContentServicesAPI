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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{

    public class ValueToValueSetCtl : BaseCtl<ValueToValueSetObject, ValueToValueSet, ValueToValueSetCtl, MediaObjectContext>
    {
        public override void UpdateIdsAfterInsert()
        {
            //PersistedBusinessObject.ValueID = PersistedDbObject.ValueID;
            //TODO
            //Write comments here on why we throw a not implemented exception
            throw new NotImplementedException("Not implemented on purpose");
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return PersistedDbObject.ValueID == NewBusinessObject.ValueId
                && PersistedDbObject.ValueSetID == NewBusinessObject.ValueSetId
                && PersistedDbObject.ValueLanguageCode == NewBusinessObject.ValueLanguageCode
                && PersistedDbObject.ValueSetLanguageCode == NewBusinessObject.ValueSetLanguageCode
                && PersistedDbObject.ValueToValueSetName == NewBusinessObject.ValueToValueSetName
                && PersistedDbObject.DisplayOrdinal == NewBusinessObject.DisplayOrdinal
                && AsBool(PersistedDbObject.IsDefault) == NewBusinessObject.IsDefault;
        }

        public override bool VersionMatches()
        {
            //TODO: Add Row Version matching
            //return PersistedDbObject.RowVersion.SequenceEqual(NewBusinessObject.RowVersion);
            return true;

        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new ValueToValueSet()
            {
                ValueID = NewBusinessObject.ValueId,
                ValueSetID = NewBusinessObject.ValueSetId,
                ValueLanguageCode = NewBusinessObject.ValueLanguageCode,
                ValueSetLanguageCode = NewBusinessObject.ValueSetLanguageCode,
                ValueToValueSetName = NewBusinessObject.ValueToValueSetName,
                DisplayOrdinal = NewBusinessObject.DisplayOrdinal,
                IsDefault = NewBusinessObject.IsDefault ? "Yes" : "No"
            };
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.ValueToValueSetName = NewBusinessObject.ValueToValueSetName;
            PersistedDbObject.DisplayOrdinal = NewBusinessObject.DisplayOrdinal;
            PersistedDbObject.IsDefault = NewBusinessObject.IsDefault ? "Yes" : "No";
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.ValueToValueSets.AddObject(PersistedDbObject);
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.ValueToValueSets.DeleteObject(PersistedDbObject);
        }

        public static IQueryable<ValueToValueSetObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            return Get(media.MediaDbEntities.ValueToValueSets, forUpdate);
        }

        public static IQueryable<ValueToValueSetObject> Get(IQueryable<ValueToValueSet> valueToValueSets, bool forUpdate = false)
        {

            return valueToValueSets.Select(v => new ValueToValueSetObject()
            {
                ValueId = v.ValueID,
                ValueLanguageCode = v.ValueLanguageCode,
                ValueSetId = v.ValueSetID,
                ValueSetLanguageCode = v.ValueSetLanguageCode,
                ValueToValueSetName = v.ValueToValueSetName,
                DisplayOrdinal = v.DisplayOrdinal,
                IsDefault = v.IsDefault == "Yes",
                DbObject = forUpdate ? v : null,
                
                ValueSetName = v.ValueSet.ValueSetName
            });
        }


    }
}
