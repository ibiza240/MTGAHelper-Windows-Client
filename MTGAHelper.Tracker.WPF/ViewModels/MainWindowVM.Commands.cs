using System;
using MTGAHelper.Tracker.WPF.Tools;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.Config;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public partial class MainWindowVM
    {
        #region Show Hide Card Images Command

        public ICommand ShowHideImagesCommand
        {
            get
            {
                return _ShowHideImagesCommand ??= new RelayCommand(param => ShowHideImages(), param => Can_ShowHideImages());
            }
        }

        private ICommand _ShowHideImagesCommand;

        private static bool Can_ShowHideImages()
        {
            return true;
        }

        private void ShowHideImages()
        {
            // Simple error check
            if (InMatchState == null) return;

            // Get the current compressed state
            bool compressed = !InMatchState.MyLibrary.ShowImage;

            // Flip the state
            compressed = !compressed;

            // Store the state to the config
            Config.CardListCollapsed = compressed;

            // Set the state of the card lists
            SetCompressedCardList(compressed);

        }

        #endregion

        #region Minimize Window Command

        public ICommand MinimizeWindowCommand
        {
            get
            {
                return _MinimizeWindowCommand ??= new RelayCommand(param => MinimizeWindow(), param => Can_MinimizeWindow());
            }
        }

        private ICommand _MinimizeWindowCommand;

        private static bool Can_MinimizeWindow()
        {
            return true;
        }

        public void MinimizeWindow()
        {
            // Handle the multiple minimize options
            switch (Config.Minimize)
            {
                // Just hide the window to effectively minimize to the tray
                case MinimizeOption.Tray:
                {
                    IsWindowVisible = false;
                    break;
                }
                // Set the window state to minimize to the taskbar
                case MinimizeOption.Taskbar:
                {
                    WindowState = WindowState.Minimized;
                    break;
                }
                // Set the minimized height of the window
                case MinimizeOption.Height:
                {
                    // Set the current window settings
                    Config.WindowSettingsOriginal = WindowSettings.Copy();

                    // Set the width and height to zero, which will be bounded by the window min size
                    WindowWidth = 0;
                    WindowHeight = 0;

                    // Set the flag, to flip the button
                    IsHeightMinimized = true;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void HideWindow()
        {
            IsWindowVisible = false;
        }

        #endregion

        #region Restore Window Command

        public ICommand RestoreWindowCommand
        {
            get
            {
                return _RestoreWindowCommand ??= new RelayCommand(param => RestoreWindow(), param => Can_RestoreWindow());
            }
        }

        private ICommand _RestoreWindowCommand;

        private static bool Can_RestoreWindow()
        {
            return true;
        }

        public void RestoreWindow()
        {         
            // If the height is minimized, restore the original size
            if (IsHeightMinimized)
            {
                // Restore the original window size
                WindowWidth = Config.WindowSettingsOriginal.Size.X;
                WindowHeight = Config.WindowSettingsOriginal.Size.Y;

                // Clear the flag, to flip the button
                IsHeightMinimized = false;
            }

            // Set the window visible and state to normal
            IsWindowVisible = true;
            WindowState = WindowState.Normal;

            // Bring the Arena process to the front
            Process process = Process.GetProcessesByName("MTGA").FirstOrDefault();
            if (process != null) Utilities.SetForegroundWindow(process.MainWindowHandle);
        }

        #endregion

        #region Exit Application Command

        public ICommand ExitApplicationCommand
        {
            get
            {
                return _ExitApplicationCommand ??= new RelayCommand(param => ExitApplication(), param => Can_ExitApplication());
            }
        }

        private ICommand _ExitApplicationCommand;

        private static bool Can_ExitApplication()
        {
            return true;
        }

        private static void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Reset Window Positions Command

        public ICommand ResetWindowsCommand
        {
            get
            {
                return _ResetWindowsCommand ??= new RelayCommand(param => ResetWindows(), param => Can_ResetWindows());
            }
        }

        private ICommand _ResetWindowsCommand;

        private static bool Can_ResetWindows()
        {
            return true;
        }

        private void ResetWindows()
        {
            // Reset the main window position
            WindowWidth = 340;
            WindowHeight = 500;
            PositionTop = 0;
            PositionLeft = 0;
            WindowOpacity = 1;

            // Reset the opponent window position
            OpponentWindowVM.WindowWidth = 340;
            OpponentWindowVM.WindowHeight = 500;
            OpponentWindowVM.PositionTop = 0;
            OpponentWindowVM.PositionLeft = 340;

            // Clear the height minimized flag
            IsHeightMinimized = false;
        }

        #endregion

        #region Switch Icon Command

        public ICommand SwitchIconCommand
        {
            get
            {
                return _SwitchIconCommand ??= new RelayCommand(param => SwitchIcon(), param => Can_SwitchIcon());
            }
        }

        private ICommand _SwitchIconCommand;

        private static bool Can_SwitchIcon()
        {
            return true;
        }

        private void SwitchIcon()
        {
            AnimatedIcon = !AnimatedIcon;
        }

        #endregion

        #region Sign Out Command

        public ICommand SignOutCommand
        {
            get
            {
                return _SignOutCommand ??= new RelayCommand(param => SignOut(), param => Can_SignOut());
            }
        }

        private ICommand _SignOutCommand;

        private static bool Can_SignOut()
        {
            return true;
        }

        private void SignOut()
        {
            SetMainWindowContext(WindowContext.Welcome);
        }

        #endregion

        #region Upload Log Command

        public ICommand UploadLogCommand
        {
            get
            {
                return _UploadLogCommand ??= new RelayCommand(param => UploadLog(), param => Can_UploadLog());
            }
        }

        private ICommand _UploadLogCommand;

        private bool Can_UploadLog()
        {
            return Account.IsAuthenticated && !IsUploading;
        }

        private void UploadLog()
        {
            UploadLogAction?.Invoke();
        }

        #endregion

        #region Show Option Command

        public ICommand ShowOptionsCommand
        {
            get
            {
                return _ShowOptionsCommand ??= new RelayCommand(param => ShowOptions(), param => Can_ShowOptions());
            }
        }

        private ICommand _ShowOptionsCommand;

        private static bool Can_ShowOptions()
        {
            return true;
        }

        private void ShowOptions()
        {
            ShowOptionsAction?.Invoke();
        }

        #endregion

        #region Launch Arena Command

        public ICommand LaunchArenaCommand
        {
            get
            {
                return _LaunchArenaCommand ??= new RelayCommand(param => LaunchArena(), param => Can_LaunchArena());
            }
        }

        private ICommand _LaunchArenaCommand;

        private bool Can_LaunchArena()
        {
            return !IsGameRunning;
        }

        private void LaunchArena()
        {
            LaunchArenaAction?.Invoke();
        }

        #endregion

        #region Validate User Command

        public ICommand ValidateUserCommand
        {
            get
            {
                return _ValidateUserCommand ??= new RelayCommand(ValidateUser, param => Can_ValidateUser());
            }
        }

        private ICommand _ValidateUserCommand;

        private static bool Can_ValidateUser()
        {
            return true;
        }

        private void ValidateUser(object obj)
        {
            ValidateUserAction?.Invoke(obj);
        }

        #endregion
    }
}
