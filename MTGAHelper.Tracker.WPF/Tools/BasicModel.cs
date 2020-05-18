using System;
using System.Collections.Generic;
using System.ComponentModel;

// ReSharper disable DelegateSubtraction

namespace MTGAHelper.Tracker.WPF.Tools
{
    /// <summary>
    /// Base class for objects that intend to be bound to the UI
    /// </summary>
    public abstract class BasicModel : INotifyPropertyChanged, IDisposable
    {
        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Event to subscribe for objects that track property updates
        /// </summary>
        private PropertyChangedEventHandler _PropertyChanged;

        /// <summary>
        /// Event to subscribe for objects that track property updates
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                // Attempt to remove the handler to avoid duplicate subscription
                // Quicker than checking the invocation list and equally safe
                if (_PropertyChanged != null)
                    _PropertyChanged -= value;

                // Add the handler
                _PropertyChanged += value;
            }

            remove
            {
                // Do nothing when attempting to unsubscribe from disposed object
                if (_PropertyChanged == null) return;

                // Attempt to remove the handler
                _PropertyChanged -= value;
            }
        }

        /// <summary>
        /// Function to post event when property changes
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property change function which automatically posts a property changed event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Set the PropertyChanged event handler to null to allow garbage collection
        /// </summary>
        public virtual void Dispose()
        {
            _PropertyChanged = null;
        }

        #endregion
    }
}
