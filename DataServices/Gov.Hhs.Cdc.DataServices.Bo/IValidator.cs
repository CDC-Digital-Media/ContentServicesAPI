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
using System.Collections;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{

    /// <summary>
    /// The business object validator class is used by the update framework logic to validate a business object
    /// and update the validation messages appropriately.  Generic parameters are:
    ///     T   The type of the business object to be validated
    ///     vT  The type of the validator business object.  Normally, this will be the business object type.
    ///         However, there are instances that a more complex object will need to be created with additional
    ///         information to support the validation logic.
    ///     
    /// </summary>
    public interface IValidator<T, vT> where vT : IValidationObject
    {
        /// <summary>
        /// Validate the business object on insert or update, before an object context is created.  This is
        /// intended to validate anything that does not require the database
        /// </summary>
        /// <param name="validationMessages">The current set of validation messages</param>
        /// <param name="items">The list of business objects to be validated</param>
        void PreSaveValidate(ref ValidationMessages validationMessages, IList<T> items);

        /// <summary>
        /// Validate the business object on delete, before an object context is created.  This is
        /// intended to validate anything that does not require the database
        /// </summary>
        /// <param name="validationMessages">The current set of validation messages</param>
        /// <param name="items">The list of business objects to be validated</param>
        void PreDeleteValidate(ValidationMessages validationMessages, IList<T> items);

        /// <summary>
        /// Run any validation rules on the business object that requires database access
        /// </summary>
        /// <param name="objectContext">The database object context</param>
        /// <param name="validationMessages">The current set of validation messages</param>
        /// <param name="items">The list of business objects to be validated</param>
        void ValidateSave(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<vT> items);

        /// <summary>
        /// For any updates that has the RequiresLogicalCommit set, execute a set of validations after the
        /// business object have been saved to the database.  If any errors occurr in this validation, the 
        /// LogicalRollBack method of the update manager will be called.  If this validation is sucessfull, 
        /// the LogicalCommit method of the update manager will be called.
        /// </summary>
        /// <param name="objectContext">The database object context</param>
        /// <param name="validationMessages">The current set of validation messages</param>
        /// <param name="items">The list of business objects to be validated</param>
        void PostSaveValidate(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<vT> items);

        /// <summary>
        /// Run any validation rules on the business object that requires database access
        /// </summary>
        /// <param name="objectContext">The database object context</param>
        /// <param name="validationMessages">The current set of validation messages</param>
        /// <param name="items">The list of business objects to be validated</param>
        void ValidateDelete(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, IList<vT> items);

        /// <summary>
        /// This method is not normally needed, but in the event that a special ValidationObject is needed,
        /// wrap the business object with a new object, containing everything required to validate.  This
        /// is executed before any validations are executed.
        /// </summary>
        /// <param name="objectContext">The database object context</param>
        /// <param name="validationMessages">The current set of validation messages</param>
        /// <param name="theObject">Instance of the object to wrap with a "ValidationObject"</param>
        /// <returns></returns>
        vT GetValidationObject(IDataServicesObjectContext objectContext, ValidationMessages validationMessages, T theObject);
    }
}
