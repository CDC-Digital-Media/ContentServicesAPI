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

namespace Gov.Hhs.Cdc.Email
{
    public class FormattedMessage : EmailGeneral
    {

        #region "PROTECTED MEMBERS"

        protected string emailAppendBody;

        protected const string BODY_PLACEHOLDER = "<<!replace body here!>>";

        #endregion

        #region "CONSTRUCTORS"

        public FormattedMessage(string fromEmailAddress, string toEmailAddress, string subject, string appendBody,
            bool isHtml = true, string header = "", string footer = "")
            : base(fromEmailAddress, toEmailAddress)
        {
            this.Subject = subject;
            this.IsBodyHtml = isHtml;

            // Only modify the body if we have something the add/update
            if (!string.IsNullOrEmpty(appendBody))
            {
                // If there is a placeholder in the template body then replace it with 
                // the new content.  Otherwise, just append it to the end.
                if (appendBody.IndexOf(BODY_PLACEHOLDER) > -1)
                    appendBody = appendBody.Replace(BODY_PLACEHOLDER, appendBody);
            }

            string newline = string.Empty;
            if (!this.IsBodyHtml)
                newline = "\n\n";
            else
                newline = "<br/><br/>";

            this.Body = (string.IsNullOrEmpty(header) ? "" : header + newline) + appendBody + (string.IsNullOrEmpty(footer) ? "" : newline + footer);
        }

        #endregion

        //#region "PROTECTED METHODS"

        //protected void ConstructMessageBody(string appendBody)
        //{
        //    string mailBody = "";

        //    // Only modify the body if we have something the add/update
        //    if (!String.IsNullOrEmpty(appendBody))
        //    {
        //        // If there is a placeholder in the template body then replace it with 
        //        // the new content.  Otherwise, just append it to the end.
        //        if (mailBody.IndexOf(BODY_PLACEHOLDER) > -1)
        //            mailBody = mailBody.Replace(BODY_PLACEHOLDER, appendBody);
        //        else
        //            mailBody += appendBody;
        //    }
        //}

        //#endregion

        #region "PUBLIC METHODS"

        public override void Send()
        {
            base.Send();
        }

        #endregion

    }
}
