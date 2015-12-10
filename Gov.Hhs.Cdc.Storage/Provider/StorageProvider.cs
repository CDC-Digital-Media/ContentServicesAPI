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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.DataServices;

namespace Gov.Hhs.Cdc.Storage.Provider
{
    public class StorageProvider : IStorageProvider
    {
        private DataManager StorageDataManager { get { return new DataManager(ObjectContextFactory); } }
        private static IObjectContextFactory ObjectContextFactory { get; set; }

        private ICallParser _parser;
        private IOutputWriter _writer;

        public StorageProvider(ICallParser parser, IOutputWriter writer)
        {
            _parser = parser;
            _writer = writer;
        }

        public IList<StorageObject> Get()
        {
            IList<StorageObject> storeObj = new List<StorageObject>();
            using (Model.StorageObjectContext media = (Model.StorageObjectContext)ObjectContextFactory.Create())
            {
                storeObj = StorageCtl.Get(media, forUpdate: false).ToList();                
            }

            return storeObj;
        }

        public bool IsClientApplicationKeyValid(string appKey)
        {
            //using (IDataServicesObjectContext registrationDb = ObjectContextFactory.Create())
            //{
            //    return ApiClientCtl.Get((RegistrationObjectContext)registrationDb)
            //        .Where(a => a.ApplicationKey == appKey).Any();
            //}

            throw new NotImplementedException();
        }
    }
}
