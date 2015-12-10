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
using System.Web;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class ValueCtl : BaseCtl<ValueObject, Value, ValueCtl, MediaObjectContext>
    {
        public override void UpdateIdsAfterInsert()
        {
            PersistedBusinessObject.ValueId = PersistedDbObject.ValueID;            
        }

        public override string ToString()
        {            
            return PersistedBusinessObject.GetType().Name + ", ValueID=" + PersistedBusinessObject.ValueId.ToString();
        }

        public override bool DbObjectEqualsBusinessObject()
        {
            return PersistedDbObject.ValueID == NewBusinessObject.ValueId
                && PersistedDbObject.ValueName == NewBusinessObject.ValueName
                && PersistedDbObject.LanguageCode == NewBusinessObject.LanguageCode
                && PersistedDbObject.Description == NewBusinessObject.Description
                && PersistedDbObject.DisplayOrdinal == NewBusinessObject.DisplayOrdinal
                && AsBool(PersistedDbObject.Active) == NewBusinessObject.IsActive;
        }

        public override bool VersionMatches()
        {
            //TODO: Add Row Version matching
            //return PersistedDbObject.RowVersion.SequenceEqual(NewBusinessObject.RowVersion);
            return true;
        }

        public override object Get(bool forUpdate)
        {
            return GetWithChildren(TheObjectContext, forUpdate);
        }


        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject = new Value()
            {
                ValueID = NewBusinessObject.ValueId,
                LanguageCode = HttpUtility.HtmlDecode(NewBusinessObject.LanguageCode ?? ""),

                CreatedDateTime = modifiedDateTime,
                CreatedByGuid = modifiedGuid
            };
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            PersistedDbObject.ValueName =  HttpUtility.HtmlDecode(NewBusinessObject.ValueName ?? "");
            PersistedDbObject.Description = HttpUtility.HtmlDecode(NewBusinessObject.Description ?? "");

            PersistedDbObject.DisplayOrdinal = NewBusinessObject.DisplayOrdinal;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";
                        
            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;                       
        }

        public override void AddToDb()
        {
            TheObjectContext.MediaDbEntities.Values.AddObject(PersistedDbObject);

            //We don't have the ID yet, and will have to get it after the commit.
            //NewBusinessObject.Id = PersistedDbObject.MediaId;
            //PersistedBusinessObject.Id = PersistedDbObject.MediaId;
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.Values.DeleteObject(PersistedDbObject);
        }

        public static IQueryable<ValueObject> GetWithChildren(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<ValueToValueSetObject> valueToValueSets = ValueToValueSetCtl.Get(media, forUpdate);
            IQueryable<ValueRelationshipObject> valueRelationships = ValueRelationshipCtl.Get(media, forUpdate);

            IQueryable<ValueObject> mediaItems = from v in media.MediaDbEntities.Values
                                                 select new ValueObject()
                                                 {
                                                     ValueId = v.ValueID,
                                                     ValueName = v.ValueName,
                                                     LanguageCode = v.LanguageCode,
                                                     Description = v.Description,
                                                     DisplayOrdinal = v.DisplayOrdinal,
                                                     IsActive = v.Active == "Yes",
                                                     ValueToValueSetObjects = valueToValueSets
                                                        .Where(vvs => new { ValueID = vvs.ValueId, LanguageCode = vvs.ValueLanguageCode } == new { v.ValueID, v.LanguageCode }),
                                                     ValueRelationships = valueRelationships.Where(vr => vr.ValueId == v.ValueID),
                                                     //ValueRelationships = v.ValueRelationships1.Select(vr =>
                                                     //new ValueRelationshipObject()
                                                     //{
                                                     //    RelationshipTypeName = vr.RelationshipTypeName,
                                                     //    ValueId = v.ValueID,
                                                     //    ValueLanguageCode = vr.ValueLanguageCode,
                                                     //    RelatedValueId = vr.RelatedValueID,
                                                     //    RelatedValueLanguageCode = vr.RelatedValueLanguageCode,
                                                     //    DisplayOrdinal = vr.DisplayOrdinal,
                                                     //    IsActive = vr.Active == "Yes"
                                                     //}),
                                                     DbObject = forUpdate ? v : null
                                                 };
            return mediaItems;
        }

        public static IQueryable<ValueObject> Get(MediaObjectContext media)
        {
            IQueryable<ValueObject> mediaItems =
                                              from v in media.MediaDbEntities.Values
                                              select new ValueObject()
                                              {
                                                  ValueId = v.ValueID,
                                                  ValueName = v.ValueName,
                                                  LanguageCode = v.LanguageCode,
                                                  Description = v.Description,
                                                  DisplayOrdinal = v.DisplayOrdinal,
                                                  IsActive = (v.Active == "Yes"),
                                              };
            return mediaItems.AsQueryable();
        }

        public static IQueryable<ListItem> GetListItems(MediaObjectContext media, List<string> valueSetNames)
        {

            IQueryable<ListItem> mediaItems = from m in media.MediaDbEntities.Values
                                              where (m.ValueToValueSets.Where(vvs => valueSetNames.Contains(vvs.ValueSet.ValueSetName))).Any()
                                              //where m.Active == "Yes"
                                              select new ListItem()
                                              {
                                                  EfSafeKeyType = (int)ListItem.KeyType.IntKey,
                                                  IntKey = m.ValueID,
                                                  Code = m.ValueName,
                                                  LongDisplayName = m.ValueName,
                                                  DisplayName = m.ValueName,
                                                  DisplayOrdinal = m.DisplayOrdinal
                                              };
            return mediaItems.AsQueryable();
        }


    }


}
