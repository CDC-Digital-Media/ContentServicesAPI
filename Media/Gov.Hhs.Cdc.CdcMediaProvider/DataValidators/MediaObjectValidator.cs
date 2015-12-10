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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using System.Net;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class MediaObjectValidator : IValidator<MediaObject, MediaObject>
    {
    //    private const int FeedTitleMaxLength = 100;
        //MAR - 9/9/2015
            //The spec says the max limit should be 100, but the title coming from youtube, can be longer.  
            //The database field that will store the title is 1024, so let's use this as the upper limit
        private const int FeedTitleMaxLength = 1024;
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<MediaObject> items)
        {
            foreach (MediaObject mediaObject in items)
            {
                RegExValidator v = new RegExValidator(validationMessages, mediaObject.ValidationKey);

                // required fields for all Media Types
                v.IsValid(v.AlphanumericSpacesPunctuation, mediaObject.MediaTypeCode, required: true, message: "MediaType is invalid");
                v.IsValid(v.AlphanumericSpacesPunctuationAmpersand, mediaObject.SourceCode, required: true, message: "Source is invalid");
                v.IsValid(v.AlphanumericSpacesPunctuation, mediaObject.LanguageCode, required: true, message: "Language is invalid");

                if (string.IsNullOrEmpty(mediaObject.CharacterEncodingCode))
                {
                    validationMessages.AddError(mediaObject.ValidationKey + ".Encoding", "Encoding is invalid");
                }
                else
                {
                    if (string.Compare("utf-8", mediaObject.CharacterEncodingCode, true) != 0)
                    {
                        validationMessages.AddError(mediaObject.ValidationKey + ".Encoding", "Encoding is invalid");
                    }
                }

                if (string.IsNullOrEmpty(mediaObject.Title))
                {
                    validationMessages.AddError(mediaObject.ValidationKey + ".Title", "Title is invalid");
                }

                if (mediaObject.MediaStatusCodeValue == MediaStatusCodeValue.Invalid)
                {
                    validationMessages.AddError(mediaObject.ValidationKey + ".Status", "Status is invalid");
                }

                if (mediaObject.PublishedDateTime == null && mediaObject.MediaStatusCodeValue == MediaStatusCodeValue.Published)
                {
                    mediaObject.PublishedDateTime = DateTime.UtcNow;
                }

                if (mediaObject.MediaTypeParms.IsFeedImage) { return; }

                if (mediaObject.MediaStatusCodeValue == MediaStatusCodeValue.Published
                        && (mediaObject.GetAttributeValues("Topic").Count() < 1))
                {
                    validationMessages.AddError(mediaObject.ValidationKey + ".Topics", "At least 1 topic is required");
                }

                if (mediaObject.MediaTypeParms.IsFeed)
                {
                    FeedPreSaveValidate(ref validationMessages, mediaObject);
                }
                else if (mediaObject.MediaTypeParms.IsFeedItem)
                {
                    var messages = FeedItemPreSaveValidate(mediaObject);
                    validationMessages = validationMessages.Union(messages);
                    AuditLogger.LogAuditEvent("feed item error message count: " + validationMessages.NumberOfErrors.ToString());
                }
                else
                {
                    bool urlsAreRequired = !mediaObject.MediaTypeParms.IsCollection;
                    v.IsValid(RegExValidator.Url, mediaObject.SourceUrlForSave, required: urlsAreRequired, message: "SourceUrl is invalid");
                    v.IsValid(RegExValidator.Url, mediaObject.TargetUrl, required: urlsAreRequired, message: "TargetUrl is invalid");
                }

                //TODO: Fix this so that we are handling SourceUrl correctly

                //if( mediaObject.MediaTypeSpecificDetail != null 
                //    && mediaObject.MediaTypeSpecificDetail.GetType() == typeof(ECardDetail)
                //    && !string.IsNullOrEmpty(((ECardDetail)mediaObject.MediaTypeSpecificDetail).Html5Source))
                //{
                //    //If this is eCards and we have an HTML5Source, don't validate the SourceUrl
                //}
                //else
            }
        }

        private ValidationMessages FeedItemPreSaveValidate(MediaObject media)
        {
            var messages = new ValidationMessages();
            if (media == null)
            {
                return messages;
            }

            if (media.Title.Length > FeedTitleMaxLength)
            {
                messages.AddError(media.ValidationKey + ".Title", "Title must be " + FeedTitleMaxLength + " characters or fewer.");
                AuditLogger.LogAuditEvent("invalid feed item title: *" + media.Title + "*");
            }
            if (!media.HasRelationships)
            {
                messages.AddError(media.ValidationKey, "Feed Item does not have a parent.");
                return messages;
            }
            var parent = media.MediaRelationships.FirstOrDefault(mr => mr.RelationshipTypeName == "Is Child Of");
            if (parent == null)
            {
                messages.AddError(media.ValidationKey, "Feed Item does not have a parent.");
                return messages;
            }

            if (string.IsNullOrEmpty(media.SourceUrl))
            {
                messages.AddError(media.ValidationKey, "Source URL is invalid.");
                return messages;
            }

            var criteria = new CsBusinessObjects.Media.SearchCriteria { MediaId = parent.RelatedMediaId.ToString() };
            var results = CsMediaSearchProvider.Search(criteria);
            var item = results.Select(m => m).ToList().FirstOrDefault();
            if (item == null)
            {
                messages.AddError(media.ValidationKey, "Parent media Id " + parent.RelatedMediaId.ToString() + " not found.");
                return messages;
            }
            if (!item.MediaTypeParms.IsFeed)
            {
                messages.AddError(media.ValidationKey, "Parent media (" + parent.RelatedMediaId.ToString() + ") is not a feed.");
            }

            if (item.MediaTypeCode == "Feed")
            {
                //check the other children
                for (int i = 0; i < item.Children.Count; i++)
                {
                    //get the child
                    criteria = new CsBusinessObjects.Media.SearchCriteria { MediaId = item.Children[i].MediaId.ToString() };
                    var childObjectToLookAt = CsMediaSearchProvider.Search(criteria);
                    var childItemToLookAt = childObjectToLookAt.ToList().FirstOrDefault();
                    if (childItemToLookAt.SourceUrl == media.SourceUrl 
                        && WebUtility.HtmlDecode(WebUtility.HtmlDecode(childItemToLookAt.Title)) == WebUtility.HtmlDecode(WebUtility.HtmlDecode(media.Title)) 
                        && childItemToLookAt.MediaId != media.Id)
                    {
                        messages.AddError(media.ValidationKey, "Parent media (" + parent.RelatedMediaId.ToString() + ") has child (" + childItemToLookAt.MediaId.ToString() + ") with the same sourceUrl.");
                        break;
                    }
                }
            }
            //var otherChildrenCount = item.Children.Where(c => c.SourceUrl == media.SourceUrl).ToList().Count;
            //if (otherChildrenCount > 0)
            //{
            //    messages.AddError(media.ValidationKey, "Parent media (" + parent.RelatedMediaId.ToString() + ") has other children with the same sourceUrl.");
            //}

            return messages;
        }
        
        private void FeedPreSaveValidate(ref ValidationMessages validationMessages, MediaObject mediaObject)
        {
            RegExValidator v = new RegExValidator(validationMessages, mediaObject.ValidationKey);
            v.IsValid(RegExValidator.Url, mediaObject.TargetUrl, required: true, message: "TargetUrl is invalid");

            if (string.IsNullOrEmpty(mediaObject.Description))
            {
                validationMessages.AddError(mediaObject.ValidationKey + ".Description", "Description is invalid");
            }

            if (!string.IsNullOrEmpty(mediaObject.Title))
            {
                if (mediaObject.Title.Length > FeedTitleMaxLength)
                {
                    validationMessages.AddError(mediaObject.ValidationKey + ".Title", "Title character count is more than " + FeedTitleMaxLength);
                }
            }

            var feed = mediaObject.MediaTypeSpecificDetail as FeedDetailObject;
            if (feed != null)
            {   
                bool IsRequired = !string.IsNullOrEmpty(feed.Copyright) || !string.IsNullOrEmpty(feed.WebMasterEmail) ||
                    !string.IsNullOrEmpty(feed.EditorialManager);

                v.IsValid(v.Any, feed.Copyright, required: IsRequired, message: "Copyright is invalid");
                if (!string.IsNullOrEmpty(feed.Copyright))
                {
                    if (feed.Copyright.Length > 255)
                    {
                        validationMessages.AddError(mediaObject.ValidationKey + ".Copyright", "Copyright character count is more than 255");
                    }
                }

                //Validate the Email
                v.IsValid(v.Email, feed.WebMasterEmail, required: IsRequired, message: "WebMasterEmail is invalid");
                if (!string.IsNullOrEmpty(feed.WebMasterEmail))
                {
                    if (feed.WebMasterEmail.Length > 100)
                    {
                        validationMessages.AddError(mediaObject.ValidationKey + ".WebMasterEmail", "WebMasterEmail character count is more than 100");
                    }
                }

                v.IsValid(v.Any, feed.EditorialManager, required: IsRequired, message: "EditorialManager is invalid");
                if (!string.IsNullOrEmpty(feed.EditorialManager))
                {
                    if (feed.EditorialManager.Length > 100)
                    {
                        validationMessages.AddError(mediaObject.ValidationKey + ".EditorialManager", "EditorialManager character count is more than 100");
                    }
                }

                FeedImagePreSaveValidate(ref validationMessages, mediaObject, feed);
            }
        }

        private void FeedImagePreSaveValidate(ref ValidationMessages validationMessages, MediaObject mediaObject, FeedDetailObject feed)
        {
            var image = feed.AssociatedImage;
            if (image == null)
            {
                return;
            }


            RegExValidator v = new RegExValidator(validationMessages, mediaObject.ValidationKey);
            if (!string.IsNullOrEmpty(image.Title))
            {
                if (image.Title.Length > 200)
                {
                    validationMessages.AddError(mediaObject.ValidationKey + ".ImageTitle", "ImageTitle character count is more than 200");
                }
            }

            v.IsValid(RegExValidator.Url, image.SourceUrl, required: false, message: "Image Url is invalid");
            if (!string.IsNullOrEmpty(image.SourceUrl))
            {
                if (image.SourceUrl.Length > 1024)
                {
                    validationMessages.AddError(mediaObject.ValidationKey + ".ImageUrl", "Image Url character count is more than 1024");
                }
            }

            v.IsValid(RegExValidator.Url, image.TargetUrl, required: false, message: "Image Link is invalid");
            if (!string.IsNullOrEmpty(image.TargetUrl))
            {
                if (image.TargetUrl.Length > 1024)
                {
                    validationMessages.AddError(mediaObject.ValidationKey + ".ImageLink", "Image Link character count is more than 1024");
                }
            }

            if (image.Height != null)
            {
                v.IsValid(v.Numeric, image.Height.ToString(), required: false, message: "Image Height is invalid");

                //if (image.Height > 400)
                //{
                //    validationMessages.AddError(mediaObject.ValidationKey + ".ImageHeight", "Image Height cannot be greater than 400");
                //}
            }

            if (image.Width != null)
            {
                v.IsValid(v.Numeric, image.Width.ToString(), required: false, message: "Image Width is invalid");

                //if (image.Width > 144)
                //{
                //    validationMessages.AddError(mediaObject.ValidationKey + ".ImageWidth", "Image Width cannot be greater than 144");
                //}
            }
        }

        public void PreDeleteValidate(ValidationMessages validationMessages, IList<MediaObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<MediaObject> items)
        {
            foreach (var item in items)
            {
                // sourceCode validation
                var sourceObj = SourceItemCtl.Get((MediaObjectContext)objectContext)
                                    .Where(a => a.Code == item.SourceCode).FirstOrDefault();
                if (sourceObj == null)
                {
                    validationMessages.AddError(item.ValidationKey + ".SourceCode", item.Id.ToString(), "Source is invalid", "");
                }

                // languageCode validation
                var languageObj = LanguageItemCtl.Get((MediaObjectContext)objectContext)
                                    .Where(a => a.Code == item.LanguageCode).FirstOrDefault();
                if (languageObj == null)
                {
                    validationMessages.AddError(item.ValidationKey + ".Language", item.Id.ToString(), "Language is invalid", "");
                }

                // mediaType validation
                var mediaTypeObj = MediaTypeItemCtl.Get((MediaObjectContext)objectContext)
                                    .Where(a => a.MediaTypeCode == item.MediaTypeCode).FirstOrDefault();
                if (mediaTypeObj == null)
                {
                    validationMessages.AddError(item.ValidationKey + ".MediaType", item.Id.ToString(), "MediaType is invalid", "");
                }

                // OwningOrg validation
                var orgObj = BusinessUnitItemCtl.Get((MediaObjectContext)objectContext);
                if (item.OwningBusinessUnitId != null)
                {
                    var owning = orgObj.Where(a => a.Id == item.OwningBusinessUnitId).FirstOrDefault();
                    if (owning == null)
                    {
                        validationMessages.AddError(item.ValidationKey + ".OwningOrgId", item.Id.ToString(), "OwningOrgId is invalid", "");
                    }
                }

                if (item.MaintainingBusinessUnitId != null)
                {
                    var maintaining = orgObj.Where(a => a.Id == item.MaintainingBusinessUnitId).FirstOrDefault();
                    if (maintaining == null)
                    {
                        validationMessages.AddError(item.ValidationKey + ".MaintainingOrgId", item.Id.ToString(), "MaintainingOrgId is invalid", "");
                    }
                }
            }
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<MediaObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<MediaObject> items)
        {
        }

        public MediaObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, MediaObject theObject)
        {
            return theObject;
        }
    }
}
