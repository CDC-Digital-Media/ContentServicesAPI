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

namespace Gov.Hhs.Cdc.MediaValidatonProvider
{

    public class ExtractionResult : DataSourceBusinessObject
    {
        public bool IsValid { get; set; }

        public CollectedData CollectedData { get; set; }
        public ExtractedDetail ExtractedDetail { get; set; }
        public MediaAddress MediaAddress { get; set; }
        
        private ValidationMessages _messages = null;
        public ValidationMessages Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new ValidationMessages();
                }
                return _messages;
            }
            set { _messages = value; }
        }

        public ExtractionResult()
        {
            ExtractedDetail = new ExtractedDetail();
        }

        public ExtractionResult(MediaAddress mediaAddress, CollectedData collectedData, ExtractedDetail extractedDetail)
        {
            CollectedData = collectedData ?? new CollectedData();
            ExtractedDetail = extractedDetail ?? new ExtractedDetail();
            MediaAddress = mediaAddress;
            Initialize();
            //MediaTypeCode = mediaAddress.MediaTypeCode;
            //ParentSource = mediaAddress.SourceTable;
            //ParentId = mediaAddress.SourceId;
        }

        private void Initialize()
        {
            _messages = CollectedData.Messages.Union(ExtractedDetail.Messages);
            IsValid = CollectedData.IsCollectionValid && ExtractedDetail.ExtractionIsValid;
        }








  
    }
}
