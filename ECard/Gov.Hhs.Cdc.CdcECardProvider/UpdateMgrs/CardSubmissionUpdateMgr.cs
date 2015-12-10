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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.ECardProvider;
using Gov.Hhs.Cdc.CdcECardProvider.DataAccess;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcECardProvider
{
    public class CardSubmissionUpdateMgr  : IUpdateMgr
    {
        public string ObjectName { get { return "CardInstance"; } }

        protected IValidator<CardInstanceObject, CardInstanceObject> InstanceValidator { get; set; }
        protected IValidator<CardMessageObject, CardMessageObject> MessageValidator { get; set; }

        protected List<CardInstanceValidationItem> ValidationItems;
        public IList<CardInstanceObject> AllCardInstanceObjects { get { return ValidationItems.SelectMany(i => i.Instances).ToList(); } }
        public IList<CardMessageObject> AllCardMessageObjects { get { return ValidationItems.SelectMany(i => i.CardMessagesForValidation).ToList(); } }

        public List<IDataCtl> InsertedDataControls { get; set; }

        public class CardInstanceValidationItem
        {
            public IList<CardInstanceObject> Instances { get; set; }
            public IList<CardMessageObject> CardMessagesForValidation { get; set; }
            public CardMessageObject Message { get { return CardMessagesForValidation.FirstOrDefault(); } }

            public CardInstanceValidationItem(CardSubmission cardSubmission)
            {
                Instances = ValidatorHelper.GetWithValidationKey<CardInstanceObject>(cardSubmission.Instances, "CardInstanceObject");
                CardMessagesForValidation = ValidatorHelper.GetWithValidationKey<CardMessageObject>(cardSubmission.Message, "CardMessageObject");
            }
        }

        public bool RequiresLogicalCommit { get { return false; } }

        public CardSubmissionUpdateMgr()
        {
            InstanceValidator = new CardInstanceObjectValidator();
            MessageValidator = new CardMessageObjectValidator();
        }
        public CardSubmissionUpdateMgr(IList<CardSubmission> items) : this()
        {
            ValidationItems = (from i in items select new CardInstanceValidationItem(i)).ToList();
        }

        public CardSubmissionUpdateMgr(CardSubmission item) : this()
        {
            ValidationItems = new List<CardInstanceValidationItem>(){new CardInstanceValidationItem(item)};
        }

        public void Save(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }

        public void Insert(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            InsertedDataControls = new List<IDataCtl>();
            foreach (CardInstanceValidationItem item in ValidationItems)
            {
                CardMessageObjectCtl newMessage = CardMessageObjectCtl.Create(media, item.Message);
                InsertedDataControls.Add(newMessage);
                foreach (CardInstanceObject instance in item.Instances)
                {
                    instance.CardInstanceId = Guid.NewGuid();
                    CardInstanceObjectCtl instanceObjectCtl = CardInstanceObjectCtl.Create(media, instance);
                    //TODO: Move this to the data layer
                    newMessage.PersistedDbObject.CardInstances.Add(instanceObjectCtl.PersistedDbObject);
                }

                newMessage.AddToDb();
            }
        }

        public void Update(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            var db = media as ECardObjectContext;
            foreach (CardInstanceValidationItem item in ValidationItems)
            {
                CardMessageObject peristedMessage = CardMessageObjectCtl
                    .Get(db, forUpdate: true)
                    .Where(u => u.CardMessageId == item.Message.CardMessageId).FirstOrDefault();

                List<CardInstanceObject> persistedInstances = CardInstanceObjectCtl
                    .Get(db, forUpdate: true)
                    .Where(u => u.CardMessageId == item.Message.CardMessageId).ToList();

                //TODO: Create Update logic
                //CardInstanceObject persistedObject = CardInstanceObjectCtl
                //                .Get((ECardObjectContext)objectContext, forUpdate: true)
                //                .Where(u => u.CardInstanceId == item.CardInstanceId).FirstOrDefault();
                //CardInstanceObjectCtl.Update(objectContext, persistedObject, item, enforceConcurrency: false);
            }
            
        }



        public void Delete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages)
        {
            foreach (CardInstanceValidationItem item in ValidationItems)
            {
                CardMessageObject peristedMessage = CardMessageObjectCtl
                    .Get((ECardObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.CardMessageId == item.Message.CardMessageId).FirstOrDefault();

                List<CardInstanceObject> persistedInstances = CardInstanceObjectCtl
                    .Get((ECardObjectContext)objectContext, forUpdate: true)
                    .Where(u => u.CardMessageId == item.Message.CardMessageId).ToList();
                foreach (CardInstanceObject persistedInstance in persistedInstances)
                    CardInstanceObjectCtl.Delete(objectContext, persistedInstance);

                if (peristedMessage != null)
                {
                    CardMessageObjectCtl.Delete(objectContext, peristedMessage);
                }
            }
        }


        

        public void PreSaveValidate(ref ValidationMessages validationMessages)
        {
            InstanceValidator.PreSaveValidate(ref validationMessages, AllCardInstanceObjects);
            MessageValidator.PreSaveValidate(ref validationMessages, AllCardMessageObjects);
        }

        public void PreDeleteValidate(ValidationMessages validationMessages)
        {
            InstanceValidator.PreDeleteValidate(validationMessages, AllCardInstanceObjects);
            MessageValidator.PreDeleteValidate(validationMessages, AllCardMessageObjects);
        }

        public void ValidateSave(IDataServicesObjectContext eCardDb, ValidationMessages validationMessages)
        {
            InstanceValidator.ValidateSave(eCardDb, validationMessages, AllCardInstanceObjects);
            MessageValidator.ValidateSave(eCardDb, validationMessages, AllCardMessageObjects);
        }

        public void ValidateDelete(IDataServicesObjectContext eCardDb, ValidationMessages validationMessages)
        {
            InstanceValidator.ValidateDelete(eCardDb, validationMessages, AllCardInstanceObjects);
            MessageValidator.ValidateDelete(eCardDb, validationMessages, AllCardMessageObjects);
        }


        public void AdditionalSave(IDataServicesObjectContext eCardDb, ValidationMessages validationMessages)
        {
        }

        public void PostSaveValidate(IDataServicesObjectContext eCardDb, ValidationMessages validationMessages)
        {
        }

        public void LogicalCommit(IDataServicesObjectContext eCardDb, ValidationMessages validationMessages)
        {
        }

        public void LogicalRollback(IDataServicesObjectContext media, ValidationMessages validationMessages)
        {
            throw new NotImplementedException();
        }
        

        public void UpdateIdsAfterInsert()
        {
            if (InsertedDataControls != null)
            {
                foreach (IDataCtl dataCtl in InsertedDataControls)
                {
                    dataCtl.UpdateIdsAfterInsert();
                }
            }
        }


        public void BuildValidationObjects(IDataServicesObjectContext eCardDb, ValidationMessages validationMessages)
        {
        }








    }
}
