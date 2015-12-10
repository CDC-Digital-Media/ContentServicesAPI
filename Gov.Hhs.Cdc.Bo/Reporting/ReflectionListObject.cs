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
using System.Reflection;
using System.Collections;

namespace Gov.Hhs.Cdc.Bo
{
    /// <summary>
    /// Used for reporting
    /// </summary>
    public class ReflectionListObject : ReflectionObject
    {
        public IList TheList
        {
            get
            {
                return (IList)TheObject;
            }
        }

        public ReflectionListObject(ReflectionObject reflectionObject)
            : base(reflectionObject)
        {
        }   

        public int Count 
        {
            get
            {
                return TheList.Count;
            }
        }

        public ReflectionObject this[int index] 
        { 
            get 
            {
                object selectedObject = TheList[index];
                ReflectionObject childObject = ReflectionObject.Create(selectedObject);
                childObject.MemberName = GetSingularName(this.MemberName);
                return childObject;
             } 
        } 

        public override void AddValuesTo(ref List<string> values)
        {
            for (int i = 0; i < Count; ++i)
            {
                ReflectionObject childObject = this[i];
                childObject.AddValuesTo(ref values);
            }
        }


        public string ToFormattedString()
        {

            if (TheList == null)
                return "";
            List<string> values = new List<string>();
            for (int i = 0; i < Count; ++i)
            {
                values.Add(this[i].ToString());

            }

            return string.Join("|", values.ToArray());
        }


    }
}
