using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Tools
{
    /// <summary>
    /// The abstract base class for Configurable templates
    /// </summary>
    public abstract class Configurable : ConfigurableObject
    {
        #region Constructor

        /// <summary>
        /// Complete constructor
        /// </summary>
        protected Configurable(Type valueType)
        {
            ValueType = valueType;
        }

        #endregion

        #region Basic Properties

        /// <summary>
        /// The value type for this Configurable
        /// </summary>
        public Type ValueType { get; }

        #endregion

        #region Abstract Public Properties

        /// <summary>
        /// Bind-able string for validating, resetting, and updating the underlying value
        /// </summary>
        public abstract string ViewValueString { get; set; }

        #endregion

        #region Abstract Commands

        /// <summary>
        /// Update Button Command
        /// </summary>
        public abstract ICommand UpdateCmd { get; }

        /// <summary>
        /// Reset Button Command
        /// </summary>
        public abstract ICommand ResetCmd { get; }

        #endregion
    }

    /// <summary>
    /// A Configurable with a specific type of value
    /// </summary>
    /// <typeparam name="T">Value type which must be IComparable</typeparam>
    public class Configurable<T> : Configurable where T : IComparable
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Configurable(T initial) : base(typeof(T))
        {
            // Set the initial value
            ModelValue = initial;
        }

        /// <summary>
        /// Constructor for complex inherited objects
        /// </summary>
        protected Configurable(Type valueType) : base(valueType)
        {

        }

        #endregion

        #region Virtual Properties

        /// <summary>
        /// The value of the given type for use on the model side
        /// </summary>
        public virtual T ModelValue
        {
            get => _ModelValue;
            set
            {
                // Set the underlying field
                SetField(ref _ModelValue, value, nameof(ModelValue));

                // Set the user value string
                ViewValueString = ValueTypeToString(value);
            }
        }

        /// <summary>
        /// The value of the given type for use on the view side
        /// </summary>
        public virtual T ViewValue
        {
            get => _ViewValue;
            protected set
            {
                // Set the underlying field
                SetField(ref _ViewValue, value, nameof(ViewValue));

                // Check whether the view matches the model to determine if an update is necessary
                if (EqualityComparer<T>.Default.Equals(ModelValue, ViewValue))
                    RemoveUpdate(nameof(ViewValue), "UpdateModel");
                else
                    AddUpdate(nameof(ViewValue), "UpdateModel");
            }
        }

        #endregion

        #region Property Overrides

        /// <summary>
        /// Bind-able string for validating, resetting, and updating the underlying value
        /// </summary>
        public override string ViewValueString
        {
            get => _ViewValueString;
            set
            {
                SetField(ref _ViewValueString, value, nameof(ViewValueString));

                // Attempt to convert the ViewValueString to the type of the underlying value, add error on invalid conversion otherwise set the view value
                if (!StringToValueType(ViewValueString, out T valueT))
                {
                    // Add an error
                    AddError(nameof(ViewValueString), "Invalid input");
                }
                else
                {
                    // Remove the error
                    RemoveError(nameof(ViewValueString), "Invalid input");

                    // Set the view layer value
                    ViewValue = valueT;

                    // Validate the view
                    Validate();
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Use a default 'Any' number style
        /// </summary>
        public NumberStyles NumberStyles { get; } = NumberStyles.Any;

        #endregion

        #region Operators

        /// <summary>
        /// Handle implicit type conversions
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator T(Configurable<T> p)
        {
            return p != null ? p.ModelValue : default;
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Local storage for model side value
        /// </summary>
        protected T _ModelValue;

        /// <summary>
        /// Local storage for view side value
        /// </summary>
        protected T _ViewValue;

        /// <summary>
        /// Local storage for user value string
        /// </summary>
        protected string _ViewValueString = "";

        #endregion

        #region Method Overrides

        /// <summary>
        /// Method for setting the ModelValue to the ViewValue
        /// </summary>
        public override void UpdateModel()
        {
            ModelValue = ViewValue;
        }

        /// <summary>
        /// Method for resetting the ViewValueString to the ModelValue
        /// </summary>
        public override void ResetView()
        {
            ViewValueString = ValueTypeToString(ModelValue);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Attempt conversion of a string to the ValueType
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="value">Output value</param>
        /// <param name="cultureOverride"></param>
        /// <returns>Success of conversion</returns>
        protected bool StringToValueType(string input, out T value, CultureInfo cultureOverride = null)
        {
            // Set the output to default (in case conversion fails)
            value = default;

            try
            {
                // Check for blatantly invalid entries first
                if (string.IsNullOrEmpty(input) && ValueType != typeof(string))
                {
                    return false;
                }

                // Handle enumeration conversion separately
                if (ValueType.IsEnum)
                {
                    value = (T) Enum.Parse(typeof(T), input ?? throw new ArgumentNullException(nameof(input)));
                }
                // Handle boolean parsing separately
                else if (ValueType == typeof(bool))
                {
                    // Get the appropriate TryParse method for this type
                    MethodInfo tryParse =
                        ValueType.GetMethod("TryParse", new[] {typeof(string), ValueType.MakeByRefType()});

                    // Required to create object outside of parse invoke to use out value
                    var objValue = new object[] {input, null};

                    // Run the TryParse with parameters and branch on the success
                    if (tryParse != null && (bool) tryParse.Invoke(ValueType, objValue))
                        value = (T) objValue[1];
                    else
                        return false;
                }
                else if (ValueType == typeof(Guid))
                {
                    if (Guid.TryParse(input, out Guid tempGuid))
                        value = (T) (object) tempGuid;  // Cannot directly cast to (T) as compiler does not know type
                    else
                        value = (T) (object) Guid.Empty;
                }
                else
                {
                    // Get the appropriate TryParse method for this type
                    MethodInfo tryParse = ValueType.GetMethod("TryParse",
                        new[] {typeof(string), typeof(NumberStyles), typeof(CultureInfo), ValueType.MakeByRefType()});

                    // Required to create object outside of parse invoke to use out value
                    var objValue = new object[] {input, NumberStyles, cultureOverride ?? CultureInfo.InvariantCulture, null};

                    // If the TryParse was found, it is much faster than a given ChangeType (and the exception handling is faster as well)
                    if (tryParse != null)
                    {
                        // Run the TryParse with parameters and branch on the success
                        if ((bool) tryParse.Invoke(ValueType, objValue))
                            value = (T) objValue[3];
                        else
                            return false;
                    }
                    else
                    {
                        // This will throw on invalid conversion (slow but fall-back)
                        value = (T) Convert.ChangeType(input, typeof(T), CultureInfo.InvariantCulture);
                    }
                }
            }
            catch
            {
                // We encountered an exception so conversion was not successful
                return false;
            }

            // If we get here, the value has been set correctly
            return true;
        }

        /// <summary>
        /// Convert a ValueType to a string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cultureOverride"></param>
        /// <returns></returns>
        protected string ValueTypeToString(T input, CultureInfo cultureOverride = null)
        {
            try
            {
                // Handle DateTimes specially using the round-trip specifier
                if (input is DateTime dt)
                    return dt.ToString("o", cultureOverride ?? CultureInfo.InvariantCulture);

                // Handle all other types using the standard ToString call with culture
                return Convert.ToString(input, cultureOverride ?? CultureInfo.InvariantCulture);
            }
            catch
            {
                return input.ToString();
            }
        }

        #endregion

        #region Update Command

        public override ICommand UpdateCmd
        {
            get { return _UpdateCmd ??= new RelayCommand(param => UpdateModel(), param => Can_Update()); }
        }

        private ICommand _UpdateCmd;

        #endregion

        #region Reset Command

        public override ICommand ResetCmd
        {
            get { return _ResetCmd ?? (_ResetCmd = new RelayCommand(param => ResetView(), param => Can_Reset())); }
        }

        private ICommand _ResetCmd;

        #endregion
    }
}
