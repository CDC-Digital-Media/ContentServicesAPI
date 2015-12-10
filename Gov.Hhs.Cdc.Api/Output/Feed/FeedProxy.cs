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
using System.Configuration;
using System.IO;
using System.Web;

using System.Net;

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using System.ServiceModel.Syndication;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;

using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.CdcMediaProvider;


namespace Gov.Hhs.Cdc.Api
{
    public class FeedProxy : FeedBase
    {
        private Uri Url { get; set; }

        public FeedProxy(List<MediaObject> mediaList, ICallParser parser)
            : base(mediaList, parser)
        {
            Uri proxyUrl;
            bool validUrl = Uri.TryCreate(this.Media.SourceUrl, UriKind.Absolute, out proxyUrl);
            if (validUrl)
            {
                this.Url = proxyUrl;
            }
        }

        public override string Generate()
        {
            string strProxyFeed = "";
            if (this.Url.AbsoluteUri.IndexOf("//.....[domain]...../") > 0)
            {
                strProxyFeed = Facebook();
            }
            else if (this.Url.AbsoluteUri.IndexOf("//.....[domain]...../") > 0)
            {
                strProxyFeed = Twitter();
            }
            else if (this.Url.AbsoluteUri.IndexOf("//api.flickr.com/") > 0)
            {
                strProxyFeed = Flickr();
            }

            return strProxyFeed;
        }

        private string Facebook()
        {
            string facebookFeed = "";

            // Make the request to get the Facebook access token.
            var oAuthRequestUrl = ConfigurationManager.AppSettings["OAUTH_FACEBOOK_TOKEN_REQUEST_URL"] +
                "?client_id=" + ConfigurationManager.AppSettings["OAUTH_FACEBOOK_APP_ID"] +
                "&client_secret=" + ConfigurationManager.AppSettings["OAUTH_FACEBOOK_APP_SECRET"] +
                "&grant_type=client_credentials";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(oAuthRequestUrl);

            // The response from Facebook is the access token formatted 
            // in a way that can just be appended to the subsequent API call. 
            string accessToken = PerformWebRequest(request);

            if (this.Url.AbsoluteUri.IndexOf("?") > 0)
                request = (HttpWebRequest)WebRequest.Create(this.Url.AbsoluteUri + "&" + accessToken);
            else
                request = (HttpWebRequest)WebRequest.Create(this.Url.AbsoluteUri + "?" + accessToken);

            // make request with access token
            facebookFeed = PerformWebRequest(request);
            return facebookFeed;
        }

        private string Twitter()
        {
            string twitterFeed = "";

            // Encode the Twitter API keys for use in getting bearer token.
            string basicCode = ConfigurationManager.AppSettings["OAUTH_TWITTER_CONSUMER_KEY"] + ":" + ConfigurationManager.AppSettings["OAUTH_TWITTER_CONSUMER_SECRET"];
            basicCode = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(basicCode));

            string body = "grant_type=client_credentials";
            string oAuthRequestUrl = ConfigurationManager.AppSettings["OAUTH_TWITTER_TOKEN_REQUEST_URL"];

            // Make the request to get the bearer token.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(oAuthRequestUrl);
            request.Method = "POST";
            request.Headers.Add("Authorization", "Basic " + basicCode);
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            request.ContentLength = body.Length;

            // write POST data
            byte[] bodyByteArray = System.Text.Encoding.UTF8.GetBytes(body);
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(bodyByteArray, 0, bodyByteArray.Length);
            dataStream.Close();

            string bearerToken = PerformWebRequest(request);

            // Extract the token from the JSON response.
            bearerToken = bearerToken.Replace(@"""access_token"":""", "");
            bearerToken = bearerToken.Replace(@"""token_type"":""bearer""", "");
            bearerToken = bearerToken.Replace(@"{", "");
            bearerToken = bearerToken.Replace(@"}", "");
            bearerToken = bearerToken.Replace(@",", "");
            bearerToken = bearerToken.Replace(@"""", "");

            // Now send the actual API request with the bearer token.
            request = (HttpWebRequest)WebRequest.Create(this.Url.AbsoluteUri);
            request.Headers.Add("Authorization", "Bearer " + bearerToken);

            // make request with bearer token
            twitterFeed = PerformWebRequest(request);
            return twitterFeed;
        }

        private string Flickr()
        {
            string flickrFeed = "";
            string theAddress = this.Url.AbsoluteUri; //.Replace("format=rss", "format=rest");
            string addendum = "";

            //get the api key


            if (this.Url.Query.Length == 0 || this.Url.Query.IndexOf("api_key") == -1)
            {
                //hard code a api key, since it was not included in the source url
                addendum = "api_key=" + ConfigurationManager.AppSettings["OAUTH_FLICKR_API_KEY"];

                //addendum = "photoset_id=72157646018355339&per_page=50";
            }
            //handle if the api key is empty
            else
            {
                //split the query into components
                string[] queryArray = this.Url.Query.Substring(0, 1).Equals("?") ? this.Url.Query.Substring(1).Split('&') : this.Url.Query.Split('&'); //have to remove the ?
                foreach (string piece in queryArray)
                {
                    string key = piece.Split('=')[0];
                    string value = piece.Split('=')[1];

                    if (!key.Equals("api_key"))
                    {
                        continue;
                    }
                    else if (String.IsNullOrEmpty(value))
                    {

                        value = ConfigurationManager.AppSettings["OAUTH_FLICKR_API_KEY"];
                        theAddress = theAddress.Replace("api_key=", "api_key=" + value);
                        break;
                    }
                    break;
                }
            }


            ////xmlHttp.setTimeouts EXTERNAL_API_TIMEOUT_RESOLVE, EXTERNAL_API_TIMEOUT_CONNECT, EXTERNAL_API_TIMEOUT_SEND, EXTERNAL_API_TIMEOUT_RECEIVE
            ////On Error Resume Next
            ////xmlHttp.open "GET", formattedUrl, False 
            ////xmlHttp.send ""

            addendum = theAddress.IndexOf('?') == 0 ? "?" : "&" + addendum;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(theAddress + (addendum.Length > 1 ? addendum : ""));
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.Url.AbsoluteUri);

            flickrFeed = PerformWebRequest(request);

            //If Left(responseText, 15) = "jsonFlickrApi({" Then
            //Response.ContentType = "application/json"
            //If Request.QueryString("callback") <> "" Then
            //    Response.Write Replace(responseText, "jsonFlickrApi({", Request.QueryString("callback") & "({")
            //Else
            //    Response.Write responseText
            //End If
            //Set requestedItem = Nothing

            
            //MARC 8/19/2015  the callback is not in the querystring at this point.  For now, just remove the jsonFlickrApi({.  Subsequent processing will handle the callback.
            if (flickrFeed.IndexOf("jsonFlickrApi({") > -1) {
                flickrFeed = flickrFeed.Replace("jsonFlickrApi({", "");
                flickrFeed = flickrFeed.Substring(0, flickrFeed.LastIndexOf('}'));
            }


                
            return flickrFeed;
        }
        
    }
}
