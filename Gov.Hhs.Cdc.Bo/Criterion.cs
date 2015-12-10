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
    public class Criterion
    {

        public string Code { get; set; }
        public bool IsFiltered { get; set; }
        public string Value { get; set; }
        public List<string> Values { get; set; }

        public string DateType { get; set; }
        public string DateOperator { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }


        public Criterion(string code, string value)
        {
            Code = code;
            IsFiltered = true;
            Value = value;
            Values = null;
        }

        public Criterion(string code, bool value)
        {
            Code = code;
            IsFiltered = true;
            Value = value ? "true" : "false";
            Values = null;
        }

        public Criterion(string code, int value)
        {
            Code = code;
            IsFiltered = true;
            Value = value.ToString();
            Values = null;
        }

        public Criterion(string code, IEnumerable<string> values)
        {
            Code = code;
            IsFiltered = true;
            Values = values.ToList();
            Value = null;
        }

        public Criterion(string code, IEnumerable<int> values)
        {
            Code = code;
            IsFiltered = true;
            Values = values.Select(v => v.ToString()).ToList();
            Value = null;
        }
        public Criterion(string code, IEnumerable<long> values)
        {
            Code = code;
            IsFiltered = true;
            Values = values.Select(v => v.ToString()).ToList();
            Value = null;
        }

        public Criterion(string code, DateTime? startDate, DateTime? endDate)
        {
            Code = code;
            IsFiltered = true;
            DateType = "DateRange";
            DateOperator = "";
            Date1 = startDate.ToString();
            Date2 = endDate.ToString();
        }

        public Criterion(string code, DateTime? theDate, SingleDateType.SingleDateOperator singleDateOperator)
        {
            Code = code;
            IsFiltered = true;
            DateType = "SingleDate";
            DateOperator = singleDateOperator.ToString();
            Date1 = theDate.ToString();
        }

        public Criterion(string code, DateTime? startDate)
        {
            Code = code;
            IsFiltered = true;
            DateType = "SingleDate";
            DateOperator = "";
            Date1 = startDate.ToString();
        }

        //public Criterion(string code, DateTime? theDate, string dateOperator)
        //{
        //    Code = code;
        //    IsFiltered = true;
        //    DateType = "SingleDate";
        //    DateOperator = dateOperator;
        //    Date1 = theDate.ToString();
        //}

    }
}
