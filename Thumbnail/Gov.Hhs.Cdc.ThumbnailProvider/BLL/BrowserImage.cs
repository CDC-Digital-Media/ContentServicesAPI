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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Gov.Hhs.Cdc.DataServices.Bo;
using System.Timers;

/*
 Copyright (c) 2011 Robert Pohl, robert@sugarcubesolutions.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */
namespace Gov.Hhs.Cdc.ThumbnailProvider.BLL
{
    public static class BrowserImage
    {
        public static Bitmap GetHtmlThumbnail(string htmlContent, int browserWidth, int browserHeight, int thumbnailWidth, int thumbnailHeight, int pause)
        {
            htmlContent = htmlContent.Replace("='//", "='https://");

            var thumbnailGenerator = new HtmlThumbnailImage(htmlContent, browserWidth, browserHeight, thumbnailWidth, thumbnailHeight, pause);
            return thumbnailGenerator.GenerateHtmlThumbnailImage();
        }

        public static Bitmap GetUrlThumbnail(Uri url, int browserWidth, int browserHeight, int thumbnailWidth, int thumbnailHeight, int pause)
        {
            var thumbnailGenerator = new HtmlThumbnailImage(url, browserWidth, browserHeight, thumbnailWidth, thumbnailHeight, pause);
            return thumbnailGenerator.GenerateUrlThumbnailImage();
        }
    }


    /// <summary>
    /// Added HtmlThumbnailImage for generating image from html content string.
    /// </summary>
    internal class HtmlThumbnailImage
    {
        private Bitmap _mBitmap;
        private string _HtmlContent;
        private int _mThumbnailHeight;
        private int _mThumbnailWidth;
        private int _mPause;
        private Uri _mUrl;

        public HtmlThumbnailImage(string htmlContent, int browserWidth, int browserHeight, int thumbnailWidth, int thumbnailHeight, int pause)
        {
            this.isDone = false;
            _HtmlContent = htmlContent;

            BrowserWidth = browserWidth;
            BrowserHeight = browserHeight;
            _mThumbnailHeight = thumbnailHeight;
            _mThumbnailWidth = thumbnailWidth;
            _mPause = pause <= 0 ? 1 : pause;
        }

        public HtmlThumbnailImage(Uri webPageUrl, int browserWidth, int browserHeight, int thumbnailWidth, int thumbnailHeight, int pause)
        {
            this.isDone = false;

            BrowserWidth = browserWidth;
            BrowserHeight = browserHeight;
            _mThumbnailHeight = thumbnailHeight;
            _mThumbnailWidth = thumbnailWidth;
            _mPause = pause <= 0 ? 1 : pause;
            _mUrl = webPageUrl;
        }


        #region PROPERTIES
        private bool isDone { get; set; }


        public int BrowserWidth
        {
            get;

            set;
        }

        public int BrowserHeight
        {
            get;

            set;
        }

        public int Pause
        {
            get { return _mPause; }

            set { _mPause = value; }
        }

        //public Uri Url
        //{
        //    get { return _mUrl; }
        //    set { _mUrl = value; }
        //}

        #endregion

        public Bitmap GenerateUrlThumbnailImage()
        {
            var mThread = new Thread(_GenerateUrlThumbnailImage);
            mThread.SetApartmentState(ApartmentState.STA);
            mThread.Start();
            mThread.Join();
            return _mBitmap;
        }

        private void _GenerateUrlThumbnailImage()
        {

            var mWebBrowser = new WebBrowser();
            mWebBrowser.ScrollBarsEnabled = false;
            mWebBrowser.Url = _mUrl;

            mWebBrowser.ScriptErrorsSuppressed = true;
            mWebBrowser.Height = BrowserHeight;
            mWebBrowser.Width = BrowserWidth;

            System.Timers.Timer Timer1 = new System.Timers.Timer(Pause);
            Timer1.Elapsed += Timer1_Tick;
            Timer1.Start();

            while (isDone == false)
            {
                Application.DoEvents();
            }

            DocumentCompletedFully(mWebBrowser, null);
            mWebBrowser.Dispose();

        }

        public Bitmap GenerateHtmlThumbnailImage()
        {
            var mThread = new Thread(_GenerateHtmlThumbnailImage);
            mThread.SetApartmentState(ApartmentState.STA);
            mThread.Start();
            mThread.Join();
            return _mBitmap;
        }

        private void _GenerateHtmlThumbnailImage()
        {
            try
            {
                var mWebBrowser = new WebBrowser();
                mWebBrowser.ScrollBarsEnabled = false;
                mWebBrowser.DocumentText = _HtmlContent;            //New - generating from string html fragment or page
                AuditLogger.LogAuditEvent("Thumbnail: HTML for thumbnail: " + _HtmlContent);

                mWebBrowser.ScriptErrorsSuppressed = true;
                mWebBrowser.Height = BrowserHeight;
                mWebBrowser.Width = BrowserWidth;

                System.Timers.Timer Timer1 = new System.Timers.Timer(Pause);
                Timer1.Elapsed += Timer1_Tick;
                Timer1.Start();

                while (isDone == false)
                {
                    Application.DoEvents();
                }

                DocumentCompletedFully(mWebBrowser, null);
                mWebBrowser.Dispose();
            }
            catch (Exception ex)
            {
                AuditLogger.LogAuditEvent("Thumbnail error thrown: " + ex.Message);
                throw;
            }
        }

        private void Timer1_Tick(object sender, ElapsedEventArgs e)
        {
            System.Timers.Timer Timer1 = (System.Timers.Timer)sender;
            Timer1.Enabled = false;
            isDone = true;
        }

        private void DocumentCompletedFully(object sender, WebBrowserDocumentCompletedEventArgs args)
        {
            var webBrowser = (WebBrowser)sender;
            AuditLogger.LogAuditEvent("webBrowser:" + webBrowser.ProductName + " " + webBrowser.ProductVersion);
            AuditLogger.LogAuditEvent("Thumbnail web browser status:" + webBrowser.StatusText);
            var bitmap = new Bitmap(BrowserWidth, BrowserHeight);
            var bitmapRect = new Rectangle(0, 0, BrowserWidth, BrowserHeight);
            webBrowser.DrawToBitmap(bitmap, bitmapRect);
            _mBitmap = bitmap;
            AuditLogger.LogAuditEvent("Thumbnail completed");
            AuditLogger.LogAuditEvent("Thumbnail web browser status:" + webBrowser.StatusText);
        }
    }
}



