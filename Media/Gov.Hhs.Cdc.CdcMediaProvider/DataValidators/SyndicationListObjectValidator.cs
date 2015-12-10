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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class SyndicationListObjectValidator : IValidator<SyndicationListObject, SyndicationListObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<SyndicationListObject> items)
        {
        }
        public void PreDeleteValidate(ValidationMessages validationMessages, IList<SyndicationListObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, 
            IList<SyndicationListObject> items)
        {
            foreach (SyndicationListObject syndicationList in items)
            {
                if (syndicationList.SyndicationListGuid == null 
                    && SyndicationListExistsForListName(objectContext, syndicationList.ListName))
                { validationMessages.AddError(syndicationList.ValidationKey + ".DomainName", 
                    "A Syndication List with this name (" + syndicationList.DomainName + ") already exists"); }
               
            }
        }

        private static bool SyndicationListExistsForListName(IDataServicesObjectContext objectContext, string listName)
        {
            return SyndicationListCtl.Get((MediaObjectContext)objectContext, forUpdate: false)
                .Where(l => l.ListName == listName).Any();
        }

        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<SyndicationListObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<SyndicationListObject> items)
        {
        }

        public SyndicationListObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, SyndicationListObject theObject)
        {
            return theObject;
        }

    }

}
