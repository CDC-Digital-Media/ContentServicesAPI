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

namespace Gov.Hhs.Cdc.Api
{
    /// <summary>
    /// Base class for creating a mapping factory.  This is used to map the API sort column names to the
    /// middle tier column names
    /// </summary>
    /// <typeparam name="T">SerialType this factory supports.  This is required because the static 
    /// dictionary is based on this type</typeparam>
    public abstract class ColumnMapFactory<T>
    {
        private Dictionary<string, ColumnMapper> _columnMaps = null;
        private Dictionary<string, ColumnMapper> ColumnMaps
        {
            get
            {
                return _columnMaps ?? (_columnMaps = new Dictionary<string, ColumnMapper>());
            }
        }

        /// <summary>
        /// Get the appriate mapping class, depending on version and whether or not this is public facing
        /// </summary>
        /// <param name="version"></param>
        /// <param name="isPublicFacing"></param>
        /// <returns></returns>
        public ColumnMapper GetMapper(int version, bool isPublicFacing)
        {
            string dictionaryKey = string.Format("{0}|{1}", version, isPublicFacing);
            if (!ColumnMaps.ContainsKey(dictionaryKey))
            {
                ColumnMapper mapper = new ColumnMapper(CreateMapping(version, isPublicFacing),
                    CreateDefaultSortList(version, isPublicFacing));
                ColumnMaps.Add(dictionaryKey, mapper);
            }

            return ColumnMaps[dictionaryKey];

        }

        /// <summary>
        /// Create the appropriate mapping depending on the version of the API
        /// </summary>
        /// <param name="version"></param>
        /// <param name="isPublicFacing"></param>
        /// <returns></returns>
        protected abstract Dictionary<string, string> CreateMapping(int version, bool isPublicFacing);

        /// <summary>
        /// Create the appropriate default sort columns, depending on the version of the API
        /// </summary>
        /// <param name="version"></param>
        /// <param name="isPublicFacing"></param>
        /// <returns></returns>
        protected abstract List<SortColumn> CreateDefaultSortList(int version, bool isPublicFacing);

    }

}
