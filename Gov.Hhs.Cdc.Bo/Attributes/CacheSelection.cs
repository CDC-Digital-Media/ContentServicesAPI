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
using System.Reflection;

namespace Gov.Hhs.Cdc.Bo
{
    /// <summary>
    /// A business object can be configured for caching by adding the [CacheSelection] attribute to the class.  If multiple 
    /// keys will be used to access the cached object, then more than one [CacheSelection] can be added.  The SelectionTypeCode 
    /// will default to null, so if more than one is cacheSelection each attribute other than the first must have a 
    /// SelectionTypeCode specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CacheSelection : System.Attribute
    {
        /// <summary>
        /// The application code of the business object to be used in creating the cache key for the dictionary.  This will 
        /// default to the current application (This statement needs to be verified).
        /// </summary>
        /// 

        public string ApplicationCode { get; set; }
        /// <summary>
        /// A descriptive name to be used for the object when creating the cache key.  This must be specified if it is not 
        /// the first CacheSelection for an object.
        /// </summary>
        /// 
        public string SelectionTypeCode { get; set; }
        
        /// <summary>
        /// If true, the cached object will also be persisted to the file system.  It will be loaded at the instantiation 
        /// of the ObjectContext.  If all data access for a business rule is through persisted cached objects, then the 
        /// business rule should succeed even with the database down.
        /// </summary>
        public bool IsPersisted { get; set; }

        /// <summary>
        /// The specified key will may not return back a unique row, so the result will be a list of 0 to many objects.
        /// </summary>
        public bool ReturnsList { get; set; }

        /// <summary>
        /// How often to refresh the dictionary in the cache/persistence layer
        /// </summary>
        public int RefreshSeconds { get; set; }

        /// <summary>
        /// Note: Code to manage groups has been “disconnected”, due to issues.  Groups were used for the Filter objects 
        /// before they were moved into properties in the business logic, so GroupCode is not currently used.  Read on 
        /// for the original intended use…
        /// 
        ///
        /*Refresh the object at the same time as other objects in the group.  
         * This will ensure that updates made to multiple objects will be cached together and will reduce the likelihood of a mismatch.  
         * Transactions are employed to guarantee this, so there is still a chance to return different versions of related tables 
         * if the timing is just right.
        */
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// If the ReturnsList parameter is set, then columns may be specified to order the result list.
        /// </summary>
        public List<string> OrderByColumns { get; set; }


        public CacheSelection()
        {
            SelectionTypeCode = "";
            IsPersisted = false;
            ReturnsList = false;
            RefreshSeconds = 0;
            GroupCode = null;
        }

    }
}
