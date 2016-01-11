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

using System.Configuration;
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;

namespace Gov.Hhs.Cdc.Api
{
    public class FeedImportMgrHandler : MediaHandlerBase
    {
        /// <summary>
        /// Update "Feed - Import" using the sourceUrl.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="stream"></param>
        /// <param name="appKey"></param>
        /// <param name="mediaId"></param>
        /// <param name="adminUser"></param>
        public static void Update(IOutputWriter writer, string stream, string appKey, int mediaId, AdminUser adminUser, ICallParser parser)
        {
            if (!UserCanEdit(writer, adminUser) || (mediaId <= 0))
            {
                return;
            }

            try
            {
                ValidationMessages messages = new ValidationMessages();

                List<MediaObject> resultMediaList = new List<MediaObject>();
                if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(appKey) != null)
                {
                    var criteria = new SearchCriteria { MediaId = mediaId.ToString() };
                    MediaObject theObject = CsMediaSearchProvider.Search(criteria).FirstOrDefault();
                    if (theObject != null)
                    {
                        //Is this youtube
                        if (theObject.SourceUrl.ToLower().Contains("googleapis.com"))
                        {
                            theObject = FeedImport.CreateYouTubeFeedMedia(theObject, adminUser);
                        }
                        else
                        {
                            theObject = FeedImport.CreateMedia(theObject, adminUser);
                        }
                    }
                    else
                    {
                        writer.Write(ValidationMessages.CreateError("MediaId", "Invalid mediaId"));
                        return;
                    }

                    if (theObject != null && theObject.Children != null)
                    {
                        criteria = new SearchCriteria { ParentId = mediaId.ToString() };
                        var existingFeedItems = CsMediaSearchProvider.Search(criteria);

                        // check if param was passed in
                        if (parser.ParamDictionary.ContainsKey(ApiParam.deactivatemissingitems.ToString()))
                        {
                            // convert value to bool
                            bool deactivate = false;
                            bool.TryParse(parser.ParamDictionary[ApiParam.deactivatemissingitems.ToString()], out deactivate);

                            if (deactivate)
                            {
                                // get items to deactivate
                                var urlLists = theObject.Children.Select(a => a.SourceUrl);
                                var feedItemsToDeactivate = existingFeedItems.Where(fi => !urlLists.Contains(fi.SourceUrl) && fi.MediaStatusCode != "Archived").ToList();
                                if (feedItemsToDeactivate != null && feedItemsToDeactivate.Count > 0)
                                {
                                    foreach (var item in feedItemsToDeactivate)
                                    {
                                        item.MediaStatusCode = "Archived";

                                        // add relationship
                                        item.MediaRelationships = new List<MediaRelationshipObject>()                    
                                        {
                                            new MediaRelationshipObject ()
                                            {
                                                IsActive = true,
                                                IsCommitted = true,
                                                RelationshipTypeName = "Is Child Of",
                                                RelatedMediaId = theObject.Id == 0 ? theObject.MediaId : theObject.Id,
                                                MediaId =item.Id == 0 ? item.MediaId : item.Id
                                            }
                                        }.AsEnumerable();

                                        messages.Add(MediaProvider.SaveMedia(item));
                                    }
                                }
                            }
                        }

                        int i = 0;
                        string timestamp = ""; // passed in from API call to populate response. need this to find freshly imported items.
                        if (parser.ParamDictionary.ContainsKey(ApiParam.timestamp.ToString()))
                        {
                            timestamp = parser.ParamDictionary[ApiParam.timestamp.ToString()];
                        }

                        var itemsToBulkSave = new List<MediaObject>();
                        foreach (var item in theObject.Children)
                        {
                            i++;

                            // add timestamp using extended attributes so that we can identify when the record was imported
                            // used to assist in determining when import is finished.
                            addImportTimestamp(theObject.Children.Count, i, timestamp, item);

                            var mediaToUpdate = existingFeedItems.Where(fi => fi.SourceUrl == item.SourceUrl).FirstOrDefault();

                            if (mediaToUpdate != null)
                            {
                                // if exists = update existing item
                                mediaToUpdate.Name = item.Name;
                                mediaToUpdate.Title = item.Title;
                                mediaToUpdate.Description = item.Description;
                                mediaToUpdate.MediaStatusCode = item.MediaStatusCode;
                                mediaToUpdate.PublishedDateTime = item.PublishedDateTime;
                                mediaToUpdate.ModifiedByGuid = item.ModifiedByGuid;
                                mediaToUpdate.ModifiedDateTime = item.ModifiedDateTime;
                                mediaToUpdate.MediaRelationships = item.MediaRelationships;
                                mediaToUpdate.Enclosures = item.Enclosures;
                                mediaToUpdate.ExtendedAttributes = item.ExtendedAttributes;
                                mediaToUpdate.MediaStatusCode = "Published";

                                itemsToBulkSave.Add(mediaToUpdate);
                            }
                            else
                            {
                                // if not exists = add new item
                                item.MediaStatusCode = "Published";
                                itemsToBulkSave.Add(item);
                            }

                        }

                        MediaProvider.MediaBulkSave(itemsToBulkSave);
                    }

                    resultMediaList = CsMediaSearchProvider.Search(criteria).ToList();
                }
                else
                {
                    messages.AddError("Media", "Invalid Request");
                }

                //TODO: the initial design did not use the writer in a MgrHandler.
                // this will only work for version 1 of admin
                writer.Write(
                    TransformationFactory.GetMediaTransformation(1, false).CreateSerialResponse(resultMediaList),
                    messages);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                var messages = new ValidationMessages();
                messages.AddError("", "", "Exception has been logged updating the media object", ex.Message);
                writer.Write(messages);
            }
        }

        private static void addImportTimestamp(int countTotal, int i, string timestamp, MediaObject item)
        {
            List<ExtendedAttribute> newlist = item.ExtendedAttributes == null ? new List<ExtendedAttribute>() : item.ExtendedAttributes.ToList();
            ExtendedAttribute eaLastImport = new ExtendedAttribute()
            {
                Name = "LastImport",
                Value = timestamp
            };
            newlist.Remove(newlist.Where(x => x.Name == "LastImport").FirstOrDefault());
            newlist.Add(eaLastImport);

            ExtendedAttribute eaImportCount = new ExtendedAttribute()
            {
                Name = "ImportCount",
                Value = i.ToString() + " of " + countTotal
            };
            newlist.Remove(newlist.Where(x => x.Name == "ImportCount").FirstOrDefault());
            newlist.Add(eaImportCount);

            item.ExtendedAttributes = newlist;
        }


    }
}
