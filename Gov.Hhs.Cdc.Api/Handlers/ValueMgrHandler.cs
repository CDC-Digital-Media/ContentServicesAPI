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
using Gov.Hhs.Cdc.Authorization;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;


namespace Gov.Hhs.Cdc.Api
{
    public class ValueMgrHandler : MediaHandlerBase
    {
        private static IValueProvider ValueProvider { get; set; }

        public static void Inject(IValueProvider valueProvider)
        {
            ValueProvider = valueProvider;
        }

        public static ValidationMessages InsertValueSet(string data, AdminUser adminUser) //, out int id)
        {
            if (!adminUser.CanEditVocabulary())
            {
                return ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary");
            }
            ValueSetObject valueSet = DeserializeValueSet(data);
            return ValueProvider.InsertValueSet(valueSet);
        }

        public static void UpdateValueSet(string data, int id, IOutputWriter writer, AdminUser adminUser)
        {
            if (!adminUser.CanEditVocabulary())
            {
                writer.Write(ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary"));
                return;
            }

            writer.Write(ValueProvider.UpdateValueSet(DeserializeValueSet(data, id)));
        }

        public static ValidationMessages DeleteValueSet(int id, AdminUser adminUser)
        {
            if (!adminUser.CanEditVocabulary())
            {
                return ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary");
            }

            if (id < 3)
            {
                return new ValidationMessages(new ValidationMessage(ValidationMessage.ValidationSeverity.Error, "ValueSet.Id",
                    "Cannot delete value set < 3"));
            }
            return ValueProvider.DeleteValueSet(new ValueSetObject() { Id = id });
        }

        public static ValueSetObject DeserializeValueSet(string data, int id = 0)
        {
            ValueSetObject valueSet = new ValueSetObject();
            SerialValueSetObj serialObj = BaseJsSerializer.Deserialize<SerialValueSetObj>(data);
            if (serialObj != null)
            {
                if (!string.IsNullOrEmpty(serialObj.name))
                    valueSet.Name = serialObj.name;

                if (!string.IsNullOrEmpty(serialObj.language))
                    valueSet.LanguageCode = serialObj.language;

                if (!string.IsNullOrEmpty(serialObj.description))
                    valueSet.Description = serialObj.description;

                if (!string.IsNullOrEmpty(serialObj.displayOrdinal))
                    valueSet.DisplayOrdinal = Convert.ToInt32(serialObj.displayOrdinal);

                if (!string.IsNullOrEmpty(serialObj.isActive))
                    valueSet.IsActive = Convert.ToBoolean(serialObj.isActive);

                if (!string.IsNullOrEmpty(serialObj.isDefaultable))
                    valueSet.IsDefaultable = Convert.ToBoolean(serialObj.isDefaultable);

                if (!string.IsNullOrEmpty(serialObj.isOrderable))
                    valueSet.IsOrderable = Convert.ToBoolean(serialObj.isOrderable);

                if (!string.IsNullOrEmpty(serialObj.isHierachical))
                    valueSet.IsHierachical = Convert.ToBoolean(serialObj.isHierachical);
            }
            valueSet.Id = id;
            return valueSet;

        }

        public static ValidationMessages UpdateValue(int id, string data, IOutputWriter writer, AdminUser adminUser)
        {
            ValidationMessages messages = new ValidationMessages();
            string valueId = "0";
            if (!adminUser.CanEditVocabulary())
                writer.Write(ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary"));
            else
                messages = UpdateValue(id, data, adminUser, out valueId);

            return messages;
        }

        public static ValidationMessages UpdateValue(int id, string data, AdminUser adminUser, out string valueId)
        {
            //output param
            valueId = "0";

            if (!adminUser.CanEditVocabulary())
            {
                return ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary");
            }

            SerialValueObj serialObj = BaseJsSerializer.Deserialize<SerialValueObj>(data);

            var vs = ValueProvider.GetValueSets(serialObj.valueSet).Find(a => a.LanguageCode == serialObj.languageCode);
            serialObj.valueSetId = vs.Id.ToString();

            ValueObject objToSave = serialObj.GetValue(id);
            var validation = ValueProvider.SaveValue(objToSave);

            valueId = objToSave.ValueId.ToString();
            return validation;
        }

        public static ValidationMessages DeleteValue(int id, AdminUser adminUser)
        {
            if (!adminUser.CanEditVocabulary())
            {
                return ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary");
            }

            if (id < 4)
            {
                throw new ApplicationException("Temporarily block main value set from being deleted");
            }
            ValueObject value = new ValueObject(id);
            return ValueProvider.DeleteValue(value);
        }

        public static ValidationMessages AddRelationship(int id, string data, IOutputWriter writer, AdminUser adminUser)
        {
            ValidationMessages messages = new ValidationMessages();

            if (!adminUser.CanEditVocabulary())
            {
                writer.Write(ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary"));
            }
            else
            {
                messages = AddRelationship(id, data, adminUser);
            }

            return messages;
        }

        public static ValidationMessages AddRelationship(int id, string data, AdminUser adminUser)
        {
            if (!adminUser.CanEditVocabulary())
            {
                return ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary");
            }

            SerialValueObj serialObj = BaseJsSerializer.Deserialize<SerialValueObj>(data);
            serialObj.languageCode = ValueProvider.GetValue(id).LanguageCode;

            var vs = ValueProvider.GetValueSets(serialObj.valueSet);

            return ValueProvider.AddRelationships(serialObj.GetValue(id));
        }

        public static ValidationMessages DeleteRelationship(int id, string data, IOutputWriter writer, AdminUser adminUser)
        {
            ValidationMessages messages = new ValidationMessages();

            if (!adminUser.CanEditVocabulary())
            {
                writer.Write(ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary"));
            }
            else
            {
                messages = DeleteRelationship(id, data, adminUser);
            }

            return messages;
        }

        public static ValidationMessages DeleteRelationship(int id, string data, AdminUser adminUser)
        {
            if (!adminUser.CanEditVocabulary())
            {
                return ValidationMessages.CreateError("auth", "User not authorized to edit vocabulary");
            }
            SerialValueObj serialObj = BaseJsSerializer.Deserialize<SerialValueObj>(data);
            serialObj.languageCode = ValueProvider.GetValue(id).LanguageCode;

            return ValueProvider.DeleteRelationships(serialObj.GetValue(id));
        }
    }
}
