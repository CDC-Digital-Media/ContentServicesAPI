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

namespace Gov.Hhs.Cdc.Bo
{
    public class ReflectionQualifiedNames
    {
        private static Dictionary<Type, ReflectionQualifiedNames> qualifiedNames;
        private static Dictionary<Type, ReflectionQualifiedNames> QualifiedNames 
        {
            get { return qualifiedNames ?? (qualifiedNames = new Dictionary<Type, ReflectionQualifiedNames>()); }
        }


        Dictionary<string, string> names;
        private Dictionary<string, string> Names { get { return names ?? (names = new Dictionary<string, string>()); } }

        private ReflectionQualifiedNames(Type type)
        {
            BuildQualifiedNames(new ReflectionObjectType(type), "");
        }

        public static ReflectionQualifiedNames Get(Type type)
        {
            try
            {
                if (!QualifiedNames.ContainsKey(type))
                {
                    QualifiedNames.Add(type, new ReflectionQualifiedNames(type));
                }

            }
            catch (KeyNotFoundException ex)
            {
                //adding error details for production error
                throw new KeyNotFoundException(type.ToString(), ex);
            }
            catch (ArgumentException ex)
            {
                //attempt to address another production error
                Logging.Logger.LogError(ex, "ReflectionQualifiedNames.Get(" + type.ToString() + ")");

            }
            return QualifiedNames[type];
        }

        private void BuildQualifiedNames(ReflectionObjectType objectType, string prefix)
        {
            IEnumerable<ReflectionObjectType> types = from p in objectType.PublicProperties
                                               select new ReflectionObjectType(p);
            types = types.Where(p => !p.IsAListType()).ToList();

            foreach (ReflectionObjectType propertyType in types.Where(p => !p.IsContainer()))
            {
                string name = propertyType.Name.ToLower();
                if (!Names.ContainsKey(name))
                    Names.Add(name, prefix + propertyType.Name);
            }

            foreach (ReflectionObjectType propertyType in types.Where(p => p.IsContainer()))
            {
                BuildQualifiedNames(propertyType, prefix + propertyType.Name + ".");
            }
        }

        public static string GetQualifiedName<t>(string name)
        {
            return Get(typeof(t)).GetQualifiedName(name);
        }

        public string GetQualifiedName(string name)
        {
            string lowerName = name.ToLower();
            return Names.ContainsKey(lowerName) ? Names[lowerName] : name;
        }

    }
}
