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
using System.Web;

using System.Web.Script.Serialization;
using System.Reflection;
using Gov.Hhs.Cdc.DataServices.Bo;
using Newtonsoft.Json;

namespace Gov.Hhs.Cdc.Api
{
    public class MediaV2CustomJsonSerializer : JsonConverter   //, JavaScriptConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return (
                objectType == typeof(SerialMediaV2)
                || objectType == typeof(SerialMediaChildren)
                || objectType == typeof(SerialMediaParent)
                || objectType == typeof(SerialLanguageV2)
                || objectType == typeof(SerialValueItemTag)
                || objectType == typeof(SerialGeoTag)
                || objectType == typeof(SerialCampaign)
                || objectType == typeof(SerialSource)
                || objectType == typeof(SerialAlternateImage)
                );
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value.GetType().Name)
            {
                case "SerialMediaV2":
                    CustomSerialMediaV2(writer, value, serializer);
                    break;
                case "SerialMediaChildren":
                    CustomSerialMediaSecondLevel<SerialMediaChildren>(writer, value, serializer, GetSecondLevelFields("children"));
                    break;
                case "SerialMediaParent":
                    CustomSerialMediaSecondLevel<SerialMediaParent>(writer, value, serializer, GetSecondLevelFields("parents"));
                    break;
                case "SerialLanguageV2":
                    CustomSerialMediaSecondLevel<SerialLanguageV2>(writer, value, serializer, GetSecondLevelFields("language"));
                    break;
                case "SerialValueItemTag":
                    CustomSerialMediaSecondLevel<SerialValueItemTag>(writer, value, serializer, GetSecondLevelFields("tags"));
                    break;
                case "SerialGeoTag":
                    CustomSerialMediaSecondLevel<SerialGeoTag>(writer, value, serializer, GetSecondLevelFields("geotags"));
                    break;
                case "SerialCampaign":
                    CustomSerialMediaSecondLevel<SerialCampaign>(writer, value, serializer, GetSecondLevelFields("campaigns"));
                    break;
                case "SerialSource":
                    CustomSerialMediaSecondLevel<SerialSource>(writer, value, serializer, GetSecondLevelFields("source"));
                    break;
                case "SerialAlternateImage":
                    CustomSerialMediaSecondLevel<SerialAlternateImage>(writer, value, serializer, GetSecondLevelFields("alternateimages"));
                    break;
            }

        }

        private void SerializeObject(JsonWriter writer, JsonSerializer serializer, object input, List<string> fields)
        {
            PropertyInfo[] propertyInfos = input.GetType().GetProperties();
            if (fields.Count > 0)
            {
                propertyInfos = input.GetType().GetProperties().Where(a => fields.Contains(a.Name.ToLower())).ToArray();
            }

            writer.WriteStartObject();
            foreach (PropertyInfo prop in propertyInfos)
            {
                writer.WritePropertyName(prop.Name);
                serializer.Serialize(writer, input.GetType().GetProperty(prop.Name).GetValue(input, null));
            }
            writer.WriteEndObject();
        }

        private void CustomSerialMediaV2(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var input = value as SerialMediaV2;
            if (input == null)
            {
                return;
            }

            SerializeObject(writer, serializer, input, GetFirstLevelFields());
        }

        private void CustomSerialMediaSecondLevel<T>(JsonWriter writer, object value, JsonSerializer serializer, List<string> fields) where T : class
        {
            var input = value as T;
            if (input == null)
            {
                return;
            }

            SerializeObject(writer, serializer, input, fields);
        }

        private List<string> GetFirstLevelFields()
        {
            var fields = new List<string>();
            try
            {
                fields = ChangeSecondLevelCommasToPipes(HttpContext.Current.Request.QueryString[Param.FIELDS]).Split(',')
                    .Select(a => GetFirstLevelField(a))
                    .ToList();
            }
            catch (Exception)
            {
            }

            return fields;
        }

        private string GetFirstLevelField(string str)
        {
            int index = str.IndexOf('{');
            if (index > 0)
            {
                str = str.Substring(0, index);
            }

            return str.Trim().ToLower();
        }

        private string ChangeSecondLevelCommasToPipes(string str)
        {
            bool buildTempString = false;
            string tempString = string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '{')
                {
                    buildTempString = true;
                }
                if (str[i] == '}')
                {
                    buildTempString = false;
                }

                if (buildTempString)
                {
                    tempString += str[i] == ',' ? '|' : str[i];
                }
                else
                {
                    if (tempString.Length == 0)
                    {
                        sb.Append(str[i]);
                    }
                    else
                    {
                        tempString += str[i];
                        sb.Append(tempString);
                        tempString = "";
                    }
                }
            }

            return sb.ToString();
        }

        private List<string> GetSecondLevelFields(string root)
        {
            var fields = new List<string>();
            try
            {
                fields = ChangeSecondLevelCommasToPipes(HttpContext.Current.Request.QueryString[Param.FIELDS]).Split(',')
                    .Where(a => GetFirstLevelField(a) == root)
                    .Select(a => a.Trim().ToLower()).ToList();
            }
            catch (Exception)
            {
            }

            if (fields.Count > 0)
            {
                int startIndex = fields[0].IndexOf('{') + 1;
                int endIndex = fields[0].IndexOf('}');

                if (startIndex > 0 && endIndex > startIndex + 1)
                {
                    fields = fields[0].Substring(startIndex, (endIndex - startIndex)).Split('|').ToList<string>();
                }
                else
                {
                    fields = new List<string>();
                }
            }
            else
            {
                fields = new List<string>();
            }

            return fields;
        }

    }
}
