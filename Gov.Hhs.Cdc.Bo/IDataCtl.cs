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

namespace Gov.Hhs.Cdc.Bo
{
    public interface IDataCtl
    {      
        //Methods Implemented by the derived class      
        void Delete();
        void SetInitialValues(DateTime modifiedDateTime, Guid modifiedGuid);
        void SetUpdatableValues(DateTime modifiedDateTime, Guid modifiedGuid);
        bool DbObjectEqualsBusinessObject();
        bool VersionMatches();

        //Methods to consider removing because they are not required in the interface
        void UpdateIdsAfterInsert();
        void AddToDb();
        
        //New methods to begin using, implemented by the base control
        void Update(IDataServicesObjectContext dataEntities, object persistedBusinessObject, object newBusinessObject, bool enforceConcurrency);
        void Create(IDataServicesObjectContext dataEntities, object newBusinessObject);
        void Delete(IDataServicesObjectContext dataEntities, object persistedBusinessObject);

        //Deprecated Methods
        void Delete(object persistedBusinessObject);
        object Get(bool forUpdate);
        void SetBusinessObjects(object newBusinessObject, object persistedBusinessObject);

    }
}
