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
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Gov.Hhs.Cdc.Bo
{
    public static class XElementExtensions
    {
        public static XElement ToXElement<T>(this object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }
        }

        public static T FromXElement<T>(this XElement element)
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(element.ToString())))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(memoryStream);
            }
        }

        public static string GetAttributeValueAsString(this XElement element, string attributeName)
        {
            XAttribute attribute = element.Attribute(attributeName);
            return attribute == null ? null : attribute.Value;
        }

        public static DateTime GetAttributeValueAsDateTime(this XElement element, string attributeName)
        {
            string value = element.GetAttributeValueAsString(attributeName);
            if (value != null)
            {
                DateTime theDateTime;
                if (DateTime.TryParse(value, out theDateTime))
                    return theDateTime;
            }
            return DateTime.MinValue;
        }
    }
}
