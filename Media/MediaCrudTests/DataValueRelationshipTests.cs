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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Connection;

namespace MediaCrudTests
{
    [TestClass]
    public class DataValueRelationshipTests
    {
        public static IValueProvider ValueProvider = new CsValueProvider();
        private static int testValueId = 25221;
        private static int testValueId2 = 25222;

        static DataValueRelationshipTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void TestSavingValueWithModifyingRelationships()
        {
            ValueObject theObject = CreateAndSaveValueObject();
            int valueId = theObject.ValueId;

            theObject.ValueRelationships = new List<ValueRelationshipObject>()
                {
                    CreateNewRelationship("Is Child Of", valueId, testValueId),
                    CreateNewRelationship("Is Child Of", valueId, testValueId2)
                };
            ValidationMessages messages2 = ValueProvider.SaveValue(theObject);
            foreach (var err in messages2.Errors())
            {
                Console.WriteLine(err.Message);
            }
            if (messages2.Errors().Count() > 0)
            {
                Assert.Fail(messages2.Errors().First().Message);
            }

            ValueObject savedValueObject2 = ValueProvider.GetValue(valueId);
            Assert.AreEqual(2, savedValueObject2.ValueRelationships.Count());
            Assert.AreEqual(testValueId2, savedValueObject2.ValueRelationships.ToList()[1].RelatedValueId);

            theObject.ValueRelationships = new List<ValueRelationshipObject>()
                {
                    CreateNewRelationship("Is Child Of", valueId, testValueId2)
                };
            ValidationMessages messages3 = ValueProvider.SaveValue(theObject);
            foreach (var err in messages3.Errors())
            {
                Console.WriteLine(err.Message);
            }
            if (messages3.Errors().Count() > 0)
            {
                Assert.Fail(messages3.Errors().Count() + " errors");
            }

            ValueObject savedValueObject3 = ValueProvider.GetValue(valueId);
            Assert.AreEqual(1, savedValueObject3.ValueRelationships.Count());
            Assert.AreEqual(testValueId2, savedValueObject3.ValueRelationships.ToList()[0].RelatedValueId);

            ValueProvider.DeleteValue(savedValueObject3);
        }

        private static ValueObject CreateAndSaveValueObject()
        {
            ValueObject theObject = new ValueObject()
            {
                ValueName = "DataValueRelationshipTests " + DateTime.Now.Ticks.ToString(),
                LanguageCode = "English",
                Description = "Test Data",
                //DisplayOrdinal = 0,                
            };

            theObject.ValueToValueSetObjects = new List<ValueToValueSetObject>()
            {
                GetValueToValueSet(2, 0, theObject.LanguageCode)  //, value.LanguageCode)
            };
            ValidationMessages messages1 = ValueProvider.SaveValue(theObject);
            if (messages1.NumberOfErrors > 0)
            {
                Assert.Fail(messages1.Errors().First().Message);
            }

            return theObject;
        }

        [TestMethod]
        public void TestAddDeleteRelationships()
        {
            ValueObject theObject = CreateAndSaveValueObject();
            int valueId = theObject.ValueId;

            // Add relationship #1
            theObject.ValueRelationships = new List<ValueRelationshipObject>()
                {
                    CreateNewRelationship("Is Child Of", valueId, testValueId)
                };
            ValueProvider.AddRelationships(theObject);
            ValueObject savedValueObject3 = ValueProvider.GetValue(valueId);
            Assert.AreEqual(1, savedValueObject3.ValueRelationships.Count());

            // Add relationship #2
            theObject.ValueRelationships = new List<ValueRelationshipObject>()
                {
                    CreateNewRelationship("Is Child Of", valueId, testValueId2)
                };
            ValueProvider.AddRelationships(theObject);
            savedValueObject3 = ValueProvider.GetValue(valueId);
            Assert.AreEqual(2, savedValueObject3.ValueRelationships.Count());

            // Delete relationship #1
            theObject.ValueRelationships = new List<ValueRelationshipObject>()
                {
                    CreateNewRelationship("Is Child Of", valueId, testValueId)
                };
            ValueProvider.DeleteRelationships(theObject);
            savedValueObject3 = ValueProvider.GetValue(valueId);
            Assert.AreEqual(1, savedValueObject3.ValueRelationships.Count());

            ValueProvider.DeleteValue(savedValueObject3);
        }


        [TestMethod]
        public void TestDataValueRelationshipValidations()
        {
            ValueRelationshipObject relationship = new ValueRelationshipObject()
            {
                RelationshipTypeName = null,
                ShortType = "BT",
                ValueId = testValueId,
                ValueLanguageCode = "English",
                RelatedValueId = testValueId,
                RelatedValueLanguageCode = "English",
                IsActive = true
            };

            //save object
            var errors = ValueProvider.SaveValueRelationship(relationship).Errors().ToList();
            Assert.AreEqual(1, errors.Count());
            Assert.AreEqual("Value cannot have a reference to itself", errors[0].Message);
            //delete object
            var deleteWarnings = ValueProvider.DeleteValueRelationship(relationship).Warnings().ToList();
            Assert.AreEqual(2, deleteWarnings.Count());
            Assert.AreEqual("Relationship not found to delete", deleteWarnings[0].Message);
        }

