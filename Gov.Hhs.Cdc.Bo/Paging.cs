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

namespace Gov.Hhs.Cdc.Bo
{
    public class Paging
    {
        public bool IsPaged { get; set; }
        public int Offset { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public bool ReturnMultiplePages { get; set; }
        public bool ReturnAllPages { get; set; }
        public int LastPageNumber { get; set; }


        public int FirstRecordNumber
        {
            get
            {
                return Offset > 0 ? Offset + 1 : ((PageNumber - 1) * PageSize) + 1;
            }
        }

        public int LastRecordNumber
        {
            get
            {
                return Offset > 0 ? (LastPageNumber * PageSize) + Offset : LastPageNumber * PageSize;
            }
        }

        public int RecordsToSkip
        {
            get
            {
                return FirstRecordNumber - 1;
            }
        }

        public int NumberOfRecordsToSelect
        {
            get
            {
                return ReturnMultiplePages ? LastRecordNumber - RecordsToSkip : PageSize;
            }
        }

        public Paging()
        {
        }

        public Paging(int pageSize, int pageNumber)
        {
            IsPaged = true;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public Paging(Paging original)
        {
            this.IsPaged = original.IsPaged;
            this.PageSize = original.PageSize;
            this.PageNumber = original.PageNumber;
            this.ReturnMultiplePages = original.ReturnMultiplePages;
            this.ReturnAllPages = original.ReturnAllPages;
            this.LastPageNumber = original.LastPageNumber;
            this.Offset = original.Offset;
        }

        public Paging(int pageSize, int pageNumber, int offset)
        {
            IsPaged = true;
            PageSize = pageSize;
            PageNumber = pageNumber;
            Offset = offset > 0 ? offset + ((PageNumber - 1) * pageSize) : offset;
        }

        public Paging WithAllPages()
        {
            return new Paging(this)
            {
                ReturnAllPages = true
            };
        }

        public Paging WithPages(int startPage, int throughPage)
        {
            return new Paging(this)
            {
                IsPaged = true,
                PageNumber = startPage,
                ReturnMultiplePages = true,
                ReturnAllPages = false,
                LastPageNumber = throughPage
            };
        }

        public Paging WithPage(int pageNumber)
        {
            return new Paging(this)
            {
                IsPaged = true,
                ReturnMultiplePages = false,
                ReturnAllPages = false,
                PageNumber = pageNumber
            };
        }

    }
}
