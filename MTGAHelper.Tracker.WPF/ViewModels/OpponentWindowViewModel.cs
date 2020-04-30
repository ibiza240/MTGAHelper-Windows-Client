using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Tools;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class OpponentWindowViewModel : ObservableObject
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <param name="mvm"></param>
        public OpponentWindowViewModel(string windowTitle, MainWindowVM mvm)
        {
            // Set the window title
            WindowTitle = windowTitle;

            // Set the reference to the main window
            MainWindowVM = mvm;

            // Subscribe to changes to the main window properties
            MainWindowVM.PropertyChanged += MainWindowVMOnPropertyChanged;

            // Set the card list
            CardList = mvm.InMatchState.OpponentCardsSeen;

            // Handle property changes
            PropertyChanged += OnPropertyChanged;

            // Set the initial window position
            PositionTop = WindowSettings?.Position.Y ?? 0;
            PositionLeft = WindowSettings?.Position.X ?? 0;

            // Set the initial window size
            WindowWidth = WindowSettings != null && WindowSettings.Size.X > double.Epsilon
                ? WindowSettings.Size.X
                : 300;
            WindowHeight = WindowSettings != null && WindowSettings.Size.Y > double.Epsilon
                ? WindowSettings.Size.Y
                : 500;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Window Title
        /// </summary>
        public string WindowTitle
        {
            get => _WindowTitle;
            set => SetField(ref _WindowTitle, value, nameof(WindowTitle));
        }

        /// <summary>
        /// List of cards
        /// </summary>
        public CardsListVM CardList { get; set; }

        /// <summary>
        /// Reference to the MainWindow ViewModel
        /// </summary>
        public MainWindowVM MainWindowVM { get; }

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
        /// The visibility of the window
        /// </summary>
        public bool IsWindowVisible
        {
            get => _IsWindowVisible;
            set => SetField(ref _IsWindowVisible, value, nameof(IsWindowVisible));
        }

        #endregion

        #region Private Backing Fields

        /// <summary>
        /// Window Title
        /// </summary>
        private string _WindowTitle = "Cards";

        /// <summary>
        /// The window position
        /// </summary>
        private double _PositionTop;

        /// <summary>
        /// The window position
        /// </summary>
        private double _PositionLeft;

        /// <summary>
        /// The window dimension
        /// </summary>
        private double _WindowWidth;

        /// <summary>
        /// The window dimension
        /// </summary>
        private double _WindowHeight;

        /// <summary>
        /// The visibility of the window
        /// </summary>
        private bool _IsWindowVisible;

        #endregion

        #region Private Fields

        /// <summary>
        /// Config Window Settings
        /// </summary>
        private WindowSettings WindowSettings => MainWindowVM?.ConfigModel?.WindowSettingsOpponentCards;

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PositionLeft):
                {
                    WindowSettings.Position.X = PositionLeft;
                    break;
                }
                case nameof(PositionTop):
                {
                    WindowSettings.Position.Y = PositionTop;
                    break;
                }
                case nameof(WindowWidth):
                {
                    WindowSettings.Size.X = WindowWidth;
                    break;
                }
                case nameof(WindowHeight):
                {
                    WindowSettings.Size.Y = WindowHeight;
                    break;
                }
            }
        }

        /// <summary>
        /// Handle property changes on the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowVMOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                // Handle changes to the main window visibility
                case nameof(MainWindowVM.IsWindowVisible):
                {
                    if (MainWindowVM.IsWindowVisible)
                    {
                        // Check that the context is Playing, and the options for external and auto are both set
                        if (MainWindowVM.MainWindowContext == WindowContext.Playing &&
                            MainWindowVM.ConfigModel.ShowOpponentCardsExternal &&
                            MainWindowVM.ConfigModel.ShowOpponentCardsAuto)
                        {
                            // Set the window to visible
                            IsWindowVisible = true;
                        }
                    }
                    else
                    {
                        // Hide the opponent window anytime the main window is hidden
                        IsWindowVisible = false;
                    }
                    break;
                }

                // Handle changes to the main window state
                case nameof(MainWindowVM.WindowState):
                {
                    switch (MainWindowVM.WindowState)
                    {
                        case WindowState.Normal:
                        {
                            // Check that the context is Playing, and the options for external and auto are both set
                            if (MainWindowVM.MainWindowContext == WindowContext.Playing &&
                                MainWindowVM.ConfigModel.ShowOpponentCardsExternal &&
                                MainWindowVM.ConfigModel.ShowOpponentCardsAuto)
                            {
                                // Set the window to visible
                                IsWindowVisible = true;
                            }
                            break;
                        }
                        case WindowState.Minimized:
                        {
                            // Hide the opponent window anytime the main window is minimized
                            IsWindowVisible = false;
                            break;
                        }
                        case WindowState.Maximized:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                }
            }
        }

        #endregion

        #region Show Window Command

        public ICommand ShowWindowCommand
        {
            get
            {
                return _ShowWindowCommand ??= new RelayCommand(param => ShowWindow(), param => Can_ShowWindow());
            }
        }

        private ICommand _ShowWindowCommand;

        private static bool Can_ShowWindow()
        {
            return true;
        }

        private void ShowWindow()
        {
            IsWindowVisible = true;
        }

        #endregion

        #region Hide Window Command

        public ICommand HideWindowCommand
        {
            get
            {
                return _HideWindowCommand ??= new RelayCommand(param => HideWindow(), param => Can_HideWindow());
            }
        }

        private ICommand _HideWindowCommand;

        private static bool Can_HideWindow()
        {
            return true;
        }

        private void HideWindow()
        {
            IsWindowVisible = false;
        }

        #endregion
    }
}
