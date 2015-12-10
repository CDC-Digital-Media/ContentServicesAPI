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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsCaching;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.ThumbnailProvider;

namespace Gov.Hhs.Cdc.Api
{
    public static class ThumbnailHandler
    {
        public static void GetThumbnail(string id, ICallParser parser, IOutputWriter writer)
        {
            int mediaId = ServiceUtility.ParsePositiveInt(id);
            QueryParams queryParams = parser.Query;

            parser.Query.Format = FormatType.png;
            ValidationMessages messages = new ValidationMessages();
            ContentThumbnail contentThumbnail = BuildThumbnailDefinition(mediaId, parser);

            ThumbnailObject theThumbnail = ParamsPassedToGenerateNewThumbnail(queryParams) ?
                GenerateDynamicThumbnail(contentThumbnail.MediaId, contentThumbnail, queryParams, out messages) :
                GetThumbnailFromDatabase(parser, contentThumbnail.MediaId, queryParams);

            CacheThumbnail(theThumbnail, parser);
            if (writer != null)
            {
                var thumb = new byte[0];
                if (theThumbnail != null && theThumbnail.Thumbnail != null)
                {
                    thumb = theThumbnail.Thumbnail;
                }
                writer.Write(thumb);
            }
        }

        public static void UpdateThumbnail(string mediaId, ICallParser parser, IOutputWriter writer)
        {
            int id;
            ValidationMessages messages = new ValidationMessages();

            if (!int.TryParse(mediaId, out id))
            {
                messages.AddError("MediaId", "Invalid Media Id");
            }

            if (!messages.Errors().Any())
            {
                ContentThumbnail contentThumbnail = BuildThumbnailDefinition(id, parser);
                ThumbnailObject newThumbnail = GenerateDynamicThumbnail(id, contentThumbnail, parser.Query, out messages);
                messages.Add(ThumbnailProvider.SaveThumbnail(newThumbnail));
                CacheThumbnail(newThumbnail, parser);
                var response = GetThumbnailSerialResponse(newThumbnail, parser.Query);
                writer.Write(response, messages);
            }
            else
            {
                writer.Write(messages);
            }
        }

        public static void Delete(string mediaId, ICallParser parser, IOutputWriter writer)
        {
            int id;
            if (!int.TryParse(mediaId, out id))
            {
                writer.Write(ValidationMessages.CreateError("MediaId", "Invalid Media Id"));
                return;
            }

            ThumbnailProvider.DeleteThumbnail(id);
            DeleteThumbnailCache(parser.Query);
            writer.Write(new ValidationMessages());
        }

        private static bool ParamsPassedToGenerateNewThumbnail(QueryParams queryParams)
        {
            return queryParams.Width > 0 || queryParams.Height > 0 || queryParams.BrowserWidth > 0 || queryParams.BrowserHeight > 0;
        }

        private static CsThumbnailProvider ThumbnailProvider = new CsThumbnailProvider();

        private static ThumbnailObject GetThumbnailFromDatabase(ICallParser parser, int mediaId, QueryParams queryParams)
        {
            ThumbnailObject thumbnailObject = ThumbnailProvider.GetThumbnail(mediaId);
            //if scale parm then write to image, scale and get NEW byte string      
            if (!parser.IsPublicFacing && thumbnailObject.MediaThumbnailIsNull)
            {
                return null;
            }

            if (queryParams.Scale != 1)
            {
                thumbnailObject.Thumbnail = ScaleImage(thumbnailObject.Thumbnail, queryParams.Scale);
            }
            return thumbnailObject;
        }

        private static ThumbnailObject GenerateDynamicThumbnail(int mediaId, ContentThumbnail thumbnailDetail, QueryParams queryParams, out ValidationMessages messages)
        {
            Rectangle? newRectangle = null;
            if (queryParams.CropH > 0 && queryParams.CropW > 0 && queryParams.CropX >= 0 && queryParams.CropY >= 0)
            {
                newRectangle = new Rectangle(queryParams.CropX, queryParams.CropY, queryParams.CropW, queryParams.CropH);
            }

            byte[] newThumbnail = ThumbnailProvider.GenerateDynamicThumbnail(thumbnailDetail, queryParams.WebRoot, queryParams.BrowserWidth,
                queryParams.BrowserHeight, queryParams.Width, queryParams.Height, queryParams.Pause, newRectangle, out messages);
            return new ThumbnailObject()
            {
                MediaId = mediaId,
                Thumbnail = newThumbnail,
                Height = queryParams.Height,
                Width = queryParams.Width,
            };
        }

