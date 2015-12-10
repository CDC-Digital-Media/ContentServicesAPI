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
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Bo;


namespace SearchUnitTests
{
    [TestClass]
    public class PublicMediaOutputTests
    {
        private static SerialMediaV2 media = TestApiUtility.PublicHtml();
        private static MediaObject searchResult = null;

        public PublicMediaOutputTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            CurrentConnection.Name = "ContentServicesDb";

            var mediaId = media.id;
            var criteria = new SearchCriteria { MediaId = mediaId.ToString() };
            var result = CsMediaSearchProvider.Search(criteria);

            Assert.AreEqual(1, result.Count(), mediaId);

            searchResult = result.First();
        }

        [TestMethod]
        public void IdsMatch()
        {
            Assert.IsNotNull(searchResult);
            Assert.AreEqual(media.id, searchResult.Id);
        }

        [TestMethod]
        public void NamesMatch()
        {
            Assert.AreEqual(media.name, searchResult.Name);
        }

        [TestMethod]
        public void DescriptionsMatch()
        {
            Assert.AreEqual(media.description, searchResult.Description);
        }

        [TestMethod]
        public void MediaTypesMatch()
        {
            Assert.AreEqual(media.mediaType, searchResult.MediaTypeCode);
        }

        [TestMethod]
        public void LanguageNamesMatch()
        {
            Assert.AreEqual(media.language.name, searchResult.LanguageCode);
        }

        [TestMethod]
        public void LanguageIsoCodesMatch()
        {
            Assert.AreEqual(media.language.isoCode, searchResult.LanguageIsoCode);
        }

        [TestMethod]
        public void TopicIdsMatch()
        {
            //TODO:  Think about convertic TopicJSON to plain AttributeJSON so we can support more types

            var ids1 = media.tags.Where(t => t.type == "Topic").Select(t => t.id).ToList();
            var ids2 = searchResult.AttributeValues.Where(a => a.AttributeName == "Topic").Select(t => t.ValueKey.Id).ToList();
            CollectionAssert.AreEqual(ids1, ids2);
        }

        [TestMethod]
        public void GeoTagCountsMatch()
        {
            if (media.geoTags.Count() == 0 && !searchResult.HasGeoData) return;
            //TODO:  Write test that uses one of Scott's IDs
            Assert.AreEqual(media.geoTags.Count(), searchResult.MediaGeoData.Count());
        }

        [TestMethod]
        public void CampaignsAreEmtpy()
        {
            Assert.AreEqual(0, media.campaigns.Count());
        }

        [TestMethod]
        public void SourceIsNotNull()
        {
            Assert.IsNotNull(searchResult.Source);
        }

        [TestMethod]
        public void SourceNamesMatch()
        {
            Assert.AreEqual(media.source.name, searchResult.Source.Code);
        }

        [TestMethod]
        public void SourceAcronymsMatch()
        {
            Assert.AreEqual(media.source.acronym, searchResult.Source.Acronym);
            Assert.AreEqual(media.source.acronym, searchResult.SourceAcronym);
        }

        [TestMethod]
        public void SourceLargeLogosMatch()
        {
            Assert.AreEqual(media.source.largeLogoUrl, searchResult.Source.LogoLargeUrl);
        }

        [TestMethod]
        public void SourceSmallLogosMatch()
        {
            Assert.AreEqual(media.source.smallLogoUrl, searchResult.Source.LogoSmallUrl);
        }

        [TestMethod]
        public void AttributionsMatch()
        {
            Assert.AreEqual(media.attribution, searchResult.Attribution);
        }

        [TestMethod]
        public void DomainNamesMatch()
        {
            Assert.AreEqual(media.domainName, searchResult.DomainName);
        }

        [TestMethod]
        public void OwningOrgNamesMatch()
        {
            Assert.AreEqual(media.owningOrgName, searchResult.OwningBusinessUnitName);
        }

        [TestMethod]
        public void OwningOrgIdsMatch()
        {
            Assert.AreEqual(media.owningOrgId, searchResult.OwningBusinessUnitId);
        }

        [TestMethod]
        public void MaintainingOrgNamesMatch()
        {
            Assert.AreEqual(media.maintainingOrgName, searchResult.MaintainingBusinessUnitName);
        }

        [TestMethod]
        public void MaintainingOrgIdsMatch()
        {
            Assert.AreEqual(media.maintainingOrgId, searchResult.MaintainingBusinessUnitId);
        }

        [TestMethod]
        public void SourceUrlsMatch()
        {
            Assert.AreEqual(media.sourceUrl, searchResult.SourceUrl);
        }

        [TestMethod]
        public void TargetUrlsMatch()
        {
            Assert.AreEqual(media.targetUrl, searchResult.TargetUrl);
        }

        [TestMethod]
        public void PersistentUrlsMatch()
        {
            Assert.AreEqual(media.persistentUrlToken, searchResult.PersistentUrlToken);
        }

        [TestMethod]
        public void AlternateImageCountsMatch()
        {
            if (media.alternateImages.Count() == 0 && searchResult.AlternateImages == null) return;
            //TODO:  Write test that uses ID list ID that has these
            Assert.AreEqual(media.alternateImages.Count(), searchResult.AlternateImages.Count());
        }

