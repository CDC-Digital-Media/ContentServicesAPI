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
using System.Net;
using System.Text;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidatonProvider;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public class UrlMediaCollector 
    {
        public CollectedData Get(MediaAddress mediaAddress, MediaValidatorConfig config, MediaTypeValidationItem mediaType)
        {
            HtmlMediaAddress htmlMediaAddress = (HtmlMediaAddress)mediaAddress;
            List<string> validMimeTypes = mediaType.ValidMimeTypes.ToList();

            HttpWebResponse response;
            CollectedData collectedData = ValidateUrl(htmlMediaAddress.Url, validMimeTypes, out response);
            if (collectedData.IsAvailable && collectedData.Messages.NumberOfErrors == 0)
            {
                return GetTheData(htmlMediaAddress, collectedData, response);
            }
            else
            {
                return collectedData;
            }
        }

        public static CollectedData ValidateUrl(string url, IEnumerable<string> validMimeTypes, out HttpWebResponse response, string validationKey = null)
        {
            CollectedData collectedData = new CollectedData()
            {
                Data = ""
            };

            response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Accept = "*/*";
                request.UserAgent = "CDC Syndication Engine";
                request.Timeout = 20000;
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode >= HttpStatusCode.BadRequest)
                {
                    collectedData.Messages.AddError(validationKey, "The URL is invalid and resulted in a bad request");
                    return collectedData;
                }
                else
                {
                    string[] theValues = response.ContentType.Split(';');
                    collectedData.MimeType = theValues.Count() > 0 ? theValues[0] : "";
                }
            }
            catch (UriFormatException uex)
            {
                collectedData.Messages.AddException(validationKey, "The URL not properly formatted: " + url, uex);
                return collectedData;
            }
            catch (WebException ex)
            {
                collectedData.Messages.AddException(validationKey, "The URL is invalid: " + url, ex);
                return collectedData;
            }
            catch (Exception ex)
            {
                collectedData.Messages.AddException(validationKey, "Exception while validating url: " + url, ex);
                return collectedData;
            }
            collectedData.IsAvailable = true;

            if (validMimeTypes.Count() > 0) //Only check Mime Type if we have mime types to check
            {
                if (!validMimeTypes.Where(m => m.Equals(collectedData.MimeType, StringComparison.OrdinalIgnoreCase)).Any())
                {
                    collectedData.Messages.AddError(validationKey, "Content not of the correct mime type");
                }
            }

            return collectedData;
        }

        private static CollectedData GetTheData(HtmlMediaAddress htmlMediaAddress, CollectedData collectedData, HttpWebResponse response)
        {
            try
            {
                using (ResponseStream responseStream = new ResponseStream(response))
                {
                    MediaTypeParms mediaTypeCode = new MediaTypeParms(htmlMediaAddress.MediaTypeCode);
                    if (mediaTypeCode.SetCollectedDataFromResponseStream)
                    {
                        collectedData.Data = responseStream.Read(Encoding.UTF8);
                    }
                }
            }
            catch (Exception ex)
            {
                collectedData.Messages.AddException("Failure trying to read from the URL: " + htmlMediaAddress.Url, ex);
                return collectedData;
            }
            collectedData.IsLoadable = true;

            return collectedData;
        }
    }
}
