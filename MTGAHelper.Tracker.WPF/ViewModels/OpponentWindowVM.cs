using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Tools;
using static MTGAHelper.Tracker.WPF.Business.ServerApiCaller;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class OpponentWindowVM : BasicModel
    {
        private string _WindowTitle = "Cards";
        public string WindowTitle
        {
            get => _WindowTitle;
            set => SetField(ref _WindowTitle, value, nameof(WindowTitle));
        }

        private int _SelectedTabIndex;
        public int SelectedTabIndex
        {
            get => _SelectedTabIndex;
            set => SetField(ref _SelectedTabIndex, value, nameof(SelectedTabIndex));
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

        private bool _IsWindowVisible;
        public bool IsWindowVisible
        {
            get => _IsWindowVisible;
            set => SetField(ref _IsWindowVisible, value, nameof(IsWindowVisible));
        }

        private ICollection<DecksByCardsResponseItem> _DecksUsingCards;
        private readonly ServerApiCaller ServerApiCaller;

        public CardsListVM CardList { get; set; }
        public MainWindowVM MainWindowVM { get; }

        public OpponentWindowVM(
            string windowTitle,
            MainWindowVM mvm,
            ServerApiCaller serverApiCaller
            )
        {
            WindowTitle = windowTitle;
            MainWindowVM = mvm;
            ServerApiCaller = serverApiCaller;
            CardList = mvm.InMatchState.OpponentCardsSeen;
            CardList.CardsUpdated += OnCardsGotUpdated;

            // Subscribe to changes to the main window properties
            MainWindowVM.PropertyChanged += MainWindowVMOnPropertyChanged;

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

        private void OnCardsGotUpdated(object sender, EventArgs e)
        {
            if (IsWindowVisible == false ||
                SelectedTabIndex != 1 ||
                CardList.Cards.Count < 3
                )
                return;

            RefreshDecksUsingCards(CardList.Cards.Select(i => i.Name).ToArray());
        }

        public ICollection<DecksByCardsResponseItem> DecksUsingCards
        {
            get => _DecksUsingCards;
            set => SetField(ref _DecksUsingCards, value, nameof(DecksUsingCards));
        }

        /// <summary>
        /// Config Window Settings
        /// </summary>
        private WindowSettings WindowSettings => MainWindowVM?.Config?.WindowSettingsOpponentCards;

        /// <summary>
        /// Track whether the window was previously visible
        /// </summary>
        private bool WasVisible;

        /// <summary>
        /// Track the new match status of auto opening the window
        /// </summary>
        private bool NewMatch;

        /// <summary>
        /// Common use method for showing or hiding the window based on main window state and options
        /// </summary>
        public void ShowHideWindow(bool manual = false)
        {
            if (MainWindowVM.IsWindowVisible && MainWindowVM.WindowState == WindowState.Normal &&
                MainWindowVM.Context == WindowContext.Playing && MainWindowVM.Config.ShowOpponentCardsExternal && !MainWindowVM.IsHeightMinimized)
            {
                // If the AutoShow setting is enabled, the window was previously visible, or the manual argument is set, show the window
                if ((MainWindowVM.Config.ShowOpponentCardsAuto && NewMatch) || WasVisible || manual)
                {
                    // Clear the new match flag
                    NewMatch = false;

                    // Show the window
                    ShowWindow();
                }
            }
            else
            {
                // Get the current status of the window visibility
                bool v = IsWindowVisible;

                // Always hide the opponent window if the main window is not visible
                HideWindow();

                // Set the last status of the window visibility
                WasVisible = v;
            }
        }

        internal void RefreshDecksUsingCards(ICollection<string> cards = null)
        {
            if (cards == null && CardList?.Cards?.Any() == true)
                cards = CardList.Cards.Select(i => i.Name).ToArray();

            if (cards?.Any() != true) return;

            var decks = ServerApiCaller.GetDecksFromCards(cards);
            DecksUsingCards = decks;
            OnPropertyChanged(nameof(DecksUsingCards));
        }

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

        private void MainWindowVMOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                // Handle changes to the main window visibility, window state, or window context
                case nameof(MainWindowVM.IsWindowVisible):
                case nameof(MainWindowVM.WindowState):
                case nameof(MainWindowVM.IsHeightMinimized):
                    {
                        ShowHideWindow();
                        break;
                    }
                // Handle changes to the main
                case nameof(MainWindowVM.Context):
                    {
                        if (MainWindowVM.Context == WindowContext.Playing && !NewMatch)
                            NewMatch = true;

                        ShowHideWindow();
                        break;
                    }
            }
        }

        internal void ResetDecks()
        {
            DecksUsingCards = null;
            OnPropertyChanged(nameof(DecksUsingCards));
        }

        private static bool Can_ShowWindow() => true;
        private void ShowWindow() => IsWindowVisible = true;
        private ICommand _ShowWindowCommand;
        public ICommand ShowWindowCommand
        {
            get
            {
                return _ShowWindowCommand ??= new RelayCommand(param => ShowWindow(), param => Can_ShowWindow());
            }
        }

        private static bool Can_HideWindow() => true;
        private void HideWindow() => IsWindowVisible = false;
        private ICommand _HideWindowCommand;
        public ICommand HideWindowCommand
        {
            get
            {
                return _HideWindowCommand ??= new RelayCommand(param => HideWindow(), param => Can_HideWindow());
            }
        }
    }
}
