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

using System.Runtime.Serialization;
using System.Web.Script.Serialization;

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    [DataContract(Name = "pagination")]
    public sealed class SerialPagination
    {
        [DataMember(Name = "total", Order = 1, EmitDefaultValue = true)]
        public int total { get; set; }

        [DataMember(Name = "count", Order = 2, EmitDefaultValue = true)]
        public int count { get; set; }

        [DataMember(Name = "max", Order = 3, EmitDefaultValue = true)]
        public int max { get; set; }

        [DataMember(Name = "offset", Order = 4, EmitDefaultValue = true)]
        public int offset { get; set; }

        [DataMember(Name = "pageNum", Order = 5, EmitDefaultValue = true)]
        public int pageNum { get; set; }

        [DataMember(Name = "totalPages", Order = 6, EmitDefaultValue = true)]
        public int totalPages { get; set; }             //Number of Pages

        [DataMember(Name = "sort", Order = 7, EmitDefaultValue = true)]
        public string sort { get; set; }

        [DataMember(Name = "previousUrl", Order = 8, EmitDefaultValue = true)]
        public string previousUrl { get; set; }

        [DataMember(Name = "currentUrl", Order = 9, EmitDefaultValue = true)]
        public string currentUrl { get; set; }

        [DataMember(Name = "nextUrl", Order = 10, EmitDefaultValue = true)]
        public string nextUrl { get; set; }

        public SerialPagination()
        {
        }

        public SerialPagination(ICallParser parser, Sorting sort, int total, int count, int max, int offset, int pageNumber, int totalPages, int currentId = 0, int parentId = 0, bool forExport = false)
        {
            this.total = total;
            this.count = count;
            this.max = max;
            this.offset = offset;
            this.pageNum = pageNumber;
            this.totalPages = totalPages;

            this.sort = GetSorting(sort);

            SetPagingUrls(parser, currentId, parentId, forExport);
        }

        private string GetSorting(Sorting sorting)
        {
            if (sorting == null || !sorting.IsSorted || sorting.SortColumns.Count == 0)
                return null;

            string sort = string.Empty;
            for (int i = 0; i < sorting.SortColumns.Count; i++)
                sort += string.Format("{0}{1},",
                    sorting.SortColumns[i].SortOrder == SortOrderType.Desc ? "-" : "", ApiColumnMap.GetKey(sorting.SortColumns[i].Column));

            return sort.TrimEnd(",");
        }

        private string GetApiQueryString(ICallParser parser)
        {
            string qs = RestHelper.ConstructCanonicalQueryStringFromDictionary(parser.RemoveDynamicParamsFromDictionaryForPaging());
            return qs;
        }

        private void SetPagingUrls(ICallParser parser, int currentId, int parentId, bool forExport = false)
        {
            //set meta url
            string mainUrl = ServiceUtility.GetCurrentUrlPath(true, null, forExport);

            if (forExport)
            {
                if (parentId > 0)
                {
                    mainUrl += "?parentid=" + parentId.ToString();
                }
                else
                {
                    if (currentId > 0)
                    {
                        mainUrl += currentId.ToString();
                    }
                }
            }

            if (mainUrl.IndexOf(".") == -1)
            {
                mainUrl += "." + parser.Query.Format.ToString();
            }
            string qs = GetApiQueryString(parser);

            this.currentUrl = ServiceUtility.HtmlEncodeByFormat(mainUrl + "?" + qs, parser.Query.Format.ToString()).TrimEnd('?');
            if (parser.Query.PageSize > 0 && (parser.Query.PageNumber > 0 || parser.Query.Offset >= 0))
            {
                this.previousUrl = CreatePreviousUrl(parser, mainUrl, qs).TrimEnd('?');
                this.nextUrl = CreateNextUrl(parser, mainUrl, qs).TrimEnd('?');
            }
        }

        private string CreatePreviousUrl(ICallParser parser, string urlPath, string urlQuery)
        {
            if (parser.Query.PageNumber == 0)
            {
                int prevOffset = (this.offset - this.max) < 0 ? 0 : this.offset - this.max;
                if (prevOffset == parser.Query.Offset)
                    return "";

                return urlPath + "?" + CreatePagingQueryString(parser, urlQuery, prevOffset);
            }
            else
            {
                int prevPageNum = this.pageNum - 1 == 0 ? this.pageNum : this.pageNum - 1;
                if (prevPageNum == this.pageNum)    // || (this.offset > 0 && prevPageNum != (CalculateDefaultPageNumber() - 1)))
                    return "";

                return urlPath + "?" + CreatePagingQueryString(parser, urlQuery, prevPageNum);
            }
        }

        private string CreateNextUrl(ICallParser parser, string urlPath, string urlQuery)
        {
            int nextPageNum = this.pageNum + 1 > this.totalPages ? this.totalPages : this.pageNum + 1;
            if (this.pageNum != this.totalPages && this.pageNum < this.totalPages)
            {
                if (parser.Query.PageNumber == 0)
                {
                    int nextOffset = (this.offset + this.max) > this.total ? this.total - max : (this.offset + this.max);
                    return urlPath + "?" + CreatePagingQueryString(parser, urlQuery, nextOffset);
                }
                else
                    return urlPath + "?" + CreatePagingQueryString(parser, urlQuery, nextPageNum);
            }
            else
                return "";
        }

        private string CreatePagingQueryString(ICallParser parser, string queryString, int pageNum)
        {
            string pageFormat, link = string.Empty;

            queryString = GetQueryStringForPaging(queryString, true);

            pageFormat = "{0}&{1}={2}";
            if (parser.Query.PageNumber == 0)
                queryString = string.Format(pageFormat, queryString, Param.PAGE_OFFSET, pageNum.ToString());
            else
                queryString = string.Format(pageFormat, queryString, Param.PAGE_NUMBER, pageNum.ToString());

            link = ServiceUtility.HtmlEncodeByFormat(GetQueryStringForPaging(queryString, false), parser.Query.Format.ToString());
            return link;
        }

        private string GetQueryStringForPaging(string queryString, bool removePageNum)
        {
            IDictionary<string, string> dict = RestHelper.CreateDictionary(queryString);

            if (removePageNum)
            {
                dict.Remove(Param.RESULT_SET_ID);
                dict.Remove(Param.PAGE_NUMBER);
                dict.Remove(Param.PAGE_OFFSET);

                return RestHelper.ConstructQueryStringFromDictionary(dict);
            }
            else
            {
                return RestHelper.ConstructCanonicalQueryStringFromDictionary(dict);
            }
        }

    }
}
