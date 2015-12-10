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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Gov.Hhs.Cdc.ThumbnailProvider.BLL;

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

namespace Gov.Hhs.Cdc.ThumbnailProvider
{
    public class ImageScale
    {
        public bool ConstrainPorportions = true;
        private int _height = -1;
        private int _width = -1;
        private Bitmap mySource;
        private ImageFormat imageFormat = ImageFormat.Png;

        /// <summary>
        /// Get image from image URI
        /// </summary>
        /// <param name="uri">URI to an image</param>
        /// <param name="format">ex. ".jpg"</param>


        /// <summary>
        /// Get screenshot from html string
        /// </summary>

        public ImageScale(string argHtml, int argThumbWidth, int argThumbHeight, int argBrowserWidth, int argBrowserHeight, int argPause)
        {
            const string format = ".png";
            mySource = BrowserImage.GetHtmlThumbnail(argHtml, argBrowserWidth, argBrowserHeight, argThumbWidth, argThumbHeight, argPause);
            if (mySource != null)
            {
                this._height = argThumbHeight;
                this._width = argThumbWidth;
            }
            this.imageFormat = ParseImageFormat(format);
        }

        /// <summary>
        /// Get screenshot from html string
        /// </summary>

        public ImageScale(Uri argUri, int argThumbWidth, int argThumbHeight, int argBrowserWidth, int argBrowserHeight, int argPause)
        {
            const string format = ".png";
            mySource = BrowserImage.GetUrlThumbnail(argUri, argBrowserWidth, argBrowserHeight, argThumbWidth, argThumbHeight, argPause);
            if (mySource != null)
            {
                this._height = argThumbHeight;
                this._width = argThumbWidth;
            }
            this.imageFormat = ParseImageFormat(format);
        }


        /// <summary>
        /// Image width in pixels.
        /// By default ConstrainPorportions is true and this will also affect the height.
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                int v = 1;
                if (value > 0)
                {
                    v = value;
                }

                if (ConstrainPorportions)
                {
                    _height = Convert.ToInt32(_height * (Convert.ToDouble(v) / Convert.ToDouble(_width)));
                }
                _width = v;
            }
        }
        /// <summary>
        /// Image height in pixels.
        /// By default ConstrainPorportions is true and this will also affect the width.
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                int v = 1;
                if (value > 0)
                {
                    v = value;
                }

                if (ConstrainPorportions)
                {
                    _width = Convert.ToInt32((float)_width * (float)((float)v / (float)this._height));
                }

                _height = v;
            }
        }



        /// <summary>
        /// Save image to stream
        /// </summary>
        /// <param name="stream">ouput stream</param>
        /// <param name="quality">jpeg quality</param>
        public void SaveFile(Stream stream, int quality, Rectangle? croppingRectangle)
        {
            //Create an EncoderParameters collection to contain the
            //parameters that control the dest format's encoder
            EncoderParameters destEncParams = new EncoderParameters(1);

            //Use quality parameter
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            destEncParams.Param[0] = qualityParam;

            try
            {
                if (croppingRectangle != null)
                {
                    Rectangle rc = (Rectangle)croppingRectangle;
                    using (Bitmap target = new Bitmap(rc.Width, rc.Height))
                    {

                        using (Graphics g = Graphics.FromImage(target))
                        {
                            g.DrawImage(mySource, new Rectangle(0, 0, target.Width, target.Height),
                                             rc,
                                             GraphicsUnit.Pixel);

                        }

                        mySource = (Bitmap)target.Clone();
                    }

                    // at this point mysource = cropped image from full size browser

                }


                ResizeImage(stream, mySource, this._width, this._height);

            }
            catch (Exception)
            {
                throw new Exception("Could not scale image stream");
            }
        }

        private static void ResizeImage(Stream stream, Bitmap image, int outputWidth, int outputHeight)
        {

            int originalWidth = image.Width;
            int originalHeight = image.Height;

            Image thumbnail = new Bitmap(outputWidth, outputHeight);
            Graphics graphic = System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)outputWidth / (double)originalWidth;
            double ratioY = (double)outputHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((outputWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((outputHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            thumbnail.Save(stream, info[1], encoderParameters);
        }


        /// <summary>
        /// Get the ImageFormat type from a file extenstion string
        /// </summary>
        /// <param name="fmt"></param>
        /// <returns></returns>
        private static ImageFormat ParseImageFormat(string fmt)
        {
            switch (fmt.ToLower())
            {
                case ".jpg":
                    return ImageFormat.Jpeg;
                case ".gif":
                    return ImageFormat.Gif;
                case ".png":
                    return ImageFormat.Png;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".tiff":
                case ".tif":
                    return ImageFormat.Tiff;
                case ".emf":
                    return ImageFormat.Emf;
                default:
                    throw new Exception("Unsupported image format");
            }
        }

        public void Dispose()
        {
            this.mySource.Dispose(); // release source bitmap.
        }

    }

}
