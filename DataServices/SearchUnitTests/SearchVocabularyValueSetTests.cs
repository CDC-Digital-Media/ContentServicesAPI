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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Connection;

namespace SearchUnitTests
{
    [TestClass]
    public class SearchVocabularyValueSetTests
    {
        static ISearchControllerFactory _searchControllerFactory;
        static ISearchControllerFactory SearchControllerFactory
        {
            get
            {
                return _searchControllerFactory ?? (_searchControllerFactory = ContentServicesDependencyBuilder.SearchControllerFactory);
            }
        }

        static SearchParameters SearchParam = new SearchParameters();
        static IValueProvider ValueProvider = new CsValueProvider();
        
        public static IObjectContextFactory MediaObjectContextFactory = new MediaObjectContextFactory();
        public FlatVocabValueSearchDataMgr flatVocabValueSearchDataMgr = new FlatVocabValueSearchDataMgr();

        static SearchVocabularyValueSetTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void ValueSetGetList()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("MediaType", MediaTypeParms.DefaultHtmlMediaType);
            SearchParam = GetValueSetSearchParms(filterCriteria, "ValueSet");

            DataSetResult valueSetList = SearchControllerFactory.GetSearchController(SearchParam).Get();
            Assert.AreNotEqual(null, valueSetList);

            List<ValueSetObject> valueSets = (List<ValueSetObject>)valueSetList.Records;
            Assert.IsTrue(valueSets.Count > 0, "ValueSets exist in the database.");
            string a = valueSets.ToString();
        }

        [TestMethod]
        public void ValueSetInsertDelete()
        {
            string valueSetName = "ValueSet Insert";
            
            List<ValueSetObject> existingValueSets = ValueProvider.GetValueSets(valueSetName);
            if (existingValueSets.Count > 0)
                DeleteValueSet(existingValueSets[0]);

            ValueSetObject vso = CreateNewValueSetObject(valueSetName);

            //save object
            ValueProvider.InsertValueSet(vso);

            ValueSetObject vsoCompare = ValueProvider.GetValueSet(vso.Id);

            //compare objects
            CompareValueSet(vso, vsoCompare);
            
            //delete object
            DeleteValueSet(vso);
        }

        [TestMethod]
        public void ValueSetEditUpdate()
        {
            string valueSetName = "ValueSet Insert";
            List<ValueSetObject> existingValueSets = ValueProvider.GetValueSets(valueSetName);
            if( existingValueSets.Count > 0)
                DeleteValueSet(existingValueSets[0]);

            ValueSetObject vso = CreateNewValueSetObject(valueSetName);

            //save object
            ValueProvider.InsertValueSet(vso);

            //Edit object
            ValueSetObject vsoEdit = ValueProvider.GetValueSet(vso.Id);
            vsoEdit.Name = "ValueSet Edit";
            vsoEdit.Description = "Description for ValueSet Edit";

            //save updated object
            ValueProvider.UpdateValueSet(vsoEdit);

            //get updated object
            ValueSetObject vsoUpdated = ValueProvider.GetValueSet(vso.Id);
            
            //compare objects
            CompareValueSet(vsoEdit, vsoUpdated);

            //delete object
            DeleteValueSet(vso);
        }

        [TestMethod]
        public void ValueSetRelationSearch()
        {
            //Criteria filterCriteria = new Criteria();
            //filterCriteria.Add("ValueSet", new List<int>() { 1, 2 });
            //filterCriteria.Add("ValueSet", new List<string>() { "abc", "DEF" });

            //string[] valuesetid = "1,2".Split(',');
            //string lang = GetValueSetLanguage(filterCriteria, "Language");
            //filterCriteria.Add("ValueSet", valuesetid.ToList<string>().Select(a => a.Trim() + "|" + lang).ToList<string>());

            SearchParameters searchParameters = new SearchParameters("Media", "FlatVocabValue", new Sorting(new SortColumn("DisplayOrdinal", SortOrderType.Asc)),
                "ValueSet".Is(1, 2));

            DataSetResult valueSetList = flatVocabValueSearchDataMgr.GetData(searchParameters);
            Assert.AreNotEqual(null, valueSetList);

            List<FlatVocabValueItem> valueSets = (List<FlatVocabValueItem>)valueSetList.Records;
            Assert.IsTrue(valueSets.Count > 0, "ValueSets exist in the database.");
        }

        #region "Private Methods"

        private static ValueSetObject CreateNewValueSetObject(string title)
        {
            ValueSetObject newValueSetObject = new ValueSetObject()
            {
                Name = title,
                LanguageCode = "English",
                Description = "Description for " + title,
                DisplayOrdinal = 1,
                IsActive = true,
                IsDefaultable = false,
                IsOrderable = false,
                IsHierachical = false
            };
            return newValueSetObject;
        }

        private static void DeleteValueSet(ValueSetObject vso)
        {
            ValueProvider.DeleteValueSet(vso);

            //verify that the object was deleted
            ValueSetObject vsoDeleted = ValueProvider.GetValueSet(vso.Id);
            Assert.AreEqual(null, vsoDeleted);
        }

        private static void CompareValueSet(ValueSetObject vso, ValueSetObject vsoAlso)
        {
            //compare objects
            Assert.AreEqual(vso.Id, vsoAlso.Id);
            Assert.AreEqual(vso.Name, vsoAlso.Name);
            Assert.AreEqual(vso.LanguageCode, vsoAlso.LanguageCode);
            Assert.AreEqual(vso.Description, vsoAlso.Description);
            Assert.AreEqual(vso.DisplayOrdinal, vsoAlso.DisplayOrdinal);
            Assert.AreEqual(vso.IsActive, vsoAlso.IsActive);
            Assert.AreEqual(vso.IsDefaultable, vsoAlso.IsDefaultable);
            Assert.AreEqual(vso.IsOrderable, vsoAlso.IsOrderable);
            Assert.AreEqual(vso.IsHierachical, vsoAlso.IsHierachical);
        }

        private static SearchParameters GetValueSetSearchParms(Criteria filterCriteria, string dataSetCode)
        {
            SearchParameters searchParameters = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = dataSetCode,
                BasicCriteria = filterCriteria,
                Sorting = new Sorting(new SortColumn("DisplayOrdinal", SortOrderType.Asc))
            };
            return searchParameters;
        }

        private static string GetValueSetLanguage(Criteria Filter, string criteriaName)
        {
            if (Filter.GetCriterion(criteriaName) != null)
                Filter.GetCriterion(criteriaName).Values = new List<string>() { Filter.GetCriterion(criteriaName).Values[0] };
            else
                Filter.Add(criteriaName, new List<string>() { "English" });

            return Filter.GetCriterion(criteriaName).Values[0];
        }
        #endregion

    }
}
