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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class CsValueProvider : MediaMgrBase, IValueProvider
    {
        public ValueSetObject GetValueSet(int id)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return ValueSetObjectCtl.Get((MediaObjectContext)media, forUpdate: false)
                    .Where(m => m.Id == id).FirstOrDefault();
            }
        }

        public List<ValueSetObject> GetValueSets(string valueSetName)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return ValueSetObjectCtl.Get((MediaObjectContext)media, forUpdate: false)
                    .Where(m => m.Name == valueSetName).ToList();
            }
        }

        public ValidationMessages InsertValueSet(ValueSetObject valueObject)
        {
            try
            {
                return MediaDataManager.Save(new ValueSetUpdateMgr(valueObject));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred in CsValueProvider.Save");
                return new ValidationMessages(
                    new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "ValueSet", "", ex.Message, ex.StackTrace));
            }
        }

        public ValidationMessages UpdateValueSet(ValueSetObject valueObject)
        {
            try
            {
                return MediaDataManager.Save(new ValueSetUpdateMgr(valueObject));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred in CsValueProvider.Save");
                return new ValidationMessages(
                    new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "ValueSet", "", ex.Message, ex.StackTrace));
            }
        }

        public ValidationMessages DeleteValueSet(ValueSetObject valueObject)
        {
            return MediaDataManager.Delete(new ValueSetUpdateMgr(valueObject));
        }

        public ValidationMessages SaveValueRelationship(IList<ValueRelationshipObject> objects)
        {
            return MediaDataManager.Save(new ValueRelationshipUpdateMgr(objects));
        }

        public ValidationMessages SaveValueRelationship(ValueRelationshipObject theObject)
        {
            return MediaDataManager.Save(new ValueRelationshipUpdateMgr(theObject));
        }

        public ValidationMessages DeleteValueRelationship(IList<ValueRelationshipObject> objects)
        {
            return MediaDataManager.Delete(new ValueRelationshipUpdateMgr(objects));
        }

        public ValidationMessages DeleteValueRelationship(ValueRelationshipObject theObject)
        {
            return MediaDataManager.Delete(new ValueRelationshipUpdateMgr(theObject));
        }

        public ValueRelationshipObject GetValueRelationship(string relationTypeName, int valueId, string valueLang, int relatedValueId, string relatedValueLang)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return ValueRelationshipCtl.Get((MediaObjectContext)media)
                    .Where(m =>
                        m.RelationshipTypeName == relationTypeName &&
                        m.ValueId == valueId && m.ValueLanguageCode == valueLang &&
                        m.RelatedValueId == relatedValueId && m.RelatedValueLanguageCode == relatedValueLang
                    ).FirstOrDefault();
            }
        }

        public List<ValueRelationshipObject> GetValueRelationships(string relationTypeName)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return ValueRelationshipCtl.Get((MediaObjectContext)media)
                    .Where(m =>
                        m.RelationshipTypeName == relationTypeName
                    ).ToList();
            }
        }

        public ValueObject GetValue(int valueId)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return ValueCtl.GetWithChildren((MediaObjectContext)media).Where(m => m.ValueId == valueId).FirstOrDefault();
            }
        }

        public ValueObject GetValue(string valueName, string languageCode)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return ValueCtl.GetWithChildren((MediaObjectContext)media).Where(m => m.ValueName == valueName && m.LanguageCode == languageCode).FirstOrDefault();
            }
        }

        public ValidationMessages SaveValue(ValueObject theObject)
        {
            return MediaDataManager.Save(new ValueUpdateMgr(theObject));
        }

        public ValidationMessages DeleteValue(ValueObject theObject)
        {
            return MediaDataManager.Delete(new ValueUpdateMgr(theObject));
        }

        public ValidationMessages AddRelationships(ValueObject theObject)
        {
            return MediaDataManager.Save(new ValueRelationshipUpdateMgr(theObject.ValueRelationships.ToList(), theObject.ValueId));
        }

        public ValidationMessages DeleteRelationships(ValueObject theObject)
        {
            return MediaDataManager.Delete(new ValueRelationshipUpdateMgr(theObject.ValueRelationships.ToList(), theObject.ValueId));
        }
    }
}
