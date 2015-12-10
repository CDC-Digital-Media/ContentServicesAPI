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
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.ECardProvider
{
    public class CardMessageObject : DataSourceBusinessObject, IValidationObject
    {
        public int CardMessageId { get; set; }
        public string PersonalMessage { get; set; }
        
        public string StyleSheet { get; set; }
        public string SenderUserAgent { get; set; }
        public bool IsFromMobile { get; set; }

        #region PropertiesNotPersisted
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        #endregion

        public CardMessageObject()
        {
        }

        /// <summary>
        /// Constructor for the API to call
        /// </summary>
        /// <param name="personalMessage"></param>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        public CardMessageObject(string personalMessage, string senderName, string senderEmail, string senderUserAgent)
        {
            PersonalMessage = personalMessage;
            SenderName = senderName;
            SenderEmail = senderEmail;
            SenderUserAgent = senderUserAgent;
            IsFromMobile = senderUserAgent.ContainsAnyIgnoreCase(MobileUserAgents);
        }

        private static List<string> MobileUserAgents = new List<string>() { "CDC eCards iPhone app", "AppleWebKit", "Mobile" };

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="oldCardMessageObject"></param>
        public CardMessageObject(CardMessageObject oldCardMessageObject)
        {
            CardMessageId = oldCardMessageObject.CardMessageId;
            PersonalMessage = oldCardMessageObject.PersonalMessage;
            SenderName = oldCardMessageObject.SenderName;
            SenderEmail = oldCardMessageObject.SenderEmail;
            StyleSheet = oldCardMessageObject.StyleSheet;
        }
    }
}
