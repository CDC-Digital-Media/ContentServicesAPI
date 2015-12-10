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
using System.IO;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.ThumbnailProvider.Model;

namespace Gov.Hhs.Cdc.ThumbnailProvider
{
    public class CsThumbnailProvider
    {
        protected static IObjectContextFactory ObjectContextFactory { get; set; }

        public static void Inject(IObjectContextFactory objectContextFactory)
        {
            ObjectContextFactory = objectContextFactory;
        }

        #region Public

        public byte[] GenerateDynamicThumbnail(ContentThumbnail thumbnail, string webRoot, int? browserWidth,
            int? browserHeight, int? outputWidth, int? outputHeight, int? pause, Rectangle? croppingRectangle,
            out ValidationMessages validationMessages)
        {
            AuditLogger.LogAuditEvent("Thumbnail: Generating Dynamic Thumbnail");

            validationMessages = new ValidationMessages();
            thumbnail.WebRoot = webRoot;

            thumbnail.Width = outputWidth != null && (int)outputWidth > 0 ? (int)outputWidth : thumbnail.Width;
            thumbnail.Height = outputHeight != null && (int)outputHeight > 0 ? (int)outputHeight : thumbnail.Height;
            thumbnail.BrowserWidth = browserWidth != null && (int)browserWidth > 0 ? (int)browserWidth : thumbnail.BrowserWidth;
            thumbnail.BrowserHeight = browserHeight != null && (int)browserHeight > 0 ? (int)browserHeight : thumbnail.BrowserHeight;
            thumbnail.Pause = pause != null && (int)pause > 0 ? (int)pause : 1;

            thumbnail.CroppingRectangle = croppingRectangle;

            Media_Media mm = GetMediaObject(thumbnail.MediaId);
            if (mm == null)
            {
                AuditLogger.LogAuditEvent("Thumbnail: Media not found");
                validationMessages.AddError("MediaId", "Media object was not found");
                return new byte[0];
            }
            else
            {
                string content = "";
                Uri url = null;
                GetHtmlToRender(thumbnail, mm, out content, out url);

                if (string.IsNullOrEmpty(content))
                {
                    return new byte[0];
                }
                ImageScale imageScale = url == null ? thumbnail.GetNewImageScale(content) : thumbnail.GetNewImageScale(url);
                byte[] newThumbnail = GetThumbnailImage(thumbnail, imageScale);
                return newThumbnail;
            }
        }

        public ValidationMessages GenerateAndSaveDefaultThumbnail(ContentThumbnail thumbnailDetail)
        {
            ValidationMessages messages = new ValidationMessages();

            Media_Media mediaObject = GetMediaObject(thumbnailDetail.MediaId);
            thumbnailDetail.SetDefaultsForType(mediaObject);

            string content = "";
            Uri url = null;

            GetHtmlToRender(thumbnailDetail, mediaObject, out content, out url);
            ThumbnailObject thumbnailObject = new ThumbnailObject()
            {
                MediaId = thumbnailDetail.MediaId,
                Thumbnail = GetThumbnailImage(thumbnailDetail, thumbnailDetail.GetNewImageScale(content))
            };

            messages.Add(SaveThumbnail(thumbnailObject));
            return messages;
        }

        public ThumbnailObject GetThumbnail(int mediaId)
        {
            using (var context = CsMediaProvider.ObjectContextFactory.Create())
            {

                var storage = StorageCtl.GetThumbnailStorage(context as MediaObjectContext, mediaId).ToList();
                var stored = storage.FirstOrDefault();
                if (stored == null)
                {
                    var mediaTypeThumb = MediaCtl.GetThumbnailForMediaType(context as MediaObjectContext, mediaId);
                    return new ThumbnailObject
                    {
                        MediaId = mediaId,
                        Thumbnail = mediaTypeThumb
                    };
                }
                return new ThumbnailObject
                {
                    MediaId = mediaId,
                    Thumbnail = stored.ByteStream,
                    Height = stored.Height.HasValue ? stored.Height.Value : 0,
                    Width = stored.Width.HasValue ? stored.Width.Value : 0,
                    Name = stored.Name
                };
            }
        }

        public ValidationMessages SaveThumbnail(ThumbnailObject thumbnail)
        {
            var messages = new ValidationMessages();

            using (var context = CsMediaProvider.ObjectContextFactory.Create())
            {
                var persistedThumbnail = StorageCtl.GetThumbnailStorage(context as MediaObjectContext, thumbnail.MediaId, forUpdate: true).FirstOrDefault();

                var storage = new StorageObject
                {
                    Name = thumbnail.Name,
                    MediaId = thumbnail.MediaId,
                    ByteStream = thumbnail.Thumbnail,
                    FileExtension = FormatType.png.ToString(),
                    Height = thumbnail.Height
                    ,
                    Width = thumbnail.Width
                };

                if (persistedThumbnail == null)
                {

                    StorageCtl.Create(context, storage).AddToDb();
                }
                else
                {
                    StorageCtl.Update(context, persistedThumbnail, storage, enforceConcurrency: false);
                }
                context.SaveChanges();
            }

            return messages;
        }

