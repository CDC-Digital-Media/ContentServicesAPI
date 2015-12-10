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
    public class RelatedMediaTransformationV2 : IRelatedMediaTransformation
    {
        /// <summary>
        /// Only navigate down either the children or parents.  Otherwise we could get into a loop
        /// </summary>
        private enum SearchDirection { Children, Parents };
        MediaTransformationV2 MediaTransformation { get; set; }

        public RelatedMediaTransformationV2()
        {
            MediaTransformation = new MediaTransformationV2();
        }

        public SerialResponse CreateSerialResponse(CompositeMedia source, bool forExport = false)
        {
            SerialMediaV2 serialItem = MediaTransformation.CreateSerialObject(source.TheMediaObject, forExport);
            SetRelatedItems(source, serialItem, SearchDirection.Children, forExport);
            SetRelatedItems(source, serialItem, SearchDirection.Parents, forExport);

            return new SerialResponse(new List<SerialMediaV2>() { serialItem });
        }

        private void SetRelatedItems(IRelatedMediaItemContainer source, ISerialRelatedMediaV2 serialItem, SearchDirection direction, bool forExport)
        {
            if (direction == SearchDirection.Children)
            {
                serialItem.children = CreateRelatedMediaItemChildren(source.Children, SearchDirection.Children, forExport);
                serialItem.childCount = serialItem.children.Count();
            }
            if (direction == SearchDirection.Parents)
            {
                serialItem.parents = CreateRelatedMediaItemParent(source.Parents, SearchDirection.Parents, forExport);
                serialItem.parentCount = serialItem.parents.Count();
            }
        }

        private List<SerialMediaChildren> CreateRelatedMediaItemChildren(List<MediaObject> items, SearchDirection direction, bool forExport)
        {
            if (items == null)
            {
                return new List<SerialMediaChildren>();
            }
            return items.Select(c => CreateRelatedMediaItemChildren(c, direction,forExport)).ToList();
        }

        private List<SerialMediaParent> CreateRelatedMediaItemParent(List<MediaObject> items, SearchDirection direction, bool forExport)
        {
            if (items == null)
            {
                return new List<SerialMediaParent>();
            }
            return items.Select(c => CreateRelatedMediaItemParent(c, direction, forExport)).ToList();
        }

        private SerialMediaChildren CreateRelatedMediaItemChildren(MediaObject source, SearchDirection direction, bool forExport)
        {
            var obj = MediaTransformation.CreateSerialMediaChildren(source,forExport);
       
            SetRelatedItems(source, obj, direction, forExport);

            return obj;
        }

        private SerialMediaParent CreateRelatedMediaItemParent(MediaObject source, SearchDirection direction, bool forExport)
        {
            var obj = MediaTransformation.CreateSerialMediaParent(source, forExport);

            SetRelatedItems(source, obj, direction, forExport);

            return obj;
        }
    }
}
