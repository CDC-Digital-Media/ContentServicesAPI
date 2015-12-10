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
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidation.Dal;
using Gov.Hhs.Cdc.MediaValidatonProvider;


namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public class CsMediaValidationProvider
    {
        private static string elem_width = @" width=""{0}""";
        private static string elem_height = @" height=""{0}""";

        private static string hyperlink_image_start = @"<a href=""{0}"" target=""_blank"" title=""{1}""><img alt=""{3}"" src=""{2}""";
        private static string hyperlink_image_end = @"></a>";

        private static string anchor_start = @"<a href=""{0}"" target=""_blank"">{1}";
        private static string anchor_end = @"</a>";

        private static string div_id_start_tag = @"<div id=""{0}"" class=""{1}"">";
        private static string div_end_tag = @"</div>";

        private static string embed_tag_start = @"<embed name=""plugin"" src=""{0}"" type=""application/x-shockwave-flash""";
        private static string embed_tag_end = @" >";

        private static string span_title = @"<span id=""{0}_title_{1}"" >{2}</span>";
        private static string p_description = @"<p id=""{0}_description_{1}"" >{2}</p>";

        private static string videoFrame = @"<iframe width=""{1}"" height=""{2}"" src=""{0}"" frameborder=""0"" allowfullscreen=""""></iframe>";


        protected static IObjectContextFactory MediaValidationObjectContextFactory { get; set; }
        protected static IObjectContextFactory MediaObjectContextFactory { get; set; }

        protected MediaExtractor _mediaExtractor;
        public MediaExtractor TheMediaExtractor
        {
            get
            {
                if (_mediaExtractor == null)
                {
                    _mediaExtractor = new MediaExtractor();
                }
                return _mediaExtractor;
            }
            set { _mediaExtractor = value; }
        }

        public CsMediaValidationProvider()
        {
            MediaValidationObjectContextFactory = new MediaValidationObjectContextFactory();
            MediaObjectContextFactory = new MediaObjectContextFactory();
        }

        public ValidationMessages ValidateResources(List<ResourceObject> resources)
        {
            using (var media = (MediaValidationObjectContext)MediaValidationObjectContextFactory.Create())
            {
                IQueryable<ResourceMimeTypeObject> mimeTypes = ResourceMimeTypeCtl.Get(media);

                var collection = resources
                    .Select((r, i) => new
                    {
                        Resource = r,
                        Index = i,
                        ResourceMimeTypes = mimeTypes.Where(m => m.ResourceTypeCode == r.ResourceTypeCode).Select(m => m.MimeType).ToList()
                    }).ToList();

                var messages = collection.SelectMany(c =>
                    ValidateResource(c.Resource, c.Index, c.ResourceMimeTypes).Messages.Select(m => (ValidationMessage)m)).ToList();
                return new ValidationMessages(messages);
            }
        }

        public static ValidationMessages ValidateResource(ResourceObject resourceObject, int index, List<string> validMimeTypes)
        {
            HttpWebResponse response;
            CollectedData collectedData = UrlMediaCollector.ValidateUrl(resourceObject.Url, validMimeTypes,
                out response, "Resource[" + index.ToString() + "]");
            return collectedData.Messages;
        }

        public ExtractionResult ExtractContent(MediaObject media)
        {
            return ExtractContent(media, null);
        }

        public ExtractionResult ExtractContent(MediaObject media, ICallParser parser)
        {
            try
            {

                return ExtractValidateTransform(media, parser);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public ExtractionResult ExtractContent(int mediaId, string url = null, MediaPreferenceSet urlOverridenMediaPreferences = null, ICallParser parser = null)
        {
            try
            {
                return ExtractValidateTransform(mediaId, url, urlOverridenMediaPreferences, parser);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public ExtractionResult ExtractContent(int mediaId, string url, MediaPreferenceSet pref)
        {
            return ExtractValidateTransform(mediaId, url, pref);
        }

        private ExtractionResult ExtractValidateTransform(MediaObject media, ICallParser parser)
        {
            ExtractionResult er = ExtractAndValidate(media);

            if (parser != null)
            {
                GetSyndicatedContent(parser, er);
            }

            return er;
        }

        private ExtractionResult ExtractValidateTransform(int mediaId, string url, MediaPreferenceSet pref)
        {
            var er = ExtractAndValidate(mediaId, url, pref, isExtraction: true);
            var dict = new Dictionary<string, string>();
            GetSyndicatedContent(dict, "cdc", 2, er);
            return er;
        }

        private ExtractionResult ExtractValidateTransform(int mediaId, string url, MediaPreferenceSet urlOverridenMediaPreferences, ICallParser parser)
        {
            ExtractionResult er = ExtractAndValidate(mediaId, url, urlOverridenMediaPreferences, isExtraction: true);

            if (parser != null)
            {
                GetSyndicatedContent(parser, er);
            }

            return er;
        }

        public ExtractionResult ValidateHtml(int mediaId = 0, string url = null, MediaPreferenceSet mediaPreferences = null)
        {
            try
            {
                return ExtractAndValidate(mediaId, url, mediaPreferences, isExtraction: false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public ExtractionResult ValidateHtmlForUrl(MediaObject media)
        {
            return ExtractAndValidate(media);
        }

        private static ExtractionResult GetSyndicatedContent(IDictionary<string, string> dict, string contentNamespace, int apiVersion, ExtractionResult extractionResult)
        {
            MediaObject media = (extractionResult != null && extractionResult.MediaAddress != null) ? extractionResult.MediaAddress.MediaObject : null;
            string data = (extractionResult != null && extractionResult.ExtractedDetail != null) ? extractionResult.ExtractedDetail.Data : "";

            if (string.IsNullOrEmpty(data) && media != null && !media.MediaTypeParms.SetRequestUrl)
            {
                data = BuildNonHtmlContent(dict, media, contentNamespace, apiVersion).ToString();
            }

            // adding this to remove attribution from all non-html media types.
            if (media != null && media.MediaTypeCode.ToUpper() == "HTML")
            {
                string attribution = (media.Attribution ?? "");
                if (!string.IsNullOrEmpty(attribution))
                {
                    data = HttpUtility.HtmlDecode(data + attribution);
                }
            }
            else
            {
                data = HttpUtility.HtmlDecode(data);
            }

            data = SetCustomDimensions(dict, data);

            if (extractionResult.ExtractedDetail == null)
            {
                extractionResult.ExtractedDetail = new ExtractedDetail() { Data = data };
            }
            else
            {
                extractionResult.ExtractedDetail.Data = data;
            }

            return extractionResult;
        }

        private static ExtractionResult GetSyndicatedContent(ICallParser parser, ExtractionResult extractionResult)
        {
            MediaObject media = (extractionResult != null && extractionResult.MediaAddress != null) ? extractionResult.MediaAddress.MediaObject : null;
            string data = (extractionResult != null && extractionResult.ExtractedDetail != null) ? extractionResult.ExtractedDetail.Data : "";

            if (string.IsNullOrEmpty(data) && media != null && !media.MediaTypeParms.SetRequestUrl)
            {
                data = BuildNonHtmlContent(parser, media).ToString();
            }

            // adding this to remove attribution from all non-html media types.
            if (media != null && media.MediaTypeCode.ToUpper() == "HTML")
            {
                string attribution = (media.Attribution ?? "");
                if (!string.IsNullOrEmpty(attribution))
                {
                    data = HttpUtility.HtmlDecode(data + attribution);
                }
            }
            else
            {
                data = data.Replace("&quot;",""); // removing double quotes from alt tags and title info needed for 508 compliance.
                data = HttpUtility.HtmlDecode(data);
            }

            data = SetCustomDimension(parser, data);

            if (extractionResult.ExtractedDetail == null)
            {
                extractionResult.ExtractedDetail = new ExtractedDetail() { Data = data };
            }
            else
            {
                extractionResult.ExtractedDetail.Data = data;
            }

            return extractionResult;
        }

        private static StringBuilder BuildNonHtmlContent(ICallParser parser, MediaObject media)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine(string.Format(div_id_start_tag, parser.ReqOptions.ContentNamespace + "_" + media.Id, Param.DEFAULT_CLASS_EXTRACTOR));

            if (parser.Version == 1)
            {
                output.AppendLine(string.Format(span_title, parser.ReqOptions.ContentNamespace, media.Id, media.Title));
                output.AppendLine(string.Format(p_description, parser.ReqOptions.ContentNamespace, media.Id, media.Description));
            }

            if (media.MediaTypeParms.IsWidget)
            {
                output.AppendLine(media.Embedcode);
            }

            if (media.MediaTypeParms.IsVideo)
            {
                output.AppendLine(string.Format(videoFrame,
                    media.SourceUrlForApi,
                    parser.ParamDictionary.ContainsKey(Param.WIDTH) ? parser.ParamDictionary[Param.WIDTH] : "",
                    parser.ParamDictionary.ContainsKey(Param.HEIGHT) ? parser.ParamDictionary[Param.HEIGHT] : ""
                    ));
            }

            if (media.MediaTypeParms.IsStaticImageMedia)
            {
                output.AppendLine(string.Format(hyperlink_image_start, media.TargetUrl, media.Title, media.SourceUrlForApi, media.Title + ":" + media.Description));

                if (parser.ParamDictionary.ContainsKey(Param.WIDTH))
                {
                    output.Append(string.Format(elem_width, parser.ParamDictionary[Param.WIDTH]));
                }

                if (parser.ParamDictionary.ContainsKey(Param.HEIGHT))
                {
                    output.Append(string.Format(elem_height, parser.ParamDictionary[Param.HEIGHT]));
                }

                output.Append(hyperlink_image_end);
            }

            if (media.MediaTypeParms.IsEcard)
            {
                Uri theUri;
                if (Uri.TryCreate(media.SourceUrlForApi, UriKind.Relative, out theUri))
                {
                    if (!theUri.IsAbsoluteUri)
                    {
                        media.SourceUrlForApi = ConfigurationManager.AppSettings["ecardlocation"].Trim('/') + "/" + media.SourceUrlForApi.TrimStart('/');
                    }
                }

                output.AppendLine(string.Format(embed_tag_start, media.SourceUrlForApi));

                if (parser.ParamDictionary.ContainsKey(Param.WIDTH))
                {
                    output.Append(string.Format(elem_width, parser.ParamDictionary[Param.WIDTH]));
                }
                else
                {
                    output.Append(string.Format(elem_width, Param.DEFAULT_ECARD_WIDTH));
                }

                if (parser.ParamDictionary.ContainsKey(Param.HEIGHT))
                {
                    output.Append(string.Format(elem_height, parser.ParamDictionary[Param.HEIGHT]));
                }
                else
                {
                    output.Append(string.Format(elem_height, Param.DEFAULT_ECARD_HEIGHT));
                }

                output.Append(embed_tag_end);
            }

            if (media.MediaTypeParms.IsPdf)
            {
                output.AppendLine(string.Format(anchor_start, media.SourceUrlForApi, media.Title + FileSize(media.MediaTypeParms.Code, media.DocumentDataSize)));
                output.Append(anchor_end);
            }

            output.AppendLine(div_end_tag);
            return output;
        }

        private static StringBuilder BuildNonHtmlContent(IDictionary<string, string> dict, MediaObject media, string contentNamespace, int apiVersion)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine(string.Format(div_id_start_tag, contentNamespace + "_" + media.Id, Param.DEFAULT_CLASS_EXTRACTOR));

            if (apiVersion == 1)
            {
                output.AppendLine(string.Format(span_title, contentNamespace, media.Id, media.Title));
                output.AppendLine(string.Format(p_description, contentNamespace, media.Id, media.Description));
            }

            if (media.MediaTypeParms.IsWidget)
            {
                output.AppendLine(media.EffectiveEmbedParameters);
            }

            if (media.MediaTypeParms.IsVideo)
            {
                output.AppendLine(string.Format(videoFrame,
                    media.SourceUrlForApi,
                    dict.ContainsKey(Param.WIDTH) ? dict[Param.WIDTH] : "",
                    dict.ContainsKey(Param.HEIGHT) ? dict[Param.HEIGHT] : ""
                    ));
            }

            if (media.MediaTypeParms.IsStaticImageMedia)
            {
                output.AppendLine(string.Format(hyperlink_image_start, media.TargetUrl, media.Title, media.SourceUrlForApi, media.Title + ":" + media.Description));

                if (dict.ContainsKey(Param.WIDTH))
                {
                    output.Append(string.Format(elem_width, dict[Param.WIDTH]));
                }

                if (dict.ContainsKey(Param.HEIGHT))
                {
                    output.Append(string.Format(elem_height, dict[Param.HEIGHT]));
                }

                output.Append(hyperlink_image_end);
            }

            if (media.MediaTypeParms.IsEcard)
            {
                Uri theUri;
                if (Uri.TryCreate(media.SourceUrlForApi, UriKind.Relative, out theUri))
                {
                    if (!theUri.IsAbsoluteUri)
                    {
                        media.SourceUrlForApi = ConfigurationManager.AppSettings["ecardlocation"].Trim('/') + "/" + media.SourceUrlForApi.TrimStart('/');
                    }
                }

                output.AppendLine(string.Format(embed_tag_start, media.SourceUrlForApi));

                if (dict.ContainsKey(Param.WIDTH))
                {
                    output.Append(string.Format(elem_width, dict[Param.WIDTH]));
                }
                else
                {
                    output.Append(string.Format(elem_width, Param.DEFAULT_ECARD_WIDTH));
                }

                if (dict.ContainsKey(Param.HEIGHT))
                {
                    output.Append(string.Format(elem_height, dict[Param.HEIGHT]));
                }
                else
                {
                    output.Append(string.Format(elem_height, Param.DEFAULT_ECARD_HEIGHT));
                }

                output.Append(embed_tag_end);
            }

            if (media.MediaTypeParms.IsPdf)
            {
                output.AppendLine(string.Format(anchor_start, media.SourceUrlForApi, media.Title + FileSize(media.MediaTypeParms.Code, media.DocumentDataSize)));
                output.Append(anchor_end);
            }

            output.AppendLine(div_end_tag);
            return output;
        }

        private static string FileSize(string mediaType, string fileSize)
        {
            if (!string.IsNullOrEmpty(fileSize))
            {
                fileSize = string.Format(@" [{0} - {1}]", mediaType.ToUpper(), fileSize);
            }

            return fileSize;
        }

        private static string SetCustomDimensions(IDictionary<string, string> dict, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            string regWidth = @"width=""\d+""";
            if (dict.ContainsKey(Param.WIDTH))
            {
                str = Regex.Replace(str, regWidth, "width=\"" + dict[Param.WIDTH] + "\"",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            else
            {
                regWidth = @"width=""0""";
                str = Regex.Replace(str, regWidth, "width=\"\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            regWidth = @"width:\d+";
            if (dict.ContainsKey(Param.WIDTH))
            {
                str = Regex.Replace(str, regWidth, "width:" + dict[Param.WIDTH],
                     RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            else
            {
                regWidth = @"width:""0""";
                str = Regex.Replace(str, regWidth, "width:\"\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            string regHeight = @"height=""\d+""";
            if (dict.ContainsKey(Param.HEIGHT))
            {
                str = Regex.Replace(str, regHeight, "height=\"" + dict[Param.HEIGHT] + "\"",
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            else
            {
                regWidth = @"height=""0""";
                str = Regex.Replace(str, regWidth, "height=\"\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            regHeight = @"height:\d+";
            if (dict.ContainsKey(Param.HEIGHT))
            {
                str = Regex.Replace(str, regHeight, "height:" + dict[Param.HEIGHT],
                      RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            else
            {
                regWidth = @"height:""0""";
                str = Regex.Replace(str, regWidth, "height:\"\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            return str;
        }

        private static string SetCustomDimension(ICallParser parser, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            string regWidth = @"width=""\d+""";
            if (parser.ParamDictionary.ContainsKey(Param.WIDTH))
            {
                str = Regex.Replace(str, regWidth, "width=\"" + parser.ParamDictionary[Param.WIDTH] + "\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            else
            {
                regWidth = @"width=""0""";
                str = Regex.Replace(str, regWidth, "width=\"\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            regWidth = @"width:\d+";
            if (parser.ParamDictionary.ContainsKey(Param.WIDTH))
            {
                str = Regex.Replace(str, regWidth, "width:" + parser.ParamDictionary[Param.WIDTH],
                     RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            else
            {
                regWidth = @"width:""0""";
                str = Regex.Replace(str, regWidth, "width:\"\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            string regHeight = @"height=""\d+""";
            if (parser.ParamDictionary.ContainsKey(Param.HEIGHT))
            {
                str = Regex.Replace(str, regHeight, "height=\"" + parser.ParamDictionary[Param.HEIGHT] + "\"",
                     RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            else
            {
                regWidth = @"height=""0""";
                str = Regex.Replace(str, regWidth, "height=\"\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            regHeight = @"height:\d+";
            if (parser.ParamDictionary.ContainsKey(Param.HEIGHT))
            {
                str = Regex.Replace(str, regHeight, "height:" + parser.ParamDictionary[Param.HEIGHT],
                     RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }
            else
            {
                regWidth = @"height:""0""";
                str = Regex.Replace(str, regWidth, "height:\"\"",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            return str;
        }

        private static void Validate(MediaPreferenceSet mediaPreferences)
        {
            HtmlPreferences htmlExtractionCriteria = mediaPreferences.MediaPreferences;
            htmlExtractionCriteria.ContentNamespace = ValidateNamespace(htmlExtractionCriteria.ContentNamespace);
            if (!IncludedElementsIsValid(htmlExtractionCriteria.IncludedElements))
            {
                htmlExtractionCriteria.IncludedElements = null;
            }
        }

        private static bool IncludedElementsIsValid(ExtractionPath extractionPath)
        {
            return extractionPath != null && (extractionPath.UseXPath || extractionPath.UseElementIds || extractionPath.UseClassNames);
        }

        private static string ValidateNamespace(string nameSpace)
        {
            if (string.IsNullOrEmpty(nameSpace))
            {
                return null;
            }

            // now we need to handle the content namespace parameter
            var trimmedNamespace = nameSpace.TrimEnd('_');

            // make sure it is alpha_only
            if (Regex.IsMatch(trimmedNamespace, "^[a-zA-Z]*$"))
            {
                return StripTrim(trimmedNamespace, 255);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Function to strip out disallowed characters from parameters
        /// and ensure a maximum length is not exceeded.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        private static string StripTrim(String param, int maxLen)
        {
            if (param != null)
            {
                // The following regular expression is used to remove the disallowed
                // characters <>"%;)(&+ from URL/form parameter values.
                Regex re = new Regex("\\<|\\>|\\\"|\\%|\\;|\\)|\\(|\\+|\\`|\\'|\\~|\\+");
                param = re.Replace(param, "");
                param = param.Trim();
                if (param.Length > maxLen)
                {
                    param = param.Substring(0, maxLen);
                }
            }
            return param;
        }


        private ExtractionResult ExtractAndValidate(int mediaId, string url, MediaPreferenceSet urlSpecifiedMediaPreferences, bool isExtraction)
        {
            if (mediaId < 1 && url == "")
            {
                throw new ApplicationException("Media id must be > 0 if no URL is passed");
            }
            if (urlSpecifiedMediaPreferences != null)
            {
                Validate(urlSpecifiedMediaPreferences);
            }
            MediaTypeValidationItem mediaTypeValidationItem = null;
            MediaObject media = null;

            var specifiedPref = new PreferencesSet
            {
                PreferencesPersistedForMediaItem = new MediaPreferenceSetCollection
                {
                    PreferencesSets = new List<MediaPreferenceSet> { urlSpecifiedMediaPreferences }
                }
            };

            using (MediaObjectContext db = (MediaObjectContext)MediaObjectContextFactory.Create())
            {
                if (mediaId > 0)
                {
                    media = MediaCtl.GetComplete(db, mediaId);
                    if (media == null)
                    {
                        throw new ApplicationException("Media Id " + mediaId + " not found.");
                    }

                    if (urlSpecifiedMediaPreferences != null && media.Preferences.Effective != null)
                    {
                        var pref = media.Preferences.Effective.PreferencesSets
                            .FirstOrDefault(p => p.PreferenceType == urlSpecifiedMediaPreferences.PreferenceType);

                        if (pref != null)
                        {
                            media.Preferences = new PreferencesSet
                            {
                                PreferencesPersistedForMediaItem = new MediaPreferenceSetCollection
                                {
                                    PreferencesSets = new List<MediaPreferenceSet> { pref.Merge(urlSpecifiedMediaPreferences) }
                                }
                            };

                        }
                        else
                        {
                            media.Preferences = specifiedPref;
                        }
                    }

                }
            }

            string mediaType = media.MediaTypeCode ?? MediaTypeParms.DefaultHtmlMediaType;

            using (var mediaDb = (MediaValidationObjectContext)MediaValidationObjectContextFactory.Create())
            {
                var mediaMimeTypeItems = MediaMimeTypeItemCtl.GetMediaMimeType(mediaDb);
                var filterItems = MediaTypeFilterItemCtl.GetMediaTypeFilterItems(mediaDb)
                    .Where(m => m.MediaTypeCode == mediaType);

                var mimes = mediaMimeTypeItems.Where(m => m.MediaTypeCode == mediaType).Select(m => m.MimeTypeCode).AsEnumerable();
                mediaTypeValidationItem = new MediaTypeValidationItem
                {
                    ValidMimeTypes = mimes.ToList(),
                    FilterItems = filterItems.ToList()
                };
            }

            MediaAddress mediaAddress = BuildValidationMediaAddress(media ?? new MediaObject
            {
                MediaTypeCode = media.MediaTypeCode ?? MediaTypeParms.DefaultHtmlMediaType,
                SourceUrl = url,
                MediaId = mediaId,
                Preferences = specifiedPref
            });

            if (RequiresExtraction(urlSpecifiedMediaPreferences, mediaAddress))
            {
                return TheMediaExtractor.ExtractAndValidateHtml(isExtraction, mediaTypeValidationItem, mediaAddress);
            }
            else
            {
                return new ExtractionResult()
                {
                    MediaAddress = mediaAddress
                };
            }
        }

        public static List<MediaMimeTypeItem> ValidMimeTypes()
        {
            using (var mediaDb = (MediaValidationObjectContext)MediaValidationObjectContextFactory.Create())
            {
                var mediaMimeTypeItems = MediaMimeTypeItemCtl.GetMediaMimeType(mediaDb)
                    .ToList();

                return mediaMimeTypeItems;
            }
        }

        private ExtractionResult ExtractAndValidate(MediaObject media)
        {
            string mediaType = MediaTypeParms.DefaultHtmlMediaType;

            if (media != null && media.MediaTypeCode != null)
            {
                mediaType = media.MediaTypeCode;
            }

            MediaTypeValidationItem mediaTypeValidationItem = null;
            using (MediaValidationObjectContext mediaDb = (MediaValidationObjectContext)MediaValidationObjectContextFactory.Create())
            {
                IQueryable<MediaMimeTypeItem> mediaMimeTypeItems = MediaMimeTypeItemCtl.GetMediaMimeType(mediaDb);
                var mimes = mediaMimeTypeItems.Where(m => m.MediaTypeCode == mediaType).Select(m => m.MimeTypeCode).AsEnumerable();
                var filterItems = MediaTypeFilterItemCtl.GetMediaTypeFilterItems(mediaDb)
                    .Where(m => m.MediaTypeCode == mediaType);

                mediaTypeValidationItem = new MediaTypeValidationItem
                {
                    ValidMimeTypes = mimes.ToList(),
                    FilterItems = filterItems.ToList()
                };
            }

            var address = BuildValidationMediaAddress(media);
            //MediaAddress mediaAddress = BuildValidationMediaAddress(media.MediaTypeCode, 0, media.SourceUrl, urlSpecifiedMediaPreferences: null, isExtraction: false);

            return TheMediaExtractor.ExtractAndValidateHtml(isExtraction: false, mediaTypeValidationItem: mediaTypeValidationItem, address: address);

        }

        private static List<string> MediaTypesToExtract = new List<string>() { "Html", "Feed Item" };
        private static bool RequiresExtraction(MediaPreferenceSet mediaPreferenceSet, MediaAddress mediaAddress)
        {
            var urlSpecifiedMediaPreferences = mediaPreferenceSet == null ? null : mediaPreferenceSet.MediaPreferences;
            if (mediaAddress.MediaObject != null)
            {
                return MediaTypesToExtract.Contains(mediaAddress.MediaObject.MediaTypeCode, StringComparer.OrdinalIgnoreCase);
            }

            if (urlSpecifiedMediaPreferences == null || urlSpecifiedMediaPreferences.GetType() == typeof(HtmlPreferences))
            {
                var preferences = (HtmlPreferences)urlSpecifiedMediaPreferences;
                if (preferences.HasExtractionCriteria)
                {
                    return true;
                }
            }
            return false;
        }

        //[Obsolete("Use version below")]
        //private static MediaAddress BuildValidationMediaAddress(string mediaType, int mediaId, string url,
        //    MediaPreferenceSet urlSpecifiedMediaPreferences, bool isExtraction)
        //{
        //    using (MediaObjectContext mediaDb = (MediaObjectContext)MediaObjectContextFactory.Create())
        //    {
        //        MediaObject media = null;
        //        if (mediaId > 0)
        //        {
        //            media = MediaCtl.GetComplete(mediaDb, mediaId);
        //            if (media == null) throw new ApplicationException("Media Id " + mediaId + " not found.");
        //        }
        //        //PreferenceTypeEnum preferenceType = MediaPreferenceSet.GetPreferenceType(urlSpecifiedMediaPreferences);
        //        //IMediaPreferences preferences = urlSpecifiedMediaPreferences == null ? null : urlSpecifiedMediaPreferences.MediaPreferences;
        //        //preferences = GetEffectiveMediaPreferences(mediaDb, mediaType, media, preferences, isExtraction, preferenceType);


        //        string overriddenUrl = (string.IsNullOrEmpty(url)) ? null : url;
        //        string urlToUse = overriddenUrl;
        //        if (urlToUse == null)
        //        {
        //            urlToUse = SafeGetUrl(media);
        //        }

        //        DomainItem domain = GetDomainForUrl(mediaDb, urlToUse);
        //        string sourceCode = domain == null ? null : domain.SourceCode;
        //        string domainName = domain == null ? null : domain.DomainName;

        //        //MediaTypeItem mediaTypeItem = media == null ? null : MediaCacheController.GetMediaType(mediaDb, mediaType);
        //        var address = new HtmlMediaAddress(mediaId, urlToUse, sourceCode, domainName);
        //        address.MediaObject = media;

        //        var encoded = System.Web.HttpUtility.UrlPathEncode(urlToUse);

        //        List<MediaObject> allMatchingMediasWithThisUrl = MediaCtl.Get(mediaDb)
        //            .Where(m => (m.SourceUrl == urlToUse || m.SourceUrl == encoded) && m.MediaTypeCode == mediaType)
        //            .ToList();

        //        address.AddressIsAlreadyPersisted = allMatchingMediasWithThisUrl.Where(m => m.MediaId != mediaId).Any();
        //        //var matches = allMatchingMediasWithThisUrl
        //        //    .Where(m => ExtractionCriteriaMatches(mediaDb, m, preferences, preferenceType));
        //        var matches = allMatchingMediasWithThisUrl.Where(m => ExtractionCriteriaMatches(m, media));
        //        address.AddressIsAlreadyPersistedWithSameExtractionCriteria = matches.Any();
        //        address.ExistingMediaId = matches.FirstOrDefault() == null ? 0 : matches.FirstOrDefault().MediaId;
        //        return address;
        //    }
        //}

        private static MediaAddress BuildValidationMediaAddress(MediaObject media)
        {
            var encodedUrl = System.Web.HttpUtility.UrlPathEncode(media.SourceUrl);

            using (MediaObjectContext mediaDb = (MediaObjectContext)MediaObjectContextFactory.Create())
            {
                media.SourceCode = SourceCodeForUrl(media, mediaDb);


                var address = new HtmlMediaAddress(media.MediaId, media.SourceUrl, media.SourceCode);

                List<MediaObject> allMatchingMediasWithThisUrl = MediaCtl.Get(mediaDb)
                    .Where(m => (m.SourceUrl.Equals(media.SourceUrl, StringComparison.InvariantCultureIgnoreCase) || m.SourceUrl.Equals(encodedUrl, StringComparison.InvariantCultureIgnoreCase)
                        || m.SourceUrl.Equals(media.SourceUrl + "/", StringComparison.InvariantCultureIgnoreCase) || m.SourceUrl.Equals(encodedUrl + "/", StringComparison.InvariantCultureIgnoreCase))
                        && m.MediaTypeCode == media.MediaTypeCode)
                    .ToList();

                if (HasNoExtractionCriteria(media))
                {
                    var mediaTypeItem = MediaTypeItemCtl.Get(mediaDb).Where(mt => mt.MediaTypeCode == media.MediaTypeCode).FirstOrDefault();
                    if (string.IsNullOrEmpty(media.ExtractionClasses))
                    {
                        media.ExtractionClasses = media.Preferences.ClassNames();
                    }
                    if (media.Preferences == null)
                    {
                        media.Preferences = mediaTypeItem.Preferences;
                    }
                }

                address.MediaObject = media;
                address.AddressIsAlreadyPersisted = allMatchingMediasWithThisUrl.Where(m => m.MediaId != media.MediaId).Any();

                var matches = allMatchingMediasWithThisUrl.Where(m => ExtractionCriteriaMatches(m, media)).ToList();
                address.AddressIsAlreadyPersistedWithSameExtractionCriteria = matches.Any();
                address.ExistingMediaId = matches.FirstOrDefault() == null ? 0 : matches.FirstOrDefault().MediaId;
                return address;
            }

        }

        private static string SourceCodeForUrl(MediaObject media, MediaObjectContext mediaDb)
        {
            if (!string.IsNullOrEmpty(media.SourceCode))
            {
                return media.SourceCode;
            }

            var domain = GetDomainForUrl(mediaDb, media.SourceUrl);
            if (domain == null)
            {
                return null;
            }

            return domain.SourceCode;
        }

        public static bool ExtractionCriteriaMatches(MediaObject existingMedia, MediaObject newMedia)
        {
            if (existingMedia == null || newMedia == null)
            {
                return false;
            }
            PopulateFromPreferences(existingMedia);
            PopulateFromPreferences(newMedia);

            if (existingMedia.ExtractionXpath != newMedia.ExtractionXpath
                && !string.IsNullOrEmpty(existingMedia.ExtractionXpath)
                && !string.IsNullOrEmpty(newMedia.ExtractionXpath))
            {
                return false;
            }

            var m1c = existingMedia.ExtractionClasses.Split(',');
            var m2c = newMedia.ExtractionClasses.Split(',');

            foreach (var criteria in m2c)
            {
                if (!m1c.Contains(criteria))
                {
                    return false;
                }
            }

            var m1e = existingMedia.ExtractionElementIds.Split(',');
            var m2e = newMedia.ExtractionElementIds.Split(',');

            foreach (var el in m2e)
            {
                if (!m1e.Contains(el))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasNoExtractionCriteria(MediaObject media)
        {
            PopulateFromPreferences(media);
            return string.IsNullOrEmpty(media.ExtractionClasses + media.ExtractionElementIds + media.ExtractionXpath);
        }
        private static void PopulateFromPreferences(MediaObject media)
        {
            if (string.IsNullOrEmpty(media.ExtractionClasses))
            {
                media.ExtractionClasses = media.Preferences.ClassNames();
            }
            if (string.IsNullOrEmpty(media.ExtractionElementIds))
            {
                media.ExtractionElementIds = media.Preferences.ElementIds();
            }
            if (string.IsNullOrEmpty(media.ExtractionXpath))
            {
                media.ExtractionXpath = media.Preferences.XPath();
            }
        }

        private static DomainItem GetDomainForUrl(MediaObjectContext media, string urlToUse)
        {
            if (string.IsNullOrEmpty(urlToUse))
            {
                return null;
            }
            Uri uri = new Uri(urlToUse);
            string domainName = uri.Scheme + "://" + uri.Host;
            List<DomainItem> domains = DomainItemCtl.Get(media).Where(d => d.DomainName.StartsWith(domainName) && d.IsActive).ToList();

            domains = (from d in domains
                       let len = d.DomainName.Length
                       where urlToUse.Length >= len && string.Equals(urlToUse.Substring(0, len), d.DomainName, StringComparison.OrdinalIgnoreCase)
                       orderby len descending
                       select d).ToList();

            return domains.FirstOrDefault();
        }

        //private static HtmlPreferences GetEffectiveMediaPreferences(MediaObjectContext media, string mediaType, MediaObject mediaObject,
        //    IMediaPreferences urlSpecifiedMediaPreferences, bool isExtraction, PreferenceTypeEnum preferenceType)
        //{
        //    var thePreferences = GetSavedMediaPreferences(media, mediaType, mediaObject, preferenceType);
        //    if (urlSpecifiedMediaPreferences != null)
        //        thePreferences = urlSpecifiedMediaPreferences.Merge(thePreferences);

        //    return thePreferences.Merge(thePreferences.GetDefaultPreferences());
        //}

        //private static HtmlPreferences GetSavedMediaPreferences(MediaObjectContext media, string mediaType, MediaObject mediaObject, PreferenceTypeEnum preferenceType)
        //{
        //    if (mediaObject != null)
        //    {
        //        var preferences = mediaObject.Preferences.GetEffectiveExtractionCriteria(preferenceType);
        //        return preferences == null ? null : preferences.MediaPreferences;
        //    }
        //    else
        //    {
        //        var mediaTypeItem = MediaCacheController.GetMediaType(media, mediaType);
        //        var mediaPreferenceSet = mediaTypeItem.Preferences.GetEffectiveExtractionCriteria(preferenceType);
        //        return mediaPreferenceSet == null ? null : mediaPreferenceSet.MediaPreferences;
        //    }
        //}

        //private static string SafeGetUrl(MediaObject mediaObject)
        //{
        //    if (mediaObject == null)
        //        throw new ApplicationException("URL is required.");
        //    return mediaObject.ValidationUrl;
        //}

    }
}
