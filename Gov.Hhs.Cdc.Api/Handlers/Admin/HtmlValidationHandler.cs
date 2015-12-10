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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaValidatonProvider;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class HtmlValidationHandler : MediaHandlerBase
    {

        public static void ValidateHtml(IOutputWriter writer, string id, ICallParser parser)
        {
            if (parser.ParamDictionary.ContainsKey(Param.RESOURCE_TYPE) && !string.IsNullOrEmpty(parser.ParamDictionary[Param.RESOURCE_TYPE]))
            {
                ValidateResources(writer, parser);
            }
            else
            {
                Validations(writer, id, parser);
            }
        }

        private static void Validations(IOutputWriter writer, string id, ICallParser parser)
        {
            ValidationMessages messages = new ValidationMessages();

            //MediaPreferenceSet mediaPreferences = PreferenceTransformation.GetHtmlExtractionCriteria(parser.ParamDictionary, parser.Version);
            var obj = PreferenceTransformation.GetHtmlExtractionCriteria(parser.ParamDictionary);
            obj.MediaId = ServiceUtility.ParseInt(id);
            obj.SourceUrl = parser.Query.Url;
            if (obj.MediaTypeCode == null)
            {
                obj.MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType;
            }

            var extractionResult = new CsMediaValidationProvider().ValidateHtmlForUrl(obj);
            SerialResponse response = MediaValidationTransformation.CreateSerialMediaValidation(extractionResult, parser.Query.Url);
            response.meta.resultSet.id = Guid.NewGuid().ToString();
            //response.meta.resultSet.isComplete = true;8

            if (extractionResult.Messages.Errors().Any())
            {
                extractionResult.Messages.Errors().Select(a => messages.Add(a));
            }

            writer.Write(response, messages);
        }

        public static void ValidateResources(ICallParser parser, IOutputWriter writer, string stream)
        {
            List<SerialResource> serialResources = BaseJsSerializer.Deserialize<List<SerialResource>>(stream);

            List<ResourceObject> resources = serialResources.Where(sr => !string.IsNullOrEmpty(sr.url)).Select(r => new ResourceObject()
            {
                Url = r.url,
                ResourceTypeCode = r.resourceType
            }).ToList();

            writer.Write(new CsMediaValidationProvider().ValidateResources(resources));
        }

        private static void ValidateResources(IOutputWriter writer, ICallParser parser)
        {
            string url = parser.Query.Url;
            string resourceType = parser.ParamDictionary[Param.RESOURCE_TYPE];

            List<ResourceObject> resources =
                new List<ResourceObject> 
                { 
                    new ResourceObject() { Url = url, ResourceTypeCode = resourceType } 
                };

            writer.Write(new CsMediaValidationProvider().ValidateResources(resources));
        }
    }
}
