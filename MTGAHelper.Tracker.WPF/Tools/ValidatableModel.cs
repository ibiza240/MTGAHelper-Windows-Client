using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable DelegateSubtraction

namespace MTGAHelper.Tracker.WPF.Tools
{
    public abstract class ValidatableModel : BasicModel, INotifyDataErrorInfo
    {
        #region Public Events

        /// <summary>
        /// Event to subscribe for objects that track validation errors
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add
            {
                // Attempt to remove the handler to avoid duplicate subscription
                // Quicker than checking the invocation list and equally safe
                if (_ErrorsChanged != null)
                    _ErrorsChanged -= value;

                // Add the handler
                _ErrorsChanged += value;
            }

            remove
            {
                // Do nothing when attempting to unsubscribe from disposed object
                if (_ErrorsChanged == null) return;

                // Attempt to remove the handler
                _ErrorsChanged -= value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Does the model have errors?
        /// </summary>
        public bool HasErrors
        {
            get
            {
                lock (ValidateLock)
                {
                    return Errors?.Any(kv => kv.Value != null && kv.Value.Count > 0) ?? false;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method for getting enumerable errors
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            lock (ValidateLock)
            {
                if (propertyName == null || Errors == null)
                    return new List<string>();

                // Attempt to get the errors list
                return Errors.TryGetValue(propertyName, out var errorsForName) ? errorsForName : new List<string>();
            }
            
        }

        /// <summary>
        /// Validate the model asynchronously
        /// </summary>
        /// <returns></returns>
        public Task ValidateAsync()
        {
            return Task.Run(Validate);
        }

        /// <summary>
        /// Add specific property errors to the validation model
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="error"></param>
        /// <param name="isWarning"></param>
        public void AddError(string propertyName, string error, bool isWarning = false)
        {
            lock (ValidateLock)
            {
                if (propertyName == null || error == null || Errors == null)
                    return;

                if (!Errors.ContainsKey(propertyName))
                    Errors[propertyName] = new List<string>();

                if (!Errors[propertyName].Contains(error))
                {
                    if (isWarning) Errors[propertyName].Add(error);
                    else Errors[propertyName].Insert(0, error);
                    OnErrorsChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// Remove specific property errors from the validation model
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="error"></param>
        public void RemoveError(string propertyName, string error)
        {
            lock (ValidateLock)
            {
                if (propertyName == null || error == null || Errors == null)
                    return;

                if (Errors.ContainsKey(propertyName) && Errors[propertyName].Contains(error))
                {
                    Errors[propertyName].Remove(error);
                    if (Errors[propertyName].Count == 0) Errors.TryRemove(propertyName, out _);
                    OnErrorsChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// Clear property errors from the validation model
        /// </summary>
        /// <param name="propertyName"></param>
        public void ClearErrors(string propertyName)
        {
            lock (ValidateLock)
            {
                if (propertyName == null || Errors == null)
                    return;

                if (Errors.ContainsKey(propertyName))
                {
                    Errors.TryRemove(propertyName, out _);
                    OnErrorsChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// Clear all errors for the modal
        /// </summary>
        public void ClearAllErrors()
        {
            lock (ValidateLock)
            {
                if (Errors == null)
                    return;

                foreach (var pN in Errors.ToList())
                {
                    Errors.TryRemove(pN.Key, out _);
                    OnErrorsChanged(pN.Key);
                }
            }
        }

        #endregion

        #region BasicModel Overrides

        /// <summary>
        ///  Used to dispose of the object
        /// </summary>
        public override void Dispose()
        {
            // Set the dictionary and event handler to null
            lock (ValidateLock)
            {
                Errors = null;
                _ErrorsChanged = null;
            }

            // Dispose the base object
            base.Dispose();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Remove specific property errors from the validation model
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="errorSubstring"></param>
        protected void RemoveErrorsContaining(string propertyName, string errorSubstring)
        {
            lock (ValidateLock)
            {
                if (Errors == null)
                    return;

                if (Errors.ContainsKey(propertyName))
                {
                    var errs = Errors[propertyName].Where(s => s.Contains(errorSubstring)).ToList();
                    foreach (string err in errs)
                    {
                        Errors[propertyName].Remove(err);
                        if (Errors[propertyName].Count == 0) Errors.TryRemove(propertyName, out _);
                        OnErrorsChanged(propertyName);
                    }
                }
            }

        }

        #endregion

        #region Virtual Protected Methods

        /// <summary>
        /// Handle asynchronous validation in each inherited class
        /// </summary>
        protected virtual void Validate() { }

        #endregion

        #region Protected Fields

        /// <summary>
        /// Simple lock for prevent dual validation
        /// </summary>
        protected readonly object ValidateLock = new object();

        #endregion

        #region Private Fields

        /// <summary>
        /// Event to subscribe for objects that track validation errors
        /// </summary>
        private EventHandler<DataErrorsChangedEventArgs> _ErrorsChanged;

        /// <summary>
        /// Dictionary of errors
        /// </summary>
        private ConcurrentDictionary<string, List<string>> Errors = new ConcurrentDictionary<string, List<string>>();

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to post when errors occur
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnErrorsChanged(string propertyName)
        {
            _ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            OnPropertyChanged(nameof(HasErrors));
        }

        #endregion
    }
}
