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
using System.Configuration;
using System.Xml;
using Gov.Hhs.Cdc.Logging;

namespace Gov.Hhs.Cdc.Bo
{
    public class DataServicesConfig
    {

        private static bool? ConvertToBool(string setting)
        {
            string lowerValue = setting.ToLower();
            if (lowerValue == "y" || lowerValue == "yes" || lowerValue == "t" || lowerValue == "true")
            {
                return true;
            }
            if (lowerValue == "n" || lowerValue == "no" || lowerValue == "f" || lowerValue == "false")
            {
                return false;
            }
            return null;
        }



        protected static int GetTheIntValue(XmlAttributeCollection theAttributes, string sectionName, string attributeName, bool required, int defaultValue, ICollection<string> configErrors)
        {
            string setting = GetValueFromSection(configErrors, theAttributes, sectionName, attributeName, required);
            if (string.IsNullOrEmpty(setting))
            {
                return defaultValue;
            }

            int intValue = 0;
            if (int.TryParse(setting, out intValue))
            {
                return intValue;
            }

            configErrors.Add(string.Format("{0} attribute '{1}' is not a valid integer", sectionName, attributeName));
            return defaultValue;
        }

        protected static TimeSpan GetTheTimeSpanValue(ICollection<string> configErrors, XmlAttributeCollection theAttributes, string sectionName, string attributeName, bool required, TimeSpan defaultValue)
        {
            string setting = GetValueFromSection(configErrors, theAttributes, sectionName, attributeName, required);
            if (string.IsNullOrEmpty(setting))
            {
                return defaultValue;
            }
            TimeSpan timeSpanValue;
            if (TimeSpan.TryParse(setting, out timeSpanValue))
            {
                return timeSpanValue;
            }

            configErrors.Add(string.Format("<eMail>{0} attribute '{1}' is not a valid TimeSpan", sectionName, attributeName));
            return defaultValue;
        }

        protected static bool GetTheBoolValue(ICollection<string> configErrors, XmlAttributeCollection theAttributes, string sectionName, string attributeName, bool required, bool defaultValue)
        {
            string setting = GetValueFromSection(configErrors, theAttributes, sectionName, attributeName, required);
            if (string.IsNullOrEmpty(setting))
            {
                return defaultValue;
            }
            bool? boolValue = ConvertToBool(setting);
            if (boolValue != null)
            {
                return (bool)boolValue;
            }
            configErrors.Add(string.Format("{0} attribute '{1}' is invalid", sectionName, attributeName));
            return defaultValue;
        }

        protected static string GetTheStringValue(ICollection<string> configErrors, XmlAttributeCollection theAttributes, string sectionName, string attributeName, bool required)
        {
            string setting = GetValueFromSection(configErrors, theAttributes, sectionName, attributeName, required);
            if (string.IsNullOrEmpty(setting))
            {
                return string.Empty;
            }
            else
            {
                return setting;
            }
        }

        protected static string GetValueFromSection(ICollection<string> configErrors, XmlAttributeCollection theAttributes, string sectionName, string attributeName, bool required)
        {
            XmlAttribute xmlAttribute = theAttributes[attributeName];
            if (xmlAttribute == null || string.IsNullOrEmpty(xmlAttribute.Value))
            {
                if (configErrors != null && required)
                {
                    configErrors.Add(string.Format("{0} attribute '{1}' missing or empty", sectionName, attributeName));
                }
                return "";
            }
            else
            {
                return xmlAttribute.Value;
            }
        }

    }
}
