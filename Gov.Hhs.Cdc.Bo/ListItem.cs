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

namespace Gov.Hhs.Cdc.Bo
{
    public class InvalidKeyTypeException : ApplicationException
    {
        public InvalidKeyTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class ListItem
    {
        public enum KeyType { StringKey, IntKey, Null }

        private int efSafeKeyType = (int)KeyType.StringKey;

        public int EfSafeKeyType
        {
            get { return efSafeKeyType; }
            set { efSafeKeyType = value; }
        }

        public KeyType EnumKeyType
        {
            get { return (KeyType)efSafeKeyType; }
            set { efSafeKeyType = (int)value; }
        }

        private string key;

        public int IntKey { get; set; }
        public string Value
        {
            get
            {

                return (this.EnumKeyType == KeyType.StringKey)
                    ? key
                    : IntKey.ToString();
            }
            set
            {
                if (this.EnumKeyType == KeyType.StringKey)
                {
                    key = value;
                }
                else
                {
                    try
                    {
                        IntKey = value == null ? 0 : int.Parse(value);
                    }
                    catch (FormatException ex)
                    {
                        //To Do: Create an exception class to raise this error
                        throw new InvalidKeyTypeException("List item key is not numeric(" + value + ")", ex);
                    }
                }
            }
        }

        public string DisplayName { get; set; }
        public string LongDisplayName { get; set; }
        public string Code { get; set; }
        public int DisplayOrdinal { get; set; }

        public ListItem()
        {
        }

        public ListItem(KeyType keyType)
        {
            this.EnumKeyType = keyType;
        }

        public ListItem(string key, string displayName)
        {
            this.EnumKeyType = KeyType.StringKey;
            this.key = key;
            this.DisplayName = displayName;
            this.LongDisplayName = displayName;
        }

        public ListItem(int key, string displayName)
        {
            this.EnumKeyType = KeyType.IntKey;
            this.IntKey = key;
            this.DisplayName = displayName;
            this.LongDisplayName = displayName;
        }

        public static KeyType GetKeyType(string keyType)
        {
            string type = (keyType ?? "").ToLower();
            if (type == "string" || type == "s" || type == "stringkey")
            {
                return ListItem.KeyType.StringKey;
            }
            if (type == "int" || type == "i" || type == "intkey")
            {
                return ListItem.KeyType.IntKey;
            }
            //Default
            return ListItem.KeyType.IntKey;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

}
