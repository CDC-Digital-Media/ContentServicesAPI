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
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;
//using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider.SchedulerInterfaces;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.MediaValidatonProvider;

namespace Gov.Hhs.Cdc.MediaValidation
{

    public class MediaValidationTask : ICDCDataRetrievalTask
    {

        static List<string> errors = new List<string>();
        //Media Types
        //MediaTypeCode     MediaAddressSource              
        //HtmlContent       Url, PersistentUrl, Mobile

        private Guid id = new Guid("D693D99C-6E84-40CC-A838-9AD47DB2F03C");
        public Guid Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return "Media Validation"; }
        }

        public string Description
        {
            get { return "Media Validation"; }
        }
        
        

        public CDCDataRetrivalTaskResult Execute(string name, System.Xml.Linq.XElement xmlConfig)
        {
            try
            {
                MediaValidationConfig config = new MediaValidationConfig(xmlConfig);
                Validate(config);
                Logger.LogInfo("Media ValidationChange Completed Successfully");
                if (errors.Count == 0)
                    return new CDCDataRetrivalTaskResult()
                    {
                        Sucess = true,
                        ResultText = "Completed Successfully"
                    };
                else
                {
                    return new CDCDataRetrivalTaskResult()
                    {
                        Sucess = false,
                        ResultText = string.Join(", ", errors.ToArray())
                    };
                }
                

            }
            catch (Exception ex)
            {
                ErrorLogger.GetExceptionAndLogError(ex, "Execute", "");
                return new CDCDataRetrivalTaskResult()
                {
                    Sucess = false,
                    ResultText = "Exception: " + ex.ToString()
                };


            }

        }


        public static void Validate(MediaValidationConfig config)
        {
            ContentServicesDependencyBuilder.BuildAssemblies();
            AuditLogger.LogApplicationStartEvent(string.Format("Media validation has started using {0} threads", config.NumberOfThreads));
            try
            {
                ResultsAndReviewMgr mgr = new ResultsAndReviewMgr();
                List<MediaTypeValidationItem> mediaTypes = ResultsAndReviewMgr.GetValidationMediaTypesToValidate();
                foreach (MediaTypeValidationItem mediaType in mediaTypes)
                {
                    //List<MediaTypeFilterItem> mediaTypeFilters = MediaValidationCacheController.GetMediaTypeFilters(media, mediaTypeValidationItem.MediaTypeCode);
                    //MediaValidationConfiguration mediaTypeConfig = MediaValidationConfigurationCtl.Get
                        //new EntitiesConfigurationItems(mediaType.SourceConnectionString, new PersistenceCacheController(), new CacheController(), false));
                    IMediaAddressSource mediaAddressSource = MediaAddressSourceFactory.Create(mediaType);
                    
                    //TODO: Wire up the MediaObjectContextFactory here
                    //MediaObjectContextFactory objectContextFactory = new MediaObjectContextFactory();


                    List<MediaAddress> addresses = mediaAddressSource.GetAddresses(mediaType.SourceConnectionStringName);

                    IMediaCollector collector = MediaCollectorFactory.Create(mediaType);
                    IMediaValidator validator = ValidatorFactory.Create(mediaType);
                    ValidationManager validationManager = new ValidationManager();

                    //Commented code is used to time the validation
                    //List<TimeSpan> times = new List<TimeSpan>();
                    //for (int i = 0; i < 5; ++i)
                    //{
                        //DateTime startTime = DateTime.Now;
                    validationManager.Validate(mediaType, collector, validator, addresses, config);
                        //TimeSpan timeToProcess = DateTime.Now.Subtract(startTime);
                        //times.Add(timeToProcess);
                    //}
                    //string x = "abc";

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.GetExceptionAndLogError(ex, "Validate", "");
                errors.Add(ex.Message);
            }

            AuditLogger.LogAuditEvent("Media Validation Has Completed" + (errors.Count == 0 ? "" : " with " + errors.Count.ToString() + " errors"));
        }



        //public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) 
        //{ 
        //    return source.MinBy(selector, Comparer<TKey>.Default); 
        //}  
        
        //public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer) 
        //{ 
        //    source.ThrowIfNull("source"); 
        //    selector.ThrowIfNull("selector"); 
        //    comparer.ThrowIfNull("comparer"); 
        //    using (IEnumerator<TSource> sourceIterator = source.GetEnumerator()) 
        //    { 
        //        if (!sourceIterator.MoveNext()) 
        //        { 
        //            throw new InvalidOperationException("Sequence was empty"); 
        //        } 
        //        TSource min = sourceIterator.Current; 
        //        TKey minKey = selector(min); 
        //        while (sourceIterator.MoveNext()) { TSource candidate = sourceIterator.Current; TKey candidateProjected = selector(candidate); if (comparer.Compare(candidateProjected, minKey) < 0) { min = candidate; minKey = candidateProjected; } } return min; } } 

        //public static List<MediaAddressSource> GetMediaAddressSources()
        //{
        //    List<MediaAddressSource> addressSources = new List<MediaAddressSource>();
        //    addressSources.Add(new HtmlContentAddressSource());
        //    return addressSources;
        //}

        public static List<IMediaValidator> GetMediaValidators()
        {
            List<IMediaValidator> validators = new List<IMediaValidator>();
            validators.Add(new HtmlMediaValidator());
            return validators;
        }

    }

}