        public ValidationMessages DeleteThumbnail(int mediaId)
        {
            ValidationMessages messages = new ValidationMessages();
            using (var context = CsMediaProvider.ObjectContextFactory.Create())
            {
                var persistedThumbnail = StorageCtl.GetThumbnailStorage(context as MediaObjectContext, mediaId, forUpdate: true).FirstOrDefault();

                if (persistedThumbnail == null)
                {
                    messages.AddWarning("Thumbnail", "Thumbnail does not exist");
                }
                else
                {
                    StorageCtl.Delete(context, persistedThumbnail);
                }
                context.SaveChanges();
            }
            return messages;
        }

        private static Media_Media GetMediaObject(int mediaId)
        {
            using (ThumbnailObjectContext context = (ThumbnailObjectContext)ObjectContextFactory.Create())
            {
                IQueryable<Media_Media> mediaItem = from m in context.ThumbnailDbEntities.Media_Media
                                                    where m.MediaId == mediaId
                                                    select m;
                return mediaItem.FirstOrDefault();
            }
        }


        #endregion

        #region Utility



        private void GetHtmlToRender(ContentThumbnail thumbnail, Media_Media mm, out string content, out Uri url)
        {

            content = "<!DOCTYPE html><html><head><meta http-equiv='X-UA-Compatible'>";

            if (mm.MediaTypeCode.ToLower() != "widget")
            {
                content += "<script src='" + thumbnail.WebRoot + "/js/libs/jquery.js' type='text/javascript'></script>";
                content += "<style type='text/css'> body {padding: 0px; margin: 0px; text-align:center;}</style>";
            }


            content += "</head><body>";

            url = null;

            switch (mm.MediaTypeCode.ToLower())
            {
                case MediaTypeConstants.PodcastSeries:
                    content += "<script src='" + thumbnail.WebRoot + "/Scripts/Marketplace.PodcastChannel_0.1.js' type='text/javascript'></script>";
                    content += "<div class='CDCPodcastSeries_00'></div><script language='javascript' type='text/javascript'>$('.CDCPodcastSeries_00').podcastChannel({";
                    content += "mediaId: " + mm.MediaId.ToString() + ",";
                    content += "apiRoot: '" + thumbnail.APIRoot + "',";
                    content += "showImage: true})</script>";
                    break;

                case MediaTypeConstants.Widget:

                    content += mm.EmbedCode;
                    break;

                case MediaTypeConstants.HtmlContent:
                    content += "<style type='text/css'> body {font-family: Segoe UI, Arial;padding: 0px 10px 10px 10px; text-align:left; overflow:hidden;} .featuretext{line-height:40px;} img {margin:0px 20px 20px 0px; border:1px solid black; float:left;} </style>";
                    content += mm.EmbedCode;
                    break;

                case MediaTypeConstants.eCard:


                    content += "<div style='width: 580px;' id='flashALTcontent'>";
                    content += "<object id='Banner' classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000' width='580' height='400'>";
                    content += "<PARAM NAME='_cx' VALUE='15345'><PARAM NAME='_cy' VALUE='10583'>";
                    content += "<PARAM NAME='FlashVars' VALUE=''>";
                    content += "<PARAM NAME='Movie' VALUE='" + mm.SourceUrl + "'>";
                    content += "<PARAM NAME='Src' VALUE='" + mm.SourceUrl + "'>";
                    content += "<PARAM NAME='WMode' VALUE='Window'>";
                    content += "<PARAM NAME='Play' VALUE='1'>";
                    content += "<PARAM NAME='Loop' VALUE='-1'><PARAM NAME='Quality' VALUE='High'>";
                    content += "<PARAM NAME='SAlign' VALUE=''>";
                    content += "<PARAM NAME='Menu' VALUE='-1'>";
                    content += "<PARAM NAME='Base' VALUE=''>";
                    content += "<PARAM NAME='AllowScriptAccess' VALUE='sameDomain'>";
                    content += "<PARAM NAME='Scale' VALUE='ExactFit'>";
                    content += "<PARAM NAME='DeviceFont' VALUE='0'>";
                    content += "<PARAM NAME='EmbedMovie' VALUE='0'>";
                    content += "<PARAM NAME='BGColor' VALUE=''>";
                    content += "<PARAM NAME='SWRemote' VALUE=''>";
                    content += "<PARAM NAME='MovieData' VALUE=''>";
                    content += "<PARAM NAME='SeamlessTabbing' VALUE='1'>";
                    content += "<PARAM NAME='Profile' VALUE='0'>";
                    content += "<PARAM NAME='ProfileAddress' VALUE=''>";
                    content += "<PARAM NAME='ProfilePort' VALUE='0'>";
                    content += "<PARAM NAME='AllowNetworking' VALUE='all'>";
                    content += "<PARAM NAME='AllowFullScreen' VALUE='false'>";
                    content += "<PARAM NAME='AllowFullScreenInteractive' VALUE='false'>";
                    content += "<PARAM NAME='IsDependent' VALUE='0'>";
                    content += "<param name='movie' value='" + mm.SourceUrl + "'>";
                    content += "<param name='quality' value='best'><param name='scale' value='exactfit'>";
                    content += "<param name='allowScriptAccess' value='sameDomain'>";
                    content += "<param name='align' value='top'>";
                    content += "<param name='type' value='application/x-shockwave-flash'>";
                    content += "<param name='wmode' value='window'></object></div>";

                    break;

                case MediaTypeConstants.Image:
                case MediaTypeConstants.Button:
                case MediaTypeConstants.Badge:
                case MediaTypeConstants.Infographic:
                    content += "<style type='text/css'> body {overflow:hidden;}</style>";
                    //Rectangle r = ScaleImage((int)mm.Width, (int)mm.Height, 700, 380);

                    string style = "";
                    if ((int)mm.Width > (int)mm.Height)
                    {
                        style = "width:100%; height:auto;";
                    }
                    else
                    {
                        style = "width:auto; height:100%;";
                    }

                    content += "<img src='" + mm.SourceUrl + "' style='" + style + "'>";
                    break;

                case MediaTypeConstants.PDF:
                    //content += "TESTING";
                    //content += "<object data='" + mm.SourceUrl + "' type='application/pdf' width='100%' height='100%'>";

                    content += "<iframe src='http://docs.google.com/gview?url=" + mm.SourceUrl + "&embedded=true' style='width:680px;height:380px;' frameborder='0'></iframe>";

                    break;

                case MediaTypeConstants.Video:

                    if (mm.SourceUrl.ToLower().IndexOf("youtube") > -1)
                    {
                        // source: http://www.youtube.com/embed/byeckM-zMb4
                        // image:  http://img.youtube.com/vi/byeckM-zMb4/hqdefault.jpg
                        int startIndex = mm.SourceUrl.IndexOf("/embed/");
                        if (startIndex > -1)
                        {
                            startIndex += 7;
                            int endIndex = mm.SourceUrl.IndexOf("?", startIndex);
                            string youtubeId = "";
                            if (endIndex > -1)
                            {
                                youtubeId = mm.SourceUrl.Substring(startIndex, endIndex);
                            }
                            else
                            {
                                youtubeId = mm.SourceUrl.Substring(startIndex);
                            }
                            content += "<style type='text/css'> body {overflow:hidden;}</style>";
                            content += "<img src='http://img.youtube.com/vi/" + youtubeId + "/hqdefault.jpg'>";
                        }

                    }
                    else
                    {
                        content += "<style type='text/css'> body {overflow:hidden;}</style>";
                        content += "<iframe width='420' height='315' src='" + mm.SourceUrl + "' frameborder='0' allowfullscreen border=1></iframe>";
                    }

                    break;
            }

            content += "</body></html>";

        }

