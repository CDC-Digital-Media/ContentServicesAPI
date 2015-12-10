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
using Gov.Hhs.Cdc.Search.Provider;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace SearchUnitTests
{
    public class SearchProviderTester : ISearchProvider
    {
        public string ApplicationCode { get { return "Test"; } }
        public string Code { get; set; }
        public string Name { get; set; }


        public bool CanPage
        {
            get { return true; }
        }

        public bool CanSort
        {
            get { return true; }
        }

        public bool SupportsEfficientIntermediateRequests
        {
            get { return true; }
        }

        public bool CanIndexByLocation
        {
            get { return false; }
        }

        public bool SupportsPartialResults
        {
            get { return false; }
        }

        public DataSetResult Get(SearchParameters searchParameters, ResultSetParms resultSetParms)
        {
            int numberOfRows = 1000;
            return new DataSetResult(
                resultSetId: resultSetParms.ResultSetId,
                records: GetTestItems(numberOfRows),
                resultsAreSorted: false,
                resultsArePaged: false,
                listIsComplete:true,
                totalRecordCount: numberOfRows,
                recordCount: numberOfRows,
                firstRecord: 1,
                pageSize: searchParameters.Paging.PageSize);
        }

        public IEnumerable<TestItem> GetTestItems(int numberOfRows)
        {
            List<TestItem> testItems = new List<TestItem>();
            for (int i = 0; i < numberOfRows; ++i)
            {
                testItems.Add(
                    new TestItem()
                    {
                        IntKey1 = numberOfRows - i,
                        StringKey1 = "Key" + i.ToString(),
                        DateTimeValue1 = DateTime.Now.AddHours(i),
                        StringValue1 = "Value" + (numberOfRows - 1).ToString()
                    });
            };
            return testItems.AsEnumerable();
        }

        public ISearchDataManager GetSearchDataManager(string filterCode)
        {
            throw new NotImplementedException();
        }


        public List<System.Reflection.Assembly> BusinessObjectAssemblies
        {
            get { return new List<System.Reflection.Assembly>(); }
        }
    }
}
