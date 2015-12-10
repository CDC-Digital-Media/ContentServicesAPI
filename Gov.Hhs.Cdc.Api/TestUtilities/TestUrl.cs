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


namespace Gov.Hhs.Cdc.Api
{
    public enum ServiceType { PublicApi, AdminApi };

    public class TestUrl
    {
        //public static string PublicApiServer = "localhost:44301";
        //public static string AdminApiServer = "localhost:44301";

        public static string PublicApiServer = ".....[devApiServer].....";
        public static string AdminApiServer = ".....[devReportingApplicationServer2].....";

        public static string Protocol = "https";

        public string Path { get; set; }
        public int Version { get; set; }
        public string Resource { get; set; }
        public string Id { get; set; }
        public string Action { get; set; }
        public string QueryString { get; set; }

        public TestUrl(ServiceType type, string resource, string queryParms, int version)
            : this(type, resource, null, null, queryParms, version)
        { }
        

        public TestUrl(ServiceType serviceType, string resource, string id, string action, string queryParms, int version)
        {
            switch (serviceType)
            {
                case ServiceType.PublicApi:
                    Path = string.Format("{0}://{1}/api/v{2}/resources", Protocol, PublicApiServer, version);
                    break;
                case ServiceType.AdminApi:
                    Path = string.Format("{0}://{1}/adminapi/v{2}/resources", Protocol, AdminApiServer, version);
                    break;
            }

            Version = version;
            Resource = resource;
            Id = id;
            Action = action;
            QueryString = queryParms.Replace(" ", "%20");
        }

        public override string ToString()
        {
            return Path + @"/" + Resource
                + (string.IsNullOrEmpty(Id) ? "" : @"/" + Id)
                + (string.IsNullOrEmpty(Action) ? "" : @"/" + Action)
                + (string.IsNullOrEmpty(QueryString) ? "" : @"?" + QueryString);

        }
    }
}
