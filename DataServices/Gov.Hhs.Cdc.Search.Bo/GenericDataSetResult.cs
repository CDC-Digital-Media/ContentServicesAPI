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
    public class GenericDataSetResult<T> : DataSetResult
        where T : class, new()
    {
        //public GenericDataSetResult() { }

        public GenericDataSetResult(DataSetResult dataSetResult): this()
        {
            var items = dataSetResult.Records as T;

            if (items == null)
            {
                throw new ArgumentException(
                    @"'dataSetResult' must contain a 'Records' object that is convertible to 
                      a '" + typeof(T) + "'.");
            }

            SynchToDataSetResult(dataSetResult, items);
        }

        private void SynchToDataSetResult(DataSetResult dataSetResult, T items)
        {
            //Id = dataSetResult.Id;
            //NumberRecordsReturned = dataSetResult.NumberRecordsReturned;
            //PageNumber = dataSetResult.PageNumber;
            //dataSetResult.Records = items;
            //ResultsArePaged = dataSetResult.ResultsArePaged;
            //ResultsAreSorted = dataSetResult.ResultsAreSorted;
            //TotalPages = dataSetResult.TotalPages;
            //TotalRecordCount = dataSetResult.TotalRecordCount;
            //Records = dataSetResult.Records;
        }

        public virtual T Item
        {
            get
            {
                return Records as T;
            }
        }

        public virtual bool IsEnumerable() {return false;}

        public GenericDataSetResult<T> SplitClone(int take)
        {
            return SplitClone(0, take);
        }
        
        public GenericDataSetResult<T> SplitClone(int skip, int take)
        {
            if (! IsEnumerable())
            {
                throw new InvalidOperationException(
                    "SplitClone called on a GenericDataSetResult unable to split.  Check 'IsEnumerable()' first.");
            }

            var result = Clone();
            result.Records = SplitAlgorithm(Records as T, skip, take);


            return result;
        }

        protected virtual Func<T, int, int, T> SplitAlgorithm
        {
            get 
            {
                //If we're exercising the base functionality, something
                //somewhere is going wrong,
                if (IsEnumerable())
                {
                    throw new Exception(
                        @"An internal error occurred.  The derived class 
                        reports the ability to split, but has not impemented (in the override)
                        the Split Algorithm.");
                }

                throw new InvalidOperationException(
                    "Trying to split on a GenericDataSetResult which cannot be split. Check 'IsEnumerable()' first.");
            }
        }

        public GenericDataSetResult<T> Clone()
        {
            return new GenericDataSetResult<T>
            {
                Id = Id,
                NumberRecordsReturned = NumberRecordsReturned,
                PageNumber = PageNumber,
                ResultsArePaged = ResultsArePaged,
                ResultsAreSorted = ResultsAreSorted,
                TotalPages = TotalPages,
                TotalRecords = TotalRecords,
                Records = Records
            };
        }
    }
}
