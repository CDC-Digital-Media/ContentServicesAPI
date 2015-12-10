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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.MediaProvider
{
    public interface IMediaProvider
    {
        MediaObject GetMedia(int mediaId, out ValidationMessages validationMessages);
        CompositeMedia GetMediaCollection<t>(int mediaId, Sorting sorting, int childLevel, int parentLevel,
            bool onlyIsPublishedHidden, bool OnlyDisplayableMediaTypes);

        ValidationMessages SaveMedia(MediaObject theObject);
        ValidationMessages SaveMedia(MediaObject theObject, out MediaObject newObject);
        ValidationMessages DeleteMedia(int id);
        
        ValidationMessages UpdateMediaWithNewPersistentUrl(int mediaId, out MediaObject theObject);

        ValidationMessages CreateSyndicationList(SyndicationListObject syndicationList);
        ValidationMessages UpdateSyndicationList(SyndicationListObject syndicationList);
        ValidationMessages UpdateMediaItemsInSyndicationList(Guid syndicationListGuid, List<SyndicationListMediaObject> mediasToAdd,
            List<SyndicationListMediaObject> mediasToDelete, string emailAddress);
        ValidationMessages DeleteSyndicationList(params Guid[] syndicationListGuids);
        SyndicationListObject GetSyndicationList(Guid myListGuid);
        SyndicationListObject GetLatestSyndicationList(Guid userGuid);
        SyndicationListObject GetSyndicationListByName(string listName);

        ValidationMessages SaveStorage(StorageObject theObject, out StorageObject newObject);
        StorageObject GetStorage(int storageId, out ValidationMessages validationMessages);
        //IList<LocationItem> GetGeo(int mediaId);

        ValidationMessages DeleteStorage(StorageObject theObject);
    }
}
