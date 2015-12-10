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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class ECardObjectCtl : BaseCtl<ECardDetail, Card, ECardObjectCtl, MediaObjectContext>    
    {
        public override void UpdateIdsAfterInsert()
        {
            //This is handled by the CombinedMediaItem
        }

        public override string ToString()
        {
            return PersistedBusinessObject.GetType().Name;
        }
        
        public override bool DbObjectEqualsBusinessObject()
        {
            return false;
        }

        public override bool VersionMatches()
        {
            return true;    // mediaObjectCtl.VersionMatches();
        }

        public override object Get(bool forUpdate)
        {
            return Get(TheObjectContext, forUpdate);
        }

        public static IQueryable<ECardDetail> Get(MediaObjectContext media, bool forUpdate=false)
        {
            IQueryable<ECardDetail> cardItems = from c in media.MediaDbEntities.Cards
                                                select new ECardDetail()
                                                {
                                                    MediaId = c.MediaId,
                                                    MobileCardName = c.MobileCardName,
                                                    Html5Source = c.HTML5Source,
                                                    Caption = c.Caption,
                                                    CardText = c.CardText,
                                                    CardTextOutside = c.CardTextOutside,
                                                    CardTextInside = c.CardTextInside,
                                                    ImageSourceInsideLarge = c.ImageSourceInsideLarge,
                                                    ImageSourceInsideSmall = c.ImageSourceInsideSmall,
                                                    ImageSourceOutsideLarge = c.ImageSourceOutsideLarge,
                                                    ImageSourceOutsideSmall = c.ImageSourceOutsideSmall,
                                                    IsMobile = c.Mobile == "Yes",
                                                    MobileTargetUrl = c.MobileTargetUrl,
                                                    DisplayOrdinal = c.DisplayOrdinal,

                                                    IsActive = c.Active == "Yes",
                                                    
                                                    DbObject = forUpdate ? c : null
                                                };
            return cardItems;
        }

        public static IQueryable<ECardDetail> GetWithoutMedia(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<ECardDetail> cardItems = from c in media.MediaDbEntities.Cards
                                                //join m in mediaItems on c.MediaId equals m.Id
                                                select new ECardDetail()
                                                {
                                                    MediaId = c.MediaId,
                                                    MobileCardName = c.MobileCardName,
                                                    Html5Source = c.HTML5Source,
                                                    Caption = c.Caption,
                                                    CardText = c.CardText,
                                                    CardTextOutside = c.CardTextOutside,
                                                    CardTextInside = c.CardTextInside,
                                                    ImageSourceInsideLarge = c.ImageSourceInsideLarge,
                                                    ImageSourceInsideSmall = c.ImageSourceInsideSmall,
                                                    ImageSourceOutsideLarge = c.ImageSourceOutsideLarge,
                                                    ImageSourceOutsideSmall = c.ImageSourceOutsideSmall,
                                                    IsMobile = c.Mobile == null ? false : c.Mobile == "Yes",
                                                    MobileTargetUrl = c.MobileTargetUrl,
                                                    DisplayOrdinal = c.DisplayOrdinal,
                                                    IsActive = c.Active == "Yes",

                                                    DbObject = forUpdate ? c : null
                                                };
            return cardItems;
        }



        public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
            //DB Objects are not inherited, so treat them separately
            PersistedDbObject = new Card();
            PersistedDbObject.CreatedDateTime = modifiedDateTime;
            PersistedDbObject.CreatedByGuid = modifiedGuid;
        }

        public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
        {
 	        PersistedDbObject.MobileCardName = NewBusinessObject.MobileCardName;
	        PersistedDbObject.HTML5Source = NewBusinessObject.Html5Source;
	        PersistedDbObject.Caption = NewBusinessObject.Caption;
	        PersistedDbObject.CardText = NewBusinessObject.CardText;
	        PersistedDbObject.CardTextOutside = NewBusinessObject.CardTextOutside;
	        PersistedDbObject.CardTextInside = NewBusinessObject.CardTextInside;
	        PersistedDbObject.ImageSourceInsideLarge = NewBusinessObject.ImageSourceInsideLarge;
	        PersistedDbObject.ImageSourceInsideSmall = NewBusinessObject.ImageSourceInsideSmall;
	        PersistedDbObject.ImageSourceOutsideLarge = NewBusinessObject.ImageSourceOutsideLarge;
	        PersistedDbObject.ImageSourceOutsideSmall = NewBusinessObject.ImageSourceOutsideSmall;
	        PersistedDbObject.Mobile = NewBusinessObject.IsMobile ? "Yes" : "No";
            PersistedDbObject.MobileTargetUrl = NewBusinessObject.MobileTargetUrl;
	        PersistedDbObject.DisplayOrdinal = NewBusinessObject.DisplayOrdinal;
            PersistedDbObject.Active = NewBusinessObject.IsActive ? "Yes" : "No";

            PersistedDbObject.ModifiedDateTime = modifiedDateTime;
            PersistedDbObject.ModifiedByGuid = modifiedGuid;
        }

        public override void AddToDb()
        {
        }

        public override void Delete()
        {
            TheObjectContext.MediaDbEntities.Cards.DeleteObject(PersistedDbObject);
        }

        public void AddToMedia(MediaCtl reference)
        {
            reference.PersistedDbObject.Card = PersistedDbObject;
        }
        
    }
}
