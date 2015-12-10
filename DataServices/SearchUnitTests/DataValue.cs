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
    public class DataValue
    {
        //static SearchControllerFactory SearchControllerFactory = Utility.GetSearchControllerFactory();
        //static SearchParameters SearchParam = new SearchParameters();
        //static ValueObjectMgr ValueObjectMgr = CdcDataSource.GetDataManager<ValueObjectMgr>("Media");

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
        public void ValueRelationshipInsertDelete()
        {
            ValueObject newObj = CreateObject("Value Insert");

            //save object
            ValueObjectMgr.Save(newObj);

            ValueObject vsoCompare = ValueObjectMgr.Get(newObj.ValueId);

            //compare objects
            CompareObject(newObj, vsoCompare);

            //delete object
            DeleteObject(newObj);
        }


        //private FlatVocabValueItem GetValueItem(string name)
        //{
        //    Criteria filterCriteria = new Criteria();
        //    filterCriteria.Add("LanguageCode", "English");

        //    filterCriteria.
        //    {

        //    }
        //    SearchParameters searchParameters = new SearchParameters()
        //    {
        //        ApplicationCode = "Media",
        //        DataSetCode = dataSetCode,
        //        FilterCriteria = new Filter,
        //        Sorting = new Sorting(new SortColumn("DisplayOrdinal", SortOrderType.Asc))
        //    };
        //}
        [TestMethod]
        public void ValueRelationshipEditUpdate()
        {
            ValueObject newObj = CreateObject("Value Insert");
            ValueObject oldObj = ValueObjectMgr.Get(newObj.ValueName, newObj.LanguageCode);
            if( oldObj != null)
                ValueObjectMgr.Delete(oldObj);
            //save object
            ValueObjectMgr.Save(newObj);

            //Edit object
            ValueObject vsoEdit = ValueObjectMgr.Get(newObj.ValueId);

            vsoEdit.ValueName = "Update valuename";
            vsoEdit.Description = "Updated Desc.";
            vsoEdit.DisplayOrdinal = 2;
            vsoEdit.IsActive = true;

            //save updated object
            ValueObjectMgr.Save(vsoEdit);

            //get updated object
            ValueObject vsoUpdated = ValueObjectMgr.Get(newObj.ValueId);

            //compare objects
            CompareObject(vsoEdit, vsoUpdated);

            //delete object
            DeleteObject(newObj);
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
            ValueObjectMgr.Delete(newObj);

            //verify that the object was deleted
            ValueObject vsoDeleted = ValueObjectMgr.Get(newObj.ValueId);
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
