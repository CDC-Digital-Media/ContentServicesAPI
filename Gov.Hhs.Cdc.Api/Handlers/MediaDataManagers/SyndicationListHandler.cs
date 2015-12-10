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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class SyndicationListHandler : MediaHandlerBase
    {

        /// <summary>
        /// Put == Update
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// 
        public static void Update(string stream, IOutputWriter writer)
        {
            writer.Write(Update(stream));
        }
        public static ValidationMessages Update(string stream)
        {
            SerialSyndicationList serialMedia = BaseJsSerializer.Deserialize<SerialSyndicationList>(stream);
            return MediaProvider.CreateSyndicationList(CreateSyndicationListObject(serialMedia));
        }

        public static ValidationMessages Update2(string stream)
        {
            SerialSyndicationList serialMedia = BaseJsSerializer.Deserialize<SerialSyndicationList>(stream);
            return MediaProvider.CreateSyndicationList(CreateSyndicationListObject(serialMedia));
        }

        public static void Create(string stream, IOutputWriter writer)
        {
            SerialSyndicationList serialMedia = BaseJsSerializer.Deserialize<SerialSyndicationList>(stream);
            SyndicationListObject syndicationList = CreateSyndicationListObject(serialMedia);
            ValidationMessages messages = MediaProvider.CreateSyndicationList(syndicationList);
            writer.CreateAndWriteSerialResponse(CreateSerializedObject(syndicationList), messages);
        }

        public static void AddMediaItemsToSyndicationList(string stream, string id, IOutputWriter writer)
        {
            List<SyndicationListMediaObject> newMedias;
            string lastUpdatedUserEmailAddress;
            ValidationMessages messages = ParseSyndicationObjects(stream, id, out lastUpdatedUserEmailAddress, out newMedias);
            if (messages == null)
            {
                MediaProvider.UpdateMediaItemsInSyndicationList((Guid)newMedias[0].SyndicationListGuid, newMedias, null, lastUpdatedUserEmailAddress);
                Get(id, writer);

            }
            else
            {
                writer.Write(messages);
            }
        }

        public static void DeleteMediaItemsFromSyndicationList(string stream, string id, IOutputWriter writer)
        {
            List<SyndicationListMediaObject> deleteMedias;
            string lastUpdatedUserEmailAddress;
            ValidationMessages messages = ParseSyndicationObjects(stream, id, out lastUpdatedUserEmailAddress, out deleteMedias);
            if (messages == null)
            {
                MediaProvider.UpdateMediaItemsInSyndicationList((Guid)deleteMedias[0].SyndicationListGuid, null, deleteMedias, lastUpdatedUserEmailAddress);
                Get(id, writer);
            }
            else
            {
                writer.Write(messages);
            }
        }

        private static ValidationMessages ParseSyndicationObjects(string stream, string id, out string lastUpdatedUserEmailAddress, out List<SyndicationListMediaObject> medias)
        {
            SerialSyndicationList serialMedia = BaseJsSerializer.Deserialize<SerialSyndicationList>(stream);
            lastUpdatedUserEmailAddress = serialMedia.lastUpdatedUserEmailAddress;

            medias = null;

            Guid listId;
            if (!Guid.TryParse(id, out listId))
            {
                return ValidationMessages.CreateError("SyndicationListId", "Invalid syndication list id");
            }

            //SyndicationListObject syndicationList = CreateSyndicationListObject(serialMedia);
            if (serialMedia.media == null)
            {
                return ValidationMessages.CreateError("Media", "No media items were selected");
            }

            medias = serialMedia.media.Select(m => CreateSyndicationListMediaObject(m, listId)).ToList();
            return null;
        }


        public static void Delete(string syndicationListGuid, IOutputWriter writer)
        {
            writer.Write(MediaProvider.DeleteSyndicationList(ParseGuid(syndicationListGuid, "syndicationList")));
        }

        public static void Get(string syndicationListGuid, IOutputWriter writer)
        {
            if (string.IsNullOrEmpty(syndicationListGuid))
            {
                writer.Write(ValidationMessages.CreateError("SyndicationListGuid", "SyndicationListGuid is required"));
                return;
            }

            SerialSyndicationList serialSyndicationList = Get(syndicationListGuid);
            if (serialSyndicationList == null)
            {
                writer.Write(ValidationMessages.CreateError("SyndicationListGuid", "SyndicationList is not found"));
            }
            else
            {
                writer.Write(new SerialResponse(serialSyndicationList));
            }
        }

        public static SerialSyndicationList Get(string syndicationListGuid)
        {
            SyndicationListObject syndicationList = MediaProvider.GetSyndicationList(ParseGuid(syndicationListGuid, "syndicationList"));
            SerialSyndicationList serialSyndicationList = syndicationList == null ? null : CreateSerializedObject(syndicationList);
            return serialSyndicationList;
        }

        public static void GetByName(string listName, IOutputWriter writer)
        {
            SyndicationListObject syndicationList = MediaProvider.GetSyndicationListByName(listName);
            SerialSyndicationList serialSyndicationList = syndicationList == null ? null : CreateSerializedObject(syndicationList);
            writer.Write(syndicationList, null);
        }

        public static SerialSyndicationList GetLatest(string userGuid)
        {
            SyndicationListObject syndicationList = MediaProvider.GetLatestSyndicationList(ParseGuid(userGuid, "user"));
            if (syndicationList == null)
            {
                return null;
            }
            else
            {
                return CreateSerializedObject(syndicationList);
            }

        }



        private static SyndicationListObject CreateSyndicationListObject(SerialSyndicationList serialSyndicationList, int id = 0)
        {
            Guid? syndicationListGuid = ParseGuid(serialSyndicationList.syndicationListId);
            Guid? userGuid = ParseGuid(serialSyndicationList.userId);
            return new SyndicationListObject()
            {
                SyndicationListGuid = syndicationListGuid,
                DomainName = serialSyndicationList.domainName,
                UserGuid = userGuid,
                ListName = serialSyndicationList.listName,
                IsActive = serialSyndicationList.isActive,
                SyndicationListStatusCode = serialSyndicationList.statusCode,
                Medias = serialSyndicationList.media.Select(m => CreateSyndicationListMediaObject(m, syndicationListGuid)),
                RowVersion = serialSyndicationList.rowVersion.ToBytes()
            };
        }

        private static SyndicationListMediaObject CreateSyndicationListMediaObject(SerialSyndicationListMedia serialSyndicationListMedia, Guid? syndicationListGuid)
        {
            return new SyndicationListMediaObject()
            {
                SyndicationListGuid = syndicationListGuid,
                MediaId = serialSyndicationListMedia.mediaId,
                HasPulledCode = serialSyndicationListMedia.hasPulledCode,
                LastPulledCodeDateTime = serialSyndicationListMedia.lastPulledCodeDateTime,
                IsActive = true,
                //RowVersion = serialSyndicationListMedia.rowVersion.ToBytes()
            };
        }


        private static SerialSyndicationList CreateSerializedObject(SyndicationListObject syndicationList)
        {
            return new SerialSyndicationList()
            {
                syndicationListId = syndicationList.SyndicationListGuid.ToString(),
                domainName = syndicationList.DomainName,
                userId = syndicationList.UserGuid.ToString(),
                listName = syndicationList.ListName,
                isActive = syndicationList.IsActive,
                statusCode = syndicationList.SyndicationListStatusCode,
                rowVersion = syndicationList.RowVersion.ToBase64String(),
                media = syndicationList.Medias.Select(m => CreateSerializedObject(m)).ToList()

            };
        }

        private static SerialSyndicationListMedia CreateSerializedObject(SyndicationListMediaObject media)
        {
            return new SerialSyndicationListMedia()
            {
                hasPulledCode = media.HasPulledCode,
                lastPulledCodeDateTime = media.LastPulledCodeDateTime,
                mediaId = media.MediaId,

                //rowVersion = media.RowVersion.ToBase64String(),
            };
        }

    }

}
