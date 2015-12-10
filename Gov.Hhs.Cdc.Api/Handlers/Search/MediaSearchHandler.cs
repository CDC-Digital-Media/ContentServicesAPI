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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;


namespace Gov.Hhs.Cdc.Api
{
    public sealed class MediaSearchHandler : HandlerSearchBase, ISearchHandler
    {
        public MediaSearchHandler(ICallParser parser)
            : base(parser, null, null)
        {
        }

        public override SearchParameters BuildSearchParameters()
        {
            Sorting sorting = null;
            if (Parser.Version == 1)
            {
                sorting = Parser.IsPublicFacing ?
                GetSortColumns(Parser.SortColumns, ApiParam.datepublished.ToString().Direction(SortOrderType.Desc), ApiParam.mediaid.ToString().Direction(SortOrderType.Asc)) :
                GetSortColumns(Parser.SortColumns, ApiParam.datemodified.ToString().Direction(SortOrderType.Desc), ApiParam.mediaid.ToString().Direction(SortOrderType.Asc));

                if (Parser.SortColumns.Count == 0 && Parser.Criteria2 != null && string.IsNullOrEmpty(Parser.Criteria2.Sort))
                {
                    Parser.Criteria2.Sort = sorting.SortColumns.ToXmlSearchString();
                    Parser.SortColumns = sorting.SortColumns;
                }
            }
            else
            {
                if (!Parser.CanUseXmlSearch)
                {
                    sorting = Parser.IsPublicFacing ?
                          GetSortColumns(Parser.SortColumns, ApiParam.datepublished.ToString().Direction(SortOrderType.Desc), ApiParam.id.ToString().Direction(SortOrderType.Asc)) :
                          GetSortColumns(Parser.SortColumns, ApiParam.datemodified.ToString().Direction(SortOrderType.Desc), ApiParam.id.ToString().Direction(SortOrderType.Asc));
                }
                else
                {
                    sorting = Parser.IsPublicFacing ?
                        GetSortColumns(Parser.SortColumns, ApiParam.datepublished.ToString().Direction(SortOrderType.Desc), ApiParam.mediaid.ToString().Direction(SortOrderType.Asc)) :
                        GetSortColumns(Parser.SortColumns, ApiParam.datemodified.ToString().Direction(SortOrderType.Desc), ApiParam.mediaid.ToString().Direction(SortOrderType.Asc));

                    if (Parser.SortColumns.Count == 0 && Parser.Criteria2 != null && string.IsNullOrEmpty(Parser.Criteria2.Sort))
                    {
                        Parser.SortColumns = sorting.SortColumns;
                        Parser.Criteria2.Sort = sorting.SortColumns.ToXmlSearchString();
                    }
                }
            }

            SearchParam = new SearchParameters("Media", "Media", Parser.GetStringParm(ApiParam.action), Parser.Query.GetPaging(), Parser.SecondsToLive, sorting, GetFilterCriteria());
            return SearchParam;
        }        

        public override SearchParameters BuildSearchParametersForIdSearch()
        {
            var criteria = new List<Criterion>();

            if (Parser.IsPublicFacing)
            {
                criteria.Add(new Criterion("EffectiveStatus", new List<string>() { Param.MediaStatus.Published.ToString() }));
            }
            else
            {
                Criterion selectedStatusCriterion = Parser.GetListCriterion(ApiParam.status, "EffectiveStatus");
                //For Admin, Published = Pending and Published
                if (selectedStatusCriterion != null
                    && selectedStatusCriterion.Values.Contains("Published", StringComparer.OrdinalIgnoreCase)
                    && !selectedStatusCriterion.Values.Contains("Pending", StringComparer.OrdinalIgnoreCase))
                {
                    selectedStatusCriterion.Values.Add("Pending");
                }
                criteria.Add(selectedStatusCriterion);
            }

            Criterion[] results = criteria
                .Union(Parser.Criteria.List)        //For now, add the deprecated Criteria until we have moved everything over to the new style
                .Where(c => c != null).ToArray();

            SearchParam = new SearchParameters("Media", "Media", results);

            return SearchParam;
        }

