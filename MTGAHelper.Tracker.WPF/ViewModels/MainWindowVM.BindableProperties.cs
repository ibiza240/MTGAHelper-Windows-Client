using MTGAHelper.Web.Models.Response.Account;
using System.Linq;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public partial class MainWindowVM
    {
        #region Public Two-Way Bindable Properties

        /// <summary>
        /// The main window context
        /// </summary>
        public WindowContext Context
        {
            get => _Context;
            set
            {
                SetField(ref _Context, value, nameof(Context));

                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        /// <summary>
        /// The visibility of the window
        /// </summary>
        public bool IsWindowVisible
        {
            get => _IsWindowVisible;
            set
            {
                // Set the new value
                _IsWindowVisible = value;
                // Raise the event without checking equality
                OnPropertyChanged(nameof(IsWindowVisible));
            }
        }

        /// <summary>
        /// The window state of the window
        /// </summary>
        public WindowState WindowState
        {
            get => _WindowState;
            set
            {
                // Set the new value
                _WindowState = value;
                // Raise the event without checking equality
                OnPropertyChanged(nameof(WindowState));
            }
        }

        /// <summary>
        /// Authentication Account
        /// </summary>
        public AccountResponse Account
        {
            get => _Account;
            set => SetField(ref _Account, value, nameof(Account));
        }

        /// <summary>
        /// The window position
        /// </summary>
        public double PositionTop
        {
            get => _PositionTop;
            set => SetField(ref _PositionTop, value, nameof(PositionTop));
        }

        /// <summary>
        /// The window position
        /// </summary>
        public double PositionLeft
        {
            get => _PositionLeft;
            set => SetField(ref _PositionLeft, value, nameof(PositionLeft));
        }

        /// <summary>
        /// The window dimension
        /// </summary>
        public double WindowWidth
        {
            get => _WindowWidth;
            set => SetField(ref _WindowWidth, value, nameof(WindowWidth));
        }

        /// <summary>
        /// The window dimension
        /// </summary>
        public double WindowHeight
        {
            get => _WindowHeight;
            set => SetField(ref _WindowHeight, value, nameof(WindowHeight));
        }

        /// <summary>
        /// The window transparency
        /// </summary>
        public double WindowOpacity
        {
            get => _WindowOpacity;
            set => SetField(ref _WindowOpacity, value, nameof(WindowOpacity));
        }

        /// <summary>
        /// The window topmost setting
        /// </summary>
        public bool WindowTopmost
        {
            get => _WindowTopmost;
            set => SetField(ref _WindowTopmost, value, nameof(WindowTopmost));
        }

        /// <summary>
        /// Whether the icon is animated
        /// </summary>
        public bool AnimatedIcon
        {
            get => _AnimatedIcon;
            set => SetField(ref _AnimatedIcon, value, nameof(AnimatedIcon));
        }

        /// <summary>
        /// Whether the game is running
        /// </summary>
        public bool IsGameRunning
        {
            get => _IsGameRunning;
            set
            {
                SetField(ref _IsGameRunning, value, nameof(IsGameRunning));
                OnPropertyChanged(nameof(ShowLaunchMtgaGameClient));
            }
        }

        /// <summary>
        /// The users sign-in email
        /// </summary>
        public string SigninEmail
        {
            get => _SigninEmail;
            set => SetField(ref _SigninEmail, value, nameof(SigninEmail));
        }

        /// <summary>
        /// Whether to remember the email
        /// </summary>
        public bool RememberEmail
        {
            get => _RememberEmail;
            set => SetField(ref _RememberEmail, value, nameof(RememberEmail));
        }

        /// <summary>
        /// The users sign-in password
        /// </summary>
        public string SigninPassword
        {
            get => _SigninPassword;
            set => SetField(ref _SigninPassword, value, nameof(SigninPassword));
        }

        /// <summary>
        /// Whether to remember the password
        /// </summary>
        public bool RememberPassword
        {
            get => _RememberPassword;
            set => SetField(ref _RememberPassword, value, nameof(RememberPassword));
        }

        /// <summary>
        /// Whether the window has been height minimized
        /// </summary>
        public bool IsHeightMinimized
        {
            get => _IsHeightMinimized;
            set => SetField(ref _IsHeightMinimized, value, nameof(IsHeightMinimized));
        }

        #endregion

        #region Public One-Way Bindable Properties

        /// <summary>
        /// Whether the current state is Playing
        /// </summary>
        public bool IsPlaying =>
            Context == WindowContext.Playing;

        /// <summary>
        /// The displayed network status string
        /// </summary>
        public string NetworkStatusString => DictStatus[NetworkStatus] +
                                             (NetworkStatus == NetworkStatusEnum.Ready ? $" ({SizeOfLogToSend})" : "");

        /// <summary>
        /// Whether the network is currently working
        /// </summary>
        public bool IsWorking => StatusBlinker.HasFlag(NetworkStatusEnum.Uploading) ||
                                 StatusBlinker.HasFlag(NetworkStatusEnum.Downloading) ||
                                 StatusBlinker.HasFlag(NetworkStatusEnum.ProcessingLogFile);

        /// <summary>
        /// Whether the network is ready to upload
        /// </summary>
        public bool CanUpload =>
            IsInitialSetupDone && IsUploading == false;

        /// <summary>
        /// Number of cards in users collection
        /// </summary>
        public string CardsOwned =>
            $"{Collection.Cards.Sum(i => i.Amount):#,##0} cards owned{CollectionDateAsOf}";

        /// <summary>
        /// Application version
        /// </summary>
        public string Version =>
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        /// <summary>
        /// Whether to show the game launcher
        /// </summary>
        public bool ShowLaunchMtgaGameClient =>
            Problems.HasFlag(ProblemsFlags.GameClientFileNotFound) == false && IsGameRunning == false;

        #endregion
    }
}
