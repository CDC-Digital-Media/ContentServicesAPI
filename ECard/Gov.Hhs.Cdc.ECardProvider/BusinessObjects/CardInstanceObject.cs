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


namespace Gov.Hhs.Cdc.ECardProvider
{
    public class CardInstanceObject : DataSourceBusinessObject, IValidationObject
    {
        public Guid CardInstanceId { get; set; }
        public int MediaId { get; set; }
        public int CardMessageId { get; set; }
        
        public bool IsSender { get; set; }
        public int ViewCount { get; set; }
        public DateTime? LastViewedDateTime { get; set; }
        public bool IsFromMobile { get; set; }
        public bool IsActive { get; set; }

        #region PropertiesNotPersisted
        public string RecipientName { get; set; }
        public string RecipientEmailAddress { get; set; }
        #endregion

        public CardInstanceObject()
        {
        }

        /// <summary>
        /// Constructor for the API to call
        /// </summary>
        /// <param name="recipientName"></param>
        /// <param name="recipientEmailAddress"></param>
        public CardInstanceObject(string recipientName, string recipientEmailAddress, bool isFromMobile)
        {
            RecipientName = recipientName;
            RecipientEmailAddress = recipientEmailAddress;
            IsFromMobile = isFromMobile;
            IsActive = true;
        }

        public CardInstanceObject(string recipientName, string recipientEmailAddress, bool isSender, int mediaId)
        {
            RecipientName = recipientName;
            RecipientEmailAddress = recipientEmailAddress;
            IsSender = isSender;
            MediaId = mediaId;
            IsActive = true;
        }
    }
}
