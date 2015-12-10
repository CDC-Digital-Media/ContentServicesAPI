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
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class SyndicationListMediaObjectValidator : IValidator<SyndicationListMediaObject, SyndicationListMediaObject>
    {
        public void PreSaveValidate(ref ValidationMessages validationMessages, IList<SyndicationListMediaObject> items)
        {
            //No presave validate at this point
            //foreach (SyndicationListMediaObject syndicationListMedia in items)
            //{
            //}
        }
        public void PreDeleteValidate(ValidationMessages validationMessages, IList<SyndicationListMediaObject> items)
        {
        }

        public void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<SyndicationListMediaObject> items)
        {
            List<int> allMediaIds = items.Select(m => m.MediaId).ToList();
            Dictionary<int, int> mediaIdsFound = MediaCtl.GetSimple((MediaObjectContext)objectContext)
                        .Where(m => allMediaIds.Contains(m.Id)).ToDictionary(m => m.Id, m => m.Id);
            foreach (SyndicationListMediaObject syndicationListMedia in items)
            {
                if (!mediaIdsFound.ContainsKey(syndicationListMedia.MediaId))
                { validationMessages.AddError(syndicationListMedia.ValidationKey, "MediaId is invalid or inactive"); }
            }
        }



        public void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<SyndicationListMediaObject> items)
        {
        }

        public void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<SyndicationListMediaObject> items)
        {
        }

        public SyndicationListMediaObject GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, SyndicationListMediaObject theObject)
        {
            return theObject;
        }

    }

}
