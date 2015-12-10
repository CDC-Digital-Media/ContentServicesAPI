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
    //This is not currently used, but is left here as an example

    //public class HtmlObjectCtl : BaseCtl<HtmlObject, object, HtmlObjectCtl, MediaObjectContext>
    //{

    //    public override void UpdateIdsAfterInsert()
    //    {
    //        //This is handled by the MediaObject
    //        //PersistedECardBusinessObject.Id = PersistedECardDbObject.MediaId;
    //    }

    //    public override string ToString()
    //    {
    //        return PersistedBusinessObject.GetType().Name;
    //    }

    //    public override bool DbObjectEqualsBusinessObject()
    //    {
    //        return true;
    //        //return
    //        //    (PersistedDbObject.LinkText == NewECardBusinessObject.LinkText
    //        //    && PersistedDbObject.TextFront == NewECardBusinessObject.TextFront
    //        //    && PersistedDbObject.TextInside == NewECardBusinessObject.TextInside
    //        //    && PersistedDbObject.HTML5Code == NewECardBusinessObject.Html5Code
    //        //    && PersistedDbObject.DisplayOrdinal == NewECardBusinessObject.DisplayOrdinal);
    //    }

    //    public override bool VersionMatches()
    //    {
    //        //TODO: Work on VersionMatches
    //        return true;    // mediaObjectCtl.VersionMatches();
    //    }

    //    public override object Get(bool forUpdate)
    //    {
    //        return Get(TheObjectContext, forUpdate);
    //    }

    //    public static IQueryable<HtmlObject> Get(MediaObjectContext media, bool forUpdate)
    //    {
    //        IQueryable<MediaObject> mediaItems = MediaCtl.Get(media, forUpdate);
    //        IQueryable<HtmlObject> htmlItems = from m in mediaItems
    //                                            select new HtmlObject()
    //                                            {
    //                                                Media = m
    //                                            };
    //        return htmlItems;
    //    }

    //    public override void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid)
    //    {
    //        //DB Objects are not inherited, so treat them separately
    //        //mediaObjectCtl.SetInitialValues(modifiedDateTime, modifiedGuid);

    //        //PersistedDbObject = new Card();
    //        //PersistedDbObject.CreatedDateTime = modifiedDateTime;
    //        //PersistedDbObject.CreatedByGuid = modifiedGuid;


    //    }

    //    public override void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid)
    //    {
    //        //mediaObjectCtl.SetUpdatableValues(modifiedDateTime, modifiedGuid);

    //        //PersistedDbObject.LinkText = NewECardBusinessObject.LinkText;
    //        //PersistedDbObject.TextFront = NewECardBusinessObject.TextFront;
    //        //PersistedDbObject.TextInside = NewECardBusinessObject.TextInside;
    //        //PersistedDbObject.HTML5Code = NewECardBusinessObject.Html5Code;
    //        //PersistedDbObject.DisplayOrdinal = NewECardBusinessObject.DisplayOrdinal;

    //        //PersistedDbObject.Active = NewECardBusinessObject.IsActive ? "Yes" : "No";

    //        //PersistedDbObject.ModifiedDateTime = modifiedDateTime;
    //        //PersistedDbObject.ModifiedByGuid = modifiedGuid;
    //    }



    //    public override void AddToDb()
    //    {
    //        //This is added to the appropriate Media object
    //        //mediaObjectCtl.AddReference(PersistedECardDbObject);
    //        //mediaObjectCtl.AddToDb();   //Add this and the eCard object to the datbase
    //    }

    //    public override void Delete()
    //    {
    //        //Media.Cards.DeleteObject(PersistedDbObject);
    //    }





    //}
}
