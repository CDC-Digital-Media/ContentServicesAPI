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
using Gov.Hhs.Cdc.ECardProvider;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.EmailProvider;
using Gov.Hhs.Cdc.CdcECardProvider.DataAccess;
using System.Web;
using System.Collections.Specialized;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcECardProvider
{
    public class CsECardProvider : ECardMgrBase, IECardProvider
    {
        private DataManager ECardDataManager { get { return new DataManager(ObjectContextFactory); } }

        public CardInstance ViewCard(Guid cardInstanceId, out ValidationMessages validationMessages)
        {
            validationMessages = new ValidationMessages();
            using (ECardObjectContext ecardDb = (ECardObjectContext)ObjectContextFactory.Create())
            {
                CardInstanceObject instance = CardInstanceObjectCtl.Get(ecardDb, forUpdate: false).Where(c => c.CardInstanceId == cardInstanceId).FirstOrDefault();
                if (instance == null)
                {
                    validationMessages.AddError("CardInstanceId", "Card was not found with the CardInstanceId passed");
                    return null;
                }
                CardInstance singleCard = new CardInstance()
                {
                    Instance = instance,
                    Message = CardMessageObjectCtl.Get(ecardDb, forUpdate: false).Where(m => m.CardMessageId == instance.CardMessageId).FirstOrDefault(),

                };
                Guid modifiedByGuid = new Guid();
                ecardDb.ECardDbEntities.UpdateCardViewedCount(cardInstanceId, modifiedByGuid);
                return singleCard;
            }
        }

        public ValidationMessages SubmitCard(CardSubmission cardSubmission, string eCardApplicationUrl, bool sendCopyToSender)
        {
            CardSubmission updatedCardSubmission = new CardSubmission(cardSubmission, cardSubmission.MediaId);
            if (sendCopyToSender)
            {
                updatedCardSubmission.Instances.Add(
                       new CardInstanceObject(cardSubmission.Message.SenderName, cardSubmission.Message.SenderEmail, /*isSender:*/true, cardSubmission.MediaId));
            }
            return SendWithUpdatedCardSubmission(updatedCardSubmission, eCardApplicationUrl);
        }

        private ValidationMessages SendWithUpdatedCardSubmission(CardSubmission cardSubmission, string eCardApplicationUrl)
        {
            ValidationMessages messages = ECardDataManager.Insert(new CardSubmissionUpdateMgr(cardSubmission));
            if (cardSubmission.Instances.Count() == 0)
            {
                messages.AddError("Instances", "An ECard submission must have at least one recipient");
            }

            if (messages.Errors().Any())
            {
                return messages;
            }

            foreach (CardInstanceObject instance in cardSubmission.Instances)
            {
                SendECardEmail(cardSubmission, eCardApplicationUrl, instance, instance.RecipientEmailAddress);
            }

            return messages;
        }

        private static void SendECardEmail(CardSubmission cardSubmission, string eCardApplicationUrl, CardInstanceObject instance, string recipientEmailAddress)
        {
            EmailProvider.Send("ECard Sent",
                new
                {
                    SenderName = cardSubmission.Message.SenderEmail,
                    ECardType = "CDC Health-e-Card",
                    ECardSystemLink = AddGidToUrl(eCardApplicationUrl, instance.CardInstanceId),
                    ExpirationDate = DateTime.Now.AddDays(90).ToLongDateString()
                },
                new EmailRouting(recipientEmailAddress),
                new EmailDetails() { From = cardSubmission.Message.SenderEmail });
        }

        private static string ECardReceiptIdParmName = "ecardReceiptId";
        private static string AddGidToUrl(string url, Guid guid)
        {
            //string gidString = EmailReceiptIdParmName + guid;

            Uri uri = new Uri(url);


            NameValueCollection queryStringItems = System.Web.HttpUtility.ParseQueryString(uri.Query);
            IEnumerable<string> gidKeys = queryStringItems.AllKeys.Where(k => string.Equals(k, ECardReceiptIdParmName, StringComparison.OrdinalIgnoreCase));
            if (gidKeys.Any())
            {
                queryStringItems.Remove(gidKeys.FirstOrDefault());
            }

            queryStringItems.Add(ECardReceiptIdParmName, guid.ToString());
            string results = DropQueryString(uri) + ToQueryString(queryStringItems);
            return results;
        }

        private static string DropQueryString(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.Query))
            {
                return uri.AbsoluteUri;
            }
            else
            {
                return uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.IndexOf('?'));
            }

        }
        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

    }
}
