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

using System.Web.Script.Serialization;
using System.Reflection;
using Gov.Hhs.Cdc.DataServices.Bo;
using Newtonsoft.Json;

namespace Gov.Hhs.Cdc.Api
{
    public class DictionaryJsonDotNetConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Dictionary<string, string>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IDictionary<string, string>)
            {
                CustomSerialDictionary(writer, value, serializer);
            }
        }

        private void CustomSerialDictionary(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var input = value as Dictionary<string, string>;
            writer.WriteStartObject();
            foreach (var item in input)
            {
                if (item.Value.GetType() == typeof(string))
                {
                    writer.WritePropertyName(item.Key);
                    serializer.Serialize(writer, item.Value.ToString());
                }
            }
            writer.WriteEndObject();
        }

    }
}
