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
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.MediaProvider
{
    public interface IValueProvider
    {
        ValidationMessages DeleteValueSet(ValueSetObject valueObject);
        ValueSetObject GetValueSet(int id);
        List<ValueSetObject> GetValueSets(string valueSetName);
        ValidationMessages InsertValueSet(ValueSetObject valueObject);
        ValidationMessages UpdateValueSet(ValueSetObject valueObject);

        ValidationMessages DeleteValueRelationship(ValueRelationshipObject theObject);
        ValidationMessages DeleteValueRelationship(IList<ValueRelationshipObject> objects);
        List<ValueRelationshipObject> GetValueRelationships(string relationTypeName);
        ValueRelationshipObject GetValueRelationship(string relationTypeName, int valueId, string valueLang, int relatedValueId, string relatedValueLang);
        ValidationMessages SaveValueRelationship(ValueRelationshipObject theObject);
        ValidationMessages SaveValueRelationship(IList<ValueRelationshipObject> objects);

        ValueObject GetValue(int valueId);
        ValueObject GetValue(string valueName, string languageCode);
        ValidationMessages SaveValue(ValueObject theObject);
        ValidationMessages DeleteValue(ValueObject theObject);

        ValidationMessages AddRelationships(ValueObject theObject);
        ValidationMessages DeleteRelationships(ValueObject theObject);
    }
}