        //public static Rectangle ScaleImage(int startWidth, int startHeight, int maxWidth, int maxHeight)
        //{
        //    var ratioX = (double)maxWidth / startWidth;
        //    var ratioY = (double)maxHeight / startHeight;
        //    var ratio = Math.Min(ratioX, ratioY);

        //    var newWidth = (int)(startWidth * ratio);
        //    var newHeight = (int)(startHeight * ratio);

        //    return new Rectangle(0, 0, newWidth, newHeight);
        //}


        /// <summary>
        /// Initialize image by decoded html
        /// Currently applies (by media id) to 
        /// ** html  
        /// ** widgets 
        /// </summary>
        /// <param name="htmlItem"></param>
        /// <returns></returns>

        private static byte[] GetThumbnailImage(ContentThumbnail thumbnailDetail, ImageScale imageScaler)
        {
            var memoryStream = new MemoryStream();
            imageScaler.SaveFile(memoryStream, 100, thumbnailDetail.CroppingRectangle);
            return memoryStream.ToArray();
        }

        private static string GetHTMLContent(int mediaId)
        {
            try
            {
                var tresults = new CsMediaValidationProvider().ExtractContent(mediaId);
                if (tresults != null && tresults.ExtractedDetail != null)
                {
                    return tresults.ExtractedDetail.Data;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        #endregion

    }
}
