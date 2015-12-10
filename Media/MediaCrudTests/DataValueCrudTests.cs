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
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Connection;

namespace SearchUnitTests
{

    [TestClass]
    public class DataValueCrudTests
    {
        public static IValueProvider ValueProvider = new CsValueProvider();
        static DataValueCrudTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";   
        }

        [TestMethod]
        public void ValueInsertDelete()
        {
            ValueObject newObj = CreateObject("Value Insert");

            //save object
            ValueProvider.SaveValue(newObj);

            ValueObject vsoCompare = ValueProvider.GetValue(newObj.ValueId);

            //compare objects
            CompareObject(newObj, vsoCompare);

            //delete object
            DeleteObject(newObj);
        }


        [TestMethod]
        public void ValueRelationshipEditUpdate()
        {
            ValueObject newObj = CreateObject("Value Insert");
            
            
            ValueObject oldObj = ValueProvider.GetValue(newObj.ValueName, newObj.LanguageCode);
            if( oldObj != null)
                ValueProvider.DeleteValue(oldObj);

            oldObj = ValueProvider.GetValue("Update valuename", newObj.LanguageCode);
            if (oldObj != null)
                ValueProvider.DeleteValue(oldObj);

            //save object
            ValueProvider.SaveValue(newObj);

            //Edit object
            ValueObject valueEdit = ValueProvider.GetValue(newObj.ValueId);

            valueEdit.ValueName = "Update valuename";
            valueEdit.Description = "Updated Desc.";
            valueEdit.DisplayOrdinal = 2;
            valueEdit.IsActive = true;

            //save updated object
            ValueProvider.SaveValue(valueEdit);

            //get updated object
            ValueObject vsoUpdated = ValueProvider.GetValue(newObj.ValueId);

            //compare objects
            CompareObject(valueEdit, vsoUpdated);


            ValueObject valueEdit2 = ValueProvider.GetValue(newObj.ValueId);
            valueEdit2.ValueId = 0;
            valueEdit2.ValueToValueSetObjects = new List<ValueToValueSetObject>()
            {
                new ValueToValueSetObject()
                {
                    ValueSetId = 120,
                    ValueSetLanguageCode = valueEdit2.LanguageCode,
                }
            };
            ValidationMessages overwriteMessages1 = ValueProvider.SaveValue(valueEdit2);

            valueEdit2.ValueId = 0;
            ValidationMessages overwriteMessages2 = ValueProvider.SaveValue(valueEdit2);


            //delete object
            DeleteObject(newObj);
        }

        #region "Private Methods"

        private static ValueObject CreateObject(string title)
        {
            ValueObject newValueToValueSetObject = new ValueObject()
            {
                ValueName = "Test Value",
                LanguageCode = "English",
                Description = "Test Description",
                DisplayOrdinal = 3,
                IsActive = true
            };
            return newValueToValueSetObject;
        }


        private static void DeleteObject(ValueObject newObj)
        {
            ValueProvider.DeleteValue(newObj);

            //verify that the object was deleted
            ValueObject vsoDeleted = ValueProvider.GetValue(newObj.ValueId);
            Assert.AreEqual(null, vsoDeleted);
        }

        private static void CompareObject(ValueObject newObj, ValueObject vsoAlso)
        {
            //compare objects
            Assert.AreEqual(newObj.ValueId, vsoAlso.ValueId);
            Assert.AreEqual(newObj.ValueName, vsoAlso.ValueName);
            Assert.AreEqual(newObj.LanguageCode, vsoAlso.LanguageCode);
            Assert.AreEqual(newObj.Description, vsoAlso.Description);
            Assert.AreEqual(newObj.DisplayOrdinal, vsoAlso.DisplayOrdinal);
            Assert.AreEqual(newObj.IsActive, vsoAlso.IsActive);
        }

        #endregion

    }
}
