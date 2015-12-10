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

using Gov.Hhs.Cdc.Bo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Hhs.Cdc.TransactionLogProvider
{
    public class TransactionEntryObject : DataSourceBusinessObject, IValidationObject
    {
        public int TransactionId {get; set;}
	    public string HttpMethod {get; set;}
	    public string Resource {get; set;}
	    public string ResourceId {get; set;}
	    public string ResourceAction {get; set;}
	    public string QueryString {get; set;}
	    public string ServicePath {get; set;}
        private string[] _servicePathArray = null;
        public string[] ServicePathArray { 
            get 
            { 
                return _servicePathArray ?? (_servicePathArray = (ServicePath ?? "").Split('/'));
            }
        }
        public string _api = null;
        public string Api { get { return _api ?? (_api = ServicePathArray.Count() > 1 ? ServicePathArray[1] : ""); } }
        public int? _version = null;
        public int Version
        {
            get
            {
                if (_version == null)
                {
                    var stringVersion = ServicePathArray.Count() > 2 ? ServicePathArray[2] : "0";
                    _version = _version = int.Parse(stringVersion.Substring(1));
                }
                return (int)_version;
            }
        }

	    public string InputData {get; set;}
	    public string OutputData {get; set;}
        public string Messages { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}/{2}/{3}/{4}", HttpMethod, Resource, ResourceId, ResourceAction, QueryString);
        }
    }
}
