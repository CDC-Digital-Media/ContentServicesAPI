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

namespace Gov.Hhs.Cdc.Bo
{
    public class ValidationMessages
    {
        public int NumberOfStandardsErrors { get; set; }
        public int NumberOfStandardsWarnings { get; set; }

        public List<ValidationMessage> Messages { get; set; }
        public ValidationMessages()
        {
            Messages = new List<ValidationMessage>();
        }

        public ValidationMessages(ValidationMessages messages)
        {
            Messages = messages.Messages;
        }

        public ValidationMessages Union(ValidationMessages other)
        {
            ValidationMessages newMessages = new ValidationMessages();
            newMessages.Messages = Messages.Union(other.Messages).ToList();
            newMessages.NumberOfStandardsErrors = NumberOfStandardsErrors + other.NumberOfStandardsErrors;
            newMessages.NumberOfStandardsWarnings = NumberOfStandardsWarnings + other.NumberOfStandardsWarnings;
            return newMessages;
        }


        public ValidationMessages(params ValidationMessage[] messages)
        {
            Messages = messages.ToList();
        }

        public ValidationMessages(List<ValidationMessage> messages)
        {
            Messages = messages;
        }

        public void Add(ValidationMessages messages)
        {
            Messages = Messages.Concat(messages.Messages).ToList();
        }

        public ValidationMessages Add(ValidationMessage message)
        {
            Messages.Add(message);
            return this;
        }
        public ValidationMessages AddError(string key, string message)
        {
            Messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, key, message));
            return this;
        }

        public ValidationMessages AddError(string key, string id, string message, string technicalMessage)
        {
            Messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, key, id, message, technicalMessage));
            return this;
        }
        public ValidationMessages AddWarning(string key, string message)
        {
            Messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Warning, key, message));
            return this;
        }
        public ValidationMessages AddWarning(string key, string id, string message, string technicalMessage)
        {
            Messages.Add(new ValidationMessage(ValidationMessage.ValidationSeverity.Warning, key, id, message, technicalMessage));
            return this;
        }
        public void AddException(string message, System.Exception exception)
        {
            Messages.Add(new ValidationMessage() { Severity = ValidationMessage.ValidationSeverity.Error, Message = message, Exception = exception });
        }

        public void AddException(string key, string message, System.Exception exception)
        {
            Messages.Add(new ValidationMessage() { Key = key, Severity = ValidationMessage.ValidationSeverity.Error, Message = message, Exception = exception });
        }
        public void AddTidyError(string message, int lineNumber, int columnNumber)
        {
            Messages.Add(new ValidationMessage()
            {
                Severity = ValidationMessage.ValidationSeverity.Error,
                Message = message,
                LineNumber = lineNumber,
                ColumnNumber = columnNumber,
                MessageSource = ValidationMessage.MessageSourceType.Tidy
            });
            NumberOfStandardsErrors++;
        }

        public void AddTidyWarning(string message, int lineNumber, int columnNumber)
        {
            Messages.Add(new ValidationMessage()
            {
                Severity = ValidationMessage.ValidationSeverity.Warning,
                Message = message,
                LineNumber = lineNumber,
                ColumnNumber = columnNumber,
                MessageSource = ValidationMessage.MessageSourceType.Tidy
            });
            NumberOfStandardsWarnings++;
        }

        public void AddTidyInformation(string message, int lineNumber, int columnNumber)
        {
            Messages.Add(new ValidationMessage()
            {
                Severity = ValidationMessage.ValidationSeverity.Information,
                Message = message,
                LineNumber = lineNumber,
                ColumnNumber = columnNumber,
                MessageSource = ValidationMessage.MessageSourceType.Tidy
            });

        }

        public int NumberOfErrors
        {
            get { return Messages.Where(m => m.Severity == ValidationMessage.ValidationSeverity.Error).Count(); }
        }

        public bool Any()
        {
            return Messages.Count > 0;
        }


        public static ValidationMessages CreateError(string key, string message)
        {
            ValidationMessages messages = new ValidationMessages();
            messages.AddError(key, message);
            return messages;
        }

        public IEnumerable<ValidationMessage> Errors()
        {
            return Messages.Where(m => m.Severity == ValidationMessage.ValidationSeverity.Error);
        }

        public IEnumerable<ValidationMessage> Warnings()
        {
            return Messages.Where(m => m.Severity == ValidationMessage.ValidationSeverity.Warning);
        }

        public IEnumerable<ValidationMessage> Infos()
        {
            return Messages.Where(m => m.Severity == ValidationMessage.ValidationSeverity.Information);
        }

        public override string ToString()
        {
            return string.Join(", ", Messages.Select(m => m.ToString()).ToArray());
        }
    }
}
