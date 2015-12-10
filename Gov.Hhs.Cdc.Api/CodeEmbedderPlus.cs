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
using System.Web;
using System.Text;
using System.Configuration;
using Gov.Hhs.Cdc.MediaProvider;

using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.Api
{
    //TODO: refactor
    public sealed class CodeEmbedderPlus
    {
        #region "Private Members"

        private string comments = @"<!-- Markup for {0} ({1}{2}{3}) -->";
        private string noscript = @"<noscript>You need javascript enabled to view this content or go to <a href='{0}'>source URL</a>.</noscript>";
        private string scriptReference = @"<script src='{0}' ></script>";
        private string div_class_open_tag = @"<div class='{0}'";
        private string div_close_tag = @"></div>";

        private string class_id { get; set; }
        private ICallParser _parser;
        private string _mediaId { get; set; }

        private HtmlPreferences _htmlPreferences { get; set; }

        #endregion

        private string GetServer
        {
            get
            {
                //    return ServiceUtility.GetServerBaseOnProtocol(false) + System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
                //return ServiceUtility.GetServerBaseOnProtocol(false) + "/api";
                return "//" + ConfigurationManager.AppSettings["PublicApiServer"];
            }
        }

        private string GetNoScript
        {
            get
            {
                //    return ServiceUtility.GetCurrentUrlPath(false, "noscript");
                return string.Format(GetServer + "/v{0}/resources/media/{1}/{2}", _parser.Version, this._mediaId, "noscript");
            }
        }

        public CodeEmbedderPlus(ICallParser parser, string mediaId)
        {
            _parser = parser;
            _mediaId = mediaId;
        }

        private StringBuilder BuildEmbedHeader(string mediatype, string title, string description, string fileSize, string server)
        {
            StringBuilder output = new StringBuilder();
            class_id = "rid" + "_" + Guid.NewGuid().ToString().Split('-')[0] + "_" + _mediaId;

            if (!string.IsNullOrEmpty(description))
            {
                description = ": " + description;
            }

            fileSize = FileSize(mediatype, fileSize);

            // start building string
            output.Append(string.Format(comments, mediatype.ToUpper(), title, description, fileSize));

            // start div
            output.Append(string.Format(div_class_open_tag, class_id));
            output.Append(string.Format(@" data-apiroot='{0}'", server));
            output.Append(string.Format(@" data-mediatype='{0}'", mediatype.ToLower()));

            return output;
        }

        private string FileSize(string mediaType, string fileSize)
        {
            if (!string.IsNullOrEmpty(fileSize))
            {
                fileSize = string.Format(@" [{0} - {1}]", mediaType.ToUpper(), fileSize);
            }

            return fileSize;
        }

        private StringBuilder BuildHtmlEmbedFooter(StringBuilder output, string server)
        {
            output.Append(div_close_tag);

            if (this._htmlPreferences.Iframe == true)
            {
                output.Append(string.Format(scriptReference, ConfigurationManager.AppSettings["iFrameWidgetSrc"]));
            }
            else
            {
                output.Append(string.Format(scriptReference, server + ConfigurationManager.AppSettings["pluginV2"]));
            }

            output.Append(string.Format(noscript, GetNoScript));

            return output;
        }

        private StringBuilder BuildNonHtmlEmbedFooter(StringBuilder output, string server)
        {
            output.Append(div_close_tag);
            output.Append(string.Format(scriptReference, server + ConfigurationManager.AppSettings["pluginV2"]));
            output.Append(string.Format(noscript, GetNoScript));

            return output;
        }

        public string EmbedHtmlMediaType(HtmlPreferences mediaPreferences, string mediatype, string title, string description)
        {
            // merge wih defaults preferences
            this._htmlPreferences = HtmlPreferences.GetDefaultPreferences().Merge(mediaPreferences);

            string server = GetServer;

            StringBuilder output = BuildEmbedHeader(mediatype, title, description, "", server);

            output.Append(string.Format(@" data-mediaid='{0}'", _mediaId));
            output = BuildHtmlByVersion(output);

            return BuildHtmlEmbedFooter(output, server).ToString();
        }

        private StringBuilder BuildHtmlByVersion(StringBuilder output)
        {
            if (_parser.Version > 1)
            {
                output.Append(Format(Param.STRIPSCRIPT_V2, this._htmlPreferences.StripScript));
                output.Append(Format(Param.STRIPANCHOR_V2, this._htmlPreferences.StripAnchor));
                output.Append(Format(Param.STRIPIMAGE_V2, this._htmlPreferences.StripImage));
                output.Append(Format(Param.STRIPCOMMENT_V2, this._htmlPreferences.StripComment));
                output.Append(Format(Param.STRIPSTYLE_V2, this._htmlPreferences.StripStyle));
                //output.Append(string.Format(@" data-{0}='{1}'", Param.IMAGE_ALIGN_V2, this._htmlPreferences.ImageAlign));
                output.Append(string.Format(@" data-{0}='{1}'", Param.SYND_CLASS_V2, this._htmlPreferences.ClassesAsString));
                output.Append(string.Format(@" data-{0}='{1}'", Param.SYND_ELEMENT_V2, this._htmlPreferences.ElementsAsString));
            }

            output.Append(string.Format(@" data-{0}='{1}'", Param.SYND_XPATH, this._htmlPreferences.XPathAsString));
            output.Append(string.Format(@" data-{0}='{1}'", Param.OUTPUT_ENCODING, this._htmlPreferences.OutputEncoding));
            output.Append(string.Format(@" data-{0}='{1}'", Param.OUTPUT_FORMAT, this._htmlPreferences.OutputFormat));
            output.Append(string.Format(@" data-{0}='{1}'", Param.CONTENT_NAMESPACE, this._htmlPreferences.ContentNamespace));
            output.Append(string.Format(@" data-{0}='{1}'", Param.POST_PROCESS, this._htmlPreferences.PostProcess));
            output.Append(Format(Param.NEW_WINDOW, this._htmlPreferences.NewWindow));
            output.Append(Format(Param.IFRAME_V2, this._htmlPreferences.Iframe));

            if (this._htmlPreferences.Iframe == true)
            {
                //data-cdc-widget="syndicationIframe"
                output.Append(string.Format(@" data-{0}='{1}'", "cdc-widget", "syndicationIframe"));
            }

            // data purposes only, so that we can switch back and forth in the storefront UI without having to callback to the API
            output.Append(string.Format(@" data-apiembedsrc='{0}'", GetServer + ConfigurationManager.AppSettings["pluginV2"]));
            output.Append(string.Format(@" data-iframeembedsrc='{0}'", ConfigurationManager.AppSettings["iFrameWidgetSrc"]));

            return output;
        }

        private static string Format(string paramName, bool? value)
        {
            return string.Format(@" data-{0}='{1}'", paramName, value == null ? "false" : value.ToString().ToLower());
        }

        public string EmbedWidget(string mediatype, string title, string description)
        {
            string server = GetServer;

            StringBuilder output = BuildEmbedHeader(mediatype, title, description, "", server);
            output.Append(string.Format(@" data-mediaid='{0}'", _mediaId));
            output.Append(string.Format(@" data-{0}='{1}'", Param.CONTENT_NAMESPACE, _parser.ReqOptions.ContentNamespace));

            return BuildHtmlEmbedFooter(output, server).ToString();
        }

        public string EmbedBadgeButtonVideoImageInfographic(string mediatype, string title, string description)
        {
            string server = GetServer;

            StringBuilder output = BuildEmbedHeader(mediatype, title, description, "", server);
            output.Append(string.Format(@" data-mediaid='{0}'", _mediaId));
            output.Append(string.Format(@" data-{0}='{1}'", Param.WIDTH, _parser.Query.Width == 0 ? "" : _parser.Query.Width.ToString()));
            output.Append(string.Format(@" data-{0}='{1}'", Param.HEIGHT, _parser.Query.Height == 0 ? "" : _parser.Query.Height.ToString()));
            output.Append(string.Format(@" data-{0}='{1}'", Param.CONTENT_NAMESPACE, _parser.ReqOptions.ContentNamespace));

            return BuildNonHtmlEmbedFooter(output, server).ToString();
        }

        public string EmbedEcard(string mediatype, string title, string description)
        {
            string server = GetServer;

            StringBuilder output = BuildEmbedHeader(mediatype, title, description, "", server);
            output.Append(string.Format(@" data-mediaid='{0}'", _mediaId));
            output.Append(string.Format(@" data-{0}='{1}'", "filepath", ""));
            output.Append(string.Format(@" data-{0}='{1}'", "navigationtext", "Choose another eCard"));
            output.Append(string.Format(@" data-{0}='{1}'", "navigationurl", ""));
            output.Append(string.Format(@" data-{0}='{1}'", "completenavigationtext", "Choose another eCard"));
            output.Append(string.Format(@" data-{0}='{1}'", "completenavigationurl", ""));
            output.Append(string.Format(@" data-{0}='{1}'", "receipturl", ""));

            return BuildNonHtmlEmbedFooter(output, server).ToString();
        }

        public string EmbedPdf(string mediatype, string title, string description, string fileSize)
        {
            string server = GetServer;

            StringBuilder output = BuildEmbedHeader(mediatype, title, description, fileSize, server);
            output.Append(string.Format(@" data-mediaid='{0}'", _mediaId));

            return BuildNonHtmlEmbedFooter(output, server).ToString();
        }

    }
}
