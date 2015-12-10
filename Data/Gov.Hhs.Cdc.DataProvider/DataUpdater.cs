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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataProvider
{
    class DataUpdater
    {
        private string url { get; set; }
        private IDataServicesObjectContext objectContext { get; set; }

        private static string BAD_URL = "Bad Url";
        public const int CONSECUTIVE_FAILURE_LIMIT = 5;

        public DataUpdater(IDataServicesObjectContext objectContext, string url)
        {
            this.url = url;
            this.objectContext = objectContext;
        }

        public void UpdateData()
        {
            Thread t = new Thread(new ThreadStart(GetData));
            t.IsBackground = true;
            t.Start();
        }

        private void GetData()
        {
            string rootUrl = ConfigurationManager.AppSettings["ProxyCacheRootDataUrl"];
            string applicationToken = ConfigurationManager.AppSettings["ProxyCacheApplicationToken"];
            WebRequest request = HttpWebRequest.Create(rootUrl + url);
            request.Headers.Add("X-App-Token", applicationToken);
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 10000;

            IAsyncResult result = request.BeginGetResponse(
                new AsyncCallback(HandleResponse), request);

            ThreadPool.RegisterWaitForSingleObject(
                result.AsyncWaitHandle,
                new WaitOrTimerCallback(ScanTimeoutCallback),
                request,
                (15 * 1000),
                true
                );
        }

        private static void ScanTimeoutCallback(object obj, bool timedOut)
        {
            if (timedOut)
            {
                if (obj != null && obj is WebRequest)
                {
                    WebRequest request = (WebRequest)obj;
                    request.Abort();
                }
            }
        }

        private void HandleResponse(IAsyncResult result)
        {
            WebRequest request = (WebRequest)result.AsyncState;
            string dataResult = BAD_URL;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8);
                    dataResult = readStream.ReadToEnd();
                }
            }
            catch (Exception)
            {//404s throw exception, just catch and move on
            }

            using (objectContext)
            {
                var dbContext = objectContext as DataObjectContext;

                ProxyCacheObject proxyCacheObject = ProxyCacheCtl.Get(dbContext, forUpdate: true)
                    .Where(d => d.Url == url).FirstOrDefault();

                if (proxyCacheObject != null)
                {
                    if (dataResult.Equals(BAD_URL))
                    {
                        proxyCacheObject.Failures++;
                        if (proxyCacheObject.Failures < CONSECUTIVE_FAILURE_LIMIT)
                        {
                            proxyCacheObject.ExpirationDateTime = DateTime.UtcNow.Add(new TimeSpan(0, 1, 0)); //Retry in > 1 minute
                        }
                    }
                    else
                    {
                        proxyCacheObject.Data = dataResult;
                        proxyCacheObject.Failures = 0;
                        proxyCacheObject.ExpirationDateTime = DateTime.UtcNow.Add(proxyCacheObject.GetExpirationIntervalTimeSpan());
                    }

                    ProxyCacheCtl.Update(dbContext, proxyCacheObject, proxyCacheObject, enforceConcurrency: false);
                    dbContext.SaveChanges();
                }
                else
                {
                    //Do Nothing???  Some error happened in creating or updating earlier
                }

            }
        }
    }
}
