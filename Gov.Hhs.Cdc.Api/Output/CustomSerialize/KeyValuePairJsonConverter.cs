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
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    public class KeyValuePairJsonConverter : JavaScriptConverter
    {
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer jss)
        {
            var result = new Dictionary<string, object>();
            var input = obj as Dictionary<string, string>;
            if (input == null) throw new InvalidOperationException("Object must be type Dictionary<string, string>");

            foreach (KeyValuePair<string, string> pair in input)
            {
                result.Add(pair.Key, pair.Value);
            }
            return result;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var result = new Dictionary<string, string>();
            foreach (var item in dictionary)
            {
                if (item.Value.GetType() == typeof(string))
                {
                    result.Add(item.Key, item.Value.ToString());
                }
            }
            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new ReadOnlyCollection<Type>(new Type[] { typeof(Dictionary<string, string>) });
            }
        }
    }
}