        private Criterion[] GetFilterCriteria()
        {
            List<Criterion> criteria = new List<Criterion>()
            {
                //new Criterion("GetAttributes", true),
                Parser.GetListCriterion(ApiParam.owningorg, "OwningBusinessUnitId"),
                Parser.GetListCriterion(ApiParam.maintainingorg, "MaintainingBusinessUnitId"),
                Parser.GetListCriterion(ApiParam.source, "SourceCode"),
                Parser.GetCriterion(ApiParam.domain, "DomainName"),
                Parser.GetDateRangeCriterion(ApiParam.fromdatepublished, ApiParam.todatepublished, "PublishedDateTime"),
                Parser.GetDateRangeCriterion(ApiParam.fromdatelastreviewed, ApiParam.todatelastreviewed, "LastReviewedDate"),
                Parser.GetDateRangeCriterion(ApiParam.fromdatecreated, ApiParam.todatecreated, "CreatedDate"),
                Parser.GetDateRangeCriterion(ApiParam.fromdatemodified, ApiParam.todatemodified, "ModifiedDateTime")            
            };
            
            //name-value pair search
            if (Parser.ParamDictionary.Where(a => a.Key.ToLower().StartsWith(Param.NAME_VALUE_PAIR_PREFIX)).Any())
            {
                criteria.Add(new Criterion("VocabNameValuePair",
                    GetNameValuePairFromDictionary(Parser.ParamDictionary.Where(a => a.Key.StartsWith(Param.NAME_VALUE_PAIR_PREFIX))
                    .Select(a => a).ToList())));
            }

            criteria.Add(Parser.GetListCriterion(ApiParam.syndicationlistid, "SyndicationListGuid"));

            if (Parser.IsPublicFacing)
            {
                if (Parser.Version > 1)
                {
                    GetTextFilterCriteria(ref criteria);
                    GetDateFilterCriteria(ref criteria);

                    // geolocation params
                    GeoLocationSearch(ref criteria);
                }
                else
                {
                    LanguageFilter(ref criteria);
                }

                criteria.Add(new Criterion("EffectiveStatus", new List<string>() { Param.MediaStatus.Published.ToString() }));
            }
            else
            {
                Criterion selectedStatusCriterion = Parser.GetListCriterion(ApiParam.status, "EffectiveStatus");
                if (selectedStatusCriterion != null
                    && selectedStatusCriterion.Values.Contains("Published", StringComparer.OrdinalIgnoreCase)
                    && !selectedStatusCriterion.Values.Contains("Pending", StringComparer.OrdinalIgnoreCase))
                    selectedStatusCriterion.Values.Add("Pending");
                {
                    criteria.Add(selectedStatusCriterion);
                }

                // Language v1
                LanguageFilter(ref criteria);

                // geolocation params
                GeoLocationSearch(ref criteria);
            }

            Criterion[] results = criteria
                .Union(Parser.Criteria.List)        //For now, add the deprecated Criteria until we have moved everything over to the new style
                .Where(c => c != null).ToArray();

            return results;
        }

        private void LanguageFilter(ref List<Criterion> criteria)
        {
            //language
            var crit = Parser.GetListCriterion(ApiParam.Language, ApiParam.Language.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }
        }

        private static List<string> GetNameValuePairFromDictionary(List<KeyValuePair<string, string>> dict)
        {
            var nameValueList = new List<string>();

            foreach (KeyValuePair<string, string> pair in dict)
            {
                nameValueList.Add(pair.Key.Replace(Param.NAME_VALUE_PAIR_PREFIX, string.Empty) + "|" + pair.Value);
            }

            return nameValueList;
        }

        private void GeoLocationSearch(ref List<Criterion> criteria)
        {
            var crit = new List<Criterion>()
            {
                Parser.GetListCriterion(ApiParam.geoname, "geoname"),
                Parser.GetListCriterion(ApiParam.geonameid, "geonameid"),
                Parser.GetListCriterion(ApiParam.geoparentid, "geoparentid"),
                Parser.GetListCriterion(ApiParam.countrycode, "countrycode"),
                Parser.GetListCriterion(ApiParam.latitude, "latitude"),
                Parser.GetListCriterion(ApiParam.longitude, "longitude"),
                Parser.GetListCriterion(ApiParam.timezone, "timezone"),                
                Parser.GetListCriterion(ApiParam.admin1code, "admin1code")
            };

            if (crit.Where(a => a != null).Any())
            {
                criteria.AddRange(crit);
            }
        }

