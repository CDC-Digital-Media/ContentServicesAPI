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
using System.Web;
using System.Configuration;

namespace Gov.Hhs.Cdc.Api.Handlers.MediaDataManagers
{
    public class MetricModule : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.PreRequestHandlerExecute += new EventHandler(context_PreRequestHandlerExecute);
            context.EndRequest += new EventHandler(context_EndRequest);
            context.AuthorizeRequest += new EventHandler(context_AuthorizeRequest);
        }

        void context_AuthorizeRequest(object sender, EventArgs e)
        {

            //do the metrics stuff
        }

        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            //Redirect the browser to the content that it requested
            HttpContext context = ((HttpApplication)sender).Context;

            if (context.Items["destinationUrl"] != null)
            {
                context.Response.Redirect((string)context.Items["destinationUrl"]);
            }
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            //The request was processed
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            //Received a request, so we chck if it is a metrics request and has a url in the querystring.  If so, save the destination URL here
            HttpContext context = ((HttpApplication)sender).Context;

            if (context.Request.Url.AbsoluteUri.Contains(ConfigurationManager.AppSettings["METRICS_PAGE"]) && context.Request.QueryString["url"] != null)
            {
                context.Items["destinationUrl"] = context.Request.QueryString["url"];
            }
        }
    }
}
