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

using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace VocabularyTests
{
    [TestClass]
    public class DataValueToValueSet
    {
        const int testValueSetId = 3;
        const int testValueId = 25221;

        public DataValueToValueSet()
        {
            CurrentConnection.Name = "ContentServicesDb";
        }

        static ValueToValueSetObject newValueToValueSetObject = new ValueToValueSetObject()
        {
            ValueId = testValueId,
            ValueLanguageCode = "English",
            ValueSetId = testValueSetId,
            ValueSetLanguageCode = "English",
            ValueToValueSetName = "Test Set"
        };

        [TestMethod]
        public void ValueToValueSetInsertDelete()
        {
            ValueToValueSetObject newObj = newValueToValueSetObject;

            var provider = ValueToValueSetProvider.Save(newObj);

            ValueToValueSetObject vsoCompare = ValueToValueSetProvider.Get(newObj.ValueId, newObj.ValueLanguageCode,
                newObj.ValueSetId, newObj.ValueSetLanguageCode);

            Assert.IsNotNull(vsoCompare);

            CompareValueToValueSet(newObj, vsoCompare);

            DeleteValueToValueSet(newObj);
        }

        [TestMethod]
        public void ValueToValueSetEditUpdate()
        {
            ValueToValueSetObject newObj = newValueToValueSetObject;

            Assert.IsNotNull(newObj);
            ValueToValueSetProvider.Save(newObj);

            ValueToValueSetObject vsoEdit = ValueToValueSetProvider.Get(newObj.ValueId, newObj.ValueLanguageCode,
                newObj.ValueSetId, newObj.ValueSetLanguageCode);

            Assert.IsNotNull(vsoEdit);
            vsoEdit.ValueToValueSetName = "This was updated";
            vsoEdit.DisplayOrdinal = 2;
            vsoEdit.IsDefault = true;

            ValueToValueSetProvider.Save(vsoEdit);

            ValueToValueSetObject vsoUpdated = ValueToValueSetProvider.Get(newObj.ValueId, newObj.ValueLanguageCode,
                newObj.ValueSetId, newObj.ValueSetLanguageCode);

            CompareValueToValueSet(vsoEdit, vsoUpdated);

            DeleteValueToValueSet(newObj);
        }

        #region "Private Methods"

        private static void DeleteValueToValueSet(ValueToValueSetObject newObj)
        {
            ValueToValueSetProvider.Delete(newObj);

            //verify that the object was deleted
            ValueToValueSetObject vsoDeleted = ValueToValueSetProvider.Get(newObj.ValueId, newObj.ValueLanguageCode,
                newObj.ValueSetId, newObj.ValueSetLanguageCode);
            Assert.AreEqual(null, vsoDeleted);
        }

        private static void CompareValueToValueSet(ValueToValueSetObject newObj, ValueToValueSetObject vsoAlso)
        {
            Assert.AreEqual(newObj.ValueId, vsoAlso.ValueId);
            Assert.AreEqual(newObj.ValueLanguageCode, vsoAlso.ValueLanguageCode);
            Assert.AreEqual(newObj.ValueSetId, vsoAlso.ValueSetId);
            Assert.AreEqual(newObj.ValueSetLanguageCode, vsoAlso.ValueSetLanguageCode);
            Assert.AreEqual(newObj.ValueToValueSetName, vsoAlso.ValueToValueSetName);
            Assert.AreEqual(newObj.DisplayOrdinal, vsoAlso.DisplayOrdinal);
            Assert.AreEqual(newObj.IsDefault, vsoAlso.IsDefault);
        }

        #endregion

    }
}
