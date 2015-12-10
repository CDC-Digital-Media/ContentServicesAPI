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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public enum FormatType
    {
        json, jsonp, xml, jpg, jpeg, png, html, rss, atom, itunes
    }

    public sealed class QueryParams
    {
        public QueryParams()
        {
            this.PageNumber = 0;
            this.Offset = 0;
            this.Format = FormatType.json;

            this.ShowChild = false;
            this.Scale = 1;
            this.Height = 0;
            this.Width = 0;
            this.BrowserHeight = 0;
            this.BrowserWidth = 0;

            this.CropX = 0;
            this.CropY = 0;
            this.CropW = 0;
            this.CropH = 0;
            this.Pause = 0;
        }

        public bool IsKindOfFeedFormatType
        {
            get
            {
                return 
                    this.Format == FormatType.rss ||
                    this.Format == FormatType.atom ||
                    this.Format == FormatType.itunes;
            }
        }


        //public int TimeToLive { get; set; }
        //public int TimeToWait { get; set; }
        public int Offset { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public FormatType Format { get; set; }
        public string ApiKey { get; set; }
        public string ResultSetId { get; set; }

        public string CacheKey { get; set; }

        public List<string> MediaType { get; set; }
        public List<string> Campaign { get; set; }
        public List<string> Title { get; set; }
        public string Url { get; set; }

        public bool ShowChild { get; set; }
        public int ShowChildLevel { get; set; }
        public int ShowParentLevel { get; set; }
        public double Scale { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BrowserWidth { get; set; }
        public int BrowserHeight { get; set; }
        public int CropX { get; set; }
        public int CropY { get; set; }
        public int CropW { get; set; }
        public int CropH { get; set; }
        public int Pause { get; set; }
        public string ApiRoot { get; set; }
        public string WebRoot { get; set; }

        public bool HasResultSetIdAndPagingOrOffset
        {
            get { return !string.IsNullOrEmpty(ResultSetId) && PageSize > 0 && (PageNumber > 0 || Offset > 0); }
        }

        public bool HasResultSetIdAndIsPaged
        {
            get
            {
                return !string.IsNullOrEmpty(ResultSetId) && PageSize > 0 && PageNumber > 0;
            }
        }

        public bool HasResultSetIdAndOffset
        {
            get
            {
                return !string.IsNullOrEmpty(ResultSetId) && PageSize > 0 && Offset > 0;
            }
        }

        public Paging GetPaging()
        {
            if (PageSize > 0 && (PageNumber > 0 || Offset > 0))
                return new Paging(PageSize, PageNumber, Offset);
            else
                return null;
        }
    }
}
