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
using System.Web;

using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public static class TransformationAdminFactory
    {
        public static object CreateLanguage(int version, DataSetResult dataset)
        {
            switch (version)
            {
                case 1:
                    return new LanguageAdminTransformationV1().CreateSerialObjectList(dataset);
                default:
                    return new object();
            }
        }

        public static object CreateSource(int version, DataSetResult dataset)
        {
            if (version == 1)
            {
                return new SourceAdminTransformationV1().CreateSerialObjectList(dataset);
            }
            else
            {
                return new object();
            }
        }
        
        public static object CreateFeedFormat(int version, DataSetResult dataset)
        {
            switch (version)
            {
                case 1:
                    return new FeedFormatAdminTransformationV1().CreateSerialObjectList(dataset);
                default:
                    return new object();
            }
        }
    }
}
