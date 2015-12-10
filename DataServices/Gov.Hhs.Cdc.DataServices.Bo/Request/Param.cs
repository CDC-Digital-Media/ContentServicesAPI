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

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    /// <summary>
    /// Using the ApiParm enum will help in reusability of api parms across different calls, but will also improve
    /// readability because the actual API parm will be visiable in the definition of that call.
    /// </summary>
    public enum ApiParam
    {
        audience, topic, topicid, topicids,
        domain,
        id, mediaid,
        showchild,
        Language,
        mediatype, mediatypes,
        q,  //Generic Query
        source,    //SourceCode
        startswith,

        owningorg, maintainingorg,
        //Date Sort
        datepublished, datemodified,
        status, cdctv,
        persistenturl,
        syndicationlistid,
        title,
        valuesetid, valueset,

        //Date Parameters
        fromdatepublished, todatepublished,
        fromdatelastreviewed, todatelastreviewed,
        fromdatecreated, todatecreated,
        fromdatemodified, todatemodified,

        //v2
        Name, NameContains,
        Description, DescriptionContains,
        SourceUrl, SourceUrlContains,
        SourceName, SourceNameContains,
        SourceAcronym, SourceAcronymContains,
        LanguageName, LanguageIsoCode,
        Type, TypeId, TypeName, TagIds,

        DateContentAuthored, ContentAuthoredSinceDate,
        ContentAuthoredBeforeDate, ContentAuthoredInRange,

        DateContentUpdated, ContentUpdatedSinceDate,
        ContentUpdatedBeforeDate, ContentUpdatedInRange,

        DateContentPublished, ContentPublishedSinceDate,
        ContentPublishedBeforeDate, ContentPublishedInRange,

        DateContentReviewed, ContentReviewedSinceDate,
        ContentReviewedBeforeDate, ContentReviewedInRange,

        DateSyndicationCaptured, SyndicationCapturedSinceDate,
        SyndicationCapturedBeforeDate, SyndicationCapturedInRange,

        DateSyndicationUpdated, SyndicationUpdatedSinceDate,
        SyndicationUpdatedBeforeDate, SyndicationUpdatedInRange,

        DateSyndicationVisible, SyndicationVisibleSinceDate,
        SyndicationVisibleBeforeDate, SyndicationVisibleInRange,

        // geo search params
        geoname, countrycode, geonameid, geoparentid,
        latitude, longitude, timezone, admin1code,

        // Data search params
        url, urlcontains, datasetid,
        expiresafterdate, expiresbeforedate,
        isexpired, needsrefresh, isactive, proxycacheappkeyid,
        descriptioncontains,

        // Feed search param
        //importfeedurl
        deactivatemissingitems,
        timestamp,

        action
    }

    public static class Param
    {
        public const string API_ROOT = "resources";
        public static readonly string API_STORAGE = "links";
        public static readonly string MEDIA_THUMBNAIL = "thumbnail.png";
        public static readonly string MEDIA_EMBED = "embed";
        public static readonly string MEDIA_SYNDICATE = "syndicate";
        public static readonly string MEDIA_CONTENT = "content.html";
        public static readonly string PERSISTENT_URL_RESOURCE = "purls";

        public static readonly int VALID_REQUEST_WINDOW = 900;
        //public static readonly int MAX_CACHE_SECONDS = 900;                               // 15 mins        

        //public static readonly int DEFAULT_MAX_PAGE_RECORD = 1000;
        public static readonly int DEFAULT_PAGE_RECORD_MAX = 100;
        public static readonly int DEFAULT_PAGE_NUMBER = 1;
        public static readonly int DEFAULT_PAGE_OFFSET = 0;

        public static readonly string DEFAULT_ECARD_WIDTH = "590";
        public static readonly string DEFAULT_ECARD_HEIGHT = "414";

        public const string NAME_VALUE_PAIR_PREFIX = "nvp-";
        public const string NAME_VALUE_PAIR_CONTAINS = "contains";
        public const string NAME_VALUE_PAIR_STARTSWITH = "startswith";
        public const string NAME_VALUE_PAIR_ENDSWITH = "endswith";

        // combined media search parameter
        public const string MEDIA_TYPE = "mediatype";
        public const string MEDIA_TYPE_V2 = "mediatypes";

        public const string CAMPAIGN = "campaign";

        #region ToRemove //TODO: Remove after original Media is removed
        public const string TOPIC = "topic";
        public const string AUDIENCE = "audience";

        public const string TOPIC_ID = "topicid";
        public const string TOPIC_ID_V2 = "topicids";
        #endregion

        public const string CONTENT_GROUP = "contentgroup";

        public const string TITLE = "title";

        public const string PERSISTENT_URL = "persistenturl";
        public const string URL_PARAM = "url";
        public const string URL_PARAM_CONTAINS = "urlcontains";
        public const string RESOURCE_TYPE = "resourceType";

        //public const string Language = "language";
        //public const string TIME_TO_WAIT = "ttw";
        
        public const string TIME_TO_LIVE = "ttl";
        public const string FORMAT = "format";
        
        public const string PAGE_RECORD_MAX = "max";
        public const string PAGE_NUMBER = "pagenum";
        public const string PAGE_OFFSET = "offset";

        public const string SORT = "sort";
        public static readonly string ORDER = "order";

        public static readonly string CALLBACK = "callback";
        public const string RESULT_SET_ID = "rsid";

        // rest service Authorization parameter
        public const string API_KEY = "apiKey";

        public static readonly string DEFAULT_CLASS_EXTRACTOR = "syndicate";

        // content processing controls
        public static readonly string STRIPBREAK_V2 = "stripbreaks";

        public static readonly string IFRAME_V2 = "iframe";

        public static readonly string STRIPANCHOR = "stripanchor";
        public static readonly string STRIPANCHOR_V2 = "stripanchors";

        public static readonly string STRIPIMAGE = "stripimage";
        public static readonly string STRIPIMAGE_V2 = "stripimages";

        public static readonly string STRIPSCRIPT = "stripscript";
        public static readonly string STRIPSCRIPT_V2 = "stripscripts";

        public static readonly string STRIPCOMMENT = "stripcomment";
        public static readonly string STRIPCOMMENT_V2 = "stripcomments";

        public static readonly string STRIPSTYLE = "stripstyle";
        public static readonly string STRIPSTYLE_V2 = "stripstyles";

        public static readonly string IMAGE_ALIGN = "imageplacement";
        public static readonly string IMAGE_ALIGN_V2 = "imagefloat";

        // syndication selectors
        public static readonly string SYND_CLASS = "clsids";
        public static readonly string SYND_CLASS_V2 = "cssclasses";

        public static readonly string SYND_ELEMENT = "elemids";
        public static readonly string SYND_ELEMENT_V2 = "ids";

        public static readonly string SYND_XPATH = "xpath";
        public static readonly string OUTPUT_ENCODING = "oe";
        public const string CONTENT_NAMESPACE = "ns";
        public static readonly string NEW_WINDOW = "nw";
        public static readonly string OUTPUT_FORMAT = "of";
        public static readonly string POST_PROCESS = "postprocess";

        // Vocabulary Admin
        public const string VALUESET_ID = "id";

        // misc
        public const string FullSearch = "q";
        public const string HEIGHT = "h";
        public const string WIDTH = "w";
        public static string FIELDS = "fields";

        // embed params
        public static readonly string INCLUDE_JQUERY = "includejq";
        public static readonly string JQ_SRC = "jqsrc";
        public static readonly string JS_PLUGIN_SRC = "jspluginsrc";
        public static readonly string CSS_HREF = "csshref";

        // ThumbnailViewer Param
        public const string BROWSER_HEIGHT = "bh";
        public const string BROWSER_WIDTH = "bw";
        public const string SCALE = "scale";
        public const string CROP_X = "cx";
        public const string CROP_Y = "cy";
        public const string CROP_W = "cw";
        public const string CROP_H = "ch";
        public const string PAUSE = "pause";
        public const string APIROOT = "apiroot";
        public const string WEBROOT = "webroot";

        // Feed Param
        public const string SHOW_CHILD = "showchild";
        public const string SHOW_CHILD_LEVEL = "showchildlevel";
        public const string SHOW_PARENT_LEVEL = "showparentlevel";

        public enum SearchType
        {
            Media,
            MediaWithChildren,
            MediaType,
            VocabValue,
            VocabValueItem,
            Language,
            Syndication,
            Valueset,
            ValuesetRelation
        }

        public enum MediaStatus
        {
            Archived,
            Hidden,
            Published,
            Staged
        }
    }
}
