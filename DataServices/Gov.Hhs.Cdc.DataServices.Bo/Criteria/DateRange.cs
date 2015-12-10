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

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class DateRange
    {
        private DateTime beginDate;
        public DateTime BeginDate
        {
            get
            {
                return beginDate;
            }
            set
            {
                beginDate = value;
            }

        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                endDate = value;
            }

        }

        public bool HasBeginDate { get; set; }
        public bool HasEndDate { get; set; }

        public DateRange(DateTime? beginDate, DateTime? endDate)
        {
            if (beginDate == null)
            {
                HasBeginDate = false;
            }
            else
            {
                HasBeginDate = true;
                BeginDate = (DateTime)beginDate;
            }
            if (endDate == null)
            {
                HasEndDate = false;
            }
            else
            {
                HasEndDate = true;
                EndDate = (DateTime)endDate;
            }
        }

        
        
        /// <summary>
        /// Our dates have no time.  To properly compare with DB dates that may have time, we need to compare
        /// with the date at 00:00 hours and the next day at 00:00 hours.
        /// </summary>
        /// <returns></returns>
        public DateTime BeginningOfDayAfterEndDate()
        {
            return EndDate.AddDays(1);
        }
    }
}
