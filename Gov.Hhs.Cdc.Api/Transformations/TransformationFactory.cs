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

using System.Web;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public static class TransformationFactory
    {

        public static IMediaTransformation GetMediaTransformation(int version, bool isPublicFacing)
        {
            if (!isPublicFacing)
            {
                return new MediaAdminTransformation();
            }
            else if (version == 1)
            {
                return new MediaTransformationV1();
            }
            else
            {
                return new MediaTransformationV2();
            }
        }

        public static ITagTransformation GetTagTransformation()
        {
            return new TagTransformationV2();
        }

        public static ITagTypeTransformation GetTagTypeTransformation()
        {
            return new TagTypeTransformationV2();
        }

        public static IAToZTransformation GetAToZTransformation()
        {
            return new AToZTransformation();
        }

        public static IRelatedMediaTransformation GetRelatedMediaTransformation(int version, bool isPublicFacing)
        {
            if (!isPublicFacing)
            {
                return new RelatedMediaTransformationAdmin();
            }
            else if (version == 1)
            {
                return new RelatedMediaTransformationV1();
            }
            else
            {
                return new RelatedMediaTransformationV2();
            }
        }

        public static object CreateLanguage(int version, DataSetResult dataset)
        {
            switch (version)
            {
                case 1:
                    return new LanguageTransformationV1().CreateSerialObjectList(dataset);
                case 2:
                    return new LanguageTransformationV2().CreateSerialObjectList(dataset);
                default:
                    return new object();
            }
        }

        public static object CreateSource(int version, DataSetResult dataset)
        {
            if (version >= 2)
            {
                return new SourceTransformationV2().CreateSerialObjectList(dataset);
            }
            else
            {
                return new object();
            }
        }

    }
}
