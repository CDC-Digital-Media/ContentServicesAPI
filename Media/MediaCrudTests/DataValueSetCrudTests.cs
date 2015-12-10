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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Connection;

namespace MediaCrudTests
{
    [TestClass]
    public class DataValueSetCrudTests
    {
        static DataValueSetCrudTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }
        
        private static IValueProvider ValueProvider = new CsValueProvider();

        [TestMethod]
        public void TestCreateValueSet()
        {
            //string jsonString = "{'name':'I have no values','languageCode':'English','description':'no, really','displayOrdinal':'-1','isActive':'true','isDefaultable':'true','isOrderable':'true','isHierachical':'true'}";
            
            ValueSetObject valueSet = new ValueSetObject()
            {
                Name = "I have no values",
                LanguageCode = "English",
                Description = "no, really",
                DisplayOrdinal = -1,
                IsActive = true,
                IsDefaultable = true,
                IsOrderable = true,
                IsHierachical = true
            };

            ValueProvider.InsertValueSet(valueSet);
            ValueProvider.DeleteValueSet(valueSet);
            
        }
    }
}