        [TestMethod]
        public void TestRelationshipToItself()
        {
            ValueRelationshipObject relationship = new ValueRelationshipObject()
            {
                RelationshipTypeName = "Is Parent Of",
                ValueId = testValueId,
                ValueLanguageCode = "English",
                RelatedValueId = testValueId,
                RelatedValueLanguageCode = "English",
                IsActive = true
            };

            //save object
            var errors = ValueProvider.SaveValueRelationship(relationship).Errors().ToList();
            Assert.AreEqual(1, errors.Count());
            Assert.AreEqual("Value cannot have a reference to itself", errors[0].Message);
            //delete object
            var deleteWarnings = ValueProvider.DeleteValueRelationship(relationship).Warnings().ToList();
            Assert.AreEqual(2, deleteWarnings.Count());
            Assert.AreEqual("Relationship not found to delete", deleteWarnings[0].Message);
        }

        private ValueRelationshipObject CreateNewRelationship(string relationshipTypeName, int valueId, int relatedValueId)
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

        private static ValueToValueSetObject GetValueToValueSet(int valueSetId, int valueId, string languageCode)
        {
            ValueToValueSetObject valueToValueSet = new ValueToValueSetObject();
            valueToValueSet.ValueId = valueId;
            valueToValueSet.ValueLanguageCode = languageCode;
            //valueToValueSet.ValueToValueSetName 

            valueToValueSet.ValueSetId = valueSetId;
            valueToValueSet.ValueSetLanguageCode = languageCode;
            //valueToValueSet.IsDefault = false;

            return valueToValueSet;
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
                CreateNewRelationship("Use", 452, 447), 
                CreateNewRelationship("Use", 452, 449) };
            var errors1 = (new CsValueProvider()).SaveValueRelationship(relationships1).Errors().ToList();
            Assert.AreEqual(1, errors1.Count(), errors1.First().Message);
            Assert.AreEqual(multipleUseMessage, errors1[0].Message);

            //446, 450, 451, 
            //453, 454, 455

            //446 -> 450
            //446 -> 451    Illegal
            //
            //450 -> 446 Use For Illegal



            //Save 446-450 (make persisted)
            var errors2a = ValueProvider.SaveValueRelationship(CreateNewRelationship("Use", 446, 450)).Errors().ToList();
            Assert.AreEqual(0, errors2a.Count());

            var errors2b = ValueProvider.SaveValueRelationship(CreateNewRelationship("Use", 446, 451)).Errors().ToList();
            Assert.AreEqual(1, errors2b.Count());
            Assert.AreEqual(multipleUseMessage, errors2b[0].Message);

            //This is ok
            var errors3a = ValueProvider.SaveValueRelationship(CreateNewRelationship("Use", 453, 454)).Errors().ToList();
            Assert.AreEqual(0, errors3a.Count());
            var errors3b = ValueProvider.SaveValueRelationship(CreateNewRelationship("Use", 453, 454)).Errors().ToList();
            Assert.AreEqual(0, errors3b.Count());

            //There is a use for 446->451, so a Use For for 451-446 should be illegal
            var relationships2 = new List<ValueRelationshipObject>() { 
                CreateNewRelationship("Use", 453, 454), 
                CreateNewRelationship("Used For", 451, 446) };
            //
            var errors4b = ValueProvider.SaveValueRelationship(relationships2).Errors().ToList();
            Assert.AreEqual(1, errors4b.Count());
            Assert.AreEqual(multipleUseMessage, errors4b[0].Message);


            var relationships5 = new List<ValueRelationshipObject>() { 
                CreateNewRelationship("Use", 453, 454), 
                CreateNewRelationship("Use", 446, 450),
                CreateNewRelationship("Use", 446, 451)
            };
            relationships5[1].IsActive = false; //Make 446->450 inactive, so that 446->451 should be legal
            var errors5 = ValueProvider.SaveValueRelationship(relationships5).Errors().ToList();
            Assert.AreEqual(0, errors5.Count());

            var relationships6 = new List<ValueRelationshipObject>() { 
                CreateNewRelationship("Use", 446, 450),
                CreateNewRelationship("Use", 446, 451),
                CreateNewRelationship("Use", 453, 454)};
            ValueProvider.DeleteValueRelationship(relationships6);
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

            ValidationMessages msgSave1 = ValueProvider.SaveValueRelationship(rel1);
            Assert.AreEqual(4, msgSave1.Messages.Count());
            Assert.AreEqual("ValueRelationship[0]", msgSave1.Messages[0].Key);
            Assert.AreEqual("ValueRelationship[1]", msgSave1.Messages[1].Key);
            Assert.AreEqual("ValueRelationship[2]", msgSave1.Messages[2].Key);
            Assert.AreEqual("ValueRelationship[3]", msgSave1.Messages[3].Key);

            List<ValueRelationshipObject> rel2 = new List<ValueRelationshipObject>(){
                NewRelationship(1, 1),
                NewRelationship(999999, 2314323)};

