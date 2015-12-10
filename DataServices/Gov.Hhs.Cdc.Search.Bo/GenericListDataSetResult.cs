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

namespace Gov.Hhs.Cdc.Search.Bo
{
    public class GenericListDataSetResult<T> : GenericDataSetResult<List<T>>
    {
        public GenericListDataSetResult() : base() { }

        public GenericListDataSetResult(DataSetResult dataSetResult) : base(dataSetResult) { }


        public override bool IsEnumerable()
        {
            return true;
        }

        protected override Func<List<T>, int, int, List<T>> SplitAlgorithm
        {
            get
            {
                return Split;
            }
        }

        private List<T> Split(List<T> toSplit, int skip, int take)
        {
            return toSplit.Skip<T>(skip).Take<T>(take).ToList<T>();
        }

        public virtual List<T> Items
        {
            get
            {
                var result = ItemsEnumerable as List<T>;

                if (result == null)
                {
                    result = ItemsEnumerable.ToList<T>();
                }


                return result;
            }
        }

        public virtual IEnumerable<T> ItemsEnumerable
        {
            get
            {
                return Item as IEnumerable<T>;
            }
        }

        public virtual T[] ItemsArray
        {
            get
            {
                return ItemsEnumerable.ToArray<T>();
            }
        }
    }
}
