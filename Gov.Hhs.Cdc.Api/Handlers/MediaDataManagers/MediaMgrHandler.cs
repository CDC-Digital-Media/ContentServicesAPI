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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.MediaProvider;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Gov.Hhs.Cdc.Api
{
    public class MediaMgrHandler : MediaHandlerBase
    {
        /// <summary>
        /// Put == Update
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="idAsInt"></param>
        /// <returns></returns>
        public static void Update(IOutputWriter writer, string stream, string appKey, int mediaId, AdminUser adminUser)
        {
            if (!UserCanEdit(writer, adminUser))
            {
                return;
            }

            SerialMediaAdmin serialMedia = null;
            try
            {
                serialMedia = BaseJsSerializer.Deserialize<SerialMediaAdmin>(stream);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Logger.LogError("Invalid format of serialized media object: " + stream, "Save");
                writer.Write(ValidationMessages.CreateError("media", "Invalid format of serialized media object"));
                return;
            }

            try
            {
                ValidationMessages messages = new ValidationMessages();

                List<MediaObject> resultMediaList = new List<MediaObject>();
                if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(appKey) != null)
                {
                    MediaObject updatedObject;
                    serialMedia.mediaId = mediaId.ToString();
                    MediaObject theObject = new MediaAdminTransformation().CreateNewMediaAndDetail(serialMedia, adminUser.UserGuid);

                    messages.Add(MediaProvider.SaveMedia(theObject, out updatedObject));

                    if (updatedObject != null)
                    {
                        resultMediaList.Add(updatedObject);
                    }
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
                return;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                var messages = new ValidationMessages();
                messages.AddError("", "", "Exception has been logged updating the media object", ex.Message);
                writer.Write(messages);
            }
        }

        private static SerialMediaAdmin DataIsValid(IOutputWriter writer, string stream)
        {
            SerialMediaAdmin serialMedia = null;
            try
            {
                serialMedia = BaseJsSerializer.Deserialize<SerialMediaAdmin>(stream);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Logger.LogError("Invalid format of serialized media object: " + stream, "Save");
                writer.Write(ValidationMessages.CreateError("media", "Invalid format of serialized media object"));
                return null;
            }
            return serialMedia;
        }

        public static void Insert(IOutputWriter writer, string stream, string apiKey, AdminUser adminUser, ICallParser parser)
        {
            if (!UserCanEdit(writer, adminUser))
            {
                return;
            }
            var serialMedia = DataIsValid(writer, stream);
            if (adminUser == null)
            {
                return;
            }
            if (serialMedia == null)
            {
                return;
            }

            try
            {
                ValidationMessages messages = new ValidationMessages();

                List<MediaObject> resultMediaList = new List<MediaObject>();
                if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(apiKey) != null)
                {
                    PerformInsert(messages, serialMedia, adminUser, parser, ref resultMediaList);
                }
                else
                {
                    messages.AddError("Media", "Invalid Request");
                }

                //TODO: the initial design did not use the writer in a MgrHandler.
                // this will only work for version 1 of admin
                writer.Write(TransformationFactory.GetMediaTransformation(1, false).CreateSerialResponse(resultMediaList),
                    messages);
                return;
            }
            catch (Exception ex)
            {
                var message = "Exception has been logged inserting the media object";
                Logger.LogError(ex, ex.Message, parser.Query.CacheKey);
                var messages = new ValidationMessages();
                messages.AddError("", "", message, ex.Message);
                writer.Write(messages);
            }
        }

        private static void PerformInsert(ValidationMessages messages, SerialMediaAdmin serialMedia, AdminUser adminUser, ICallParser parser, ref List<MediaObject> resultMediaList)
        {
            MediaObject updatedObject;
            serialMedia.mediaId = 0.ToString();

            MediaObject theObject = new MediaAdminTransformation().CreateNewMediaAndDetail(serialMedia, adminUser.UserGuid);
            if (theObject == null)
            {
                return;
            }
            if (theObject.MediaTypeCode == MediaTypeParms.DefaultHtmlMediaType)
            {
                ValidateHtml(messages, theObject);
            }
            if (messages.NumberOfErrors > 0) { return; }

            if (MediaProvider == null)
            {
                return;
            }
            messages.Add(MediaProvider.SaveMedia(theObject, out updatedObject));
            if (messages.NumberOfErrors > 0) { return; }

            if (!theObject.MediaTypeParms.IsFeedItem && !theObject.MediaTypeParms.IsFeed)
            {
                updatedObject = UpdateEmbedCodeSave(messages, parser, updatedObject);
            }

            if (updatedObject != null)
            {
                resultMediaList.Add(updatedObject);
            }
        }

        private static MediaObject UpdateEmbedCodeSave(ValidationMessages messages, ICallParser parser, MediaObject updatedObject)
        {
            //update embedcode and save media
            ICallParser copy_parse = parser;
            copy_parse.Version = Convert.ToInt32(ConfigurationManager.AppSettings["PublicApiVersion"]);
            var mediaEmbedService = new MediaEmbedSearchHandler(copy_parse, updatedObject.MediaId.ToString());
            updatedObject.Embedcode = mediaEmbedService.GetCode(updatedObject);


            messages.Add(MediaProvider.SaveMedia(updatedObject, out updatedObject));
            return updatedObject;
        }

        private static void ValidateHtml(ValidationMessages messages, MediaObject theObject)
        {
            var result = new CsMediaValidationProvider().ValidateHtmlForUrl(theObject);
            if (result.MediaAddress.AddressIsAlreadyPersistedWithSameExtractionCriteria)
            {
                messages.AddError("Media", "This content has already been syndicated.");
                Logger.LogInfo("Attempt to log duplicate media, url: " + theObject.SourceUrl);
            }
        }

        public static ValidationMessages Delete(int id)
        {
            return MediaProvider.DeleteMedia(id);
        }

        public static int GetIdAsInteger(SerialMediaAdmin media)
        {
            int intId = 0;
            int.TryParse(media.mediaId, out intId);
            if (intId == 0)
            {
                int.TryParse(media.id, out intId);
            }
            return intId;
        }

        public static ValidationMessages RegeneratePersistentUrl(SerialMediaAdmin media)
        {
            MediaObject updatedObj = null;

            ValidationMessages messages = MediaProvider.UpdateMediaWithNewPersistentUrl(GetIdAsInteger(media), out updatedObj);
            media.id = updatedObj.Id.ToString();
            return messages;
        }
    }

}
