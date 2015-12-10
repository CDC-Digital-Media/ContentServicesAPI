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
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataSource.Media;
using Gov.Hhs.Cdc.Search.Provider;
using Gov.Hhs.Cdc.Search.Bo;
using Gov.Hhs.Cdc.Search.Controller;

namespace DataSourceTester
{
    class Program
    {
        //private static IContainer Container { get; set; }

        static SearchControllerFactory SearchControllerFactory;

        protected static ICacheProvider CacheProvider { get; set; }
        static void Main(string[] args)
        {
            SearchControllerFactory = new SearchControllerFactory();


            //SearchController.Wire(new CsSortProvider(), new HttpRuntimeCacheProvider());

            //var builder = new ContainerBuilder();
            //builder.RegisterType<CsSortProvider>().As<ISortProvider>();
            //Container = builder.Build();

            //using (ILifetimeScope scope = Container.BeginLifetimeScope())
            //{
            //    ISortProvider x = scope.Resolve<ISortProvider>();
            //    //x.Sort(x, y);
            //}

            //DataSetResult result = FilterMgmt.GetData("Media", "List",
            //    new List<FilterCriterion>()
            //    {
            //        new FilterCriterion("MediaType", new List<string>{"ECards", "Web"}),
            //        new FilterCriterion("Language", "English")
            //    });
            //string x = "finished";
            MediaSearchDefinition searchDefinition = new MediaSearchDefinition()
            {
                Criteria = new MediaSearchCriteria()
                {
                    Language = "English",
                    AvailableDate = new DateTime(2013, 2, 1),
                    AudienceId = "321"
                },
                MaxWaitMilliseconds = 30000,
                //MillisecondsForCacheToLive = 300000,
                MillisecondsForCacheToLive = 0,
                PageSize = 10
            };
            
            GetFilteredCombinedMedia();

            MediaSearchResponse response = Search(searchDefinition);

        }
        
        private static DataSetResult GetFilteredCombinedMedia()
        {

            Criteria filterCriteria = new Criteria();
            //filterCriteria.Add("MediaType", "eCard");
            //filterCriteria.Add("Language", "English");
            filterCriteria.Add("GetAttributes", true);
            filterCriteria.Add("TopicId", 4);
            //filterCriteria.Add("TopicId", new List<int> { 19, 26, 18 });
            //filterCriteria.Add("MediaId", 1);
            //filterCriteria.Add("MediaId", new List<int> {1, 3, 6, 7, 12, 13 });
            //filterCriteria.Add("FullSearch", "Healthy");


            SearchParameters parms = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "CombinedMedia",
                Paging = new Paging(20, 1),
                Sorting = new Sorting(new SortColumn("Id", SortOrderType.Asc)),
                SecondsToLive = 0,
                FilterCriteria = filterCriteria

                //}

            };

            ISearchController controller = SearchControllerFactory.GetSearchController(parms);
            DataSetResult results = controller.Get();
            IEnumerable<CombinedMediaItem> testItem = (IEnumerable<CombinedMediaItem>)results.Records;

            return results;
        }

        public static MediaSearchResponse Search(MediaSearchDefinition searchDefinition)
        {
            GetLanguages();
            GetTopics();
            GetMediaTypes();
            


            ISearchController controller;
            DataSetResult results = GetUnfilteredCombinedMedia();
            results = GetCombinedMediaPage(results.Id, 10);
            Guid resultSetId = results.Id;

            MediaSearchResponse response = new MediaSearchResponse();
            response.CurrentPage = results.PageNumber;
            response.PageCount = results.TotalPages;

            List<MediaItemDescriptor> theResults = (
                from sr in results.Records.Cast<CombinedMediaItem>()
                select new MediaItemDescriptor
                                                    {
                                                        //Finish this out
                                                        Language = sr.LanguageCode
                                                    }).ToList();
            response.Items = theResults;

            response.MediaCount = results.TotalRecordCount;

            response.Duration = 0;
            response.IsComplete = true;
            response.ResultSetId = "";

            
            return response;
        }

        private static DataSetResult GetCombinedMediaPage(Guid resultId, int pageNumber)
        {
            DataSetResult result = SearchControllerFactory.GetSearchController(resultId).NavigatePages(pageNumber);
            return result;
        }


        private static DataSetResult GetMediaTypes()
        {
            SearchParameters parms = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "MediaType",
                Sorting = new Sorting(new SortColumn("DisplayOrdinal", SortOrderType.Asc))
            };

            DataSetResult mediaTypes = SearchControllerFactory.GetSearchController(parms).Get();
            return mediaTypes;
        }

        private static DataSetResult GetLanguages()
        {

            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("OnlyInUseItems", false);

            SearchParameters parms = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "Language",
                Sorting = new Sorting(new SortColumn("DisplayOrdinal", SortOrderType.Asc)),
                FilterCriteria = filterCriteria
            };

            DataSetResult languages = SearchControllerFactory.GetSearchController(parms).Get();
            return languages;
        }

        private static DataSetResult GetUnfilteredCombinedMedia()
        {
            SearchParameters parms = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "CombinedMedia",
            };

            DataSetResult results = SearchControllerFactory.GetSearchController(parms).Get();
            return results;
        }


        //private static Criteria ConvertParmsToDataSource(MediaSearchCriteria criteria)
        //{
        //    Criteria filterCriteria = new Criteria();
        //    filterCriteria.Add("MediaType", criteria.MediaTypes);
        //    filterCriteria.AddInt("Campaign", criteria.CampaignId);
        //    filterCriteria.AddInt("Topic", criteria.TopicId);
        //    filterCriteria.AddInt("Audience", criteria.AudienceId);
        //    filterCriteria.Add("WebAddress", criteria.WebAddress);
        //    filterCriteria.Add("Language", criteria.Language);
        //    filterCriteria.Add("AvailableDate", criteria.AvailableDate);
        //    filterCriteria.AddInt("AuthorizationProfile", criteria.AuthorizationProfileId);
        //    filterCriteria.Add("ActiveDate", criteria.ActiveDate);
        //    filterCriteria.Add("Visible", criteria.Visibility);
        //    filterCriteria.Add("Active", criteria.IsActive);
        //    return filterCriteria;
        //}



        private static DataSetResult GetTopics()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("TopValueSet", "Categories");
            filterCriteria.Add("WithChildren", true);
            filterCriteria.Add("IsActive", true);
            SearchParameters searchParameters3 = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "Vocabulary",
                FilterCriteria = filterCriteria
            };

            DataSetResult mediaTopics = SearchControllerFactory.GetSearchController(searchParameters3).Get();
            return mediaTopics;

        }
    }
}
