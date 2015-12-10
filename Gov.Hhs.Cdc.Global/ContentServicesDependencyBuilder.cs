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

using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.CdcECardProvider;
using Gov.Hhs.Cdc.CdcEmailProvider;
using Gov.Hhs.Cdc.CdcMediaProvider;
using Gov.Hhs.Cdc.CdcRegistrationProvider;
using Gov.Hhs.Cdc.DataProvider;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.Search.Provider;
using Gov.Hhs.Cdc.ThumbnailProvider;

namespace Gov.Hhs.Cdc.Global
{

    public static class ContentServicesDependencyBuilder
    {
        private static ISearchControllerFactory _searchControllerFactory;
        public static ISearchControllerFactory SearchControllerFactory
        {
            get { return _searchControllerFactory ?? (_searchControllerFactory = new SearchControllerFactory()); }
        }

        //New
        //
        static bool IsBuilt = false;

        public static void BuildAssembliesWithTestEMailProvider()
        {

            if (IsBuilt)
            {
                return;
            }
            IocContainers containers = BuildContainers();
            containers.EmailProviderIocContainer = new TestCdcEmailProviderIocContainer();
            Inject(containers, BuildSearchProviderList());

            IsBuilt = true;

        }

        private static ISearchProviders BuildSearchProviderList()
        {
            ISearchProviders allSearchProviders = new SearchProviders(new CsMediaSearchProvider(), new RegistrationSearchProvider(), new DataSearchProvider());
            return allSearchProviders;
        }

        public static void BuildAssemblies()
        {
            if (IsBuilt)
            {
                return;
            }
            IocContainers containers = BuildContainers();
            Inject(containers, BuildSearchProviderList());

            IsBuilt = true;
        }

        private static IocContainers BuildContainers()
        {
            IocContainers containers = new IocContainers()
            {
                EmailProviderIocContainer = new CdcEmailProviderIocContainer(),
                SearchControllerIocContainer = new SearchControllerIocContainer(),
                MediaProviderIocContainer = new CsMediaProviderIocContainer(),
                ECardProviderIocContainer = new CsECardProviderIocContainer(),
            };
            return containers;
        }

        private static void Inject(IocContainers containers, ISearchProviders allSearchProviders)
        {
            CsECardProviderIocContainer.Inject(containers.EmailProviderIocContainer);
            SearchControllerIocContainer.Inject(allSearchProviders);
            DataServicesIocContainer.Inject(allSearchProviders);
            ApiIocContainer.Inject(containers.SearchControllerIocContainer,
                containers.MediaProviderIocContainer,
                containers.ECardProviderIocContainer);
            CsThumbnailProviderIocContainer.Inject();
            DataProviderIocContainer.Inject();

            CsMediaProviderIocContainer.Inject();
        }

    }
}
