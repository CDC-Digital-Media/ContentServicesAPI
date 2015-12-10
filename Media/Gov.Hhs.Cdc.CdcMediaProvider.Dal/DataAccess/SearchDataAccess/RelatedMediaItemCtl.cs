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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public static class RelatedMediaItemCtl
    {
        public static IEnumerable<RelatedMediaObject> Get(MediaObjectContext media, int mediaId, int toChildLevel, int toParentLevel)
        {
            IEnumerable<RelatedMediaObject> mediaItems =
                                              from m in media.MediaDbEntities.GetMediaRelationships(mediaId, toChildLevel, toParentLevel)
                                              select new RelatedMediaObject()
                                              {
                                                  Id = (int) m.MediaId,                                                  
                                                  MediaTypeCode = m.MediaTypeCode,
                                                  MimeTypeCode = m.MimeTypeCode,
                                                  RelationshipTypeName = m.RelationshipTypeName,
                                                  RelatedMediaId = m.RelatedMediaId,
                                                  Level = m.Lvl,
                                                  LanguageCode = m.LanguageCode,

                                                  Title = m.Title,
                                                  Description = m.Description,

                                                  SourceUrl = m.SourceUrl,
                                                  TargetUrl = m.TargetUrl,

                                                  MediaStatusCode = m.MediaStatusCode,
                                                  EffectiveStatusCode = m.EffectiveStatus,
                                                  DisplayOnSearch = m.DisplayOnSearch == "Yes", // && m.MediaType.DisplayOrdinal > 0,

                                                  ActiveDate = m.PublishedDateTime,
                                                  ModifiedDate = m.ModifiedDateTime,
                                                  CreatedDate = m.CreatedDateTime
                                              };
            return mediaItems;
        }


    }
}
