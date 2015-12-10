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

using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.CsCaching;

namespace Gov.Hhs.Cdc.Api.Public
{
    public class RedirectModuleFactory : IHttpModule
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication publicApi)
        {
            publicApi.BeginRequest += Application_BeginRequest;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;

            HttpRequest req = context.Request;
            HttpResponse res = context.Response;

            // if the request has no querystring
            if (req.QueryString.Count == 0)
            {
                string[] segments = req.Url.ToString().Split(new char[] { '/' });
                string sUrl = string.Empty;

                if (segments.Length > 0 && segments[segments.GetUpperBound(0) - 1].ToLower() == "purls")
                {
                    sUrl = PersistentUrlRedirectModule.GetUrl(segments);
                }
                else if (segments.Length > 0 && segments[segments.GetUpperBound(0)].ToLower() == "noscript")
                {
                    sUrl = WebBrowserNoScriptRedirectModule.GetUrl(segments);
                }

                if (!string.IsNullOrEmpty(sUrl))
                {
                    if (!Uri.IsWellFormedUriString(sUrl, UriKind.Absolute))
                    {
                        sUrl = "http://" + sUrl;
                        if (!Uri.IsWellFormedUriString(sUrl, UriKind.Absolute))
                            res.Write("XHTML");
                        else
                        {
                            res.Clear();
                            res.StatusCode = 301;
                            res.Status = "301 Moved Permanently";
                            res.AddHeader("Location", sUrl);
                            res.End();
                        }
                    }
                    else
                    {
                        res.Clear();
                        res.StatusCode = 301;
                        res.Status = "301 Moved Permanently";
                        res.AddHeader("Location", sUrl);
                        res.End();
                    }
                }
            }
        }   


    }
}
