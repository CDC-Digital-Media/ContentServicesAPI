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

using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidatonProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gov.Hhs.Cdc.Logging;

namespace Gov.Hhs.Cdc.CdcMediaValidationProvider
{
    public class MediaExtractor //: IMediaExtractor
    {
        public ExtractionResult ExtractAndValidateHtml(bool isExtraction, MediaTypeValidationItem mediaTypeValidationItem, MediaAddress address)
        {
            ExtractionResult validatedData = ResultForMediaType(mediaTypeValidationItem, address, isExtraction);
            return validatedData;
        }

        public ExtractionResult ResultForMediaType(MediaTypeValidationItem mediaType, MediaAddress address, bool isExtraction)
        {
            if (address.MediaObject.MediaTypeCode == "Widget")
            {
                var collected = new CollectedData
                {
                    Data = address.MediaObject.Embedcode,
                };
                if (!string.IsNullOrEmpty(collected.Data))
                {
                    collected.IsAvailable = true;
                    collected.IsLoadable = true;
                    collected.CollectionIsValid = true;
                }
                else
                {
                    var message = "Widget " + address.MediaObject.Id.ToString() + " is missing embed code.";
                    collected.Messages.AddError("Widget embed", message);
                    Logger.LogError(message);
                }
                var result = new ExtractionResult
                {
                    CollectedData = collected,
                    ExtractedDetail = new ExtractedDetail { Data = collected.Data },
                    MediaAddress = address
                };
                return result;
            }
            else
            {
                UrlMediaCollector collector = new UrlMediaCollector();

                var config = new MediaValidatorConfig();
                CollectedData collectedData = collector.Get(address, config, mediaType);
                var validator = new HtmlMediaValidator();
                ExtractionResult validatedData = validator.ExtractAndValidate(address,
                    collectedData, config, mediaType, isExtraction);
                return validatedData;
            }
        }
    }
}
