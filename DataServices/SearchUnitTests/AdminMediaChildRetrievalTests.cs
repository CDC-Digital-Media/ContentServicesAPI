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

namespace SearchUnitTests
{
    [TestClass]
    public class AdminMediaChildRetrievalTests
    {
        public AdminMediaChildRetrievalTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        static string mediaIdWithChildren = ""; //feed with 3 items
        private static SerialMediaAdmin media = new SerialMediaAdmin();
        //{
        //    mediaId = mediaIdWithChildren,
        //    childCount = 3,
        //    childRelationshipCount = 3,
        //    childRelationships = new List<SerialMediaRelationship> { 
        //        new SerialMediaRelationship {  relatedMediaId = 171279 },
        //        new SerialMediaRelationship { relatedMediaId = 195280}, 
        //        new SerialMediaRelationship { relatedMediaId = 177872}
        //    },
        //    children = new List<SerialMediaAdmin> { 
        //        new SerialMediaAdmin { mediaId = "171279" },
        //        new SerialMediaAdmin { mediaId = "195280"},
        //        new SerialMediaAdmin{ mediaId = "177872"}
        //    }
        //};
        private static MediaObject searchResult = null;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            CurrentConnection.Name = "ContentServicesDb";

            //var criteria = new SearchCriteria { MediaId = mediaIdWithChildren.ToString() };
            //get media with children

            var result = TestApiUtility.SinglePublishedFeedWithChildren(); //CsMediaSearchProvider.Search(criteria);

            Assert.IsNotNull(result);

            //searchResult = result.First();

            var mediaId = result.mediaId;
            var criteria = new SearchCriteria { MediaId = mediaId };

            searchResult = CsMediaSearchProvider.Search(criteria).First();
            mediaIdWithChildren = searchResult.MediaId.ToString();
            media.childCount = searchResult.Children.Count;
            List<MediaRelationshipObject> relationships = searchResult.MediaRelationships.Where(i => i.RelationshipTypeName == "Is Parent Of").ToList();
            media.childRelationshipCount = relationships.Count;
            media.childRelationships = new List<SerialMediaRelationship>();
            media.children = new List<SerialMediaAdmin>();
            foreach(MediaRelationshipObject relationship in relationships)
            {
                SerialMediaRelationship mediaRelationship = new SerialMediaRelationship();
                SerialMediaAdmin mediaAdmin = new SerialMediaAdmin();
                mediaRelationship.relatedMediaId = relationship.RelatedMediaId;
                mediaRelationship.displayOrdinal = relationship.DisplayOrdinal;
                media.childRelationships.Add(mediaRelationship);
                mediaAdmin.mediaId = relationship.MediaId.ToString();
                media.children.Add(mediaAdmin);
            }
        }

        [TestMethod]
        public void CanRetrieveFeedItemsByParentFeed()
        {
            var criteria = new SearchCriteria { ParentId = searchResult.MediaId.ToString() };
            var result = CsMediaSearchProvider.Search(criteria);

            Assert.IsTrue(result.Count() > 0);
        }

        [TestMethod]
        public void ChildRelationshipCountsMatch()
        {
            //TODO:  Write test that looks for media with this value
            Assert.AreEqual(media.childRelationshipCount, searchResult.Children == null ? 0 : searchResult.Children.Count());
        }

        [TestMethod]
        public void ChildRelationshipsMatch()
        {
            if (media.childRelationships.Count == 0 && searchResult.Children == null) return;

            //TODO:  Write test that looks for media with this value
            foreach (var rel in media.childRelationships)
            {
                Assert.IsTrue(searchResult.Children.Any(c => c.MediaId == rel.relatedMediaId));
            }
            foreach (var child in searchResult.Children)
            {
                Assert.IsTrue(media.childRelationships.Any(r => r.relatedMediaId == child.MediaId));
            }
        }

        [TestMethod]
        public void ChildCountsMatch()
        {
            //TODO:  Write test that looks for media with this value
            Assert.AreEqual(media.childCount, searchResult.Children == null ? 0 : searchResult.Children.Count());
        }
    }
}
