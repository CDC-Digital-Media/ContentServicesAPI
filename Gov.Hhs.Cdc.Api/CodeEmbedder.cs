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
    public sealed class CodeEmbedder
    {
        #region "Private Members"
        private string comments = @"<!-- Markup for {0} ({1}{2}) -->";
        private string noscript = @"<noscript>You need javascript enabled to view this content or go to <a href='{0}'>source URL</a>.</noscript>";
        private string scriptReference = @"<script src='{0}' ></script>";
        private string styleReference = @"<link href='{0}' rel='stylesheet' />";
        private string js_open_tag = @"<script language='javascript'>";
        private string js_close_tag = @"</script>";
        private string div_class_tag = @"<div class='{0}'></div>";


        private string scriptLoader_open_tag = @"$.ajax({{url: '{0}', dataType: 'script', success: function(){{";

        private string plugin_open_tag = @"$('.{0}').synd({{";
        private string plugin_close_tag = @"});";

        private string scriptLoader_close_tag = @"}});";

        private ICallParser _parser;
        private string _mediaId { get; set; }
        #endregion

        private string GetScriptReference(EmbedParam embed, string server)
        {
            string reference = "";

            //JQuery plugin
            if (!string.IsNullOrEmpty(embed.jsPluginSrc))
            {
                reference = embed.jsPluginSrc;
            }
            else
            {
                reference = server + ConfigurationManager.AppSettings["plugin"];
            }

            return reference;
        }

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

        public CodeEmbedder(ICallParser parser, string mediaId)
        {
            _parser = parser;
            _mediaId = mediaId;
        }

        private StringBuilder BuildEmbedHeader(string mediatype, string title, string description, EmbedParam embed)
        {
            string server = GetServer;

            StringBuilder output = new StringBuilder();

            // test for valid URI
            Uri theUri;
            if (!Uri.TryCreate(embed.jquerySrc, UriKind.RelativeOrAbsolute, out theUri))
            {
                embed.jquerySrc = string.Empty;
            }
            if (!Uri.TryCreate(embed.jsPluginSrc, UriKind.RelativeOrAbsolute, out theUri))
            {
                embed.jsPluginSrc = string.Empty;
            }
            if (!Uri.TryCreate(embed.cssHref, UriKind.RelativeOrAbsolute, out theUri))
            {
                embed.cssHref = string.Empty;
            }

            if (!string.IsNullOrEmpty(description))
            {
                description = ": " + description;
            }

            // start building string
            output.Append(string.Format(comments, mediatype.ToUpper(), title, description));

            //JQuery Source
            if (!string.IsNullOrEmpty(embed.jquerySrc))
            {
                output.Append(string.Format(scriptReference, embed.jquerySrc));
            }
            else
            {//JQuery reference
                if (embed.includeJqRef)
                {
                    output.Append(string.Format(scriptReference, server + ConfigurationManager.AppSettings["jquery"]));
                }
            }
            //JQuery plugin
            if (!string.IsNullOrEmpty(embed.jsPluginSrc))
            {
                output.Append(string.Format(scriptReference, embed.jsPluginSrc));
            }
            //else
            //{
            //    if (_parser.Version == 1)
            //        output.Append(string.Format(scriptReference, server_path + ConfigurationManager.AppSettings["plugin"]));
            //    else
            //        output.Append(string.Format(scriptReference, server_path + ConfigurationManager.AppSettings["pluginV2"]));
            //}

            //CSS reference
            if (!string.IsNullOrEmpty(embed.cssHref))
            {
                string[] cssHref = { };
                cssHref = embed.cssHref.Split(',');
                cssHref.Select(a => output.Append(string.Format(styleReference, a))).ToArray();

                //output.Append(string.Format(styleReference, embed.cssHref));
            }

            string div = "rid" + "_" + Guid.NewGuid().ToString().Split('-')[0] + "_" + _mediaId;
            output.Append(string.Format(div_class_tag, div));
            output.Append(js_open_tag);
            output.Append(string.Format(scriptLoader_open_tag, GetScriptReference(embed, server)));
            output.Append(string.Format(plugin_open_tag, div));
            output.Append(string.Format(@" apiroot:'{0}',", server));

            return output;
        }

        private StringBuilder BuildEmberFooter(StringBuilder output)
        {
            output.Append(plugin_close_tag);
            output.Append(scriptLoader_close_tag);
            output.Append(js_close_tag);
            output.Append(string.Format(noscript, GetNoScript));

            return output;
        }

        public string EmbedHtmlMediaType(HtmlPreferences mediaPreferences, string mediatype, string title, string description, EmbedParam embed)
        {
            HtmlPreferences htmlPreferences = mediaPreferences;
            StringBuilder output = BuildEmbedHeader(mediatype, title, description, embed);

            output.Append(string.Format(@" mediaid:{0},", _mediaId));
            output = BuildHtmlByVersion(htmlPreferences, output);

            return BuildEmberFooter(output).ToString();
        }

        private StringBuilder BuildHtmlByVersion(HtmlPreferences preferences, StringBuilder output)
        {
            switch (_parser.Version)
            {
                case 1:
                    output.Append(Format(Param.STRIPSCRIPT, preferences.StripScript));
                    output.Append(Format(Param.STRIPANCHOR, preferences.StripAnchor));
                    output.Append(Format(Param.STRIPIMAGE, preferences.StripImage));
                    output.Append(Format(Param.STRIPCOMMENT, preferences.StripComment));
                    output.Append(Format(Param.STRIPSTYLE, preferences.StripStyle));
                    //output.Append(string.Format(@" {0}:'{1}',", Param.IMAGE_ALIGN, preferences.ImageAlign));
                    output.Append(string.Format(@" {0}:'{1}',", Param.SYND_CLASS, preferences.ClassesAsString));
                    output.Append(string.Format(@" {0}:'{1}',", Param.SYND_ELEMENT, preferences.ElementsAsString));
                    break;
            }

            output.Append(string.Format(@" {0}:'{1}',", Param.SYND_XPATH, preferences.XPathAsString));
            output.Append(string.Format(@" {0}:'{1}',", Param.OUTPUT_ENCODING, preferences.OutputEncoding));
            output.Append(string.Format(@" {0}:'{1}',", Param.OUTPUT_FORMAT, preferences.OutputFormat));
            output.Append(string.Format(@" {0}:'{1}',", Param.CONTENT_NAMESPACE, preferences.ContentNamespace));
            output.Append(Format(Param.NEW_WINDOW, preferences.NewWindow).TrimEnd(','));

            return output;
        }

        private static string Format(string paramName, bool? value)
        {
            return string.Format(@" {0}:'{1}',", paramName, value == null ? "false" : value.ToString().ToLower());
        }

        public string EmbedWidget(string mediatype, string title, string description, EmbedParam embed)
        {
            StringBuilder output = BuildEmbedHeader(mediatype, title, description, embed);
            output.Append(string.Format(@" mediaid:{0},", _mediaId));
            output.Append(string.Format(@" {0}:'{1}'", Param.CONTENT_NAMESPACE, _parser.ReqOptions.ContentNamespace));

            return BuildEmberFooter(output).ToString();
        }

        public string EmbedBadgeButtonVideoImageInfographic(string mediatype, string title, string description, EmbedParam embed)
        {
            StringBuilder output = BuildEmbedHeader(mediatype, title, description, embed);
            output.Append(string.Format(@" mediaid:{0},", _mediaId));
            output.Append(string.Format(@" {0}:'{1}',", Param.WIDTH, _parser.Query.Width == 0 ? "" : _parser.Query.Width.ToString()));
            output.Append(string.Format(@" {0}:'{1}',", Param.HEIGHT, _parser.Query.Height == 0 ? "" : _parser.Query.Height.ToString()));
            output.Append(string.Format(@" {0}:'{1}'", Param.CONTENT_NAMESPACE, _parser.ReqOptions.ContentNamespace));

            return BuildEmberFooter(output).ToString();
        }

    }
}
