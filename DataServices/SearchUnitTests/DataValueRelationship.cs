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
    public class DataValueRelationship
    {
        //static SearchControllerFactory SearchControllerFactory = Utility.GetSearchControllerFactory();
        //static SearchParameters SearchParam = new SearchParameters();
        //static ValueRelationshipObjectMgr objMgr = CdcDataSource.GetDataManager<ValueRelationshipObjectMgr>("Media");

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
        public void TestDataValueRelationshipValidations()
        {


            ValueRelationshipObject relationship = new ValueRelationshipObject()
            {
                //RelationshipTypeName = "Is Parent Of",
                RelationshipTypeName = null,
                ShortType = "BT",
                ValueId = 1,
                ValueLanguageCode = "English",
                RelatedValueId = 1,
                RelatedValueLanguageCode = "English",
                IsActive = true
            };

            //save object
            var errors = ValueRelationshipObjectMgr.Save(relationship).Errors().ToList();
            Assert.AreEqual(1, errors.Count());
            Assert.AreEqual("Value cannot have a reference to itself", errors[0].Message);
            //delete object
            var deleteWarnings = ValueRelationshipObjectMgr.Delete(relationship).Warnings().ToList();
            Assert.AreEqual(2, deleteWarnings.Count());
            Assert.AreEqual("Relationship not found to delete", deleteWarnings[0].Message);
        }

        [TestMethod]
        public void TestRelationshipToItself()
        {


            ValueRelationshipObject relationship = new ValueRelationshipObject()
            {
                RelationshipTypeName = "Is Parent Of",
                ValueId = 1,
                ValueLanguageCode = "English",
                RelatedValueId = 1,
                RelatedValueLanguageCode = "English",
                IsActive = true
            };

            //save object
            var errors = ValueRelationshipObjectMgr.Save(relationship).Errors().ToList();
            Assert.AreEqual(1, errors.Count());
            Assert.AreEqual("Value cannot have a reference to itself", errors[0].Message);
            //delete object
            var deleteWarnings = ValueRelationshipObjectMgr.Delete(relationship).Warnings().ToList();
            Assert.AreEqual(2, deleteWarnings.Count());
            Assert.AreEqual("Relationship not found to delete", deleteWarnings[0].Message);
        }

        private ValueRelationshipObject NewRel(string relationshipTypeName, int valueId, int relatedValueId)
        {
            return new ValueRelationshipObject()
            {
                RelationshipTypeName = relationshipTypeName,
                ValueId = valueId,
                ValueLanguageCode = "English",
                RelatedValueId = relatedValueId,
                RelatedValueLanguageCode = "English",
                IsActive = true
            };

        }

        [TestMethod]
        public void TestMultipleUses()
        {
            string multipleUseMessage = "There is more than one instance of 'Use' relationships for this relationship";

            //var cleanUp = new List<ValueRelationshipObject>() { 
            //    NewRel("Use", 446, 450),
            //    NewRel("Use", 446, 451),
            //    NewRel("Use", 453, 454)};
            //ValueRelationshipObjectMgr.Delete(cleanUp);



            //452 -> 447 and 452-->449 can't go at same time
            //447, 449, 452, 
            List<ValueRelationshipObject> relationships1 = new List<ValueRelationshipObject>() { 
                NewRel("Use", 452, 447), 
                NewRel("Use", 452, 449) };
            var errors1 = ValueRelationshipObjectMgr.Save(relationships1).Errors().ToList();
            Assert.AreEqual(1, errors1.Count());
            Assert.AreEqual(multipleUseMessage, errors1[0].Message);

            //446, 450, 451, 
            //453, 454, 455

            //446 -> 450
            //446 -> 451    Illegal
            //
            //450 -> 446 Use For Illegal



            //Save 446-450 (make persisted)
            var errors2a = ValueRelationshipObjectMgr.Save(NewRel("Use", 446, 450)).Errors().ToList();
            Assert.AreEqual(0, errors2a.Count());

            var errors2b = ValueRelationshipObjectMgr.Save(NewRel("Use", 446, 451)).Errors().ToList();
            Assert.AreEqual(1, errors2b.Count());
            Assert.AreEqual(multipleUseMessage, errors2b[0].Message);

            //This is ok
            var errors3a = ValueRelationshipObjectMgr.Save(NewRel("Use", 453, 454)).Errors().ToList();
            Assert.AreEqual(0, errors3a.Count());
            var errors3b = ValueRelationshipObjectMgr.Save(NewRel("Use", 453, 454)).Errors().ToList();
            Assert.AreEqual(0, errors3b.Count());

            //There is a use for 446->451, so a Use For for 451-446 should be illegal
            var relationships2 = new List<ValueRelationshipObject>() { 
                NewRel("Use", 453, 454), 
                NewRel("Used For", 451, 446) };
            //
            var errors4b = ValueRelationshipObjectMgr.Save(relationships2).Errors().ToList();
            Assert.AreEqual(1, errors4b.Count());
            Assert.AreEqual(multipleUseMessage, errors4b[0].Message);


            var relationships5 = new List<ValueRelationshipObject>() { 
                NewRel("Use", 453, 454), 
                NewRel("Use", 446, 450),
                NewRel("Use", 446, 451)
            };
            relationships5[1].IsActive = false; //Make 446->450 inactive, so that 446->451 should be legal
            var errors5 = ValueRelationshipObjectMgr.Save(relationships5).Errors().ToList();
            Assert.AreEqual(0, errors5.Count());

            var relationships6 = new List<ValueRelationshipObject>() { 
                NewRel("Use", 446, 450),
                NewRel("Use", 446, 451),
                NewRel("Use", 453, 454)};
            ValueRelationshipObjectMgr.Delete(relationships6);

            //save object


            //delete object
        }

        [TestMethod]
        public void ValidateRelationship()
        {

            List<ValueRelationshipObject> rel1 = new List<ValueRelationshipObject>(){
                NewRelationship(1, 2), //This is here to test an inactive to active 
                NewRelationship(2, 3), //This is here to test an active to inactive
                NewRelationship(3, 4),
                NewRelationship(4, 5),
                NewRelationship(5, 6)};
            rel1[0].RelationshipTypeName = "Inavlid Characters#@";
            rel1[1].ShortType = "Invalid Characters#@";
            rel1[2].ValueLanguageCode = "Inavlid Characters#@";
            rel1[3].RelatedValueLanguageCode = "Inavlid Characters#@";
            
            ValidationMessages msgSave1 = ValueRelationshipObjectMgr.Save(rel1);
            Assert.AreEqual(4, msgSave1.Messages.Count());
            Assert.AreEqual("ValueRelationship[0]", msgSave1.Messages[0].Key);
            Assert.AreEqual("ValueRelationship[1]", msgSave1.Messages[1].Key);
            Assert.AreEqual("ValueRelationship[2]", msgSave1.Messages[2].Key);
            Assert.AreEqual("ValueRelationship[3]", msgSave1.Messages[3].Key);

            List<ValueRelationshipObject> rel2 = new List<ValueRelationshipObject>(){
                NewRelationship(1, 1),
                NewRelationship(999999, 2314323)};

            ValidationMessages msgSave2 = ValueRelationshipObjectMgr.Save(rel2);
            Assert.AreEqual(3, msgSave2.Messages.Count());
            Assert.AreEqual("ValueRelationship[0]", msgSave2.Messages[0].Key);
            Assert.AreEqual("ValueRelationship[1]", msgSave2.Messages[1].Key);
            Assert.AreEqual("ValueRelationship[1]", msgSave2.Messages[2].Key);
        }


        private ValueRelationshipObject NewRelationship(int valueId, int relatedValueId)
        {
            return new ValueRelationshipObject()
            {
                RelationshipTypeName = "TimTest",
                ValueId = valueId,
                ValueLanguageCode = "English",
                RelatedValueId = relatedValueId,
                RelatedValueLanguageCode = "English",
                IsActive = true
            };
        }




        private ValueRelationshipObject GetRelationship(int valueId, int relatedValueId)
        {
            return ValueRelationshipObjectMgr.Get("TimTest", valueId, "English", relatedValueId, "English");
        }
        

        [TestMethod]
        public void TestInvalidRelationship()
        {

            List<ValueRelationshipObject> rel1 = new List<ValueRelationshipObject>(){
                NewRelationship(9, 12), //This is here to test an inactive to active 
                NewRelationship(9, 13), //This is here to test an active to inactive
                NewRelationship(10, 11),
                NewRelationship(11, 12),
                NewRelationship(13, 14)};

            rel1[0].IsActive = false;
            rel1[1].IsActive = true;

            List<ValueRelationshipObject> rel2 = new List<ValueRelationshipObject>(){
                NewRelationship(9, 12), //This is here to test an inactive to active 
                NewRelationship(9, 13), //This is here to test an active to inactive
                NewRelationship(11, 13),
                NewRelationship(14, 11),      //Invalid  1 3 5
                NewRelationship(14, 15)};   //Make 13, 14 active
            rel2[0].IsActive = true;        //Swap these
            rel2[1].IsActive = false;

            //Clean up first
            ValueRelationshipObjectMgr.Delete(rel1);
            ValueRelationshipObjectMgr.Delete(rel2);
            
            ValidationMessages msgSave1 = ValueRelationshipObjectMgr.Save(rel1);
            Assert.AreEqual(0, msgSave1.Messages.Count());
            Assert.AreEqual(true, GetRelationship(13, 14).IsCommitted);

            ValidationMessages msgSave2 = ValueRelationshipObjectMgr.Save(rel2);
            
            Assert.AreEqual(14, msgSave2.Messages.Count());
            //Assert.AreEqual("ValueSet Topics(2,English) cannot have a relationship of 'Is Parent Of' with value set 'Categories(1,English)'", 
            //    msgSave2.Messages[0].Message);

            //Verify rollback occurred
            Assert.AreEqual(false, GetRelationship(9, 12).IsActive);
            Assert.AreEqual(true, GetRelationship(9, 13).IsActive);
            Assert.AreEqual(null, GetRelationship(11, 13));
            Assert.AreEqual(null, GetRelationship(14, 1));
            Assert.AreEqual(null, GetRelationship(14, 15));

            ValueRelationshipObjectMgr.Delete(rel1);
        }

        [TestMethod]
        public void ValueRelationshipInsertDelete()
        {
            ValueRelationshipObject newObj = CreateObject("ValueRelationship Insert");

            //save object
            ValueRelationshipObjectMgr.Save(newObj);

            ValueRelationshipObject vsoCompare = ValueRelationshipObjectMgr.Get(newObj.RelationshipTypeName, newObj.ValueId, newObj.ValueLanguageCode,
                newObj.RelatedValueId, newObj.RelatedValueLanguageCode);

            //compare objects
            CompareObject(newObj, vsoCompare);

            //delete object
            DeleteObject(newObj);
        }

        [TestMethod]
        public void ValueRelationshipEditUpdate()
        {
            ValueRelationshipObject newObj = CreateObject("ValueRelationship Insert");

            //save object
            ValidationMessages msg1 = ValueRelationshipObjectMgr.Save(newObj);

            //Edit object
            ValueRelationshipObject vsoEdit = ValueRelationshipObjectMgr.Get(newObj.RelationshipTypeName, newObj.ValueId, newObj.ValueLanguageCode,
                newObj.RelatedValueId, newObj.RelatedValueLanguageCode);
                        
            vsoEdit.DisplayOrdinal = 2;
            vsoEdit.IsActive = true;

            //save updated object
            ValueRelationshipObjectMgr.Save(vsoEdit);

            //get updated object
            ValueRelationshipObject vsoUpdated = ValueRelationshipObjectMgr.Get(newObj.RelationshipTypeName, newObj.ValueId, newObj.ValueLanguageCode,
                newObj.RelatedValueId, newObj.RelatedValueLanguageCode);

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

        private static ValueRelationshipObject CreateObject(string title)
        {
            ValueRelationshipObject newValueToValueSetObject = new ValueRelationshipObject()
            {
                RelationshipTypeName = "Is Child Of",
                ValueId = 1,
                ValueLanguageCode = "English",
                RelatedValueId = 8,
                RelatedValueLanguageCode = "English",
                IsActive = true
            };
            return newValueToValueSetObject;
        }

        private void DeleteObject(ValueRelationshipObject newObj)
        {
            ValueRelationshipObjectMgr.Delete(newObj);

            //verify that the object was deleted
            ValueRelationshipObject vsoDeleted = ValueRelationshipObjectMgr.Get(newObj.RelationshipTypeName, newObj.ValueId, newObj.ValueLanguageCode,
                newObj.RelatedValueId, newObj.RelatedValueLanguageCode);
            Assert.AreEqual(null, vsoDeleted);
        }

        private static void CompareObject(ValueRelationshipObject newObj, ValueRelationshipObject vsoAlso)
        {
            //compare objects
            Assert.AreEqual(newObj.RelationshipTypeName, vsoAlso.RelationshipTypeName);
            Assert.AreEqual(newObj.ValueId, vsoAlso.ValueId);
            Assert.AreEqual(newObj.ValueLanguageCode, vsoAlso.ValueLanguageCode);
            Assert.AreEqual(newObj.RelatedValueId, vsoAlso.RelatedValueId);
            Assert.AreEqual(newObj.RelatedValueLanguageCode, vsoAlso.RelatedValueLanguageCode);
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
