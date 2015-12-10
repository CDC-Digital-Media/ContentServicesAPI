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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public interface IAToZTransformation
    {
        SerialResponse CreateSerialResponse(IEnumerable<AToZObject> AToZObjects);
    }

    public class AToZTransformation : IAToZTransformation
    {
        public SerialResponse CreateSerialResponse(IEnumerable<AToZObject> aToZObjects)
        {
            return new SerialResponse() { results = aToZObjects.Select(i => CreateSerialAToZ(i)).ToList() };
        }

        public static SerialAToZ CreateSerialAToZ(AToZObject aToZ)
        {
            return new SerialAToZ()
            {
                letter = aToZ.Letter,
                language = aToZ.LanguageCode,
                valueSetName = aToZ.ValueSetName,
                mediaCount = aToZ.MediaCount,
                vocabCount = aToZ.VocabCount
            };
        }
    }
}
