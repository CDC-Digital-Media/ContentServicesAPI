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
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.DataSource.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace SearchUnitTests
{
    [TestClass]
    public class FrameworkCacheTest
    {
        //[TestMethod]
        //[ExpectedException(typeof(ApplicationException), "Object ~Framework~.Application.Application not found with key of ~Framework")]
        //public void TestApplicationCache()
        //{
        //    using (FrameworkObjectContext csData = new FrameworkObjectContext())
        //    {
        //        //DsApplication application = FrameworkCache.GetApplication(csData, "Media");
        //        ApplicationEnv applicationEnv = FrameworkCache.GetApplicationEnv(csData, "Media", "Development");
        //        ApplicationEnvConnection applicationEnvConnection = FrameworkCache.GetApplicationEnvConnection(csData, applicationEnv);
        //        DataSetDefinition dataSet = FrameworkCache.GetDataSet(csData, "Media", "Media");
        //        List<DataSetDefinition> dataSets = FrameworkCache.GetDataSetByFilterCode(csData, "Media", "Media");


        //        //application.Name = "test";
        //        //DsApplication application2 = FrameworkCache.GetApplication(csData, "Media");
        //        //DsApplication application3 = FrameworkCache.GetApplication(csData, "~Framework~");
                

        //        //DsApplication application4 = FrameworkCache.GetApplication(csData, "~Framework");
        //        string x = "Name";
        //    }
        //}

        //[TestMethod]
        //public void TestApplicationEnv()
        //{
        //    using (FrameworkObjectContext csData = new FrameworkObjectContext())
        //    {

        //        ApplicationEnv application = FrameworkCache.GetApplicationEnv(csData, "Media", null);
        //        ApplicationEnv application2 = FrameworkCache.GetApplicationEnv(csData, "Media", "Development");
        //        string x = "Name";
        //    }
        //}
    }
}
