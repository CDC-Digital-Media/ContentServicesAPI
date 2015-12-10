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
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.Search.Provider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;

namespace SearchUnitTests
{
    public class SearchProviderFactoryTests
    {

        public ISearchProviders Get(SearchParameters searchParameters)
        {
            List<SearchProviderReference> providerRefs = new List<SearchProviderReference>();
            providerRefs.Add(new SearchProviderReference(){
                    Code="TestProvider"
                    //, 
                    //Name="Test Search Provider", 
                    //AssemblyName="SearchUnitTests.dll",
                    //ClassName = "SearchUnitTests.SearchProviderTester"
                }
            );

            DataSetDefinition dataSetDefinition = new DataSetDefinition()
            {
                ApplicationCode = searchParameters.ApplicationCode,
                FilterCode = searchParameters.FilterCode,
                DataSetCode = searchParameters.DataSetCode,
                IsDefaultForFilter = true,
                IsSmallEnoughToBeCached = true,
                MustKeepLocationOnPaging = true,
                MustShowDeletedRecordsOnPage = true
            };

            return new SearchProviders(providerRefs, dataSetDefinition);                
        }



        public ISearchProviders Get()
        {
            throw new NotImplementedException();
        }
    }
}