        public SerialResponse Search()
        {
            Response.dataset = null;

            IEnumerable<MediaObject> mediaObjects = CsMediaSearchProvider.Search(Parser.Criteria2);
            var transformation = TransformationFactory.GetMediaTransformation(Parser.Version, Parser.IsPublicFacing);
            Response.results = transformation.CreateSerialResponse(mediaObjects).results;
            Response.mediaObjects = mediaObjects;

            var first = mediaObjects.Any() ? mediaObjects.First() : new MediaObject();
            SetPaging(first);

            return Response;
        }

        private void SetPaging(MediaObject first)
        {
            Response.meta.resultSet.id = Guid.NewGuid().ToString();
            Response.meta.pagination = new SerialPagination(Parser, new Sorting(Parser.SortColumns), first.TotalRows, first.RowsPerPage, Parser.Query.PageSize, first.PageOffset,
                first.PageNumber, first.TotalPages);
        }
        
        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {
            ServiceUtility.GetDataSet(Parser, searchParam, Response);

            if (Response.meta.message.Count > 0)
                return Response;

            IMediaTransformation transformation = TransformationFactory.GetMediaTransformation(Parser.Version, Parser.IsPublicFacing);
            IEnumerable<MediaObject> mediaObjects = Response.dataset.Records.Cast<MediaObject>();
            Response.results = transformation.CreateSerialResponse(mediaObjects).results;
            Response.mediaObjects = mediaObjects;
            return Response;
        }

        //TODO:  Make overload of above
        public SerialResponse BuildResponse(SearchParameters searchParam, IList<int> allowableMediaIds)
        {
            ServiceUtility.GetDataSet(Parser, searchParam, Response, allowableMediaIds);

            if (Response.meta.message.Count > 0)
            {
                return Response;
            }

            IMediaTransformation transformation = TransformationFactory.GetMediaTransformation(Parser.Version, Parser.IsPublicFacing);
            IEnumerable<MediaObject> mediaObjects = Response.dataset.Records.Cast<MediaObject>();

            Response.results = transformation.CreateSerialResponse(mediaObjects).results;
            Response.mediaObjects = mediaObjects;
            return Response;
        }

        //public SerialResponse BuildResponseForIdSearch(SearchParameters searchParam)
        //{
        //    ServiceUtility.GetDataSet(Parser, searchParam, Response);

        //    if (Response.meta.message.Count > 0)
        //    {
        //        return Response;
        //    }

        //    var cmi = Response.dataset.Records.Cast<MediaObject>().FirstOrDefault();
        //    if (cmi != null)
        //    {
        //        Response.results = cmi;
        //        if (cmi.MediaTypeParms.CreateEmbedHtml || cmi.MediaTypeParms.IsVideo || cmi.MediaTypeParms.IsWidget)
        //        {
        //            Response.results = new Uri(cmi.SourceUrl);
        //        }
        //        else if (cmi.MediaTypeParms.IsStaticImageMedia || cmi.MediaTypeParms.IsEcard || cmi.MediaTypeParms.IsMicrosite)
        //        {
        //            Response.results = new Uri(cmi.TargetUrl);
        //        }
        //    }
        //    return Response;
        //}

        public static Sorting GetSortColumns(List<SortColumn> sortColumns, params SortColumn[] defaultSortColumns)
        {
            if (sortColumns != null && sortColumns.Count > 0)
            {
                return new Sorting(ApiColumnMap.MapSortColumns(sortColumns));
            }
            else
            {
                return new Sorting(ApiColumnMap.MapSortColumns(defaultSortColumns.ToList()));
            }
        }

