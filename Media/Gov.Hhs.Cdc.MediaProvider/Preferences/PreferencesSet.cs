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
using System.Xml.Linq;
using System.Xml.Serialization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.Logging;

namespace Gov.Hhs.Cdc.MediaProvider
{
    public class PreferencesSet
    {
        public int MediaId { get; set; }
        public string MediaTypeCode { get; set; }

        //Using the unusual naming convention _A_, _B_, etc to make it clear in the code that we are using the appropriate
        //attributes in each situation.  We may be able to create a class for this, but it has to work with LINQ to Entities
        // (And LINQ to Entitites cannot create a class using a constructor with named parameters).

        //Use the parameters in the following order:  If A is populated, use it.  Else B, and so forth

        #region A
        #endregion A

        #region B
        #endregion B

        #region C
        private string _serializedPreferencesFor_C_MediaItem;
        public string SerializedPreferencesFor_C_MediaItem
        {
            set { _serializedPreferencesFor_C_MediaItem = value; }
            get { return _serializedPreferencesFor_C_MediaItem; }
        }
        private MediaPreferenceSetCollection _preferencesFor_C_MediaItem;
        public MediaPreferenceSetCollection PreferencesPersistedForMediaItem
        {
            get { return _preferencesFor_C_MediaItem ?? GetMediaPreferences(_serializedPreferencesFor_C_MediaItem, "ForMediaItem"); }
            set { _preferencesFor_C_MediaItem = value; }
        }
        #endregion C

        #region D
        private string _serializedPreferencesFor_D_MediaType;
        public string SerializedPreferencesFor_D_MediaType
        {
            get { return _serializedPreferencesFor_D_MediaType; }
            set { _serializedPreferencesFor_D_MediaType = value; }
        }
        private MediaPreferenceSetCollection _preferencesFor_D_MediaType { get; set; }
        public MediaPreferenceSetCollection D_ForMediaType
        {
            get { return _preferencesFor_D_MediaType ?? GetMediaPreferences(_serializedPreferencesFor_D_MediaType, "ForMediaType"); }
        }
        #endregion D

        public MediaPreferenceSetCollection Effective
        {
            get { return PreferencesPersistedForMediaItem ?? D_ForMediaType; }
        }

        public PreferencesSet()
        {
        }

        public PreferencesSet(MediaPreferenceSet pref)
        {
            PreferencesPersistedForMediaItem = new MediaPreferenceSetCollection { PreferencesSets = new List<MediaPreferenceSet> { pref } };
        }

        public PreferencesSet(int mediaId, string serializedPreferencesForMediaItem, string mediaTypeCode, string serializedPreferencesForMediaType)
        {
            MediaId = mediaId;
            SerializedPreferencesFor_C_MediaItem = serializedPreferencesForMediaItem;
            MediaTypeCode = mediaTypeCode;
            SerializedPreferencesFor_D_MediaType = serializedPreferencesForMediaType;
        }

