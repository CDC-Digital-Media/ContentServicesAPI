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
using System.Web;

namespace Gov.Hhs.Cdc.Bo
{
    public class ValidationMessage
    {

        public enum MessageSourceType { Tidy, Validation };
        public MessageSourceType MessageSource { get; set; }
        public System.Exception Exception { get; set; }
        public int? LineNumber { get; set; }
        public int? ColumnNumber { get; set; }

        public enum ValidationSeverity {Error, Warning, Information}
        public string Key { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
        public string DeveloperMessage { get; set; }
        public ValidationSeverity Severity { get; set; }

        public ValidationMessage()
        {
        }

        public ValidationMessage(string severity, string key, string message)
        {
            ValidationSeverity newSeverity;
            Severity = Enum.TryParse<ValidationSeverity>(severity, out newSeverity) ? newSeverity : ValidationSeverity.Error;
                
            Key = key;
            Message = message;
            Id = "";
            DeveloperMessage = "";
        }

        public ValidationMessage(ValidationSeverity severity, string key, string message)
        {
            Severity = severity;
            Key = key;
            Message = message;
            Id = "";
            DeveloperMessage = "";
        }

        public ValidationMessage(ValidationSeverity severity, string key, string id, string message, string technicalDetails)
        {
            Severity = severity;
            Key = key;
            Id = id;
            Message = message;
            DeveloperMessage = technicalDetails;
        }

        public override string ToString()
        {
            return string.Format("(Key={0}): {1} message [level {2}]{3}{4}: {5}",
                Key,
                MessageSource.ToString(),
                Severity.ToString(),                
                LineNumber == null ? "" : " [line " + LineNumber + "]",
                ColumnNumber == null ? "" : " [column " + ColumnNumber + "]",
                Message
                );
        }

    }
}
