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
using System.Xml;

namespace Gov.Hhs.Cdc.EmailProvider
{
    public class EmailDetails
    {
        public string EmailTypeCode { get; set; }

        public enum EmailFormatCode { HTML, Text };
        public EmailFormatCode? FormatCode {
            get
            {
                return _formatCode ?? (_formatCode = GetEmailFormatCode(_stringFormatCode));
            }
            set
            {
                _formatCode = value;
                _stringFormatCode = _formatCode.ToString();
            }
        }
        private EmailFormatCode? _formatCode { get; set; }
        public string _stringFormatCode { get; set; }

        public enum EmailPriorityCode { High, Low, Normal };
        public EmailPriorityCode? PriorityCode
        {
            get
            {
                return _priorityCode ?? (_priorityCode = GetEmailPriorityCode(_stringPriorityCode));
            }
            set
            {
                _priorityCode = value;
                _stringPriorityCode = _priorityCode.ToString();
            }
        }
        private EmailPriorityCode? _priorityCode { get; set; }
        public string _stringPriorityCode { get; set; }

        public string Subject { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }

        public string From { get; set; }

        public string Provider { get; set; }


        public bool IsDefault { get; set; }

        public EmailDetails Merge(EmailDetails otherDetails)
        {
            if (otherDetails == null)
            {
                return this;
            }
            return new EmailDetails()
            {
                EmailTypeCode = EmailTypeCode,
                FormatCode = this.FormatCode ?? otherDetails.FormatCode,
                PriorityCode = this.PriorityCode ?? otherDetails.PriorityCode,
                Subject = this.Subject ?? otherDetails.Subject,
                Header = this.Header ?? otherDetails.Header,
                Body = this.Body ?? otherDetails.Body,
                Footer = this.Footer ?? otherDetails.Footer,
                From = this.From ?? otherDetails.From,
                Provider = this.Provider ?? otherDetails.Provider,
                IsDefault = this.IsDefault
            };
        }

        private static EmailFormatCode? GetEmailFormatCode(string formatCode)
        {
            EmailFormatCode enumFormatCode;
            if (Enum.TryParse<EmailFormatCode>(formatCode, out enumFormatCode))
            {
                return enumFormatCode;
            }
            else
            {
                return null;
            }
        }

        private static EmailPriorityCode? GetEmailPriorityCode(string priorityCode)
        {
            EmailPriorityCode enumPriorityCode;
            if (Enum.TryParse<EmailPriorityCode>(priorityCode, out enumPriorityCode))
            {
                return enumPriorityCode;
            }
            else
            {
                return null;
            }
        }
    }


}
