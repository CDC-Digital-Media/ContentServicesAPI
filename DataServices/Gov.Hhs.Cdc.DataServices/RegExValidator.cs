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
using System.Text.RegularExpressions;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices
{
    public class RegExValidator
    {
        private string ValidationKey { get; set; }
        private ValidationMessages ValidationMessages { get; set; }

        public RegExValidator(ValidationMessages validationMessages, string validationKey)
        {
            ValidationMessages = validationMessages;
            ValidationKey = validationKey;
            RegularExpressions = new Dictionary<string,Regex>();
        }

        private Dictionary<string, Regex> RegularExpressions;
        private Regex Get(string name, string expression)
        {
            if (RegularExpressions.ContainsKey(name))
            {
                return RegularExpressions[name];
            }
            Regex regex = new Regex(expression);
            RegularExpressions.Add(name, regex);
            return regex;
        }

        public delegate bool StringValidationTest(string value);
        public void IsValid(StringValidationTest test, string value, bool required, string message)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (required)
                {
                    ValidationMessages.AddError(ValidationKey, message);
                }
                return;
            }
            if (string.IsNullOrEmpty(value) || !test(value))
            {
                ValidationMessages.AddError(ValidationKey, message);
            }
        }

        public bool Any(string value)
        {
            return Get("Any", @".*").IsMatch(value);
        }

        public bool Alpha(string value)
        {
            return Get("Alpha", @"^[a-zA-Z]*$").IsMatch(value);
        }

        public bool AlphaSpaces(string value)
        {
            return Get("AlphaSpaces", @"^[a-zA-Z\s]*$").IsMatch(value);
        }

        public bool Numeric(string value)
        {
            return Get("Numeric", @"^[\d]*$").IsMatch(value);
        }

        public bool AlphanumericSpacesPunctuationAmpersand(string value)
        {
            return Get("AlphaNumericSpacesPunctuationAmpersand", @"^[\w\s.,;:()-_'|&]*$")
                .IsMatch(value);
        }

        public bool AlphanumericSpacesPunctuation(string value)
        {
            return Get("AlphaNumericSpacesPunctuation", @"^[\w\s.,;:()-_'|]*$")
                .IsMatch(value);
        }

        public bool AlphanumericSpacesSymbolTastic(string value) {
            return Get("AlphanumericSpacesSymbolTastic", @"^[\w\s.,;:~{}&`%#!$\(\)\-_\'\|]*$")
                .IsMatch(value);            
        }

        public bool AlphanumericSpaces(string value)
        {
            return Get("AlphaNumericSpaces", @"^[\w\s]*$")
                .IsMatch(value);
        }

        public bool Alphanumeric(string value)
        {
            return Get("AlphaNumeric", @"^[\w]*$")
                .IsMatch(value);
        }

        public bool Email(string value)
        {
            return Get("Email", @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b")
                .IsMatch(value);
        }

        //const string UrlFormat = @"/((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[.\!\/\\w]*))?)/";
        public static bool Url(string value)
        {
            Uri theUri;
            return Uri.TryCreate(value, UriKind.Absolute, out theUri);
        }

        const string PasswordRegx = @"(?=^.{8,80}$)((?=.*\d)(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[^A-Za-z0-9])(?=.*[a-z])|(?=.*[^A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]))^.*";
        public void MeetsPasswordComplexityRequirements(string value, string message)
        {
            if (string.IsNullOrEmpty(value) || (!(new Regex(PasswordRegx)).IsMatch(value)))
            {
                ValidationMessages.AddError(ValidationKey, message);
            }
        }
    }
}
