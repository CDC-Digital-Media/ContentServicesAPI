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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Gov.Hhs.Cdc.CsBusinessObjects.Media
{
    public static class SearchCriteriaExtensions
    {
        public static string ToXmlString(this SearchCriteria criteria)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    var xns = new XmlSerializerNamespaces();
                    xns.Add(string.Empty, string.Empty);
                    var serializer = new XmlSerializer(typeof(CsBusinessObjects.Media.SearchCriteria));
                    serializer.Serialize(writer, criteria, xns);
                    var output = XElement.Parse(Encoding.UTF8.GetString(stream.ToArray())).ToString();
                    return output;
                }
            }
        }

    }
}
