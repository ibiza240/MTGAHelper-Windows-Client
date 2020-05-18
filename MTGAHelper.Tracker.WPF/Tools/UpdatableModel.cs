using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

// ReSharper disable DelegateSubtraction

namespace MTGAHelper.Tracker.WPF.Tools
{
    public abstract class UpdatableModel : ValidatableModel
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        protected UpdatableModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Event to subscribe for objects that track model updates
        /// </summary>
        public event PropertyChangedEventHandler UpdatesChanged
        {
            add
            {
                // Attempt to remove the handler to avoid duplicate subscription
                // Quicker than checking the invocation list and equally safe
                if (_UpdatesChanged != null)
                    _UpdatesChanged -= value;

                // Add the handler
                _UpdatesChanged += value;
            }

            remove
            {
                // Do nothing when attempting to unsubscribe from disposed object
                if (_UpdatesChanged == null) return;

                // Attempt to remove the handler
                _UpdatesChanged -= value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Does the view have updates?
        /// </summary>
        public bool HasUpdates
        {
            get
            {
                lock (UpdateLock)
                {
                    return Updates?.Any(kv => kv.Value != null && kv.Value.Count > 0) ?? false;
                }
            }
        }

        /// <summary>
        /// Can the model be updated without error?
        /// </summary>
        public bool CanUpdate => HasUpdates && !HasErrors;

        /// <summary>
        /// When the view has no changes
        /// </summary>
        public bool NoChanges => !HasUpdates && !HasErrors;

        #endregion

        #region BasicModel Overrides

        /// <summary>
        ///  Used to dispose of the object
        /// </summary>
        public override void Dispose()
        {
            // Set the dictionary and event handler to null
            lock (UpdateLock)
            {
                Updates = null;
                _UpdatesChanged = null;
            }

            // Dispose the base object
            base.Dispose();
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Event to subscribe for objects that track model updates
        /// </summary>
        private PropertyChangedEventHandler _UpdatesChanged;

        /// <summary>
        /// Simple lock for prevent threading issues
        /// </summary>
        private readonly object UpdateLock = new object();

        /// <summary>
        /// Dictionary of updates
        /// </summary>
        private ConcurrentDictionary<string, List<string>> Updates { get; set; } =
            new ConcurrentDictionary<string, List<string>>();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Add specific property updates from the update dictionary
        /// </summary>
        protected void AddUpdate(string objectName, string propertyName)
        {
            lock (UpdateLock)
            {
                if(objectName == null || propertyName == null || Updates == null)
                    return;

                if (!Updates.ContainsKey(objectName))
                    Updates[objectName] = new List<string>();

                if (!Updates[objectName].Contains(propertyName))
                {
                    Updates[objectName].Insert(0, propertyName);
                    OnUpdatesChanged(objectName);
                }
            }
        }

        /// <summary>
        /// Remove specific property updates from the update dictionary
        /// </summary>
        protected void RemoveUpdate(string objectName, string propertyName)
        {
            lock (UpdateLock)
            {
                if (objectName == null || propertyName == null || Updates == null)
                    return;

                if (Updates.ContainsKey(objectName) && Updates[objectName].Contains(propertyName))
                {
                    Updates[objectName].Remove(propertyName);
                    if (Updates[objectName].Count == 0) Updates.TryRemove(objectName, out List<string> _);
                    OnUpdatesChanged(objectName);
                }
            }
        }

        /// <summary>
        /// Clear property errors from the validation model
        /// </summary>
        protected void ClearUpdates(string objectName)
        {
            lock (UpdateLock)
            {
                if (objectName == null || Updates == null)
                    return;

                if (Updates.ContainsKey(objectName))
                {
                    Updates.TryRemove(objectName, out _);
                    OnUpdatesChanged(objectName);
                }
            }
        }

        /// <summary>
        /// Clear all errors for the modal
        /// </summary>
        protected void ClearAllUpdates()
        {
            lock (UpdateLock)
            {
                if (Updates == null)
                    return;

                foreach (KeyValuePair<string, List<string>> pN in Updates.ToList())
                {
                    Updates.TryRemove(pN.Key, out _);
                    OnUpdatesChanged(pN.Key);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method to post when updates occur
        /// </summary>
        /// <param name="objectName"></param>
        private void OnUpdatesChanged(string objectName)
        {
            _UpdatesChanged?.Invoke(this, new PropertyChangedEventArgs(objectName));

            // Signal to subscribers that HasUpdates has changed
            OnPropertyChanged(nameof(HasUpdates));
        }

        /// <summary>
        /// Signal changes to CanUpdate when either HasUpdates or HasErrors changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HasUpdates) || e.PropertyName == nameof(HasErrors))
            {
                OnPropertyChanged(nameof(CanUpdate));
                OnPropertyChanged(nameof(NoChanges));
            }
        }

        #endregion
    }
}
