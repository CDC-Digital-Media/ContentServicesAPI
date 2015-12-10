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
using System.Xml;
using System.Xml.Serialization;

namespace Gov.Hhs.Cdc.MediaProvider
{
    [Serializable]
    public enum PreferenceTypeEnum { WebPage, Mobile };
    public class MediaPreferenceSet : IXmlSerializable
    {
        public string PreferenceType { get; set; }
        public bool IsDefault { get; set; }
        public HtmlPreferences MediaPreferences { get; set; }

        public MediaPreferenceSet Merge(MediaPreferenceSet subordinateCriteria)
        {
            MediaPreferences = MediaPreferences.Merge(subordinateCriteria == null ? null : subordinateCriteria.MediaPreferences);
            return this;
        }

        //public static PreferenceTypeEnum GetPreferenceType(MediaPreferenceSet urlSpecifiedMediaPreferences)
        //{
        //    string stringPreferenceType = urlSpecifiedMediaPreferences == null ? null : urlSpecifiedMediaPreferences.PreferenceType;

        //    PreferenceTypeEnum preferenceType;
        //    if (!Enum.TryParse(stringPreferenceType, /*ignoreCase:*/true, out preferenceType))
        //        preferenceType = PreferenceTypeEnum.WebPage;
        //    return preferenceType;
        //}

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (!reader.HasAttributes)
                throw new FormatException("expected a type attribute!");

            PreferenceType = reader.GetAttribute("PreferenceType");
            IsDefault = bool.Parse(reader.GetAttribute("IsDefault"));

            string type = reader.GetAttribute("MediaPreferencesType");
            reader.Read(); // consume the value
            if (type == "null")
                return;// leave T at default value

            Type preferencesType = Type.GetType(type);
            XmlSerializer serializer = new XmlSerializer(preferencesType);

                this.MediaPreferences = (HtmlPreferences)serializer.Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            if( PreferenceType != null)
                writer.WriteAttributeString("PreferenceType", PreferenceType);
            writer.WriteAttributeString("IsDefault", IsDefault.ToString());
            if (MediaPreferences != null)
            {
                Type type = this.MediaPreferences.GetType();
                XmlSerializer serializer = new XmlSerializer(type);
                writer.WriteAttributeString("MediaPreferencesType", type.AssemblyQualifiedName);
                serializer.Serialize(writer, this.MediaPreferences);
            }
            
        }
    }
}
