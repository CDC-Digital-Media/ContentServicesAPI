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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class NoScriptSearchHandler
    {
        private static IEnumerable<MediaObject> Get(List<Criterion> criteria)
        {
            ValidationMessages messages = new ValidationMessages();

            SearchParameters searchParameters = new SearchParameters("Media", "Media", criteria.ToArray());

            DataSetResult dataSet = ServiceUtility.GetDataSet(searchParameters, out messages);

            return dataSet.Records.Cast<MediaObject>();
        }

        public static string RedirectUrl(int mediaId)
        {
            string url = "";

            List<Criterion> criteria =
                new List<Criterion>() 
                    { 
                        new Criterion("MediaId", mediaId),
                        new Criterion("OnlyDisplayableMediaTypes", false), // used to exclude or include secondary mediatype i.e. transcript, etc
                        new Criterion("EffectiveStatus", 
                            new List<string>() { 
                                Param.MediaStatus.Published.ToString(),
                                Param.MediaStatus.Hidden.ToString()
                            })
                    };

            var mediaItem = Get(criteria).FirstOrDefault();
            if (mediaItem != null)
            {
                if (mediaItem.MediaTypeParms.CreateEmbedHtml || mediaItem.MediaTypeParms.IsVideo || mediaItem.MediaTypeParms.IsWidget)
                {
                    url = mediaItem.SourceUrl;
                }
                else if (mediaItem.MediaTypeParms.IsStaticImageMedia || mediaItem.MediaTypeParms.IsEcard || mediaItem.MediaTypeParms.IsMicrosite)
                {
                    url = mediaItem.TargetUrl;
                }
            }

            if (!string.IsNullOrEmpty(url))
            {
                ServiceUtility.AddBasicApplicationCache(mediaId + "|noscript", new Uri(url));
            }

            return url;
        }


    }

}
