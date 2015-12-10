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
    public class CombinedMediaTextAndTopicFilter
    {
        private enum ValueSelectionType { Topic, TopicId, Audience, None }

        #region TopicIdFilter
        public bool FilterByTopicId { get; set; }
        List<int> TopicIdKeys { get; set; }
        #endregion

        #region ValueProcFilters
        public bool FilterByTopic { get; set; }
        public bool FilterByAudience { get; set; }
        public string FilterByTopicText { get; set; }
        public string FilterByAudienceText { get; set; }
        #endregion ValueProcFilters


        #region MediaProcFilters
        public bool FilterByTitle { get; set; }
        public bool FilterByDescription { get; set; }
        public string FilterByTitleOrDescriptionText { get; set; }
        public string FilterByDescriptionText { get; set; }
        #endregion


        #region UrlFilter
        public bool FilterBySourceUrl { get; set; }
        public string FilterBySourceUrlText { get; set; }
        #endregion UrlFilter

        public MediaObjectContext Media { get; set; }
        Guid SelectionSetId { get; set; }
        int MinutesToLive { get; set; }
        //public string LanguageKeys { get; set; }


        public CombinedMediaTextAndTopicFilter(MediaObjectContext media, ResultSetParms resultSetParms, FilterCriteria criteria)
        {
            FilterByTopic = false;
            FilterByAudience = false;
            FilterByTitle = false;
            FilterByDescription = false;
            FilterBySourceUrl = false;

            Media = media;
            SelectionSetId = resultSetParms.ResultSetId;
            MinutesToLive = resultSetParms.MinutesToLive;

            FilterByTopicId = criteria.IsFilteredBy("TopicId");
            if (FilterByTopicId)
                TopicIdKeys = criteria.UseCriterion("TopicId").GetIntKeys();

            FilterBySourceUrl = criteria.IsFilteredBy("SourceUrl");
            if (FilterBySourceUrl)
                FilterBySourceUrlText = criteria.UseCriterion("SourceUrl").GetStringKey();

            //LanguageKeys = criteria.IsFilteredBy("Language") ?
            //    string.Join(",", criteria.UseCriterion("Language").GetStringKeys().ToArray())
            //    : "";

            if (criteria.IsFilteredBy("FullSearch"))
            {
                string textValue = criteria.UseCriterion("FullSearch").GetStringKey();
                if (textValue.StartsWith(@"http://", StringComparison.OrdinalIgnoreCase) || textValue.StartsWith(@"https://", StringComparison.OrdinalIgnoreCase))
                {
                    FilterBySourceUrl = true;
                    FilterBySourceUrlText = textValue;
                }
                else
                {
                    FilterByTitle = true;
                    FilterByDescription = true;
                    FilterByTitleOrDescriptionText = ConvertToContainsSearch(textValue);
                    FilterByTopic = true;
                    FilterByTopicText = FilterByTitleOrDescriptionText;
                }
                criteria.UseCriterion("Title");
                criteria.UseCriterion("Topic");
                criteria.UseCriterion("Audience");
            }
            else
            {
                if (criteria.IsFilteredBy("Title"))
                {
                    FilterByTitle = true;
                    FilterByTitleOrDescriptionText = ConvertToContainsSearch(criteria.UseCriterion("Title").GetStringKey());
                }

                if (criteria.IsFilteredBy("Topic"))
                {
                    FilterByTopic = true;
                    FilterByTopicText = ConvertToContainsSearch(criteria.UseCriterion("Topic").GetStringKey());
                }

                if (criteria.IsFilteredBy("Audience"))
                {
                    FilterByAudience = true;
                    FilterByAudienceText = ConvertToContainsSearch(criteria.UseCriterion("Audience").GetStringKey());
                }
            }
        }

        public IQueryable<MediaObject> FilterByTextAndValues(IQueryable<MediaObject> mediaItems)
        {
            if (FilterByTopicId)
                BuildSelectionType(ValueSelectionType.TopicId, "Topics", "\"\"", topicIds: TopicIdKeys);
            if (FilterByTopic)
                BuildSelectionType(ValueSelectionType.Topic, "Topics", FilterByTopicText, 
                    FilterByTopicId ? ValueSelectionType.TopicId : ValueSelectionType.None);
            if (FilterByAudience)
                BuildSelectionType(ValueSelectionType.Audience, "Audiences", FilterByAudienceText);

            //0) Remove BuildSelectionType from GetValuesMatchingSelectedText and verify that all lists will be built by the time it is called
            //1) Add TopicId name to BuildSelectionType for Topic if it is there
            //2) Do I need it for audience?
            //3) Update stored proc to join on TopicId list if it is passed

            if (FilterBySourceUrl)
                mediaItems = mediaItems.Where(m => m.SourceUrl.StartsWith(FilterBySourceUrlText));

            if (FilterByTitle)
            {
                if (FilterByTopicId)
                {
                    if (FilterByAudience)
                    {

                        if (FilterByTopic)
                            //Title = Yes   TopicId = Yes   Audience = Yes  Topic = Yes
                            mediaItems = MediasIn(mediaItems, 
                                Join(GetMediaValuesForSelectedTopicIds(), GetMediaValuesForAudiencesMatchingText(), GetMediaSelectionsMatchingText())
                                    .Union(Join(GetMediaValuesForTopicsMatchingText(), GetMediaValuesForAudiencesMatchingText())));
                        else
                            //Title = Yes   TopicId = Yes   Audience = Yes  Topic = No
                            mediaItems = MediasIn(mediaItems,
                                Join(GetMediaValuesForSelectedTopicIds(), GetMediaValuesForAudiencesMatchingText(), GetMediaSelectionsMatchingText()));
                    }
                    else
                    {

                        if (FilterByTopic)
                            //Title = Yes   TopicId = Yes   Audience = No   Topic = Yes
                            //Search for Title in Media and is in ValueId and is in topic ID, and union with the Values
                            mediaItems = MediasIn(mediaItems, 
                                Join(GetMediaValuesForSelectedTopicIds(), GetMediaSelectionsMatchingText())
                                    .Union(GetMediaValuesForTopicsMatchingText()));
                        else
                            //Title = Yes   TopicId = Yes   Audience = No   Topic = No
                            mediaItems = MediasIn(mediaItems,
                                Join(GetMediaValuesForSelectedTopicIds(), GetMediaSelectionsMatchingText()));
                    }
                }
                else
                {
                    if (FilterByAudience)
                    {
                        if (FilterByTopic)
                            //Title = Yes   TopicId = No    Audience = Yes  Topic = Yes
                            mediaItems = MediasIn(mediaItems,
                                Join(GetMediaValuesForAudiencesMatchingText(), GetMediaSelectionsMatchingText())
                                .Union(Join(GetMediaValuesForTopicsMatchingText(), GetMediaValuesForAudiencesMatchingText())));
                        else
                            //Title = Yes   TopicId = No    Audience = Yes  Topic = No
                            mediaItems = MediasIn(mediaItems,
                                Join(GetMediaValuesForAudiencesMatchingText(), GetMediaSelectionsMatchingText()));
                    }
                    else
                    {
                        if (FilterByTopic)
                        //Title = Yes   TopicId = No    Audience = No   Topic = Yes
                        {
                            //IQueryable<int> matchingIds = GetMediaSelectionsMatchingText()
                            //    .Union(GetMediaValuesForTopicsMatchingText());
                            //IQueryable<Medias> z1 = Media.MediaDbEntities.Medias;
                            //z1 = z1.Where(m => m.MediaId == 3);

                            //var z = Media.MediaDbEntities.Medias.Where(m => (matchingIds.Where(x => x == m.MediaId).Count() > 0));
                            //string zQuery = ((ObjectQuery)z).ToTraceString();

                            //List<int> x1 = matchingIds.ToList();
                            //string y = ((ObjectQuery)matchingIds).ToTraceString();


                            mediaItems = MediasIn(mediaItems,
                                GetMediaSelectionsMatchingText()
                                .Union(GetMediaValuesForTopicsMatchingText()));
                        }
                        else
                            //Title = Yes   TopicId = No    Audience = No   Topic = No
                            mediaItems = MediasIn(mediaItems, GetMediaSelectionsMatchingText());
                    }
                }

            }
            else if (FilterByAudience)
            {
                if (FilterByTopic)
                    //Title = No    TopicId = n/a   Audience = Yes  Topic = Yes
                    mediaItems = MediasIn(mediaItems,
                        Join(GetMediaValuesForTopicsMatchingText(), GetMediaValuesForAudiencesMatchingText()));
                if (FilterByTopicId)
                    //Title = No    TopicId = yes   Audience = Yes  Topic = No
                    mediaItems = MediasIn(mediaItems,
                        Join(GetMediaValuesForSelectedTopicIds(), GetMediaValuesForAudiencesMatchingText()));
                else
                    //Title = No    TopicId = No  Audience = Yes  Topic = No
                    mediaItems = MediasIn(mediaItems, GetMediaValuesForAudiencesMatchingText());
            }
            else if (FilterByTopic)
                //Title = No    TopicId = n/a   Audience = No  Topic = Yes
                mediaItems = MediasIn(mediaItems, GetMediaValuesForTopicsMatchingText());
            else if (FilterByTopicId)
                //Title = No    TopicId = yes   Audience = No  Topic = No
                mediaItems = MediasIn(mediaItems, GetMediaValuesForSelectedTopicIds());
            return mediaItems;
        }

        private static IQueryable<int> Join(IQueryable<int> mediaIds1, IQueryable<int> mediaIds2)
        {
            IQueryable<int> joinedSelection = from m1 in mediaIds1
                                                     join m2 in mediaIds2 on m1 equals m2
                                                     select m1;
            return joinedSelection;
        }

        private static IQueryable<int> Join(IQueryable<int> mediaIds1, IQueryable<int> mediaIds2, IQueryable<int> mediaIds3)
        {
            IQueryable<int> joinedSelection = from m1 in mediaIds1
                                                     join m2 in mediaIds2 on m1 equals m2
                                                     join m3 in mediaIds3 on m1 equals m3
                                                     select m1;
            return joinedSelection;
        }

        public IQueryable<int> GetMediaSelectionsMatchingText()
        {
            return GetMediaIdsMatchingText("MediaFullText", FilterByTitleOrDescriptionText, FilterByTitle, FilterByDescription);
        }

        public IQueryable<int> GetMediaIdsMatchingText(string type, string fullText, bool searchTitle, bool searchDescription)
        {
            Media.MediaDbEntities.BuildMediaSelectionSet(SelectionSetId, type, MinutesToLive + 10, "" /*LanguageKeys*/, fullText,
                ToYesNo(searchTitle), ToYesNo(searchDescription));
            return Media.MediaDbEntities.SelectionIds
                    .Where(s => s.SelectionId1 == SelectionSetId && s.SelectionType == type)
                    .Select(s => s.Id);
        }

        public IQueryable<int> GetMediaValuesFor(IQueryable<SelectionValue> selectedValues, string attributeName)
        {
            IQueryable<int> itemsByValue =
                    from mv in Media.MediaDbEntities.MediaValues
                    join a in Media.MediaDbEntities.Attributes
                        on mv.AttributeID equals a.AttributeID
                    join s in selectedValues
                        on mv.ValueId equals s.ValueId
                    where a.AttributeName == attributeName && mv.Active == "Yes"
                    select mv.MediaId;

            return itemsByValue;
        }

        private IQueryable<SelectionValue> GetValuesMatchingSelectedText(ValueSelectionType selectionType)
        {
            string stringSelectionType = selectionType.ToString();
            IQueryable<SelectionValue> selectedValues = Media.MediaDbEntities.SelectionValues
                    .Where(s => s.SelectionId == SelectionSetId && s.SelectionType == stringSelectionType);
            return selectedValues;
        }

        private void BuildSelectionType(ValueSelectionType selectionType, string valueSetName, string valueWildCard, ValueSelectionType valueSetFilter = ValueSelectionType.None, List<int> topicIds = null)
        {
            string topicIdKeys = topicIds == null ? "" : string.Join(",", topicIds.ToArray());
            string stringSelectionType = selectionType.ToString();
            string stringValueSetFilter = valueSetFilter == ValueSelectionType.None ? "" : valueSetFilter.ToString();

            Media.MediaDbEntities.BuildValueSelectionSet(SelectionSetId, stringSelectionType, MinutesToLive + 10, topicIdKeys,
                valueWildCard, valueSetName, "" /*LanguageKeys*/, "Is Child Of,Used For,Use", "", stringValueSetFilter);
        }

        public IQueryable<int> GetMediaValuesForTopicsMatchingText()
        {
            IQueryable<int> mediaValuesForValuesMatchingText = GetMediaValuesFor(
                GetValuesMatchingSelectedText(ValueSelectionType.Topic)
                , "Topic");
            return mediaValuesForValuesMatchingText;
        }
        
        public IQueryable<int> GetMediaValuesForSelectedTopicIds()
        {
            IQueryable<int> mediaValuesForSelectedTopicIds = GetMediaValuesFor(
                GetValuesMatchingSelectedText(ValueSelectionType.TopicId)
                , "Topic");
            return mediaValuesForSelectedTopicIds;
        }

        public IQueryable<int> GetMediaValuesForAudiencesMatchingText()
        {
            IQueryable<int> mediaValuesForAudiencesMatchingText = GetMediaValuesFor(
                GetValuesMatchingSelectedText(ValueSelectionType.Audience)
                , "Audience");
            return mediaValuesForAudiencesMatchingText;
        }

        private static string ToYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        static string ConvertToContainsSearch(string requestedSearch)
        {
            string[] tokens = SplitBySpaceWithQuotes(requestedSearch).Select(s => s.EndsWith("*") ? ('"' + s + '"') : s).ToArray();

            string results = string.Join(" OR ", tokens);
            return string.IsNullOrEmpty(results) ? "\"\"" : results;
        }

        public static List<string> SplitBySpaceWithQuotes(string commandLine)
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            })
                              .Select(arg => arg.Trim())    
                              .Where(arg => !string.IsNullOrEmpty(arg))
                              .ToList();
        }

        public static IQueryable<MediaObject> MediasIn(IQueryable<MediaObject> mediaItems, IQueryable<int> selections)
        {
            return mediaItems.Where(m => selections.Where(id => id == m.Id).Count() > 0);
        }

    }

}