        private void GetTextFilterCriteria(ref List<Criterion> criteria)
        {
            Criterion crit = null;

            //name
            crit = Parser.GetCriterion(ApiParam.Name, ApiParam.Name.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //nameContains
            crit = Parser.GetCriterion(ApiParam.NameContains, ApiParam.NameContains.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //description
            crit = Parser.GetCriterion(ApiParam.Description, ApiParam.Description.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //descriptionContains
            crit = Parser.GetCriterion(ApiParam.DescriptionContains, ApiParam.DescriptionContains.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //sourceUrl
            crit = Parser.GetCriterion(ApiParam.SourceUrl, ApiParam.SourceUrl.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //sourceUrlContains
            if (Parser.ParamDictionary.ContainsKey(ApiParam.SourceUrlContains.ToString()))
            {
                Parser.UpdateOrAddParmInDictionaryAsString(ApiParam.SourceUrlContains.ToString(),
                    Parser.ParamDictionary[ApiParam.SourceUrlContains.ToString()].SafeUrlContainsSearch());
                crit = Parser.GetCriterion(ApiParam.SourceUrlContains, ApiParam.SourceUrlContains.ToString());
                if (crit != null)
                {
                    criteria.Add(crit);
                }
            }

            //sourceName
            crit = Parser.GetCriterion(ApiParam.SourceName, ApiParam.SourceName.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //sourceNameContains
            crit = Parser.GetCriterion(ApiParam.SourceNameContains, ApiParam.SourceNameContains.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //sourceAcronym
            crit = Parser.GetCriterion(ApiParam.SourceAcronym, ApiParam.SourceAcronym.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //sourceAcronymContains
            crit = Parser.GetCriterion(ApiParam.SourceAcronymContains, ApiParam.SourceAcronymContains.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //languageName
            crit = Parser.GetListCriterion(ApiParam.LanguageName, ApiParam.LanguageName.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //languageIsoCode
            crit = Parser.GetListCriterion(ApiParam.LanguageIsoCode, ApiParam.LanguageIsoCode.ToString());
            if (crit != null)
            {
                criteria.Add(crit);
            }

            crit = Parser.GetIntListCriterion(ApiParam.TagIds);
            if (crit != null)
            {
                criteria.Add(crit);
            }
        }

        private void GetDateFilterCriteria(ref List<Criterion> criteria)
        {
            ValidationMessages messages = new ValidationMessages();
            Criterion crit = null;

            crit = Parser.GetDateRangeCriterion(messages, ApiParam.DateContentAuthored, ApiParam.ContentAuthoredInRange,
                                                          ApiParam.ContentAuthoredSinceDate, ApiParam.ContentAuthoredBeforeDate, "DateContentAuthored");
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //-----------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------

            crit = Parser.GetDateRangeCriterion(messages, ApiParam.DateContentUpdated, ApiParam.ContentUpdatedInRange,
                                                          ApiParam.ContentUpdatedSinceDate, ApiParam.ContentUpdatedBeforeDate, "DateContentUpdated");
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //-----------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------

            crit = Parser.GetDateRangeCriterion(messages, ApiParam.DateContentPublished, ApiParam.ContentPublishedInRange,
                                                          ApiParam.ContentPublishedSinceDate, ApiParam.ContentPublishedBeforeDate, "DateContentPublished");
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //-----------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------

            crit = Parser.GetDateRangeCriterion(messages, ApiParam.DateContentReviewed, ApiParam.ContentReviewedInRange,
                                                          ApiParam.ContentReviewedSinceDate, ApiParam.ContentReviewedBeforeDate, "DateContentReviewed");
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //-----------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------

            crit = Parser.GetDateRangeCriterion(messages, ApiParam.DateSyndicationCaptured, ApiParam.SyndicationCapturedInRange,
                                                          ApiParam.SyndicationCapturedSinceDate, ApiParam.SyndicationCapturedBeforeDate, "DateSyndicationCaptured");
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //-----------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------

            crit = Parser.GetDateRangeCriterion(messages, ApiParam.DateSyndicationUpdated, ApiParam.SyndicationUpdatedInRange,
                                                          ApiParam.SyndicationUpdatedSinceDate, ApiParam.SyndicationUpdatedBeforeDate, "DateSyndicationUpdated");
            if (crit != null)
            {
                criteria.Add(crit);
            }

            //-----------------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------------

            crit = Parser.GetDateRangeCriterion(messages, ApiParam.DateSyndicationVisible, ApiParam.SyndicationVisibleInRange,
                                                          ApiParam.SyndicationVisibleSinceDate, ApiParam.SyndicationVisibleBeforeDate, "DateSyndicationVisible");
            if (crit != null)
            {
                criteria.Add(crit);
            }
        }

    }


}
