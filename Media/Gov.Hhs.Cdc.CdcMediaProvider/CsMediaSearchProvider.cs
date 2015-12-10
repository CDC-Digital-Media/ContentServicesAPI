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
using System.Reflection;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.CsBusinessObjects.Media;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class CsMediaSearchProvider : BaseCsSearchProvider
    {
        public override string ApplicationCode { get { return "Media"; } }

        public override string Code { get { return "MediaSearchProvider"; } }
        public override string Name { get { return "Content Services Media Search Provider"; } }

        public List<Assembly> _businessObjectAssemblies = new List<Assembly>() { typeof(MediaProviderSearchBusinessObjectPlaceHolder).Assembly, typeof(CdcMediaProviderSearchBusinessObjectPlaceHolder).Assembly };
        public override List<Assembly> BusinessObjectAssemblies
        {
            get { return _businessObjectAssemblies; }
        }
        
        private static IObjectContextFactory ObjectContextFactory { get; set; }

        public static void Inject(IObjectContextFactory objectContextFactory)
        {
            ObjectContextFactory = objectContextFactory;
        }

        public override ISearchDataManager GetSearchDataManager(string filterCode)
        {
            //Get DataSets for Search Objects
            switch (SafeToLower(filterCode))
            {
                case "media":
                    return new MediaSearchMgr();
                case "mediatype":
                    return new MediaTypeSearchDataMgr();
                case "language":
                    return new LanguageSearchDataMgr();
                case "source":
                    return new SourceSearchDataMgr();
                case "status":
                    return new MediaStatusSearchDataMgr();
                case "valueset":
                    return new ValueSetSearchDataMgr();
                case "location":
                    return new LocationSearchDataMgr();
                case "hiervocabvalue":
                    return new HierVocabValueSearchDataMgr();
                case "flatvocabvalue":
                    return new FlatVocabValueSearchDataMgr();
                case "businessunit":
                    return new BusinessUnitSearchDataMgr();
                case "businessunittype":
                    return new BusinessUnitTypeSearchDataMgr();
                case "tag":
                    return new TagSearchMgr();
                case "tagtype":
                    return new TagTypeSearchMgr();
                case "atoz":
                    return new AToZSearchMgr();
                case "link":
                    return new StorageSearchMgr();
                case "persistenturl":
                    return new PersistentUrlSearchMgr();
                case "feedformat":
                    return new FeedFormatSearchDataMgr();
            }

            throw new ApplicationException(
                string.Format("The search data manager '{0}' has not been defined in {1}.GetSearchDataManager()",
                    filterCode, this.GetType().Name));
        }

        public static IEnumerable<MediaObject> Search(SearchCriteria criteria)
        {
            if (ObjectContextFactory == null)
            {
                ObjectContextFactory = new MediaObjectContextFactory();
            }
            using (MediaObjectContext mediaDb = (MediaObjectContext)ObjectContextFactory.Create())
            {
                return MediaCtl.Search(mediaDb, criteria.ToXmlString());
            }
        }

    }
}
