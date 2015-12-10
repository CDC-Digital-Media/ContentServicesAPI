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
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    internal class MediaWithItems
    {
        public MediaObject TheParentPart { get; set; }
        public IEnumerable<MediaObject> TheListOfItems { get; set; }
    }

    public class MediaSearchMgr : MediaBaseSearchMgr<MediaObject>
    {
        public override DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms)
        {
            ResultSetParms resultSetParameters = resultSetParms ?? new ResultSetParms();
            AuditLogger.LogAuditEvent("Old search");
            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                IQueryable<MediaObject> mediaItems = GetData(media, searchPlan.Plan.Criteria, resultSetParameters);

                DataSetResult results = CreateDataSetResults(mediaItems, searchPlan, resultSetParameters.ResultSetId, logTime: true);

                MediaCtl.AddSubLists(media, results.Records.Cast<MediaObject>());

                FilterCriterionBoolean mediaWithChildren = (FilterCriterionBoolean)searchPlan.Plan.Criteria.UseCriterion("ShowChild");
                bool showChild = mediaWithChildren != null && mediaWithChildren.IsFiltered && mediaWithChildren.Value;

                if (showChild)
                {
                    if (((List<MediaObject>)results.Records).Count == 1)
                    {
                        AddChildItems(media, results);
                    }
                }

                return results;
            }
        }

        public override DataSetResult GetData(SearchPlan searchPlan, ResultSetParms resultSetParms, IList<int> allowableMediaIds)
        {
            ResultSetParms resultSetParameters = resultSetParms ?? new ResultSetParms();

            using (MediaObjectContext media = (MediaObjectContext)ObjectContextFactory.Create())
            {
                IQueryable<MediaObject> mediaItems = GetData(media, searchPlan, resultSetParameters, allowableMediaIds);

                DataSetResult results = CreateDataSetResults(mediaItems, searchPlan, resultSetParameters.ResultSetId, logTime: false);
                var records = results.Records.AsQueryable();//.Where(i => allowableMediaIds.Contains(i.Id));
                var items = results.Records.Cast<MediaObject>();
                if (allowableMediaIds != null)
                {
                    items = items.Where(i => allowableMediaIds.Contains(i.Id));
                }
                results.Records = items;

                MediaCtl.AddSubLists(media, results.Records.Cast<MediaObject>());

                var mediaWithChildren = (FilterCriterionBoolean)searchPlan.Plan.Criteria.UseCriterion("ShowChild");
                bool showChild = mediaWithChildren != null && mediaWithChildren.IsFiltered && mediaWithChildren.Value;

                if (showChild)
                {
                    if (((List<MediaObject>)results.Records).Count == 1)
                    {
                        AddChildItems(media, results);
                    }
                }

                return results;
            }
        }


        private static void AddChildItems(MediaObjectContext media, DataSetResult topLevel)
        {
            List<MediaObject> topLevelRecords = (List<MediaObject>)topLevel.Records;

            IEnumerable<MediaWithItems> mediaWithItems = from t in topLevelRecords
                                                         where MediaRelationshipCtl.Get(media).Select(a => a.MediaId).ToList().Contains(t.Id)
                                                         let c = MediaCtl.AddSubLists(media, MediaCtl.Get(media, t.Id))
                                                         select new MediaWithItems { TheParentPart = t, TheListOfItems = c };
        }



        private static Guid ConvertToGuid(ValidationMessages messages, string key, string id)
        {
            Guid newGuid;
            if (Guid.TryParse(id, out newGuid))
            {
                return newGuid;
            }
            else
            {
                messages.AddError(key, "Invalid Guid: " + id);
                return new Guid();
            }
        }

        public static IQueryable<MediaObject> GetData(MediaObjectContext media, FilterCriteria criteria, ResultSetParms resultSetParms)
        {
            ValidationMessages messages = new ValidationMessages();

            //set criterion.HasBeenUsed to true to bypass dynamic search columns
            criteria.UseCriterion("ShowChild");

            IQueryable<MediaObject> mediaItems = MediaCtl.Get(media);
            mediaItems = GeoLocationSearch(criteria, mediaItems);

            //OnlyDisplayableMediaTypes
            if (criteria.IsFilteredBy("OnlyDisplayableMediaTypes"))
            {
                var onlyDisplayableMediaTypes = (FilterCriterionBoolean)criteria.UseCriterion("OnlyDisplayableMediaTypes");
                bool onlyDisplayableMediaType = onlyDisplayableMediaTypes != null && onlyDisplayableMediaTypes.IsFiltered && onlyDisplayableMediaTypes.Value;
                if (onlyDisplayableMediaType)
                {
                    mediaItems = mediaItems.Where(m => m.DisplayOnSearch);
                }
            }

            //THe following is test code to isolate for a specific value to research the number of items that are coming back
            //mediaItems = mediaItems.Where(m => m.IsActive && m.EffectiveStatusCode == "Published");
            //mediaItems = from m in mediaItems 
            //        join mv in media.MediaDbEntities.MediaValues on m.Id equals mv.MediaId
            //        where mv.ValueId == 11
            //                       select m;            

            var syndicationListGuidFilter = (FilterCriterionMultiSelect)criteria.UseCriterion("SyndicationListGuid");
            if (syndicationListGuidFilter.IsFiltered)
            {
                List<Guid> listGuids = syndicationListGuidFilter.GetStringKeys()
                    .Select(i => ConvertToGuid(messages, "SyndicationListGuid", i)).ToList();
                IQueryable<int> listIds = SyndicationListMediaItemCtl.GetMedias(media, listGuids);
                mediaItems = mediaItems.Where(m => listIds.Contains(m.Id));
            }

            //FilterCriterionText domainName = (FilterCriterionText)criteria.UseCriterion("DomainName");
            //if (domainName.IsFiltered)
            //{
            //    string domainNameKey = domainName.GetStringKey();
            //    List<DomainItem> domains = DomainItemCtl.Get(media).Where(d => d.DomainName.StartsWith(domainNameKey) && d.IsActive).ToList();
            //    mediaItems = mediaItems.Where(m => domains.Where(d => d.SourceCode == m.SourceCode).Any());
            //}

            FilterCriterionMultiSelect owning = (FilterCriterionMultiSelect)criteria.UseCriterion("OwningBusinessUnitId");
            if (owning.IsFiltered)
            {
                List<int> owningIds = owning.GetIntKeys();
                mediaItems = mediaItems.Where(m => m.OwningBusinessUnitId != null && owningIds.Contains((int)m.OwningBusinessUnitId));
            }

            FilterCriterionMultiSelect maintaining = (FilterCriterionMultiSelect)criteria.UseCriterion("MaintainingBusinessUnitId");
            if (maintaining.IsFiltered)
            {
                List<int> maintainingIds = maintaining.GetIntKeys();
                mediaItems = mediaItems.Where(m => m.MaintainingBusinessUnitId != null && maintainingIds.Contains((int)m.MaintainingBusinessUnitId));
            }

            if (criteria.IsFilteredBy("MediaType"))
            {
                List<string> keys = criteria.UseCriterion("MediaType").GetStringKeys().Select(a => a.Trim().ToLower()).ToList();
                mediaItems = mediaItems.Where(i => i.MediaTypeCode != null
                    && keys.Contains(i.MediaTypeCode.Trim().ToLower()));
            }

            if (criteria.IsFilteredBy("MediaId"))
            {
                List<int> mediaIds = criteria.UseCriterion("MediaId").GetIntKeys();
                mediaItems = mediaItems.Where(i => mediaIds.Contains(i.Id));
            }

            if (criteria.IsFilteredBy("TagIds"))
            {
                List<int> tagIds = criteria.UseCriterion("TagIds").GetIntKeys();
                mediaItems = mediaItems.Where(i => i.AttributeValues.Where(a => tagIds.Contains(a.ValueKey.Id)).Any());
            }

            if (criteria.IsFilteredBy("VocabNameValuePair"))
            {
                List<string> keys = criteria.UseCriterion("VocabNameValuePair").GetStringKeys();
                foreach (string key in keys)
                {
                    string[] splitKey = key.Split('|');
                    string attributeKey = splitKey[0];
                    string attributeValue = splitKey[1];

                    if (attributeKey.ToLower().EndsWith(Param.NAME_VALUE_PAIR_CONTAINS))
                    {
                        attributeKey = attributeKey.Replace(Param.NAME_VALUE_PAIR_CONTAINS, string.Empty);
                        mediaItems = mediaItems.Where(i =>
                        i.AttributeValues.Where(a =>
                            a.AttributeName.ToLower() == attributeKey.ToLower() && a.ValueName.ToLower().Contains(attributeValue.ToLower())).Any());
                    }
                    else if (attributeKey.ToLower().EndsWith(Param.NAME_VALUE_PAIR_STARTSWITH))
                    {
                        attributeKey = attributeKey.Replace(Param.NAME_VALUE_PAIR_STARTSWITH, string.Empty);
                        mediaItems = mediaItems.Where(i =>
                        i.AttributeValues.Where(a =>
                            a.AttributeName.ToLower() == attributeKey.ToLower() && a.ValueName.ToLower().StartsWith(attributeValue.ToLower())).Any());
                    }
                    else if (attributeKey.ToLower().EndsWith(Param.NAME_VALUE_PAIR_ENDSWITH))
                    {
                        attributeKey = attributeKey.Replace(Param.NAME_VALUE_PAIR_ENDSWITH, string.Empty);
                        mediaItems = mediaItems.Where(i =>
                        i.AttributeValues.Where(a =>
                            a.AttributeName.ToLower() == attributeKey.ToLower() && a.ValueName.ToLower().EndsWith(attributeValue.ToLower())).Any());
                    }
                    else
                        mediaItems = mediaItems.Where(i =>
                            i.AttributeValues.Where(a =>
                                a.AttributeName.ToLower() == attributeKey.ToLower() && a.ValueName.ToLower() == attributeValue.ToLower()).Any());
                }
            }


            mediaItems = new CombinedMediaTextAndTopicFilter(media, resultSetParms, criteria).FilterByTextAndValues(mediaItems);

            return mediaItems;
        }

        public static IQueryable<MediaObject> GetData(MediaObjectContext media, SearchPlan plan, ResultSetParms resultSetParms, IList<int> allowableMediaIds)
        {
            ValidationMessages messages = new ValidationMessages();

            FilterCriteria criteria = plan.Plan.Criteria;

            criteria.UseCriterion("ShowChild");

            IQueryable<MediaObject> mediaItems = null;
            if (!string.IsNullOrEmpty(plan.Plan.ActionCode) && plan.Plan.ActionCode.ToLower() == "topsyndicated")
            {
                mediaItems = MediaCtl.GetTopMedia(media);
                mediaItems = mediaItems.Where(mi => mi.Popularity > 0);
            }
            else
            {
                mediaItems = MediaCtl.Get(media);
            }          

            //mediaItems = mediaItems.Where(i => allowableMediaIds.Contains(i.Id));

            //mediaItems = from m1 in mediaItems
            //             join m2 in allowableMediaIds on m1.MediaId equals m2
            //             select m1;

            mediaItems = GeoLocationSearch(criteria, mediaItems);

            if (criteria.IsFilteredBy("OnlyDisplayableMediaTypes"))
            {
                FilterCriterionBoolean onlyDisplayableMediaTypes = (FilterCriterionBoolean)criteria.UseCriterion("OnlyDisplayableMediaTypes");
                bool onlyDisplayableMediaType = onlyDisplayableMediaTypes != null && onlyDisplayableMediaTypes.IsFiltered && onlyDisplayableMediaTypes.Value;
                if (onlyDisplayableMediaType)
                {
                    mediaItems = mediaItems.Where(m => m.DisplayOnSearch);
                }
            }

            FilterCriterionMultiSelect syndicationListGuidFilter = (FilterCriterionMultiSelect)criteria.UseCriterion("SyndicationListGuid");
            if (syndicationListGuidFilter.IsFiltered)
            {
                List<Guid> listGuids = syndicationListGuidFilter.GetStringKeys()
                    .Select(i => ConvertToGuid(messages, "SyndicationListGuid", i)).ToList();
                IQueryable<int> listIds = SyndicationListMediaItemCtl.GetMedias(media, listGuids);
                mediaItems = mediaItems.Where(m => listIds.Contains(m.Id));
            }

            FilterCriterionMultiSelect owning = (FilterCriterionMultiSelect)criteria.UseCriterion("OwningBusinessUnitId");
            if (owning.IsFiltered)
            {
                List<int> owningIds = owning.GetIntKeys();
                mediaItems = mediaItems.Where(m => m.OwningBusinessUnitId != null && owningIds.Contains((int)m.OwningBusinessUnitId));
            }

            FilterCriterionMultiSelect maintaining = (FilterCriterionMultiSelect)criteria.UseCriterion("MaintainingBusinessUnitId");
            if (maintaining.IsFiltered)
            {
                List<int> maintainingIds = maintaining.GetIntKeys();
                mediaItems = mediaItems.Where(m => m.MaintainingBusinessUnitId != null && maintainingIds.Contains((int)m.MaintainingBusinessUnitId));
            }

            if (criteria.IsFilteredBy("MediaType"))
            {
                List<string> keys = criteria.UseCriterion("MediaType").GetStringKeys().Select(a => a.Trim().ToLower()).ToList();
                mediaItems = mediaItems.Where(i => i.MediaTypeCode != null
                    && keys.Contains(i.MediaTypeCode.Trim().ToLower()));
            }

            if (criteria.IsFilteredBy("MediaId"))
            {
                List<int> mediaIds = criteria.UseCriterion("MediaId").GetIntKeys();
                mediaItems = mediaItems.Where(i => mediaIds.Contains(i.Id));
            }

            if (criteria.IsFilteredBy("TagIds"))
            {
                List<int> tagIds = criteria.UseCriterion("TagIds").GetIntKeys();
                mediaItems = mediaItems.Where(i => i.AttributeValues.Where(a => tagIds.Contains(a.ValueKey.Id)).Any());
            }

            if (criteria.IsFilteredBy("VocabNameValuePair"))
            {
                List<string> keys = criteria.UseCriterion("VocabNameValuePair").GetStringKeys();
                foreach (string key in keys)
                {
                    string[] splitKey = key.Split('|');
                    string attributeKey = splitKey[0];
                    string attributeValue = splitKey[1];

                    if (attributeKey.ToLower().EndsWith(Param.NAME_VALUE_PAIR_CONTAINS))
                    {
                        attributeKey = attributeKey.Replace(Param.NAME_VALUE_PAIR_CONTAINS, string.Empty);
                        mediaItems = mediaItems.Where(i =>
                        i.AttributeValues.Where(a =>
                            a.AttributeName.ToLower() == attributeKey.ToLower() && a.ValueName.ToLower().Contains(attributeValue.ToLower())).Any());
                    }
                    else if (attributeKey.ToLower().EndsWith(Param.NAME_VALUE_PAIR_STARTSWITH))
                    {
                        attributeKey = attributeKey.Replace(Param.NAME_VALUE_PAIR_STARTSWITH, string.Empty);
                        mediaItems = mediaItems.Where(i =>
                        i.AttributeValues.Where(a =>
                            a.AttributeName.ToLower() == attributeKey.ToLower() && a.ValueName.ToLower().StartsWith(attributeValue.ToLower())).Any());
                    }
                    else if (attributeKey.ToLower().EndsWith(Param.NAME_VALUE_PAIR_ENDSWITH))
                    {
                        attributeKey = attributeKey.Replace(Param.NAME_VALUE_PAIR_ENDSWITH, string.Empty);
                        mediaItems = mediaItems.Where(i =>
                        i.AttributeValues.Where(a =>
                            a.AttributeName.ToLower() == attributeKey.ToLower() && a.ValueName.ToLower().EndsWith(attributeValue.ToLower())).Any());
                    }
                    else
                    {
                        mediaItems = mediaItems.Where(i =>
                             i.AttributeValues.Where(a =>
                                 a.AttributeName.ToLower() == attributeKey.ToLower() && a.ValueName.ToLower() == attributeValue.ToLower()).Any());
                    }
                }
            }

            mediaItems = new CombinedMediaTextAndTopicFilter(media, resultSetParms, criteria).FilterByTextAndValues(mediaItems);
            //mediaItems = mediaItems.Where(i => allowableMediaIds.Contains(i.Id));            

            return mediaItems;
        }

        private static IQueryable<MediaObject> GeoLocationSearch(FilterCriteria criteria, IQueryable<MediaObject> mediaItems)
        {
            if (criteria.IsFilteredBy("geoname"))
            {
                List<string> keys = criteria.UseCriterion("geoname").GetStringKeys().Select(a => a.ToLower()).ToList();
                foreach (string key in keys)
                {
                    mediaItems = mediaItems.Where(i => i.MediaGeoData.Where(a => a.Name.ToLower() == key).Any());
                }
            }

            if (criteria.IsFilteredBy("countrycode"))
            {
                List<string> keys = criteria.UseCriterion("countrycode").GetStringKeys().Select(a => a.ToLower()).ToList();
                foreach (string key in keys)
                {
                    mediaItems = mediaItems.Where(i => i.MediaGeoData.Where(a => a.CountryCode.ToLower() == key).Any());
                }
            }

            if (criteria.IsFilteredBy("geonameid"))
            {
                List<int> keys = criteria.UseCriterion("geonameid").GetIntKeys().ToList();
                foreach (int key in keys)
                {
                    mediaItems = mediaItems.Where(i => i.MediaGeoData.Where(a => a.GeoNameId == key).Any());
                }
            }

            if (criteria.IsFilteredBy("geoparentid"))
            {
                List<int> keys = criteria.UseCriterion("geoparentid").GetIntKeys().ToList();
                foreach (int key in keys)
                {
                    mediaItems = mediaItems.Where(i => i.MediaGeoData.Where(a => a.ParentId == key).Any());
                }
            }

            if (criteria.IsFilteredBy("latitude"))
            {
                List<double> keys = criteria.UseCriterion("latitude").GetStringKeys().Select(a =>
                {
                    double b;
                    double.TryParse(a, out b);
                    return b;
                }).ToList();

                foreach (double key in keys)
                {
                    mediaItems = mediaItems.Where(i => i.MediaGeoData.Where(a => a.Latitude == key).Any());
                }
            }

            if (criteria.IsFilteredBy("longitude"))
            {
                List<double> keys = criteria.UseCriterion("longitude").GetStringKeys().Select(s =>
                {
                    double d;
                    double.TryParse(s, out d);
                    return d;
                }).ToList();

                foreach (double key in keys)
                {
                    mediaItems = mediaItems.Where(i => i.MediaGeoData.Where(a => a.Longitude == key).Any());
                }
            }

            if (criteria.IsFilteredBy("timezone"))
            {
                List<string> keys = criteria.UseCriterion("timezone").GetStringKeys().Select(a => a.ToLower()).ToList();
                foreach (string key in keys)
                {
                    mediaItems = mediaItems.Where(i => i.MediaGeoData.Where(a => a.Timezone.ToLower() == key).Any());
                }
            }

            if (criteria.IsFilteredBy("admin1code"))
            {
                List<string> keys = criteria.UseCriterion("admin1code").GetStringKeys().Select(a => a.ToLower()).ToList();
                foreach (string key in keys)
                {
                    mediaItems = mediaItems.Where(i => i.MediaGeoData.Where(a => a.Admin1Code.ToLower() == key).Any());
                }
            }

            return mediaItems;
        }


    }
}