        public MediaPreferenceSetCollection GetMediaPreferences(string serializedPreferences, string preferenceSource)
        {
            if (string.IsNullOrEmpty(serializedPreferences))
                return null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MediaPreferenceSetCollection));
                XElement theDocument = XDocument.Parse(serializedPreferences).Root;
                MediaPreferenceSetCollection thePreferences = theDocument.FromXElement<MediaPreferenceSetCollection>();
                return thePreferences;
            }
            catch (InvalidOperationException)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MediaPreferences));
                    XElement theDocument = XDocument.Parse(serializedPreferences).Root;
                    MediaPreferences thePreferences = theDocument.FromXElement<MediaPreferences>();
                    return new MediaPreferenceSetCollection()
                    {
                        PreferencesSets = thePreferences.ExtractionCriteria.Select(e => ConvertV1ToCurrent((HtmlExtractionCriteria)e)).ToList()
                    };
                }
                catch (Exception ex2)
                {
                    Logger.LogError(ex2,
                        string.Format("MediaObject.GetMediaPreferences Invalid preferences for MediaId({0}), MediaType({1}), from {2}: {3} ",
                            MediaId, MediaTypeCode, preferenceSource, serializedPreferences));
                    return new MediaPreferenceSetCollection();
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex,
                    string.Format("MediaObject.GetMediaPreferences Invalid preferences for MediaId({0}), MediaType({1}), from {2}: {3} ",
                        MediaId, MediaTypeCode, preferenceSource, serializedPreferences));
                return new MediaPreferenceSetCollection();
            }
        }

        private static MediaPreferenceSet ConvertV1ToCurrent(HtmlExtractionCriteria o)
        {
            MediaPreferenceSet mediaPreferences = new MediaPreferenceSet()
            {
                IsDefault = o.IsDefault,
                PreferenceType = o.ExtractionName,
                MediaPreferences = new HtmlPreferences()
                {
                    IncludedElements = o.IncludedElements,
                    ExcludedElements = o.ExcludedElements,
                    StripAnchor = o.StripAnchor,
                    StripComment = o.StripComment,
                    StripImage = o.StripImage,
                    StripScript = o.StripScript,
                    StripStyle = o.StripStyle,
                    NewWindow = o.NewWindow,
                    Iframe = o.Iframe,
                    ImageAlign = o.ImageAlign,
                    OutputEncoding = o.OutputEncoding,
                    OutputFormat = o.OutputFormat,
                    ContentNamespace = o.ContentNamespace
                }

            };
            return mediaPreferences;
        }

        //public static MediaPreferenceSet GetDefaultExtractionCriteria(int mediaId, string embedParametersForMediaItem, string mediaTypeCode, string embedParametersForMediaType)
        //{
        //    PreferencesSet set = new PreferencesSet(mediaId, embedParametersForMediaItem, mediaTypeCode, embedParametersForMediaType);
        //    return set.GetEffectiveExtractionCriteria();
        //}



        //public MediaPreferenceSet GetEffectiveExtractionCriteria(PreferenceTypeEnum preferenceType = PreferenceTypeEnum.WebPage)
        //{
        //    var sets = GetPrioritizedExtractionCriteriaSets();

        //    List<MediaPreferenceSet> extractionCriteria = sets.Where(s => s != null).Select(s => GetSetContainingWeb(s, preferenceType)).ToList();
        //    return MergeExtractionCriteria(extractionCriteria);
        //}

        //private static MediaPreferenceSet MergeExtractionCriteria(List<MediaPreferenceSet> extractionCriteria)
        //{
        //    MediaPreferenceSet mergedExtractionCriteria = extractionCriteria.Count() == 0 ? new MediaPreferenceSet() :
        //        extractionCriteria[0];   //Start with the first level

        //    //Loop through the second extraction criteria set (and onward), and merge into the combined extraction criteria
        //    for (int i = 1; i < extractionCriteria.Count(); ++i)
        //    {
        //        mergedExtractionCriteria = mergedExtractionCriteria == null ? extractionCriteria[i] :
        //            mergedExtractionCriteria.Merge(extractionCriteria[i]);
        //    }
        //    return mergedExtractionCriteria;
        //}

        //private List<List<MediaPreferenceSet>> GetPrioritizedExtractionCriteriaSets()
        //{
        //    List<List<MediaPreferenceSet>> extractionCriterion = new List<List<MediaPreferenceSet>>();
        //    extractionCriterion.Add(GetDefaultExtractionCriteria(PreferencesPersistedForMediaItem));
        //    extractionCriterion.Add(GetDefaultExtractionCriteria(D_ForMediaType));
        //    return extractionCriterion;
        //}

        //HtmlPreferences
        //private List<MediaPreferenceSet> GetDefaultExtractionCriteria(MediaPreferenceSetCollection preferences)
        //{
        //    return (preferences == null || preferences.PreferencesSets == null)
        //        ? null : preferences.PreferencesSets;
        //}

    }
}
