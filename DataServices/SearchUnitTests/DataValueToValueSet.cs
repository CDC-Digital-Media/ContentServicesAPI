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
using Gov.Hhs.Cdc.DataSource.Media;
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.Media.Bll;

namespace SearchUnitTests
{
    [TestClass]
    public class DataValueToValueSet    
    {
        //static SearchControllerFactory SearchControllerFactory = Utility.GetSearchControllerFactory();
        //static SearchParameters SearchParam = new SearchParameters();
        //static ValueToValueSetObjectMgr ValueToValueSetObjectMgr = CdcDataSource.GetDataManager<ValueToValueSetObjectMgr>("Media");

        //[TestMethod]
        //public void ValueSetGetList()
        //{
        //    Criteria filterCriteria = new Criteria();
        //    SearchParam = GetValueSetSearchParms(filterCriteria, "ValueSet");

        //    DataSetResult valueSetList = SearchControllerFactory.GetSearchController(SearchParam).Get();
        //    Assert.AreNotEqual(null, valueSetList);

        //    List<ValueSetItem> valueSets = (List<ValueSetItem>)valueSetList.Records;
        //    Assert.IsTrue(valueSets.Count > 0, "ValueSets exist in the database.");
        //    string a = valueSets.ToString();
        //}

        [TestMethod]
        public void ValueToValueSetInsertDelete()
        {
            ValueToValueSetObject newObj = CreateNewValueToValueSetObject("ValueToValueSet Insert");

            //save object
            ValueToValueSetObjectMgr.Save(newObj);

            ValueToValueSetObject vsoCompare = ValueToValueSetObjectMgr.Get(newObj.ValueId, newObj.ValueLanguageCode,
                newObj.ValueSetId, newObj.ValueSetLanguageCode);

            //compare objects
            CompareValueToValueSet(newObj, vsoCompare);
            
            //delete object
            DeleteValueToValueSet(newObj);
        }

        [TestMethod]
        public void ValueToValueSetEditUpdate()
        {
            ValueToValueSetObject newObj = CreateNewValueToValueSetObject("ValueToValueSet Insert");

            //save object
            ValueToValueSetObjectMgr.Save(newObj);

            //Edit object
            ValueToValueSetObject vsoEdit = ValueToValueSetObjectMgr.Get(newObj.ValueId, newObj.ValueLanguageCode,
                newObj.ValueSetId, newObj.ValueSetLanguageCode);

            vsoEdit.ValueToValueSetName = "This was updated";
            vsoEdit.DisplayOrdinal = 2;
            vsoEdit.IsDefault = true;

            //save updated object
            ValueToValueSetObjectMgr.Save(vsoEdit);

            //get updated object
            ValueToValueSetObject vsoUpdated = ValueToValueSetObjectMgr.Get(newObj.ValueId, newObj.ValueLanguageCode,
                newObj.ValueSetId, newObj.ValueSetLanguageCode);
            
            //compare objects
            CompareValueToValueSet(vsoEdit, vsoUpdated);

            //delete object
            DeleteValueToValueSet(newObj);
        }

        //[TestMethod]
        //public void ValueToValueSetRelationSearch()
        //{
        //    Criteria filterCriteria = new Criteria();

        //    string[] valuesetid = "1,2".Split(',');
        //    string lang = GetValueSetLanguage(filterCriteria, "Language");
        //    filterCriteria.Add("ValueSet", valuesetid.ToList<string>().Select(a => a.Trim() + "|" + lang).ToList<string>());

        //    SearchParam = GetValueToValueSetSearchParms(filterCriteria, "FlatVocabValue");

        //    DataSetResult valueSetList = SearchControllerFactory.GetSearchController(SearchParam).Get();
        //    Assert.AreNotEqual(null, valueSetList);

        //    List<FlatVocabValueItem> valueSets = (List<FlatVocabValueItem>)valueSetList.Records;
        //    Assert.IsTrue(valueSets.Count > 0, "ValueSets exist in the database.");
        //}

        #region "Private Methods"

        private static ValueToValueSetObject CreateNewValueToValueSetObject(string title)
        {
            ValueToValueSetObject newValueToValueSetObject = new ValueToValueSetObject()
            {    
                ValueId = 1,
                ValueLanguageCode = "English",
                ValueSetId = 3,
                ValueSetLanguageCode = "English",
                ValueToValueSetName = "Test Set"
            };
            return newValueToValueSetObject;
        }

        private static void DeleteValueToValueSet(ValueToValueSetObject newObj)
        {
            ValueToValueSetObjectMgr.Delete(newObj);

            //verify that the object was deleted
            ValueToValueSetObject vsoDeleted = ValueToValueSetObjectMgr.Get(newObj.ValueId,newObj.ValueLanguageCode,
                newObj.ValueSetId, newObj.ValueSetLanguageCode);
            Assert.AreEqual(null, vsoDeleted);
        }

        private static void CompareValueToValueSet(ValueToValueSetObject newObj, ValueToValueSetObject vsoAlso)
        {
            //compare objects
            Assert.AreEqual(newObj.ValueId, vsoAlso.ValueId);
            Assert.AreEqual(newObj.ValueLanguageCode, vsoAlso.ValueLanguageCode);
            Assert.AreEqual(newObj.ValueSetId, vsoAlso.ValueSetId);
            Assert.AreEqual(newObj.ValueSetLanguageCode, vsoAlso.ValueSetLanguageCode);
            Assert.AreEqual(newObj.ValueToValueSetName, vsoAlso.ValueToValueSetName);
            Assert.AreEqual(newObj.DisplayOrdinal, vsoAlso.DisplayOrdinal);
            Assert.AreEqual(newObj.IsDefault, vsoAlso.IsDefault);
        }

        //private static SearchParameters GetValueToValueSetSearchParms(Criteria filterCriteria, string dataSetCode)
        //{
        //    SearchParameters searchParameters = new SearchParameters()
        //    {
        //        ApplicationCode = "Media",
        //        DataSetCode = dataSetCode,
        //        FilterCriteria = filterCriteria,
        //        Sorting = new Sorting(new SortColumn("DisplayOrdinal", SortOrderType.Asc))
        //    };
        //    return searchParameters;
        //}

        //private static string GetValueSetLanguage(Criteria Filter, string criteriaName)
        //{
        //    if (Filter.GetCriterion(criteriaName) != null)
        //        Filter.GetCriterion(criteriaName).Values = new List<string>() { Filter.GetCriterion(criteriaName).Values[0] };
        //    else
        //        Filter.Add(criteriaName, new List<string>() { "English" });

        //    return Filter.GetCriterion(criteriaName).Values[0];
        //}

        #endregion

    }
}
