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
    /// <summary>
    /// Used for reporting
    /// </summary>
    public class ReflectionObjectType
    {
        public Type TheType { get; set; }
        public string Name { get; set; }

        public ReflectionObjectType(Type objectType)
        {
            TheType = objectType;
        }

        public ReflectionObjectType(PropertyInfo propertyInfo)
        {
            TheType = propertyInfo.PropertyType;
            Name = propertyInfo.Name;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="reflectionObjectType"></param>
        public ReflectionObjectType(ReflectionObjectType reflectionObjectType)
        {
            this.TheType = reflectionObjectType.TheType;
            this.Name = reflectionObjectType.Name;
        }

        public IEnumerable<PropertyInfo> PublicProperties
        {
            get
            {
                return TheType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            }
        }


        public ReflectionObjectType GetListTypeAsReflectionObject()
        {
            if (IsAListType())
                return new ReflectionObjectType(TheType.GetGenericArguments()[0]);
            return null;

        }

        public Type GetListType()
        {
            if (IsAListType())
                return TheType.GetGenericArguments()[0];
            else
                return typeof(object);

        }

        public bool IsAListType()
        {
            if( TheType.IsGenericType )
            {
                Type genericType = TheType.GetGenericTypeDefinition();
                if( genericType == typeof(List<>) || genericType == typeof(IEnumerable<>))
                    return true;
            }
            return false;

        }

        public bool IsContainer()
        {
            //Is this a generic object?
            if (TheType.Name.Substring(0, 1) == "<")
                return true;
            string localNamespace = "Gov.Hhs.";
            if (TheType.Namespace.Length > localNamespace.Length && TheType.Namespace.Substring(0, localNamespace.Length) == localNamespace)
                return true;
            //string a = "";
            return false;
        }



        public IEnumerable<MethodInfo> SelectMethods(string name, int numberOfParameters)
        {
            MethodInfo[] methods = TheType.GetMethods();
            return from m in methods
                   where m.Name.Equals(name) && m.GetParameters().Count() == numberOfParameters
                   select m;
        }

        public static string GetPropertyValue(PropertyInfo property, object item)
        {
            object objValue = property.GetValue(item, null);
            return objValue == null ? "" : objValue.ToString();
        }

        public string GetSingularName(string pluralName)
        {
            if (string.IsNullOrEmpty(pluralName))
                return "record";
            if (pluralName.EndsWith("ies"))
                return pluralName.Substring(0, pluralName.Length - 3) + "y";
            if (pluralName.EndsWith("xes") || pluralName.EndsWith("ches") || pluralName.EndsWith("shes") || pluralName.EndsWith("sses"))
                return pluralName.Substring(0, pluralName.Length - 2);
            if (pluralName.EndsWith("s"))
                return pluralName.Substring(0, pluralName.Length - 1);
            return "oneOf" + pluralName;

        }
    }
}
