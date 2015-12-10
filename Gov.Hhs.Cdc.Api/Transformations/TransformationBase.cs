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

namespace Gov.Hhs.Cdc.Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="t">Serial Object</typeparam>
    /// <typeparam name="T">Business Object</typeparam>
    public abstract class TransformationBase<t, T> : MediaTransformationBase
    {
        public abstract List<t> CreateSerialObjectList(DataSetResult dataset);
        public abstract t CreateSerialObject(T obj);
        public abstract T CreateBusinessObject(t obj);

        public virtual IEnumerable<T> GetEnumerableBusinessObject(DataSetResult dataset)
        {
            return dataset.Records.Cast<T>();
        }
    }
}
