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
using System.Threading;
using Gov.Hhs.Cdc.DataSource.MediaValidation;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;
using Gov.Hhs.Cdc.Media.Bll;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidator;
using Gov.Hhs.Cdc.MediaValidatonProvider;

namespace Gov.Hhs.Cdc.MediaValidation
{
    public class ValidationManager
    {
        private MultiThreadedQueue<CollectionItem> CollectionItemQueue {get; set;}

        object counterLock = new object();
        object logLock = new object();
        private int numberToCollect = 0;
        private int numberToValidate = 0;

        private static FilterFileNotFoundException filterFileNotFoundException = null;
        private static object exceptionLock = new object();

        private bool AnyLeftToProcess
        {
            get
            {
                lock (counterLock)  
                {   
                    return (numberToCollect + numberToValidate) > 0; 
                }
            }
        }

        public void Validate(MediaTypeValidationItem mediaType, IMediaCollector collector, IMediaValidator validator,
            List<MediaAddress> addresses, MediaValidationConfig config)
        {
            //Don't use the queue if only 1 thread.  It makes for easier debugging
            if( config.NumberOfThreads > 1)
                CollectionItemQueue = new MultiThreadedQueue<CollectionItem>(config.NumberOfThreads, this.Collect);    //Setting the thread count to 200 (or 12) is the optimal thread count for finding lockup issues

            string previousUniqueKey = "";
            //var handles = new ManualResetEvent[addresses.Count()];

            foreach (MediaAddress mediaAddress in addresses)
            {

                //We may have duplicate URLs between mobile and regular site, so only process one of them.
                string currentUniqueKey = mediaAddress.GetUniqueKey();
                if (currentUniqueKey == null || currentUniqueKey != previousUniqueKey)
                {
                    CollectionItem collectionItem = new CollectionItem() { 
                        MediaType = mediaType, Collector = collector, Validator = validator, 
                        MediaAddress = mediaAddress, Config = config};

                    if( config.NumberOfThreads > 1)
                        CollectionItemQueue.EnqueueTask(collectionItem);
                    else
                        Collect(collectionItem);
                    lock (counterLock)
                    {
                        numberToCollect++;
                    }


                    
                    //ThreadPool.QueueUserWorkItem(o => MediaMgmt.Collect(collectionItem));
                }
                previousUniqueKey = currentUniqueKey;
            }

            int millisecondsToSleep = 500;
            while (AnyLeftToProcess)
                Thread.Sleep(millisecondsToSleep);
            if (filterFileNotFoundException != null)
                throw filterFileNotFoundException;

            //WaitHandle.WaitAll(handles);
            int x = addresses.Count;
        }

