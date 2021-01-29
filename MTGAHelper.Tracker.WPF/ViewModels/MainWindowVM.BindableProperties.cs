using MTGAHelper.Web.Models.Response.Account;
using System.Linq;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public partial class MainWindowVM
    {
        public WindowContext Context
        {
            get => _Context;
            set
            {
                SetField(ref _Context, value, nameof(Context));

                OnPropertyChanged(nameof(IsPlaying));
            }
        }

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

        public AccountResponse Account
        {
            get => _Account;
            set => SetField(ref _Account, value, nameof(Account));
        }

        private double _PositionTop;
        public double PositionTop
        {
            get => _PositionTop;
            set => SetField(ref _PositionTop, value, nameof(PositionTop));
        }

        private double _PositionLeft;
        public double PositionLeft
        {
            get => _PositionLeft;
            set => SetField(ref _PositionLeft, value, nameof(PositionLeft));
        }

        private double _WindowWidth;
        public double WindowWidth
        {
            get => _WindowWidth;
            set => SetField(ref _WindowWidth, value, nameof(WindowWidth));
        }

        private double _WindowHeight;
        public double WindowHeight
        {
            get => _WindowHeight;
            set => SetField(ref _WindowHeight, value, nameof(WindowHeight));
        }

        private double _WindowOpacity;
        public double WindowOpacity
        {
            get => _WindowOpacity;
            set => SetField(ref _WindowOpacity, value, nameof(WindowOpacity));
        }

        private bool _WindowTopmost;
        public bool WindowTopmost
        {
            get => _WindowTopmost;
            set => SetField(ref _WindowTopmost, value, nameof(WindowTopmost));
        }

        private bool _AnimatedIcon;
        public bool AnimatedIcon
        {
            get => _AnimatedIcon;
            set => SetField(ref _AnimatedIcon, value, nameof(AnimatedIcon));
        }

        private bool _IsGameRunning;
        public bool IsGameRunning
        {
            get => _IsGameRunning;
            set
            {
                SetField(ref _IsGameRunning, value, nameof(IsGameRunning));
                OnPropertyChanged(nameof(ShowLaunchMtgaGameClient));
            }
        }

        private string _SigninEmail;
        public string SigninEmail
        {
            get => _SigninEmail;
            set => SetField(ref _SigninEmail, value, nameof(SigninEmail));
        }

        private bool _RememberEmail;
        public bool RememberEmail
        {
            get => _RememberEmail;
            set => SetField(ref _RememberEmail, value, nameof(RememberEmail));
        }

        private string _SigninPassword;
        public string SigninPassword
        {
            get => _SigninPassword;
            set => SetField(ref _SigninPassword, value, nameof(SigninPassword));
        }

        private bool _RememberPassword;
        public bool RememberPassword
        {
            get => _RememberPassword;
            set => SetField(ref _RememberPassword, value, nameof(RememberPassword));
        }

        private bool _IsHeightMinimized;
        public bool IsHeightMinimized
        {
            get => _IsHeightMinimized;
            set => SetField(ref _IsHeightMinimized, value, nameof(IsHeightMinimized));
        }

        private int _PlayingSelectedTabIndex;

        public int PlayingSelectedTabIndex
        {
            get => _PlayingSelectedTabIndex;
            set => SetField(ref _PlayingSelectedTabIndex, value, nameof(PlayingSelectedTabIndex));
        }

        private int _PlayingOpponentSelectedTabIndex;

        public int PlayingOpponentSelectedTabIndex
        {
            get => _PlayingOpponentSelectedTabIndex;
            set => SetField(ref _PlayingOpponentSelectedTabIndex, value, nameof(PlayingOpponentSelectedTabIndex));
        }

        public bool IsPlaying =>
            Context == WindowContext.Playing;

        public string NetworkStatusString => DictStatus[NetworkStatus] +
                                             (NetworkStatus == NetworkStatusEnum.Ready ? $" ({SizeOfLogToSend})" : "");

        public bool IsWorking => StatusBlinker.HasFlag(NetworkStatusEnum.Uploading) ||
                                 StatusBlinker.HasFlag(NetworkStatusEnum.Downloading) ||
                                 StatusBlinker.HasFlag(NetworkStatusEnum.ProcessingLogFile);

        public bool CanUpload =>
            IsInitialSetupDone && IsUploading == false;

        public string CardsOwned =>
            $"{Collection.Cards.Sum(i => i.Amount):#,##0} cards owned{CollectionDateAsOf}";

        public string Version =>
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public bool ShowLaunchMtgaGameClient =>
            Problems.HasFlag(ProblemsFlags.GameClientFileNotFound) == false && IsGameRunning == false;
    }
}
