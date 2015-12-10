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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Gov.Hhs.Cdc.Bo
{
    public class DataSetResult
    {
        public Guid Id { get; set; }

        /// <summary>
        /// List of entity records to be returned to the caller
        /// </summary>
        public IEnumerable Records { get; set; }

        public Type GetRecordType()
        {
            ReflectionObject reflectionObject  = new ReflectionObject(Records);
            return reflectionObject.GetListType();
        }

        /// <summary>
        /// Total number of pages referenced by this result set.  Only one may be returned
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Media count - Contains the total number of media items (records) found.
        /// </summary>
        public int TotalRecordCount { get; set; }

        /// <summary>
        /// Indicates that the result list is complete (default value) or incomplete.
        /// This is currently always true, until we start referencing other search providers 
        /// that aren't internal SQL server databases.
        /// </summary>
        public bool ListIsComplete { get; set; }


        



        public bool ResultsAreSorted { get; set; }
        
        public bool ResultsArePaged { get; set; }

        //Passed in values
        
        public int RecordCount { get; set; }
        public int FirstRecord {get; set;}
        public int PageSize { get; set; }
        //public int OffSet { get; set; }
        

        //Calculated and set in SetPages
        #region SetInSetPages
        public int FirstPage { get; set; }

        /// <summary>
        /// Number of pages this dataset result contains.  The last one may be incomplete
        /// </summary>
        public int PageCount { get; set; }
        

        /// <summary>
        /// Number of complete pages that this dataset result contains
        /// </summary>
        public int CompletePageCount { get; set; }
        public bool ContainsCompleteLastPage { get; set; } 
        #endregion

        //Calculated on the fly
        #region OnTheFly
        public int PageNumber { get { return FirstPage; } }
        public int LastRecord { get { return FirstRecord + RecordCount - 1; } }
        public int LastPage { get { return FirstPage + PageCount - 1; } } 
        #endregion

        public DataSetResult() { }

        public DataSetResult(Guid resultSetId, IEnumerable records, bool resultsAreSorted, bool resultsArePaged, bool listIsComplete,
            int totalRecordCount, int recordCount, int firstRecord, int pageSize): this()
        {
            Id = resultSetId;
            Records = records;
            ResultsAreSorted = resultsAreSorted;
            ResultsArePaged = resultsArePaged;
            ListIsComplete = listIsComplete;
            SetPages(totalRecordCount, recordCount, firstRecord, pageSize);
        }

        public DataSetResult(DataSetResult dataSetResult)
            : this(dataSetResult.Id, dataSetResult.Records, dataSetResult.ResultsAreSorted,
                    dataSetResult.ResultsArePaged, dataSetResult.ListIsComplete, dataSetResult.TotalRecordCount, dataSetResult.RecordCount,
                    dataSetResult.FirstRecord, dataSetResult.PageSize)
        {

            //Do not set these values here.  They will be set by "Set Pages"
            //CompletePageCount = dataSetResult.CompletePageCount;
            //FirstPage = dataSetResult.FirstPage;
            //ContainsCompleteLastPage = dataSetResult.ContainsCompleteLastPage;
            //PageCount = dataSetResult.PageCount;
            //TotalPages = dataSetResult.TotalPages;
        }
                
        public void SetPageSize(int pageSize)
        {
            SetPages(TotalRecordCount, RecordCount, FirstRecord, pageSize);
        }

        private void SetPages(int totalRecordCount, int recordCount, int firstRecord, int pageSize)
        {
            TotalRecordCount = totalRecordCount;
            RecordCount = recordCount;
            FirstRecord = firstRecord;
            PageSize = pageSize;

            if (!ResultsArePaged || pageSize == 0)
            {
                FirstPage = 1;
                PageCount = 1;
                TotalPages = 1;
                ContainsCompleteLastPage = true;
                CompletePageCount = 1;
            }
            else
            {
                FirstPage = GetPageNumber(FirstRecord);
                PageCount = GetPageNumber(RecordCount);
                TotalPages = GetPageNumber(TotalRecordCount);

                ContainsCompleteLastPage = (RecordCount >= TotalRecordCount);
                CompletePageCount = ContainsCompleteLastPage
                    ? TotalPages
                    : (RecordCount / PageSize);
            }
        }

        private int GetPageNumber(int recordNumber)
        {
            return ((recordNumber - 1) / PageSize) + 1;
        }

        private int GetStartingRecordNumber(int pageNumber)
        {
            return ((pageNumber - 1) * PageSize) + 1;
        }

        private bool ContainsRecords(int firstRecordNeeded, int lastRecordNeeded)
        {
            return FirstRecord <= firstRecordNeeded 
                && (ContainsCompleteLastPage || LastRecord >= lastRecordNeeded);
        }

        public DataSetResult GetPage(int pageNumber, int offset = 0)
        {
            if (pageNumber < 1)
            {
                throw new ApplicationException("Page number " + pageNumber + " is invalid");
            }
            int firstRecordNeeded = offset > 0 ? (offset + (PageSize * (pageNumber - 1))) - (offset-PageSize) : GetStartingRecordNumber(pageNumber);
            int lastRecordNeeded = firstRecordNeeded + PageSize - 1;
            if (!ContainsRecords(firstRecordNeeded, lastRecordNeeded))
            {
                throw new ApplicationException(string.Format(
                      "DataSetResult with records {0} {1} does not contain records {2} to {3}",
                          FirstRecord, LastRecord, firstRecordNeeded, lastRecordNeeded));
            }
            int recordsToSkip = firstRecordNeeded - this.FirstRecord;

            IEnumerable<object> pagedRecords = Records.Cast<object>();
            if (this.PageSize != 0)
            {
                pagedRecords = Page(GetRecordType(), recordsToSkip, pagedRecords);
            }


            return new DataSetResult(
                Id,
                pagedRecords,
                ResultsAreSorted,
                /*resultsArePaged:*/ true,
                /*ListIsComplete:*/ ListIsComplete,
                TotalRecordCount, pagedRecords.Count(), firstRecordNeeded, PageSize
                );
        }

        private IEnumerable<object> Page(Type resultType, int recordsToSkip, IEnumerable<object> pagedRecords)
        {
            //TODO:  See if there is another way to invoke PageByType
            MethodInfo method = typeof(DataSetResult).GetMethod("PageByType");

            MethodInfo genericMethod = method.MakeGenericMethod(new Type[] { resultType });
            return (IEnumerable<object>)genericMethod.Invoke(this, new object[] { recordsToSkip, pagedRecords });
        }

        //invoked above using reflection
        public IEnumerable<T> PageByType<T>(int recordsToSkip, IEnumerable<T> pagedRecords)
        {
            pagedRecords = pagedRecords.Skip(recordsToSkip)
                                       .Take(PageSize)
                                       .ToList().AsEnumerable();
            return pagedRecords;
        }


    }
}
