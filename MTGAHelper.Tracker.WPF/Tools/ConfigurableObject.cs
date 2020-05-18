using System;

namespace MTGAHelper.Tracker.WPF.Tools
{
    /// <summary>
    /// Base class for WPF binding friendly objects
    /// </summary>
    public abstract class ConfigurableObject : UpdatableModel
    {
        #region Two-way Bind-able Public Properties

        /// <summary>
        /// The displayed name of the parameter
        /// </summary>
        public virtual string DisplayName => DefaultDisplayName ?? GetType().GetRealTypeNameXML();

        /// <summary>
        /// Whether the parameter is user editable
        /// </summary>
        public bool UserEditable
        {
            get => _UserEditable;
            set => SetField(ref _UserEditable, value, nameof(UserEditable));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// An integer value used to order the ConfigurableObject using reflection
        /// </summary>
        public int Order { get; set; } = int.MaxValue;

        /// <summary>
        /// Whether the ConfigurableObject is displayed using reflection
        /// </summary>
        public bool IsDisplayed
        {
            get => _IsDisplayed;
            set => SetField(ref _IsDisplayed, value, nameof(IsDisplayed));
        }

        /// <summary>
        /// Fallback string for DisplayName when translated name is unavailable
        /// </summary>
        public string DefaultDisplayName { get; set; }

        /// <summary>
        /// GUID for use in identifying the object at runtime (regenerated between application instances)
        /// </summary>
        public Guid Guid { get; } = Guid.NewGuid();

        #endregion

        #region Private Fields

        /// <summary>
        /// Whether the parameter is ever user editable
        /// </summary>
        private bool _UserEditable = true;

        /// <summary>
        /// Whether the ConfigurableObject is displayed using reflection
        /// </summary>
        private bool _IsDisplayed = true;

        #endregion

        #region Virtual Public Methods

        /// <summary>
        /// Method for updating the model from the view
        /// </summary>
        public virtual void UpdateModel() { }

        /// <summary>
        /// Whether this object can update the model from the view
        /// </summary>
        /// <returns></returns>
        public virtual bool Can_Update()
        {
            return HasUpdates && !HasErrors;
        }

        /// <summary>
        /// Method for resetting the view from the model
        /// </summary>
        public virtual void ResetView() { }

        /// <summary>
        /// Whether this object can reset the view from the model
        /// </summary>
        /// <returns></returns>
        public virtual bool Can_Reset()
        {
            return HasUpdates || HasErrors;
        }

        #endregion
    }
}
