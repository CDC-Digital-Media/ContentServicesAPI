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
using System;

namespace Gov.Hhs.Cdc.Api
{
    public class TestOutputWriter : IOutputWriter
    {
        public object TheObject { get; set; }
        public ValidationMessages ValidationMessages { get; set; }
        private Dictionary<string, string> _headers = null;
        public Dictionary<string, string> Headers
        {
            get { return _headers ?? (_headers = new Dictionary<string, string>()); }
        }

        public void Write(object theObject, ValidationMessages messages = null)
        {
            TheObject = theObject;
            ValidationMessages = messages ?? new ValidationMessages();

            if (theObject != null && theObject.GetType() == typeof(SerialResponse))
            {
                SerialResponse response = (SerialResponse)theObject;
                response.meta.message = messages == null ?
                    new List<SerialValidationMessage>() :
                    messages.Messages.Select(m => new SerialValidationMessage() { developerMessage = m.Message, userMessage = m.Message, id = m.Key }).ToList();
            }

            //return ValidationMessages;
        }

        public ValidationMessages Write(ValidationMessages messages)
        {
            ValidationMessages = messages ?? new ValidationMessages();
            return ValidationMessages;
        }
        public void CreateAndWriteSerialResponse(object results, ValidationMessages messages = null)
        {
            TheObject = results;
            ValidationMessages = messages;
        }


        public void AddHeader(string header, string value)
        {
            if (Headers.ContainsKey(header))
                Headers[header] = value;
            else
                Headers.Add(header, value);
        }

        public void Redirect(Uri url)
        {
            throw new System.NotImplementedException();
        }

        public void WriteToFile(object response, string atFilePath)
        {
        }
    }

}
