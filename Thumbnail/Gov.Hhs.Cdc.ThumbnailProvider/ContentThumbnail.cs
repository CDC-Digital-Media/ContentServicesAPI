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
using System.Web;
using Gov.Hhs.Cdc.ThumbnailProvider.Model;

namespace Gov.Hhs.Cdc.ThumbnailProvider
{
    public class ContentThumbnail
    {
 
        public int MediaId {get; set;}

        public string WebRoot{get; set;}


        public string APIRoot{get; set;}


        public bool GenerateDynamic{get; set;}


        public int Width{get; set;}


        public int Height{get; set;}


        public int BrowserWidth{get; set;}


        public int BrowserHeight{get; set;}


        public int CropX{get; set;}


        public int CropY{get; set;}


        public int CropW{get; set;}


        public int CropH{get; set;}


        public int Pause{get; set;}


        public Rectangle? CroppingRectangle {get; set;}


        //Add uri builder for relative uri
        public string ContentAsUri
        {
            set { _contentAsUri = value; }
        }





        private string _contentAsUri = string.Empty;

  
        public ContentThumbnail(int mediaId)
        {
            MediaId = mediaId;
            Width = 155;
            Height = 84;
            BrowserWidth = 800;
            BrowserHeight = 400;
            Pause = 2000;
            CroppingRectangle = null;
        }

        public void SetDefaultsForType(Media_Media m)
        {

            switch (m.MediaTypeCode.ToLower())
            {
                case MediaTypeConstants.PodcastSeries:
                    BrowserWidth = 950;
                    BrowserHeight = 600;
                    break;

                case MediaTypeConstants.Widget:
                    Pause = 5000;
                    BrowserWidth = 700;
                    BrowserHeight = 400;
                    break;

                case MediaTypeConstants.HtmlContent:
                    BrowserWidth = 700;
                    BrowserHeight = 400;
                    break;

                case MediaTypeConstants.eCard:
                    BrowserWidth = 580;
                    BrowserHeight = 400;
                    CroppingRectangle = new Rectangle(53, 75, 470, 266);
                    Pause = 4000;
                    break;

                case MediaTypeConstants.Image:
                    BrowserHeight = 438;
                    BrowserWidth = 660;
                    break;

                case MediaTypeConstants.Button:
                    BrowserHeight = 252;
                    BrowserWidth = 450;
                    break;

                case MediaTypeConstants.Badge:
                    BrowserHeight = 252;
                    BrowserWidth = 400;
                    break;

                case MediaTypeConstants.Infographic:
                    BrowserHeight = 600;
                    BrowserWidth = 800;
                    break;

                case MediaTypeConstants.Video:
                    BrowserWidth = 320;
                    BrowserHeight = 180;
                    Pause = 3000;
                    break;

                case MediaTypeConstants.PDF:
                    BrowserWidth = 800;
                    BrowserHeight = 600;
                    Pause = 4000;
                    break;
            }

        }


        public ImageScale GetNewImageScale(string content)
        {
            return new ImageScale(content, Width, Height, BrowserWidth, BrowserHeight, Pause);
        }

        public ImageScale GetNewImageScale(Uri url)
        {
            return new ImageScale(url, Width, Height, BrowserWidth, BrowserHeight, Pause);
        }
    }



}
