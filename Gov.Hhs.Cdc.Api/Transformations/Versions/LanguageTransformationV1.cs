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
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class LanguageTransformationV1 : TransformationBase<SerialLanguageV1, LanguageItem>
    {
        public override List<SerialLanguageV1> CreateSerialObjectList(DataSetResult dataset)
        {
            return GetEnumerableBusinessObject(dataset).Select(a => CreateSerialObject(a)).ToList();
        }

        public override SerialLanguageV1 CreateSerialObject(LanguageItem obj)
        {
            var lang = new SerialLanguageV1();
            if (obj != null)
            {
                lang = new SerialLanguageV1()
                {
                    name = obj.Code,
                    description = obj.Description,
                    displayOrdinal = obj.DisplayOrdinal
                };
            }

            return lang;
        }

        public override LanguageItem CreateBusinessObject(SerialLanguageV1 obj)
        {
            throw new NotImplementedException();
        }

    }

}
