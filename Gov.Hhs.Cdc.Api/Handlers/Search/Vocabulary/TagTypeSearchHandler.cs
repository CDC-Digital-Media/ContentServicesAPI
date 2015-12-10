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

using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public static class TagTypeSearchHandler
    {
        private static TagTypeColumnMapFactory MapFactory = new TagTypeColumnMapFactory();
        public static ValidationMessages Get(ICallParser parser, IOutputWriter writer)
        {
            if (parser.Version < 2)
            {
                return writer.Write(ValidationMessages.CreateError("TagTypes", "TagTypes endpoint is only version 2 and later"));
            }
            ValidationMessages messages = new ValidationMessages();
            Sorting sorting = MapFactory.GetMapper(parser.Version, parser.IsPublicFacing).GetMappedSortColumns(parser.SortColumns);
            SearchParameters searchParameters = new SearchParameters("Media", "TagType", parser.Query.GetPaging(),
                parser.SecondsToLive, sorting, GetFilterCriteria().TheFilterCriteria);
            if (messages.Errors().Any())
            {
                return writer.Write(messages);
            }

            DataSetResult dataSet = ServiceUtility.GetDataSet(parser, searchParameters, out messages);
            if (messages.Errors().Any())
            {
                return writer.Write(messages);
            }
            ITagTypeTransformation transformation = TransformationFactory.GetTagTypeTransformation();
            SerialResponse response = transformation.CreateSerialResponse(dataSet.Records.Cast<TagTypeObject>());
            ServiceUtility.SetResponse(parser, writer, searchParameters, dataSet, response);

            return messages;
        }

        public static TagTypeFilterCriteria GetFilterCriteria()
        {
            TagTypeFilterCriteria criteria = new TagTypeFilterCriteria();
            return criteria;

        }

    }


}
