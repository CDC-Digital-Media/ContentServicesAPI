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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.MediaValidatonProvider
{
    public class ExtractedDetail
    {
        public int? SizeOfElements { get; set; }
        public int? NumberOfElements { get; set; }
        public int? NumberOfInlineStyles { get; set; }
        public int? NumberOfForms { get; set; }
        public int? NumberOfOtherForms { get; set; }
        public int? NumberOfEmbeddedVideos { get; set; }
        public int? NumberOfOtherEmbeddedVideos { get; set; }

        public int NumberOfAppliedMediaTypeFilterItems { get; set; }
        public bool XHtmlLoadable { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }

        private ValidationMessages _messages { get; set; }
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
        }

        public bool ExtractionIsValid { get { return Messages.NumberOfErrors == 0; } }

        public ExtractedDetail()
        {
        }

        public ExtractedDetail(string data, ValidationMessages messages)
        {
            Data = data;
            _messages = messages;
        }

        public void PrependMessages(ValidationMessages extractionMessages)
        {
            _messages = extractionMessages.Union(Messages);
        }



    }

}
