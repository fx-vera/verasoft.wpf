using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using PropertyChanged;

namespace VeraSoft.Wpf.Validation
{
    [AddINotifyPropertyChangedInterface]
    public class ValidatableBase : IValidatableBase, INotifyDataErrorInfo, System.ComponentModel.INotifyPropertyChanged
    {
        private readonly BindableValidator _bindableValidator;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatableBase"/> class.
        /// </summary>
        public ValidatableBase()
        {
            _bindableValidator = new BindableValidator(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is validation enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if validation is enabled for this instance; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidationEnabled
        {
            get { return _bindableValidator.IsValidationEnabled; }
            set { _bindableValidator.IsValidationEnabled = value; }
        }

        /// <summary>
        /// Returns the BindableValidator instance that has an indexer property.
        /// </summary>
        /// <value>
        /// The Bindable Validator Indexer property.
        /// </value>
        public BindableValidator Errors
        {
            get
            {
                return _bindableValidator;
            }
        }
        /// <summary>
        /// Gets a value that indicates whether the entity has validation errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance contains validation errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get
            {
                return !ValidateProperties();
            }
        }

        /// <summary>
        /// Occurs when the Errors collection changed because new errors were added or old errors were fixed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { _bindableValidator.ErrorsChanged += value; }

            remove { _bindableValidator.ErrorsChanged -= value; }
        }

        /// <summary>
        /// Gets all errors.
        /// </summary>
        /// <returns> A ReadOnlyDictionary that's key is a property name and the value is a ReadOnlyCollection of the error strings.</returns>
        public ReadOnlyDictionary<string, ReadOnlyCollection<string>> GetAllErrors()
        {
            return _bindableValidator.GetAllErrors();
        }

        /// <summary>
        /// Validates the properties of the current instance.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if all properties pass the validation rules; otherwise, false.
        /// </returns>
        public bool ValidateProperties()
        {
            return !_bindableValidator.IsValidationEnabled // don't fail if validation is disabled
                || _bindableValidator.ValidateProperties();
        }

        /// <summary>
        /// Validates a single property with the given name of the current instance.
        /// </summary>
        /// <param name="propertyName">The property to be validated.</param>
        /// <returns>Returns <c>true</c> if the property passes the validation rules; otherwise, false.</returns>
        public bool ValidateProperty(string propertyName)
        {
            return !_bindableValidator.IsValidationEnabled // don't fail if validation is disabled
                || _bindableValidator.ValidateProperty(propertyName);
        }

        /// <summary>
        /// Sets the error collection of this instance.
        /// </summary>
        /// <param name="entityErrors">The entity errors.</param>
        public void SetAllErrors(IDictionary<string, ReadOnlyCollection<string>> entityErrors)
        {
            _bindableValidator.SetAllErrors(entityErrors);
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or Empty, to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (HasErrors == false)
            {
                return Enumerable.Empty<string>();
            }
            return _bindableValidator[propertyName];
        }
    }

}
