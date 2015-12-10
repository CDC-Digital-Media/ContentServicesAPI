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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Search.Provider;
using Gov.Hhs.Cdc.DataServices;

namespace Gov.Hhs.Cdc.DataSource.Media
{
    public class MediaSearchProvider : BaseCsSearchProvider
    {
        protected override string ApplicationCode { get { return "Media"; } }
        public override List<string> SupportsApplications { get { return new List<string> { "Media" }; } }

        public override string Code { get { return "MediaSearchProvider"; } }
        public override string Name { get { return "Content Services Media Search Provider"; } }

        public MediaSearchProvider(IFilterCriteriaProvider filterCriteriaMgr, IObjectContextFactory objectContextFactory)
            : base(filterCriteriaMgr, objectContextFactory)
        {
        }


        public override ISearchDataManager GetSearchDataManager(string filterCode)
        {
            //Get DataSets for Search Objects
            switch (SafeToLower(filterCode))
            {
                case "combinedmedia":
                    return new CombinedMediaSearchDataMgr(ConfigItems);
                case "ecard":
                    return new ECardSearchDataMgr(ConfigItems);
                case "mediatype":
                    return new MediaTypeSearchDataMgr(ConfigItems);
                case "language":
                    return new LanguageSearchDataMgr(ConfigItems);
                case "source":
                    return new SourceSearchDataMgr(ConfigItems);
                case "valueset":
                    return new ValueSetSearchDataMgr(ConfigItems);

                //case "vocabulary":
                    //return new VocabularySearchDataMgr(application, filter, ConfigItems);
                case "hiervocabvalue":
                    return new HierVocabValueSearchDataMgr(ConfigItems);
                case "flatvocabvalue":
                    return new FlatVocabValueSearchDataMgr(ConfigItems);

            }

            throw new ApplicationException(
                string.Format("The search data manager '{0}' has not been defined in {1}.GetSearchDataManager()",
                    filterCode, this.GetType().Name));
        }

        //public IDataManager GetDataManager(DsApplication application, Type dataManagerType, EntitiesConfigurationItems ConfigItems)
        //{
        //    IDataManager dataManager = (IDataManager)Activator.CreateInstance(dataManagerType, new object[] { application, ConfigItems });

        //    if( dataManagerType == typeof(MediaObjectMgr))
        //        return new MediaObjectMgr(application, ConfigItems);
        //    if (dataManagerType == typeof(ECardObjectMgr))
        //        return new ECardObjectMgr(application, ConfigItems);
        //    if (dataManagerType == typeof(HtmlObjectMgr))
        //        return new HtmlObjectMgr(application, ConfigItems);
        //    if (dataManagerType == typeof(ValueSetObjectMgr))
        //        return new ValueSetObjectMgr(application, ConfigItems);

        //    throw new ApplicationException(
        //        string.Format("The data manager '{0}' has not been defined in {1}.GetDataManager()",
        //        dataManagerType.ToString(), this.GetType().Name));
        //}


    }
}
