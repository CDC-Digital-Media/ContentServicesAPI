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
using System.Linq;
using System.Web;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.ECardProvider;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class ECardHandler : MediaHandlerBase
    {
        public static ValidationMessages SubmitCard(string stream, string id, IOutputWriter writer, string userAgent)
        {
            SerialCardSubmission serialMedia = BaseJsSerializer.Deserialize<SerialCardSubmission>(stream);
            int mediaId;
            if (!int.TryParse(id, out mediaId))
            {
                return writer.Write(ValidationMessages.CreateError("MediaId", "Invalid media id for eCard"));
            }
            else
            {
                return SubmitCard(serialMedia, mediaId, writer, userAgent);
            }
        }

        public static ValidationMessages SubmitCard(SerialCardSubmission submission, int mediaId, IOutputWriter writer, string userAgent)
        {
            if (submission.recipients == null)
            {
                return writer.Write(ValidationMessages.CreateError("instances", "Submission must have at least one recipient"));
            }

            CardSubmission cardSubmission = new CardSubmission(
                submission.recipients.Select(r => new CardInstanceObject(r.name, r.emailAddress, submission.isFromMobile)).ToList(),
                new CardMessageObject(submission.personalMessage, submission.senderName, submission.senderEmail, userAgent),
                mediaId);
            string url = HttpUtility.UrlDecode(submission.eCardApplicationUrl, System.Text.Encoding.UTF8);
            ECardProvider.SubmitCard(cardSubmission, url, sendCopyToSender: false);
            return writer.Write(new ValidationMessages());
        }

        public static void ViewECard(IOutputWriter writer, string cardInstanceId)
        {
            Guid cardInstanceGuid;
            if (!Guid.TryParse(cardInstanceId, out cardInstanceGuid))
            {
                writer.Write(ValidationMessages.CreateError("MediaId", "Invalid card instance id for eCard"));
            }
            else
            {
                ViewECard(cardInstanceGuid, writer);
            }
        }

        public static void ViewECard(Guid cardInstanceId, IOutputWriter writer)
        {
            ValidationMessages validationMessages;
            CardInstance cardInstance = ECardProvider.ViewCard(cardInstanceId, out validationMessages);
            if (validationMessages.Errors().Any())
            {
                writer.Write(validationMessages);
            }

            MediaObject mediaObject = MediaProvider.GetMedia(cardInstance.Instance.MediaId, out validationMessages);
            if (validationMessages.Errors().Any())
            {
                writer.Write(validationMessages);
            }

            SerialCardView cardView = new SerialCardView()
            {
                eCardDetail = ECardDetailTransformation.GetSerialECardDetail((ECardDetail)mediaObject.MediaTypeSpecificDetail),
                mediaItem = new MediaTransformationV2().CreateSerialObject(mediaObject, forExport: false),
                personalMessage = cardInstance.Message.PersonalMessage
            };
            writer.Write(new SerialResponse(cardView));
        }

    }
}