        private static ContentThumbnail BuildThumbnailDefinition(int mediaId, ICallParser parser)
        {
            var tn = new ContentThumbnail(mediaId);

            tn.BrowserHeight = parser.ParseInt(Param.BROWSER_HEIGHT) ?? tn.BrowserHeight;
            tn.BrowserWidth = parser.ParseInt(Param.BROWSER_WIDTH) ?? tn.BrowserWidth;
            tn.Height = parser.ParseInt(Param.HEIGHT) ?? tn.Height;
            tn.Width = parser.ParseInt(Param.WIDTH) ?? tn.Width;
            tn.CropX = parser.ParseInt(Param.CROP_X) ?? tn.CropX;
            tn.CropY = parser.ParseInt(Param.CROP_Y) ?? tn.CropY;
            tn.CropW = parser.ParseInt(Param.CROP_W) ?? tn.CropW;
            tn.CropH = parser.ParseInt(Param.CROP_H) ?? tn.CropH;

            if (parser.ParamDictionary.ContainsKey(Param.APIROOT))
            {
                tn.APIRoot = parser.ParamDictionary[Param.APIROOT];
            }

            if (parser.ParamDictionary.ContainsKey(Param.WEBROOT))
            {
                tn.WebRoot = parser.ParamDictionary[Param.WEBROOT];
            }

            tn.Pause = parser.ParseInt(Param.PAUSE) ?? tn.Pause;
            return tn;
        }

        private static byte[] ScaleImage(byte[] image, double scale)
        {
            byte[] ReturnedThumbnail;

            using (MemoryStream StartMemoryStream = new MemoryStream(),
                                NewMemoryStream = new MemoryStream())
            {
                // write the string to the stream  
                StartMemoryStream.Write(image, 0, image.Length);

                // create the start Bitmap from the MemoryStream that contains the image  
                Bitmap startBitmap = new Bitmap(StartMemoryStream);

                // set thumbnail height and width proportional to the original image.  
                int newHeight;
                int newWidth;

                newHeight = (int)((float)startBitmap.Height * scale);
                newWidth = (int)((float)startBitmap.Width * scale);

                // create a new Bitmap with dimensions for the thumbnail.  
                Bitmap newBitmap = new Bitmap(newWidth, newHeight);

                // Copy the image from the START Bitmap into the NEW Bitmap.  
                // This will create a thumnail size of the same image.  
                newBitmap = ResizeImage(startBitmap, newWidth, newHeight);

                // Save this image to the specified stream in the specified format.  
                newBitmap.Save(NewMemoryStream, ImageFormat.Png);

                // Fill the byte[] for the thumbnail from the new MemoryStream.  
                ReturnedThumbnail = NewMemoryStream.ToArray();
            }

            // return the resized image as a string of bytes.  
            return ReturnedThumbnail;
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        }

        private static SerialResponse GetThumbnailSerialResponse(ThumbnailObject theThumbnail, QueryParams queryParams)
        {
            if (theThumbnail == null)
            {
                return null;
            }
            SerialThumbnail serialThumbnail = new SerialThumbnail()
            {
                thumbnail = theThumbnail.Thumbnail,
                rowVersion = theThumbnail.RowVersion.ToBase64String(),
            };
            return new SerialResponse(
                new List<SerialThumbnail>() { serialThumbnail }, queryParams);
        }

        private static void CacheThumbnail(ThumbnailObject thumbnailObject, ICallParser parser)
        {
            HttpContext.Current.Response.Clear();

            if (thumbnailObject != null && thumbnailObject.Thumbnail != null) //image exists in db
            {
                SerialResponse sr = GetThumbnailSerialResponse(thumbnailObject, parser.Query);
                sr.results = thumbnailObject.Thumbnail;
            }
        }

        private static void DeleteThumbnailCache(QueryParams queryParams)
        {
            HttpContext.Current.Response.Clear();
            try
            {
                CacheManager.Remove(queryParams.CacheKey);
            }
            catch (Exception)
            {
                //Ignore error if removing from cache
            }

        }

    }
}
