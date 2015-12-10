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

namespace Gov.Hhs.Cdc.Bo
{
    public class SingleDateType
    {
        public enum SingleDateOperator { Eq, Lt, LtEq, Gt, GtEq };
        public SingleDateOperator Type { get; set; }
        public string Name { 
            get
            {
                switch(Type)
                {
                    case SingleDateOperator.Eq:
                        return "=";
                    case SingleDateOperator.Lt:
                        return "<";
                    case SingleDateOperator.LtEq:
                        return "<=";
                    case SingleDateOperator.Gt:
                        return ">";
                    case SingleDateOperator.GtEq:
                        return ">=";
                    default:
                        return "";
                }
            }
        }

        public SingleDateType(SingleDateOperator oper)
        {
            Type = oper;
        }

        public SingleDateType(string type)
        {
            Type = (SingleDateOperator)Enum.Parse(typeof(SingleDateOperator), type);
        }

        public static List<SingleDateType> TypeList()
        {
            List<SingleDateType> list = new List<SingleDateType>();
            list.Add(new SingleDateType(SingleDateOperator.Eq));
            list.Add(new SingleDateType(SingleDateOperator.Lt));
            list.Add(new SingleDateType(SingleDateOperator.LtEq));
            list.Add(new SingleDateType(SingleDateOperator.Gt));
            list.Add(new SingleDateType(SingleDateOperator.GtEq));
            return list;
        }

    }

}
