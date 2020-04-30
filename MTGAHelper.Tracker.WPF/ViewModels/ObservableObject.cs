using System.Collections.Generic;
using System.ComponentModel;
// ReSharper disable DelegateSubtraction

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    /// <summary>
    /// Abstract common usage object with WPF binding convenience functions
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
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
                // Quicker than checking the invocation list and equally safety
                if (_PropertyChanged == null)
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
        protected void RaisePropertyChangedEvent(string propertyName)
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
            RaisePropertyChangedEvent(propertyName);
            return true;
        }

        #endregion
    }

    /// <summary>
    /// Template class of common usage for WPF bindings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableProperty<T> : ObservableObject
    {
        #region Constructor

        public ObservableProperty(T value)
        {
            Value = value;
        }

        #endregion

        #region Public Properties

        public T Value
        {
            get => _Value;
            set => SetField(ref _Value, value, nameof(Value));
        }

        #endregion

        #region Private Backing Fields

        private T _Value;

        #endregion
    }
}
