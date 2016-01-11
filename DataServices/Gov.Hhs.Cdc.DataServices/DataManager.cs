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
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Transactions;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;

namespace Gov.Hhs.Cdc.DataServices
{
    public class DataManager
    {
        protected IObjectContextFactory ObjectContextFactory { get; set; }
        public DataManager(IObjectContextFactory objectContextFactory)
        {
            ObjectContextFactory = objectContextFactory;
        }

        public virtual ValidationMessages Update(IUpdateMgr updateMgr)
        {
            return Save(updateMgr, isUpdate: true);
        }

        public virtual ValidationMessages Insert(IUpdateMgr updateMgr)
        {
            return Save(updateMgr, isInsert: true);
        }
        
        public virtual ValidationMessages Save(IUpdateMgr updateMgr, bool isUpdate = false, bool isInsert = false)
        {
            ValidationMessages validationMessages = new ValidationMessages();
            try
            {
                updateMgr.PreSaveValidate(ref validationMessages);
                if (validationMessages.Errors().Any())
                {
                    return validationMessages;
                }

                using (IDataServicesObjectContext objectContext = ObjectContextFactory.Create())
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            updateMgr.BuildValidationObjects(objectContext, validationMessages);
                            if (validationMessages.Errors().Any())
                            {
                                return validationMessages;
                            }

                            updateMgr.ValidateSave(objectContext, validationMessages);
                            if (validationMessages.Errors().Any())
                            {
                                return validationMessages;
                            }

                            if (isUpdate)
                            {
                                updateMgr.Update(objectContext, validationMessages);
                            }
                            else if (isInsert)
                            {
                                updateMgr.Insert(objectContext, validationMessages);
                            }
                            else
                            {
                                updateMgr.Save(objectContext, validationMessages);
                            }

                            if (validationMessages.Errors().Any())
                            {
                                return validationMessages;
                            }

                            updateMgr.AdditionalSave(objectContext, validationMessages);

                            objectContext.SaveChanges(SaveOptions.DetectChangesBeforeSave);
                            objectContext.AcceptAllChanges();

                            updateMgr.UpdateIdsAfterInsert();


                            scope.Complete();
                            
                        }
                        catch (OptimisticConcurrencyException ex)
                        {
                            validationMessages.Add(
                                new ValidationMessage(
                                    ValidationMessage.ValidationSeverity.Error, "", "", "The data has changed since your last request", ex.StackTrace));

                        }
                    }
                }
                if (updateMgr.RequiresLogicalCommit)
                {
                    //Start a new transaction for the rollback

                    using (IDataServicesObjectContext objectContext = ObjectContextFactory.Create())
                    {
                        updateMgr.PostSaveValidate(objectContext, validationMessages);
                        if (validationMessages.Errors().Any())
                        {
                            updateMgr.LogicalRollback(objectContext, validationMessages);
                        }
                        else
                        {
                            updateMgr.LogicalCommit(objectContext, validationMessages);
                        }
                        objectContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                var message =  "An error occurred while saving " + updateMgr.ObjectName + " to the database";
                var baseEx = ex.GetBaseException();
                Logger.LogError(baseEx, message);
                validationMessages.AddError("DataManager.Save", "", message, baseEx.Message);
            }

            return validationMessages;
        }

        public ValidationMessages Delete(IUpdateMgr updateMgr)
        {
            ValidationMessages validationMessages = new ValidationMessages();
            try
            {
                updateMgr.PreDeleteValidate(validationMessages);
                if (validationMessages.Errors().Any())
                {
                    return validationMessages;
                }

                using (IDataServicesObjectContext objectContext = ObjectContextFactory.Create())
                {
                    updateMgr.BuildValidationObjects(objectContext, validationMessages);
                    updateMgr.ValidateDelete(objectContext, validationMessages);
                    if (validationMessages.Errors().Any())
                    {
                        return validationMessages;
                    }
                    updateMgr.Delete(objectContext, validationMessages);
                    if (!validationMessages.Errors().Any())
                    {
                        objectContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred in Delete for type: " + updateMgr.ObjectName);
                validationMessages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "Value", ex.Message));
            }
            return validationMessages;
        }

    }

}
