using System;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class StatusBlinker
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public StatusBlinker()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // Wait between each check of the loop
                    Task.Delay(1000).Wait();

                    NetworkStatusEnum[] flags;

                    // Lock for flag access
                    lock (LockFlags)
                    {
                        // Enumerate the flags
                        flags = Enum.GetValues(typeof(NetworkStatusEnum)).Cast<NetworkStatusEnum>()
                            .Where(i => FlagsStatus.HasFlag(i))
                            .ToArray();
                    }

                    // Loop through the active flags
                    for (var i = 0; i < flags.Length; i++)
                    {
                        // Emit the status flag
                        EmitStatus?.Invoke(flags[i]);

                        // Wait between each emission except the last
                        if (i != flags.Length - 1)
                            Task.Delay(1000).Wait();
                    }
                }
            });
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Action for handling status emissions
        /// </summary>
        public Action<NetworkStatusEnum> EmitStatus { get; set; }

        #endregion

        #region Private Fields

        /// <summary>
        /// Active Flags
        /// </summary>
        private NetworkStatusEnum FlagsStatus;

        /// <summary>
        /// Flag Lock
        /// </summary>
        private readonly object LockFlags = new object();

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the current status enumeration as active or inactive
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="isActive"></param>
        public void SetNetworkStatus(NetworkStatusEnum flag, bool isActive)
        {
            lock (LockFlags)
            {
                if (isActive)
                    FlagsStatus |= flag;  // Set flag
                else
                    FlagsStatus &= ~flag;  // Remove flag
            }
        }

        /// <summary>
        /// Check if the given flag is active
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool HasFlag(NetworkStatusEnum flag)
        {
            lock (LockFlags)
                return FlagsStatus.HasFlag(flag);
        }

        /// <summary>
        /// Provide locked access to the status flags
        /// </summary>
        /// <returns></returns>
        public NetworkStatusEnum GetFlags()
        {
            lock (LockFlags)
                return FlagsStatus;
        }

        #endregion
    }
}
