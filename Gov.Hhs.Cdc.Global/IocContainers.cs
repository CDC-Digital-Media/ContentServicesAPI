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

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.ECardProvider;
using Gov.Hhs.Cdc.EmailProvider;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.Search.Controller;

namespace Gov.Hhs.Cdc.Global
{
    public class IocContainers
    {
        public IEmailProviderIocContainer EmailProviderIocContainer { get; set; }
        public ISearchControllerIocContainer SearchControllerIocContainer { get; set; }
        public IMediaProviderIocContainer MediaProviderIocContainer { get; set; }
        public IECardProviderIocContainer ECardProviderIocContainer { get; set; }
    }
}
