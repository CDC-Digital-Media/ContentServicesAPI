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

using System.Linq;
using Gov.Hhs.Cdc.EmailProvider;

namespace Gov.Hhs.Cdc.CdcEmailProvider
{
    public class EmailDetailsCtl
    {

        public static IQueryable<EmailDetails> GetEmailDetails(EmailObjectContext db)
        {
            IQueryable<EmailDetails> emailDetails = from t in db.EmailDbEntities.EmailTemplates
                    select new EmailDetails()
                {
                    EmailTypeCode = t.EmailTypeCode,
                    _stringFormatCode = t.EmailFormatCode,
                    _stringPriorityCode = t.EmailPriorityCode,
                    Subject = t.Subject,
                    Header = t.Header,
                    Body = t.Body,
                    Footer = t.Footer,
                    From = t.From,
                    Provider = t.Provider,
                    IsDefault = false
                };
            return emailDetails;
        }

    }
}
