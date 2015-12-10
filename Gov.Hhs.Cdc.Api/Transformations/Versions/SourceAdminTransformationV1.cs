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
    public class SourceAdminTransformationV1 : TransformationBase<SerialSource, SourceItem>
    {
        public override List<SerialSource> CreateSerialObjectList(DataSetResult dataset)
        {
            return GetEnumerableBusinessObject(dataset).Select(a => CreateSerialObject(a)).ToList();
        }

        public override SerialSource CreateSerialObject(SourceItem obj)
        {
            var source = new SerialSource();
            if (obj != null)
            {
                source = new SerialSource()
                {
                    name = obj.Code,
                    acronym = obj.Acronym,
                    //comments = a.Comments,
                    websiteUrl = obj.PrimaryUrl,
                    //displayOrdinal = obj.DisplayOrdinal,
                    largeLogoUrl = obj.LogoLargeUrl,
                    smallLogoUrl = obj.LogoSmallUrl,
                    //active = a.IsActive
                };
            }

            return source;           
        }

        public override SourceItem CreateBusinessObject(SerialSource obj)
        {
            throw new NotImplementedException();
        }
    }
}
