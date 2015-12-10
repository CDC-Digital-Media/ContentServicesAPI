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
using Gov.Hhs.Cdc.DataSource.Media;
using Gov.Hhs.Cdc.DataSource;

namespace SearchUnitTests
{
    [TestClass]
    public class DataVocabTests
    {

        //[TestMethod]
        //public void TestVocabObject()
        //{
        //    VocabObjectMgr mediaMgr = (VocabObjectMgr)CdcDataSource.GetDataMgr("Media", "VocabValueBO");

        //    //Create a new Media OBject
        //    VocabValueObject newParentObject = CreateNewVocabObject("Test NewVocabObject Parent");
        //    mediaMgr.Save(newParentObject);
            
        //    VocabValueObject newObject = CreateNewVocabObject("Test NewVocabObject");
        //    newObject.ParentId = newParentObject.Id;
        //    mediaMgr.Save(newObject);
        //}


        //private static VocabValueObject CreateNewVocabObject(string title)
        //{
        //    VocabValueObject newVocabObject = new VocabValueObject()
        //    {
        //        Name = title,
        //        LanguageCode = "English",
        //        Description = "Description for " + title,
        //        DisplayOrdinal = 1,
        //        IsActive = true
        //    };
        //    return newVocabObject;
        //}
    }
}
