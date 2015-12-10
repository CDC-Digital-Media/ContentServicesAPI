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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class HierarchicalItem : ListItem
    {
        //private string parentKey;
        //public int? IntParentKey { get; set; }
        //private string ParentKey
        //{
        //    get
        //    {
        //        return (this.EnumKeyType == KeyType.StringKey)
        //            ? parentKey
        //            : (IntParentKey == null) ? null : IntParentKey.ToString();
        //    }
        //    set
        //    {
        //        if (this.EnumKeyType == KeyType.StringKey)
        //            parentKey = value;
        //        else
        //        {
        //            try
        //            {
        //                if (value == null)
        //                    IntParentKey = null;
        //                else
        //                    IntParentKey = int.Parse(value);
        //            }
        //            catch
        //            {
        //                //To Do: Create an exception class to raise this error
        //                throw;
        //            }
        //        }
        //    }
        //}

        //private string ownerKey;
        //public int? IntOwnerKey { get; set; }
        //public string OwnerKey
        //{
        //    get
        //    {
        //        return (this.EnumKeyType == KeyType.StringKey)
        //            ? ownerKey
        //            : (IntOwnerKey == null) ? null : IntOwnerKey.ToString();
        //    }
        //    set
        //    {
        //        if (this.EnumKeyType == KeyType.StringKey)
        //            ownerKey = value;
        //        else
        //        {
        //            try
        //            {
        //                if (value == null)
        //                    IntOwnerKey = null;
        //                else
        //                    IntOwnerKey = int.Parse(value);
        //            }
        //            catch
        //            {
        //                //To Do: Create an exception class to raise this error
        //                throw;
        //            }
        //        }
        //    }
        //}

        public bool SearchChildren { get; set; }
        //public List<HierarchicalItem> Children { get; set; }    //This is only used when returning the list back to the UI
        //public bool IsRootItem { get; set; }
        //public bool IsSelected { get; set; }
        //public HierarchicalItem() : base()  
        //{ 
        //}

        public HierarchicalItem(KeyType keyType)
            : base(keyType)
        {
        }

        public override string ToString()
        {
            string results = LongDisplayName == null ? DisplayName : LongDisplayName;
            results += SearchChildren ? "" : " (no children)";
            return results;
        }

        public string ToShortString()
        {
            string results = DisplayName;
            results += SearchChildren ? "" : " (no children)";
            return results;
        }

    }
}
