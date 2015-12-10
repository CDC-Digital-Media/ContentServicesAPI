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
    /// Used by the Serialization for Reporting
    /// </summary>
    public class ReflectionObject : ReflectionObjectType
    {
        public object TheObject { get; set; }
        public string MemberName { get; set; }

        public ReflectionObject(Type objectType, object theObject)
            : base(objectType)
        {
            this.TheObject = theObject;
        }


        public ReflectionObject(object theObject)
            : base(theObject.GetType())
        {
            this.TheObject = theObject;
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="reflectionObject"></param>
        public ReflectionObject(ReflectionObject reflectionObject)
            : base(reflectionObject)
        {
            this.TheObject = reflectionObject.TheObject;
            this.MemberName = reflectionObject.MemberName;
        }

        public virtual List<string> GetValues()
        {
            List<string> values = new List<string>();
            AddValuesTo(ref values);
            return values;
        }

        public virtual void AddValuesTo(ref List<string> values)
        {

            IEnumerable<MethodInfo> addValuesToMethods = SelectMethods("AddValuesTo", 1);
            if (addValuesToMethods.Any())
                Invoke(addValuesToMethods.First(), values);
            else if (!IsContainer())
                values.Add(this.ToString());
            else //if (IsContainer())
            {
                //We must write all the attributes first before the lists
                IEnumerable<ReflectionObject> allChildren = (from p in PublicProperties
                                                            select GetChild(p)).ToList();

                List<ReflectionObject> singleAttributeChildren = allChildren.Where(c => !c.IsAListType()).ToList();
                foreach (ReflectionObject singleAttributeChild in singleAttributeChildren)
                {
                    singleAttributeChild.AddValuesTo(ref values);
                }

                List<ReflectionObject> listChildren = allChildren.Where(c => c.IsAListType()).ToList();

                foreach (string formattedListObject in listChildren
                                .Select(c => new ReflectionListObject(c).ToFormattedString()))
                { 
                    values.Add(formattedListObject);
                }


            }

        }

        public string GetPropertyValue(PropertyInfo property)
        {
            return GetPropertyValue(property, TheObject);
        }
         
        public void Invoke(MethodInfo method, object parm)
        {
            object[] parms = new object[1];
            parms[0] = parm;
            method.Invoke(TheObject, parms);
        }
        public override string ToString()
        {
            return TheObject == null ? "" : TheObject.ToString();
        }

        public static ReflectionObject Create(object theObject)
        {
            ReflectionObject reflectionObject = new ReflectionObject(theObject);
            reflectionObject.MemberName = reflectionObject.TheType.Name;
            if (reflectionObject.IsAListType())
                return new ReflectionListObject(reflectionObject);
            else
                return reflectionObject;
        }

        public ReflectionObject GetChild(PropertyInfo propertyInfo)
        {
            ReflectionObject reflectionObject = new ReflectionObject(propertyInfo.PropertyType, propertyInfo.GetValue(TheObject, null));

            reflectionObject.MemberName = propertyInfo.Name;

            if (reflectionObject.IsAListType())
                return new ReflectionListObject(reflectionObject);
            else
                return reflectionObject;
        }

    }
}
