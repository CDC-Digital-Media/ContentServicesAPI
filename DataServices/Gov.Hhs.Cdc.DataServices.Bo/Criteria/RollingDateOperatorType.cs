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
    public class RollingDateOperatorType
    {
        public enum RollingDateOperator { Today, Yesterday, Tomorrow, ThisWeek, LastWeek, NextWeek, ThisMonth, LastMonth, NextMonth, ThisQuarter, LastQuarter, NextQuarter, ThisYear, LastYear, NextYear, Next30Days, Next60Days, Next90Days };
        public RollingDateOperator Type { get; set; }
        //public string Code { get { return Type.ToString(); } }
        public string Name { get; set; }
        public bool InFuture { get; set; }
        public bool InPast { get; set; }

        public RollingDateOperatorType()
        {
            InFuture = false;
            InPast = false;
        }

        public static List<RollingDateOperatorType> TypeList()
        {
            List<RollingDateOperatorType> list = new List<RollingDateOperatorType>();
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.Today, Name = "Today" });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.Yesterday, Name = "Yesterday", InPast = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.Tomorrow, Name = "Tomorrow", InFuture = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.ThisWeek, Name = "This Week" });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.LastWeek, Name = "Last Week", InPast = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.NextWeek, Name = "Next Week", InFuture = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.ThisMonth, Name = "This Month" });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.LastMonth, Name = "Last Month", InPast = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.NextMonth, Name = "Next Month", InFuture = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.ThisQuarter, Name = "This Quarter" });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.LastQuarter, Name = "Last Quarter", InPast = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.NextQuarter, Name = "Next Quarter", InFuture = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.ThisYear, Name = "This Year" });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.LastYear, Name = "Last Year", InPast = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.NextYear, Name = "Next Year", InFuture = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.Next30Days, Name = "Next 30 Days", InFuture = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.Next60Days, Name = "Next 60 Days", InFuture = true });
            list.Add(new RollingDateOperatorType() { Type = RollingDateOperator.Next90Days, Name = "Next 90 Days", InFuture = true });
            return list;
        }
    }

}
