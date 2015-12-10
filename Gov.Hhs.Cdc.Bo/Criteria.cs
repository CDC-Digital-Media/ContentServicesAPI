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
    public class Criteria
    {
        public List<Criterion> List { get; set; }
        public Criteria()
        {
            List = new List<Criterion>();
        }

        public Criteria(params Criterion[] criteria)
        {
            List = criteria.ToList();
        }

        public void Add(string name, bool? value)
        {
            if (value != null)
            {
                List.Add(new Criterion(name, (bool)value ? "true" : "false"));
            }
        }

        public void Add(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            List.Add(new Criterion(name, value));
        }

        public void Add(string name, int value)
        {
            List.Add(new Criterion(name, value));
        }

        //public void Add(string name, List<int> values)
        //{
        //    if (values != null)
        //    {
        //        List.Add(new Criterion(name, values));
        //    }
        //}

        public void Add(string name, List<string> values)
        {
            if (values != null)
            {
                List.Add(new Criterion(name, values));
            }

        }

        public void AddList(string name, string values, params char[] separator)
        {
            if (values != null)
            {
                List.Add(new Criterion(name, values.Split(separator).ToList()));
            }
        }


        public void Add(Criterion filterCriterion)
        {
            List.Add(filterCriterion);
        }

        public Criterion GetCriterion(string code)
        {
            return List.Where(c => c.Code == code).FirstOrDefault();
        }

    }
}
