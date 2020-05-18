using MTGAHelper.Tracker.DraftHelper.Shared;
using MTGAHelper.Tracker.DraftHelper.Shared.Config;
using MTGAHelper.Tracker.DraftHelper.Shared.Models;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Exceptions;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Size = MTGAHelper.Tracker.WPF.Config.Size;

namespace MTGAHelper.Tracker.WPF.Business
{
    public enum DraftHelperRunnerValidationResultEnum
    {
        Unknown,
        Success,
        SetMissing,
        UnknownConfigResolution,
        NotAvailable,
    }

    public class DraftHelperRunner
    {
        /// <summary>
        /// Reference to the Config Model
        /// </summary>
        public ConfigModel Config { get; set; }

        private ConfigResolution ConfigResolution;

        private readonly IInputOutputOrchestrator InputOutputOrchestrator;

        private readonly ICollection<ConfigResolution> ResolutionSettings;

        private readonly IEmailProvider EmailProvider;

        private Bitmap Screenshot;

        public string Set { get; internal set; }

        /// <summary>
        /// Complete constructor
        /// </summary>
        /// <param name="configModel"></param>
        /// <param name="configResolutions"></param>
        /// <param name="inputOutputOrchestrator"></param>
        /// <param name="emailProvider"></param>
        public DraftHelperRunner(
            ConfigModel configModel,
            CacheLoaderConfigResolutions configResolutions,
            IInputOutputOrchestrator inputOutputOrchestrator,
            IEmailProvider emailProvider)
        {
            Config = configModel;
            ResolutionSettings = configResolutions.LoadData();
            InputOutputOrchestrator = inputOutputOrchestrator;
            EmailProvider = emailProvider;
        }

        public DraftHelperRunnerValidationResultEnum Validate(string email)
        {
            if (InputOutputOrchestrator is InputOutputOrchestratorMock)
                throw new DraftHelperMockException();

            EmailProvider.Email = email;
            string processName = Path.GetFileNameWithoutExtension(Config.GameFilePath);

            if (Process.GetProcessesByName(processName).Any() == false)
                processName = null;

            Screenshot = ScreenshotTaker.TakeScreenshot(processName);
            CheckIfScreenshotIsBlack(Screenshot);

            Log.Information($"Processed screenshot for {processName ?? "[no process]"}");

            // Manual game resolution settings first
            bool isPanoramic = Config.GameResolutionIsPanoramic;
            if (Config.ForceGameResolution == false)
            {
                // Auto-detect game resolution settings
                isPanoramic = Screenshot.GetPixel(Screenshot.Width / 2, 4) == Color.Black;
                Config.GameResolution = new Size(Screenshot.Width, Screenshot.Height);
                Config.Save();
            }

            Log.Information("Looking for ResolutionConfig {Size} {IsPanormaic}", Config.GameResolution, isPanoramic);

            ConfigResolution = ResolutionSettings.FirstOrDefault(i =>
                i.Resolution.Width == Config.GameResolution.Width &&
                i.Resolution.Height == Config.GameResolution.Height && i.IsPanoramic == isPanoramic);

            var result = string.IsNullOrEmpty(Set)
                ? DraftHelperRunnerValidationResultEnum.SetMissing
                : ConfigResolution == null
                    ? DraftHelperRunnerValidationResultEnum.UnknownConfigResolution
                    : DraftHelperRunnerValidationResultEnum.Success;

            if (result != DraftHelperRunnerValidationResultEnum.Success)
                Log.Error("Validation failed {result}", result);

            return result;
        }

        private static void CheckIfScreenshotIsBlack(Bitmap screenshot)
        {
            var iPixel = 0;
            var isBlack = true;
            while (iPixel < screenshot.Width * screenshot.Height)
            {
                Color color = screenshot.GetPixel(iPixel % screenshot.Width, iPixel / screenshot.Width);
                 isBlack = color == Color.Black;

                if (isBlack == false)
                    break;

                iPixel++;
            }

            if (isBlack)
            {
                Log.Error("Black screenshot");
                MessageBox.Show("Screenshot could not be taken", "Problem", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public ICollection<OutputModel> Run(int nbCards, string ratingsSource)
        {
            var inputModel = new InputModel
            {
                NbCards = nbCards,
                ConfigResolution = ConfigResolution,
                RatingsSource = ratingsSource,
                Set = Set,
            };

            var result = InputOutputOrchestrator.ProcessInput(inputModel, Screenshot);
            return result;
        }
    }
}