        [TestMethod]
        public void AlternateTextsMatch()
        {
            Assert.AreEqual(media.alternateText, searchResult.AlternateText);
        }

        [TestMethod]
        public void NoScriptsMatch()
        {
            Assert.AreEqual(media.noScriptText, searchResult.NoScriptText);
        }

        [TestMethod]
        public void FeaturedTextsMatch()
        {
            Assert.AreEqual(media.featuredText, searchResult.FeaturedText);
        }

        [TestMethod]
        public void EmbedCodesMatch()
        {
            Assert.AreEqual(media.embedCode, searchResult.Embedcode, media.id.ToString());
        }

        [TestMethod]
        public void AuthorsMatch()
        {
            Assert.AreEqual(media.author, searchResult.Author);
        }

        [TestMethod]
        public void LengthsMatch()
        {
            if (!searchResult.Length.HasValue && string.IsNullOrEmpty(media.length)) return;
            Assert.AreEqual(media.length, searchResult.Length);
        }

        [TestMethod]
        public void SizesMatch()
        {
            Assert.AreEqual(media.size, searchResult.Size);
        }

        [TestMethod]
        public void HeightsMatch()
        {
            Assert.AreEqual(media.height, searchResult.Height);
        }

        [TestMethod]
        public void WidthsMatch()
        {
            Assert.AreEqual(media.width, searchResult.Width);
        }

        [TestMethod]
        public void ChildCountsMatch()
        {
            Assert.AreEqual(media.childCount, searchResult.Children == null ? 0 : searchResult.Children.Count());
        }

        [TestMethod]
        public void ParentCountsMatch()
        {
            Assert.AreEqual(media.parentCount, searchResult.Parents == null ? 0 : searchResult.Parents.Count());
        }

        [TestMethod]
        public void RatingIsNotAThing()
        {
            Assert.IsNull(media.rating); //no searchResult equivalent
        }

        [TestMethod]
        public void RatingCountIsNotAThing()
        {
            Assert.IsNull(media.ratingCount); //no searchResult equivalent
        }

        [TestMethod]
        public void RatingCommentCountIsNotAThing()
        {
            Assert.IsNull(media.ratingCommentCount);
        }

        [TestMethod]
        public void StatusesMatch()
        {
            Assert.AreEqual(media.status, searchResult.MediaStatusCode);
        }

        [TestMethod]
        public void PublishDatesMatch()
        {
            Assert.IsTrue(media.datePublished.ParseUtcDateTime().Matches(searchResult.PublishedDateTime));
        }

        [TestMethod]
        public void ModifiedDatesMatch()
        {
            Assert.IsTrue(media.dateModified.ParseUtcDateTime().Matches(searchResult.ModifiedDateTime));
        }

        [TestMethod]
        public void ContentAuthoredDatesMatch()
        {
            Assert.IsTrue(media.dateContentAuthored.ParseUtcDateTime().Matches(searchResult.DateContentAuthored));
        }

        [TestMethod]
        public void ContentUpdatedDatesMatch()
        {
            Assert.IsTrue(media.dateContentUpdated.ParseUtcDateTime().Matches(searchResult.DateContentUpdated));
        }

        [TestMethod]
        public void ContentPublishedDatesMatch()
        {
            Assert.IsTrue(media.dateContentPublished.ParseUtcDateTime().Matches(searchResult.DateContentPublished));
        }

        [TestMethod]
        public void ContentReviewedDatesMatch()
        {
            Assert.IsTrue(media.dateContentReviewed.ParseUtcDateTime().Matches(searchResult.DateContentReviewed));
        }

        [TestMethod]
        public void SyndicationCapturedDatesMatch()
        {
            Assert.IsTrue(media.dateSyndicationCaptured.ParseUtcDateTime().Matches(searchResult.DateSyndicationCaptured));
        }

        [TestMethod]
        public void SyndicationUpdatedDatesMatch()
        {
            Assert.IsTrue(media.dateSyndicationUpdated.ParseUtcDateTime().Matches(searchResult.DateSyndicationUpdated));
        }

        [TestMethod]
        public void SyndicationVisibleDatesMatch()
        {
            Assert.IsTrue(media.dateSyndicationVisible.ParseUtcDateTime().Matches(searchResult.DateSyndicationVisible));
        }

        [TestMethod]
        public void ExtendedAttributeCountsMatch()
        {
            Assert.AreEqual(media.extendedAttributes.Count(), searchResult.ExtendedAttributes == null ? 0 : searchResult.ExtendedAttributes.Count());
        }

        [TestMethod]
        public void ExtensionIsEmpty()
        {
            Assert.AreEqual("html", media.mediaType.ToLower());
            var ext = media.extension as Dictionary<string, object>;
            var keys = ext.Select(o => o.Key).ToArray();          
            var items = string.Join(",", keys);

            Assert.AreEqual(0, ext.Count, items);
        }

        [TestMethod]
        public void PageCountsMatch()
        {
            if (string.IsNullOrEmpty(media.pageCount) && !searchResult.DocumentPageCount.HasValue) return;
            Assert.AreEqual(media.pageCount, searchResult.DocumentPageCount.ToString());
        }

        [TestMethod]
        public void DataSizesMatch()
        {
            Assert.AreEqual(media.dataSize, searchResult.DocumentDataSize);
        }
    }
}
