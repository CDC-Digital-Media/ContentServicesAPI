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
using System.Web;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class RelatedMediaTransformationAdmin : IRelatedMediaTransformation
    {
        /// <summary>
        /// Only navigate down either the children or parents.  Otherwise we could get into a loop
        /// </summary>
        private enum SearchDirection { Children, Parents };
        MediaAdminTransformation MediaTransformation { get; set; }

        public RelatedMediaTransformationAdmin()
        {
            MediaTransformation = new MediaAdminTransformation();
        }

        public SerialResponse CreateSerialResponse(CompositeMedia source, bool forExport = false)
        {

            SerialMediaAdmin serialItem = MediaTransformation.CreateSerialObject(source.TheMediaObject);
            SetRelatedItems(source, serialItem, SearchDirection.Children);
            SetRelatedItems(source, serialItem, SearchDirection.Parents);
            return new SerialResponse(new List<SerialMediaAdmin>() { serialItem });

        }

        private void SetRelatedItems(IRelatedMediaItemContainer source, ISerialRelatedMediaAdmin serialItem, SearchDirection direction)
        {
            if (direction == SearchDirection.Children)
            {
                serialItem.children = CreateRelatedMediaItems(source.Children, SearchDirection.Children);
                serialItem.childCount = serialItem.children.Count();
            }
            if (direction == SearchDirection.Parents)
            {
                serialItem.parents = CreateRelatedMediaItems(source.Parents, SearchDirection.Parents);
                serialItem.parentCount = serialItem.parents.Count();
            }
        }

        private List<SerialMediaAdmin> CreateRelatedMediaItems(List<MediaObject> items, SearchDirection direction)
        {
            if (items == null)
            {
                return new List<SerialMediaAdmin>();
            }
            return items.Select(c => CreateRelatedMediaItem(c, direction)).ToList();
        }

        private SerialMediaAdmin CreateRelatedMediaItem(MediaObject source, SearchDirection direction)
        {
            var obj = MediaTransformation.CreateSerialObject(source);            
            SetRelatedItems(source, obj, direction);
            return obj;
        }



    }
}
