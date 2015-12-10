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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public static class TagSearchHandler
    {
        private static TagColumnMapFactory MapFactory = new TagColumnMapFactory();

        public static ValidationMessages GetById(ICallParser parser, IOutputWriter writer, string id)
        {
            ValidationMessages messages = new ValidationMessages();
            var criteria = GetFilterCriteria(messages, parser, id, null);

            return Get(criteria, parser, writer, messages);
        }

        public static ValidationMessages GetByRelatedId(ICallParser parser, IOutputWriter writer, string relatedId)
        {
            ValidationMessages messages = new ValidationMessages();
            var criteria = GetFilterCriteria(messages, parser, null, relatedId);

            return Get(criteria, parser, writer, messages);
        }

        private static ValidationMessages Get(TagFilterCriteria criteria, ICallParser parser, IOutputWriter writer, ValidationMessages messages)
        {
            Sorting sorting = MapFactory.GetMapper(parser.Version, parser.IsPublicFacing).GetMappedSortColumns(parser.SortColumns);
            SearchParameters searchParameters = new SearchParameters("Media", "Tag", parser.Query.GetPaging(),
                parser.SecondsToLive, sorting, criteria.TheFilterCriteria);

            if (messages.Errors().Any())
            {
                return writer.Write(messages);
            }

            DataSetResult dataSet = ServiceUtility.GetDataSet(parser, searchParameters, out messages);
            if (messages.Errors().Any())
            {
                return writer.Write(messages);
            }
            ITagTransformation transformation = TransformationFactory.GetTagTransformation();
            SerialResponse response = transformation.CreateSerialResponse(dataSet.Records.Cast<TagObject>());
            ServiceUtility.SetResponse(parser, writer, searchParameters, dataSet, response);

            return messages;
        }

        private static TagFilterCriteria GetFilterCriteria(ValidationMessages messages, ICallParser parser, string id, string relatedId)
        {
            TagFilterCriteria criteria = new TagFilterCriteria();
            criteria.TagName = parser.GetStringListParm(ApiParam.Name);
            criteria.TagNameContains = parser.GetStringParm(ApiParam.NameContains);
            criteria.MediaId = parser.GetIntParm(messages, ApiParam.mediaid);
            criteria.TagTypeId = parser.GetIntListParm(messages, ApiParam.TypeId);
            criteria.TagTypeName = parser.GetStringListParm(ApiParam.TypeName);
            criteria.LanguageCode = parser.GetStringListParm(ApiParam.Language);

            if (!string.IsNullOrEmpty(id))
            {
                criteria.TagId = new List<int> { ServiceUtility.ParsePositiveInt(id) };
            }
            if (!string.IsNullOrEmpty(relatedId))
            {
                criteria.RelatedTagId = ServiceUtility.ParsePositiveInt(relatedId);
            }

            return criteria;
        }

    }


}
