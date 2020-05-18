using AutoMapper;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using Point = System.Windows.Point;
using MTGAHelper.Tracker.WPF.Exceptions;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class OptionsWindow
    {
        #region Constructor and Initializer

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency Injection Initializer
        /// </summary>
        /// <param name="mvm"></param>
        /// <param name="configApp"></param>
        /// <param name="draftRatings"></param>
        /// <param name="draftHelperRunner"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public OptionsWindow Init(
            MainWindowVM mvm,
            ConfigModel configApp,
            CacheSingleton<Dictionary<string, DraftRatings>> draftRatings,
            DraftHelperRunner draftHelperRunner,
            string email)
        {
            MainWindowVM = mvm;
            OptionsViewModel = Mapper.Map<OptionsWindowVM>(configApp);
            OptionsViewModel.DraftRatings = draftRatings;
            DataContext = OptionsViewModel;
            DraftHelperRunner = draftHelperRunner;
            ConfigApp = configApp;
            Email = email;
            return this;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Primary view model
        /// </summary>
        public OptionsWindowVM OptionsViewModel { get; private set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// Reference to the draft helper for testing
        /// </summary>
        private DraftHelperRunner DraftHelperRunner { get; set; }

        /// <summary>
        /// Singleton configuration settings
        /// </summary>
        private ConfigModel ConfigApp { get; set; }

        /// <summary>
        /// Email used to validate the draft helper access
        /// </summary>
        private string Email { get; set; }

        /// <summary>
        /// Reference to the main window view model for testing draft helper
        /// </summary>
        private MainWindowVM MainWindowVM { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle key presses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        /// <summary>
        /// Close the window on menu icon click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Drag Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (WindowState == WindowState.Maximized)
            {
                Point mousePos = PointToScreen(Mouse.GetPosition(this));
                Top = 0;
                Left = mousePos.X - 20;
                WindowState = WindowState.Normal;
            }

            // Begin dragging the window
            DragMove();
        }

        /// <summary>
        /// Test the draft helper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestDraftHelperButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigApp.ForceGameResolution = OptionsViewModel.ForceGameResolution;
            ConfigApp.GameResolution = OptionsViewModel.GameResolution;
            ConfigApp.GameResolutionIsPanoramic = OptionsViewModel.GameResolutionIsPanoramic;

            DraftHelperRunner.Set = "IKO";

            // Hide the options dialog and hide the main window
            this.WindowState = WindowState.Minimized;
            MainWindowVM.MinimizeWindow();

            // Validate all the requirements to run draft helper (including taking the screenshot)
            DraftHelperRunnerValidationResultEnum validation = DraftHelperRunnerValidationResultEnum.Unknown;
            try
            {
                validation = DraftHelperRunner.Validate(Email);
            }
            catch (DraftHelperMockException ex)
            {
                validation = DraftHelperRunnerValidationResultEnum.NotAvailable;
                MessageBox.Show(ex.Message, "Not available", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            finally
            {
                // Restore the options dialog and main window
                this.WindowState = WindowState.Normal;
                MainWindowVM.RestoreWindow();
            }

            // If the validation failed for any reason, provide a user output
            if (validation != DraftHelperRunnerValidationResultEnum.Success)
            {
                string text = validation switch
                {
                    DraftHelperRunnerValidationResultEnum.SetMissing =>
                    "DraftHelper doesn't know which set to draft",
                    DraftHelperRunnerValidationResultEnum.UnknownConfigResolution =>
                    "Impossible to determining the DraftHelper configuration for your game resolution",
                    _ => "Unknown error",
                };
                MessageBox.Show(text, "Problem", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {

                var result = DraftHelperRunner.Run(15, "Deathsie");

                string textResult = result.Any() switch
                {
                    true =>
                    $"{result.Count} cards detected:{Environment.NewLine}{string.Join(Environment.NewLine, result.Select(i => i.CardName))}",
                    false => "No cards detected",
                };
                MessageBox.Show($"[Set: {DraftHelperRunner.Set}] {textResult}", "DraftHelper test", MessageBoxButton.OK,
                    MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }


        }

        #endregion
    }
}
