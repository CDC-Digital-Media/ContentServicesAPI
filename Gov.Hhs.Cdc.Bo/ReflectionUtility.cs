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

using System.Linq;
using System.Reflection;

namespace Gov.Hhs.Cdc.Bo
{
    public static class ReflectionUtility
    {
        public static string Replace(string template, object keys)
        {
            if (string.IsNullOrEmpty(template))
                return "";
            ReflectionObject keysObject = new ReflectionObject(keys);

            string[] allVariables = template.Split('{').Select((s, index) => GetVariable(s, index, keysObject)).ToArray();

            return string.Join("", allVariables);
        }

        private static string GetVariable(string wholeString, int itemOrdinal, ReflectionObject keys)
        {
            int endBraceLocation = wholeString.IndexOf('}');
            bool hasReplaceableVariable = itemOrdinal > 0 && endBraceLocation > -1;
            if (!hasReplaceableVariable)
            {
                return wholeString;
            }

            int variableLength = endBraceLocation;
            string variableName = wholeString.Substring(0, variableLength);
            return GetVariable(variableName.Split('.'), keys) + SafeSubstring(wholeString, endBraceLocation + 1);
        }

        private static string SafeSubstring(string value, int startIndex)
        {
            return startIndex >= value.Length ? "" : value.Substring(startIndex);
        }

        private static string GetVariable(string[] nameNodes, ReflectionObject keys, int currentNameIndex = 0)
        {
            string currentNodeName = nameNodes[currentNameIndex];
            PropertyInfo property = keys.PublicProperties.Where(p => p.Name == currentNodeName).FirstOrDefault();
            if (property == null)
            {
                return "{" + currentNodeName + "}";
            }
            if (nameNodes.Count() - 1 > currentNameIndex)
            {
                return GetVariable(nameNodes, keys.GetChild(property), currentNameIndex + 1);
            }
            else
            {
                return keys.GetPropertyValue(property);
            }
        }


    }
}
