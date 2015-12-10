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
using System.Configuration;

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidatonProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class SyndicationHandler : MediaHandlerBase
    {
        public static void GetContent(IOutputWriter writer, ICallParser parser, string id)
        {
            parser.Query.Format = FormatType.html;
            SyndicationContentResults results = GetContent(parser, id);
            SerialResponse response = TransformResponse(parser, results);
            writer.Write(response.results);
        }

        public static void GetSyndication(IOutputWriter writer, ICallParser parser, string id)
        {
            SyndicationContentResults results = GetContent(parser, id);
            SerialResponse response = TransformResponse(parser, results);
            response.meta = ServiceUtility.CreateResponseMetaDataForOneItem(parser);
            writer.Write(response, results.Messages);
        }

        private static SerialResponse TransformResponse(ICallParser parser, SyndicationContentResults results)
        {
            //Build wrapper around the response
            SerialResponse response;
            if (parser.Version < 2)
                response = SyndicationHandler.SyndicateV1(results.MediaObject, results.Content);
            else
                response = SyndicationHandler.SyndicateV2(results.MediaObject, results.Content);
            return response;
        }

        private static SyndicationContentResults GetContent(ICallParser parser, string id)
        {
            ValidationMessages messages = new ValidationMessages();
            int mediaId = ServiceUtility.ParseInt(id);

            var extractionCriteria = PreferenceTransformation.GetHtmlExtractionCriteria(parser.ParamDictionary, parser.Version);
            ExtractionResult extractionResult =
                new CsMediaValidationProvider().ExtractContent(mediaId, parser.Query.Url, extractionCriteria, parser);

            MediaObject media = (extractionResult != null && extractionResult.MediaAddress != null) ? extractionResult.MediaAddress.MediaObject : null;

            if (media != null)
            {
                if (media.MediaStatusCodeValue == MediaStatusCodeValue.Archived)
                {
                    extractionResult.ExtractedDetail.Data = ConfigurationManager.AppSettings["ArchiveSyndicationMessage"];
                }
            }

            return new SyndicationContentResults()
            {
                Content = extractionResult.ExtractedDetail.Data,
                MediaObject = media,
                Messages = messages
            };
        }

        public static SyndicationContentResults GetSyndicatedContent(string id, int apiVersion, string url)
        {
            ValidationMessages messages = new ValidationMessages();
            int mediaId = ServiceUtility.ParseInt(id);

            var dict = new Dictionary<string, string>();
            var pref = PreferenceTransformation.GetHtmlExtractionCriteria(dict, apiVersion);
            MediaObject media = null;
            var validationProvider = new CsMediaValidationProvider();
            ExtractionResult extractionResult = null;

            if (!pref.MediaPreferences.HasExtractionCriteria)
            {
                var criteria = new CsBusinessObjects.Media.SearchCriteria { MediaId = id };
                var results = CsMediaSearchProvider.Search(criteria);
                media = results.Select(m => m).ToList().FirstOrDefault();
                if (media == null)
                {
                    throw new InvalidOperationException("Media Id " + id + " not found.");
                }
                extractionResult = validationProvider.ExtractContent(media);
            }
            else
            {
                extractionResult = validationProvider.ExtractContent(mediaId, url, pref);
                media = (extractionResult != null && extractionResult.MediaAddress != null) ? extractionResult.MediaAddress.MediaObject : null;
            }

            return new SyndicationContentResults()
            {
                Content = extractionResult.ExtractedDetail.Data,
                MediaObject = media,
                Messages = messages
            };
        }

        public static SerialResponse SyndicateV1(MediaObject media, string content)
        {
            SerialSyndicationV1 syndContent = new SerialSyndicationV1();
            if (media != null)
            {
                syndContent.mediaId = media.Id.ToString();
                syndContent.mediaType = media.MediaTypeCode;
                syndContent.sourceUrl = media.SourceUrlForApi;
                syndContent.targetUrl = media.TargetUrl == null ? "" : media.TargetUrl;
                syndContent.title = media.Title;
                syndContent.description = media.Description == null ? "" : media.Description;
            }
            syndContent.content = content;

            return new SerialResponse(syndContent);
        }

        public static SerialResponse SyndicateV2(MediaObject media, string content)
        {
            SerialSyndicationV2 syndContent = new SerialSyndicationV2();
            if (media != null)
            {
                syndContent.mediaId = media.Id;
                syndContent.mediaType = media.MediaTypeCode;
                syndContent.sourceUrl = media.SourceUrlForApi;
                syndContent.targetUrl = media.TargetUrl == null ? "" : media.TargetUrl;
                syndContent.name = media.Title;
                syndContent.description = media.Description == null ? "" : media.Description;
            }
            syndContent.content = content;

            return new SerialResponse(syndContent);
        }

    }
}
