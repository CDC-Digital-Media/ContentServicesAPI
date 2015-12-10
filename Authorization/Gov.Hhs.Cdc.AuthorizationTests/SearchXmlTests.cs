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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gov.Hhs.Cdc.AuthorizationTests
{
    [TestClass]
    public class SearchXmlTests
    {
        [TestMethod]
        public void CanGenerateAdminSearchXml()
        {
            var owningOrgInUse = "11"; //OADC

            //criteria to return media id 145744
            var status = "Published";
            var mediaType = "Collection";
            var topic = "improving health metrics";
            var source = "Centers for Disease Control and Prevention";
            var language = "English";

            var search = new CsBusinessObjects.Media.SearchCriteria { OwningOrganization = owningOrgInUse, Status = status, MediaType = mediaType, Language = language, Topic = topic, Source = source };
            var output = SearchXmlTests.SerializeSearch(search);

            Assert.IsFalse(string.IsNullOrEmpty(output), output);
            Assert.IsTrue(output.Contains("<OwningOrganization>11</OwningOrganization>"), output);
        }

        [TestMethod]
        public void SearchContainsAdminuser()
        {
            var search = new CsBusinessObjects.Media.SearchCriteria { AdminUserGuid = "CEB728AA-9EAA-44BB-B6BF-8D3F43437EDD" };
            var output = SerializeSearch(search);
            Assert.IsTrue(output.Contains("<AdminUserGuid>CEB728AA-9EAA-44BB-B6BF-8D3F43437EDD</AdminUserGuid>"));
        }

        [TestMethod]
        public void ListsAreCommaDelimited()
        {
            var language = "English,Spanish";

            var search = new CsBusinessObjects.Media.SearchCriteria { Language = language };
            var output = SearchXmlTests.SerializeSearch(search);

            Assert.IsFalse(string.IsNullOrEmpty(output), output);
            Assert.IsTrue(output.Contains("<Language>" + language + "</Language>"), output);
        }

        public static string SerializeSearch(CsBusinessObjects.Media.SearchCriteria search)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    var xns = new XmlSerializerNamespaces();
                    xns.Add(string.Empty, string.Empty);
                    var serializer = new XmlSerializer(typeof(CsBusinessObjects.Media.SearchCriteria));
                    serializer.Serialize(writer, search, xns);
                    var output = XElement.Parse(Encoding.ASCII.GetString(stream.ToArray())).ToString();
                    Console.WriteLine(output);
                    return output;
                }
            }
        }

        [TestMethod]
        public void CanGenerateStorefrontSearchXml()
        {
            var search = new CsBusinessObjects.Media.SearchCriteria
            {
                MediaType = " ",
                Title = " ",
                ExactTitle  = " ",
                Description = " ",
                ExactDescription = " ",
                Topic = " ",
                ContentGroup = " ",
                TopicId = " ",
                Audience = " ",
                Language = " ",
                LanguageIsoCode = " ",
                SourceName = " ",
                SourceNameExact = " ",
                SourceAcronym = " ",
                SourceAcronymExact = " ",
                SourceUrl = " ",
                SourceUrlExact = " ",
                ShowChildLevel = " ",
                ShowParentLevel = " ",
                GeoName = " ",
                GeoNameId = " ",
                GeoParentId = " ",
                CountryCode = " ",
                Latitude = " ",
                Longitude = " ",
                DateContentAuthored = " ",
                ContentAuthoredSinceDate = " ",
                ContentAuthoredBeforeDate = " ",
                ContentAuthoredInRange = " ",
                DateContentUpdated = " ",
                ContentUpdatedSinceDate = " ",
                ContentUpdatedBeforeDate = " ",
                ContentUpdatedInRange = " ",
                DateContentPublished = " ",
                ContentPublishedSinceDate = " ",
                ContentPublishedBeforeDate = " ",
                ContentPublishedInRange = " ",
                DateContentReviewed = " ",
                ContentReviewedSinceDate = " ",
                ContentReviewedBeforeDate = " ",
                ContentReviewedInRange = " ",
                DateSyndicationCaptured = " ",
                SyndicationCapturedSinceDate = " ",
                SyndicationCapturedBeforeDate = " ",
                SyndicationCapturedInRange = " ",
                DateSyndicationUpdated = " ",
                SyndicationUpdatedSinceDate = " ",
                SyndicationUpdatedBeforeDate = " ",
                SyndicationUpdatedInRange = " ",
                DateSyndicationVisible = " ",
                SyndicationVisibleSinceDate = " ",
                SyndicationVisibleBeforeDate = " ",
                SyndicationVisibleInRange = " "
            };
            var output = SearchXmlTests.SerializeSearch(search);

        }
    }
}
