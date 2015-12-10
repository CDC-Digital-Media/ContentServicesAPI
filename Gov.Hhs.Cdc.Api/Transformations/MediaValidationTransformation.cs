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
using Gov.Hhs.Cdc.MediaValidatonProvider;

namespace Gov.Hhs.Cdc.Api
{
    public static class MediaValidationTransformation
    {
        public static SerialResponse CreateSerialMediaValidation(ExtractionResult result, string sourceUrl)
        {
            if (result == null)
            {
                return new SerialResponse() { results = new SerialMediaValidation() };
            }

            var collectedData = result.CollectedData ?? new CollectedData();
            var extractedDetail = result.ExtractedDetail ?? new ExtractedDetail();

            SerialMediaValidation mediaValidation = new SerialMediaValidation()
            {
                validation = new SerialValidation()
                {
                    urlAlreadyExists = result.MediaAddress.AddressIsAlreadyPersisted,
                    isDuplicate = result.MediaAddress.AddressIsAlreadyPersistedWithSameExtractionCriteria,
                    existingMediaId = result.MediaAddress.ExistingMediaId,
                    sourceCode = result.MediaAddress.SourceCode,
                    domainName = result.MediaAddress.DomainName,
                    numberOfElements = extractedDetail.NumberOfElements != null ? (int)result.ExtractedDetail.NumberOfElements : 0,
                    isLoadable = collectedData.IsLoadable,
                    xHtmlLoadable = extractedDetail.XHtmlLoadable,
                    isValid = result.IsValid,
                    messages = result.Messages.Messages
                    .Where(a => a.MessageSource == ValidationMessage.MessageSourceType.Tidy)
                    .Select(m => FormatTidyMessage(m))
                    .Concat(result.Messages.Messages
                    .Where(a => a.MessageSource == ValidationMessage.MessageSourceType.Validation)
                    .Select(m => FormatValidationMessage(m))).ToList()
                },
                title = extractedDetail.Title,
                sourceUrl = sourceUrl,
                contentType = collectedData.MimeType,
                content = extractedDetail.Data
            };

            SerialResponse response = new SerialResponse()
            {
                results = new List<SerialMediaValidation>() { mediaValidation }
            };

            return response;
        }
        
        private static string FormatTidyMessage(ValidationMessage vm)
        {
            return "Tidy message [level " + vm.Severity + "] " +
                                "[line " + vm.LineNumber.ToString() + "] " +
                                "[column " + vm.ColumnNumber.ToString() + "]: " + vm.Message;
        }

        private static string FormatValidationMessage(ValidationMessage vm)
        {
            return string.Format("Validation message - {0}", vm.Message);
        }

    }
}