            ValidationMessages msgSave2 = ValueProvider.SaveValueRelationship(rel2);
            Assert.AreEqual(5, msgSave2.Messages.Count());
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

        [TestMethod]
        public void TestInvalidRelationship()
        {
            ValueProvider.DeleteValueRelationship(ValueProvider.GetValueRelationships("TimTest"));
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
            ValueProvider.DeleteValueRelationship(rel1);
            ValueProvider.DeleteValueRelationship(rel2);

            ValidationMessages msgSave1 = ValueProvider.SaveValueRelationship(rel1);
            Assert.AreEqual(0, msgSave1.Messages.Count(), msgSave1.Messages.First().Message);
            Assert.AreEqual(true, GetRelationship(13, 14).IsCommitted);

            ValidationMessages msgSave2 = ValueProvider.SaveValueRelationship(rel2);

            Assert.AreEqual(14, msgSave2.Messages.Count());

            //Verify rollback occurred
            Assert.AreEqual(false, GetRelationship(9, 12).IsActive);
            Assert.AreEqual(true, GetRelationship(9, 13).IsActive);
            Assert.AreEqual(null, GetRelationship(11, 13));
            Assert.AreEqual(null, GetRelationship(14, 1));
            Assert.AreEqual(null, GetRelationship(14, 15));

            ValueProvider.DeleteValueRelationship(rel1);
        }

        private ValueRelationshipObject GetRelationship(int valueId, int relatedValueId)
        {
            return ValueProvider.GetValueRelationship("TimTest", valueId, "English", relatedValueId, "English");
        }

        [TestMethod]
        public void ValueRelationshipInsertDelete()
        {
            ValueRelationshipObject newObj = CreateObject("ValueRelationship Insert");

            var messages = ValueProvider.SaveValueRelationship(newObj);

            if (messages.Errors().Count() > 0)
            {
                Assert.Fail(messages.Errors().First().Message);
            }

            ValueRelationshipObject vsoCompare = ValueProvider.GetValueRelationship(newObj.RelationshipTypeName, newObj.ValueId, newObj.ValueLanguageCode,
                newObj.RelatedValueId, newObj.RelatedValueLanguageCode);

            Assert.IsNotNull(vsoCompare);

            CompareObject(newObj, vsoCompare);

            DeleteObject(newObj);
        }

        [TestMethod]
        public void ValueRelationshipEditUpdate()
        {
            ValueRelationshipObject newObj = CreateObject("ValueRelationship Insert");

            ValidationMessages msg1 = ValueProvider.SaveValueRelationship(newObj);

            ValueRelationshipObject vsoEdit = ValueProvider.GetValueRelationship(newObj.RelationshipTypeName, newObj.ValueId, newObj.ValueLanguageCode,
                newObj.RelatedValueId, newObj.RelatedValueLanguageCode);

            Assert.IsNotNull(vsoEdit);
            vsoEdit.DisplayOrdinal = 2;
            vsoEdit.IsActive = true;

            ValueProvider.SaveValueRelationship(vsoEdit);

            ValueRelationshipObject vsoUpdated = ValueProvider.GetValueRelationship(newObj.RelationshipTypeName, newObj.ValueId, newObj.ValueLanguageCode,
                newObj.RelatedValueId, newObj.RelatedValueLanguageCode);

            CompareObject(vsoEdit, vsoUpdated);

            DeleteObject(newObj);
        }

        #region "Private Methods"

        private static ValueRelationshipObject CreateObject(string title)
        {
            ValueRelationshipObject newValueToValueSetObject = new ValueRelationshipObject()
            {
                RelationshipTypeName = "Is Child Of",
                ValueId = testValueId,
                ValueLanguageCode = "English",
                RelatedValueId = testValueId2,
                RelatedValueLanguageCode = "English",
                IsActive = true
            };
            return newValueToValueSetObject;
        }

        private void DeleteObject(ValueRelationshipObject newObj)
        {
            ValueProvider.DeleteValueRelationship(newObj);

            //verify that the object was deleted
            ValueRelationshipObject vsoDeleted = ValueProvider.GetValueRelationship(newObj.RelationshipTypeName, newObj.ValueId, newObj.ValueLanguageCode,
                newObj.RelatedValueId, newObj.RelatedValueLanguageCode);
            Assert.AreEqual(null, vsoDeleted);
        }

        private static void CompareObject(ValueRelationshipObject newObj, ValueRelationshipObject vsoAlso)
        {
            Assert.AreEqual(newObj.RelationshipTypeName, vsoAlso.RelationshipTypeName);
            Assert.AreEqual(newObj.ValueId, vsoAlso.ValueId);
            Assert.AreEqual(newObj.ValueLanguageCode, vsoAlso.ValueLanguageCode);
            Assert.AreEqual(newObj.RelatedValueId, vsoAlso.RelatedValueId);
            Assert.AreEqual(newObj.RelatedValueLanguageCode, vsoAlso.RelatedValueLanguageCode);
            Assert.AreEqual(newObj.DisplayOrdinal, vsoAlso.DisplayOrdinal);
            Assert.AreEqual(newObj.IsActive, vsoAlso.IsActive);
        }

        #endregion

    }
}
