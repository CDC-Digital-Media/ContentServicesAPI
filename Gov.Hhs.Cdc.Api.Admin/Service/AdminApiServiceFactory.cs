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

using System.Web;

namespace Gov.Hhs.Cdc.Api.Admin
{
    public class AdminApiServiceFactory : IApiServiceFactory
    {
        private const int defaultApiVersion = 1;
        
        public RestServiceBase CreateNewService(int version)
        {
            return new Admin(version);
        }

        public TestUrl CreateTestUrl(string resource)
        {
            return CreateTestUrl(resource, string.Empty, string.Empty, string.Empty, defaultApiVersion);
        }

        public TestUrl CreateTestUrl(string resource, int id, string action, string queryParms)
        {
            return CreateTestUrl(resource, id.ToString(), action, queryParms);
        }

        public TestUrl CreateTestUrl(string resource, string id, string action, string queryParms)
        {
            return CreateTestUrl(resource, id, action, queryParms, defaultApiVersion);
        }

        public TestUrl CreateTestUrl(string resource, string id)
        {
            return CreateTestUrl(resource, id, string.Empty, string.Empty, defaultApiVersion);
        }

        public TestUrl CreateTestUrl(string resource, int id)
        {
            return CreateTestUrl(resource, id.ToString());
        }

        public TestUrl CreateTestUrl(string resource, int id, string action)
        {
            return CreateTestUrl(resource, id, action, string.Empty);
        }

        public TestUrl CreateTestUrl(string resource, string id, string action)
        {
            return CreateTestUrl(resource, id, action, string.Empty);
        }

        public TestUrl CreateTestUrl(string resource, string id, string action, string queryParms, int version)
        {
            return new TestUrl(ServiceType.AdminApi, resource, id, action, queryParms, version);
        }
    }

}