        private void Collect(CollectionItem collectionItem)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                ValidationItem validationItem;
                try
                {
                    //Force an error for log testing
                    //int a = 0;
                    //int b = a / a;
                    CollectedData collectedData = collectionItem.Collector.Get(collectionItem.MediaAddress, collectionItem.Config.ValidatorConfig);
                    validationItem = new ValidationItem() { MediaType = collectionItem.MediaType, 
                        Validator = collectionItem.Validator, 
                        MediaAddress = collectionItem.MediaAddress, 
                        CollectedData = collectedData, 
                        Config = collectionItem.Config};
                    collectedData = null;

                    //ToDo: Uncomment once the memory issue is cleaned up
                    //lock (validationItemQueue)
                    //{
                    //    validationItemQueue.EnqueueTask(validationItem);
                    //}
                    Validate(validationItem);
                    lock (counterLock)
                    {
                        numberToValidate++;
                    }
                }
                catch (FilterFileNotFoundException fex)
                {
                    filterFileNotFoundException = fex;
                    return;
                }
                catch (Exception ex)
                {
                    //Do not wait to validate if there was an error 
                    lock (logLock)
                    {
                        ErrorLogger.GetExceptionAndLogError(ex, "ValidationManager.Collect", "");
                    }
                }
                TimeSpan timeSpan = DateTime.Now.Subtract(startTime);
                if (timeSpan.Seconds >= 20)
                {
                    string timeTaken = string.Format("{0:hh\\:mm\\:ss\\.f}", timeSpan);
                    lock (logLock)
                    {
                        AuditLogger.LogAuditEvent("Accessing media item(" + collectionItem.MediaAddress.GetUniqueKey() + ") took: " + timeTaken);
                    }
                }
            }
            finally
            {
                lock (counterLock)
                {
                    numberToCollect--;
                }
            }
        }
        private void Validate(ValidationItem validationItem)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                ExtractionResult ValidatedData = validationItem.Validator.ExtractAndValidate(validationItem.MediaAddress, 
                    validationItem.CollectedData, validationItem.Config.ValidatorConfig, validationItem.MediaType, isExtraction: false);

                bool itemTemporarilyUnavailable = false;
                bool addNewReviewRecord = false;
                string parentSource = validationItem.MediaAddress.SourceTable;
                int sourceId = validationItem.MediaAddress.SourceId;
                bool resetUnavailableCount = false;

                ResultHistory oldOnes = ResultsAndReviewMgr.GetPreviousResults(parentSource, sourceId);
                ExtractionResult lastGoodResult = oldOnes.LastGood != null ? oldOnes.LastGood.Result : null;
                ResultAndReview current = new ResultAndReview()
                    {
                        Result = ValidatedData,
                        Review = validationItem.Validator.GetAnalyzer().AnalyzeResults(validationItem.MediaType, ValidatedData, lastGoodResult)
                    };

                ResultAndReview previous = oldOnes.Previous;

                ValidationResultsMgr resultsMgr = new ValidationResultsMgr();

                //bool isTheFirstTimeValidatingThisMediaItem = previous == null;
                if (previous == null)
                {
                    ExtractionResult currentResult = current.Result;
                    ValidationChange currentReview = current.Review;
                    ValidationResultsMgr.InsertTheValidationResults(current, currentResult, currentReview);
                }
                else
                {
                    bool itemCurrentlyUnderReview = (previous.Review != null && previous.Review.RequiresAttention && !previous.Review.IsResolved);
                    bool mediaItemHasChanged = current.Result.HasChanged(previous.Result);
                    bool mediaItemHasBeenCorrected = itemCurrentlyUnderReview && !current.Review.RequiresAttention;
                    bool hasChangedSinceLastKnownGoodState = current.Review.HasChangedSinceLastKnownGoodState();

                    if (previous != null && previous.Result != null)
                    {
                        itemTemporarilyUnavailable = mediaItemHasChanged && !current.Result.CollectedData.IsAvailable && previous.Result.UnavailableCount < 2;
                        resetUnavailableCount = current.Result.CollectedData.IsAvailable && previous.Result.UnavailableCount > 0;
                    }

                    //If this is not the first time and nothing has changed and the RequiresAttention hasn't changed, then do nothing
                    if (current.Review.RequiresAttention)
                    {
                        if (itemCurrentlyUnderReview)
                            addNewReviewRecord = mediaItemHasChanged ? true : false;
                        else
                            addNewReviewRecord = true;
                    }
                    else if (mediaItemHasBeenCorrected || mediaItemHasChanged || hasChangedSinceLastKnownGoodState)
                        addNewReviewRecord = true;
                    //else if (mediaItemHasChanged)
                    //    justSaveValidationResults = true;
                    ValidationResultsMgr.SaveTheValidationResults(itemTemporarilyUnavailable, addNewReviewRecord, resetUnavailableCount, current, previous);
                }

                RecordTimeTaken(itemTemporarilyUnavailable, addNewReviewRecord, resetUnavailableCount, startTime);
            }


            catch (Exception ex)
            {
                lock (logLock)
                {
                    ErrorLogger.GetExceptionAndLogError(ex, "ValidationManager.Validate(validationItem)", "");
                }
            }
            finally
            {
                validationItem = null;
                lock (counterLock)
                {
                    numberToValidate--;
                }
            }
        }



        private static void RecordTimeTaken(bool itemTemporarilyUnavailable, bool addNewReviewRecord, bool resetUnavailableCount, DateTime startTime)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(startTime);
            if (timeSpan.Seconds >= 1)
            {
                string timeTaken = string.Format("{0:hh\\:mm\\:ss\\.f}", timeSpan);

                //if (justSaveValidationResults)
                //    AuditLogger.LogAuditEvent("Save good validation results took: :" + timeTaken);
                if (addNewReviewRecord)
                    AuditLogger.LogAuditEvent("Add new review record took:" + timeTaken);
                if (itemTemporarilyUnavailable)
                    AuditLogger.LogAuditEvent("Update Unavailable Count took:" + timeTaken);
                if (resetUnavailableCount)
                    AuditLogger.LogAuditEvent("Resetting Unavailable Count took:" + timeTaken);
            }
        }

        private class CollectionItem
        {
            public IMediaCollector Collector { get; set; }
            public IMediaValidator Validator { get; set; }
            public MediaAddress MediaAddress { get; set; }
            public MediaTypeValidationItem MediaType { get; set; }
            public MediaValidationConfig Config { get; set; }
        }

        private class ValidationItem
        {
            public IMediaValidator Validator { get; set; }
            public MediaAddress MediaAddress { get; set; }
            public CollectedData CollectedData { get; set; }
            public MediaTypeValidationItem MediaType { get; set; }
            public MediaValidationConfig Config { get; set; }
        }


    }
}
