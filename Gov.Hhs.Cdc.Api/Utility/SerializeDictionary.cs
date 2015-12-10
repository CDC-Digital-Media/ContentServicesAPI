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

using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Concurrent;
using System.Reflection;

namespace Gov.Hhs.Cdc.Api
{
    public class Entry
    {
        public object Key;
        public object Value;
        public Entry()
        {
        }

        public Entry(object key, object value)
        {
            Key = key;
            Value = value;
        }
    }

    public static class SerializeDictionary
    {

        //public static void Serialize(TextWriter writer, IDictionary dictionary)
        //{
        //    List<Entry> entries = new List<Entry>(dictionary.Count);
        //    foreach (object key in dictionary.Keys)
        //    {
        //        entries.Add(new Entry(key, dictionary[key]));
        //    }
        //    XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
        //    serializer.Serialize(writer, entries);
        //}

        public static void ToFile(string appPath, string fileNameWithoutExtention, string extension, ConcurrentDictionary<string, string> diction)
        {
            //Create folder if it doesn't exist
            string cacheFolder = appPath + "Cached_Nonce";
            if (!Directory.Exists(cacheFolder))
                System.IO.Directory.CreateDirectory(cacheFolder);

            string fullFilePath = cacheFolder + "\\" + fileNameWithoutExtention;
            string tempFilePath = fullFilePath + ".New";
            string xmlFilePath = fullFilePath + "." + extension;
            using (FileStream outputStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));

                List<Entry> entries = new List<Entry>(diction.Count);
                foreach (string key in diction.Keys)
                    entries.Add(new Entry(key, diction[key]));

                serializer.Serialize(outputStream, entries);
                outputStream.Flush();
                outputStream.Close();
            }

            CopyTempToXmlFile(tempFilePath, xmlFilePath);
        }

        public static ConcurrentDictionary<string, string> FromFile(string appPath, string fileNameWithoutExtention, string extension)
        {
            string cacheFolder = appPath + "Cached_Nonce";
            string fullFilePath = cacheFolder + "\\" + fileNameWithoutExtention;
            string xmlFilePath = fullFilePath + "." + extension;
            //string xmlFilePath = appPath + "Cached_Nonce" + "\\" + fileNameWithoutExtention + "." + extension;

            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            ConcurrentDictionary<string, string> diction = new ConcurrentDictionary<string, string>();

            //check if the file exists
            if (File.Exists(xmlFilePath))
            {
                using (FileStream oStream = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (XmlTextReader oReader = new XmlTextReader(oStream))
                    {
                        List<Entry> entries = (List<Entry>)serializer.Deserialize(oReader);
                        foreach (Entry entry in entries)
                            diction.TryAdd(entry.Key.ToString(), entry.Value.ToString());

                        oReader.Close();
                    }
                }
            }

            return diction;
        }

        //public static void Deserialize(TextReader reader, IDictionary dictionary)
        //{
        //    dictionary.Clear();
        //    XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
        //    List<Entry> list = (List<Entry>)serializer.Deserialize(reader);
        //    foreach (Entry entry in list)
        //    {
        //        dictionary[entry.Key] = entry.Value;
        //    }
        //}

        private static void CopyTempToXmlFile(string tempFilePath, string xmlFilePath)
        {
            File.Delete(xmlFilePath);   //Delete the old file
            File.Copy(tempFilePath, xmlFilePath);   //Copy the temp file to the real file name
            File.Delete(tempFilePath);  //Delete the temporary file
        }

    }

}
