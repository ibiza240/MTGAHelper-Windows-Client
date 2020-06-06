//using MTGAHelper.Lib;
//using MTGAHelper.Tracker.DraftHelper.Shared;
//using MTGAHelper.Tracker.DraftHelper.Shared.Config;
//using MTGAHelper.Tracker.DraftHelper.Shared.Models;
//using MTGAHelper.Tracker.WPF.Config;
//using MTGAHelper.Tracker.WPF.Exceptions;
//using Serilog;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Windows;
//using Size = MTGAHelper.Tracker.WPF.Config.Size;

namespace MTGAHelper.Tracker.WPF.Business
{
    //    public enum DraftHelperRunnerValidationResultEnum
    //    {
    //        Unknown,
    //        Success,
    //        SetMissing,
    //        NoRatingsForSet,
    //        UnknownConfigResolution,
    //        NotAvailable,
    //    }

    public class DraftHelperRunner
    {
        //        /// <summary>
        //        /// Reference to the Config Model
        //        /// </summary>
        //        public ConfigModel Config { get; set; }

        //        private ConfigResolution ConfigResolution;
        //        private readonly CacheSingleton<Dictionary<string, Entity.DraftRatings>> draftRatings;
        //        private readonly IInputOutputOrchestrator InputOutputOrchestrator;

        //        private readonly ICollection<ConfigResolution> ResolutionSettings;

        //        private Bitmap Screenshot;

        //        public string Set { get; internal set; }

        //        /// <summary>
        //        /// Complete constructor
        //        /// </summary>
        //        /// <param name="configModel"></param>
        //        /// <param name="configResolutions"></param>
        //        /// <param name="inputOutputOrchestrator"></param>
        //        public DraftHelperRunner(
        //            ConfigModel configModel,
        //            CacheLoaderConfigResolutions configResolutions,
        //            CacheSingleton<Dictionary<string, Entity.DraftRatings>> draftRatings,
        //            IInputOutputOrchestrator inputOutputOrchestrator
        //            )
        //        {
        //            Config = configModel;
        //            this.draftRatings = draftRatings;
        //            ResolutionSettings = configResolutions.LoadData();
        //            InputOutputOrchestrator = inputOutputOrchestrator;
        //        }

        //        public DraftHelperRunnerValidationResultEnum Validate(string ratingsSource)
        //        {
        //            if (InputOutputOrchestrator is InputOutputOrchestratorMock)
        //                throw new DraftHelperMockException();

        //            string processName = Path.GetFileNameWithoutExtension(Config.GameFilePath);

        //            if (Process.GetProcessesByName(processName).Any() == false)
        //                processName = null;

        //            Screenshot = ScreenshotTaker.TakeScreenshot(processName);
        //            CheckIfScreenshotIsBlack(Screenshot);

        //            if (Config.ForceGameResolution == false)
        //            {
        //                // Auto-detect game resolution settings
        //                Config.GameResolutionBlackBorders = ConfigResolutionBlackBorders.None;

        //                // Detect top black border
        //                var iPixel = 0;
        //                var allBlack = true;
        //                while (iPixel < 5)
        //                {
        //                    var pixel = Screenshot.GetPixel(Screenshot.Width / 2, iPixel);
        //                    allBlack &= pixel.R == 0 && pixel.G == 0 && pixel.B == 0;
        //                    iPixel++;
        //                }

        //                if (allBlack)
        //                    Config.GameResolutionBlackBorders = ConfigResolutionBlackBorders.Top;
        //                else
        //                {
        //                    // Detect left black border
        //                    iPixel = 0;
        //                    allBlack = true;
        //                    while (iPixel < 5)
        //                    {
        //                        var pixel = Screenshot.GetPixel(Screenshot.Height / 2, iPixel);
        //                        allBlack &= pixel.R == 0 && pixel.G == 0 && pixel.B == 0;
        //                        iPixel++;
        //                    }

        //                    if (allBlack)
        //                        Config.GameResolutionBlackBorders = ConfigResolutionBlackBorders.Left;
        //                }

        //                Config.GameResolution = new Size(Screenshot.Width, Screenshot.Height);
        //                Config.Save();
        //            }

        //            Log.Information("Looking for ResolutionConfig [{SizeWidth}, {SizeHeight}] border:{blackBorder}",
        //                Config.GameResolution.Width,
        //                Config.GameResolution.Height,
        //                Config.GameResolutionBlackBorders);

        //            ConfigResolution = ResolutionSettings.FirstOrDefault(i =>
        //                i.Resolution.Width == Config.GameResolution.Width &&
        //                i.Resolution.Height == Config.GameResolution.Height &&
        //                i.BlackBorders == Config.GameResolutionBlackBorders);

        //            var result =
        //                string.IsNullOrEmpty(Set) ? DraftHelperRunnerValidationResultEnum.SetMissing :
        //                ConfigResolution == null ? DraftHelperRunnerValidationResultEnum.UnknownConfigResolution :
        //                Set != Constants.LIMITEDRATINGS_SOURCE_CUSTOM && draftRatings.Get()[ratingsSource].RatingsBySet.ContainsKey(Set) == false ? DraftHelperRunnerValidationResultEnum.NoRatingsForSet :
        //                DraftHelperRunnerValidationResultEnum.Success;

        //            if (result != DraftHelperRunnerValidationResultEnum.Success)
        //                Log.Error("Validation failed {result}", result);

        //            return result;
        //        }

        //        private static void CheckIfScreenshotIsBlack(Bitmap screenshot)
        //        {
        //            var iPixel = 0;
        //            var isBlack = true;
        //            while (iPixel < screenshot.Width * screenshot.Height)
        //            {
        //                Color color = screenshot.GetPixel(iPixel % screenshot.Width, iPixel / screenshot.Width);
        //                isBlack = color == Color.Black;

        //                if (isBlack == false)
        //                    break;

        //                iPixel++;
        //            }

        //            if (isBlack)
        //            {
        //                Log.Error("Black screenshot");
        //                MessageBox.Show("Screenshot could not be taken", "Problem", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            }
        //        }

        //        public ICollection<OutputModel> Run(int nbCards)
        //        {
        //            var inputModel = new InputModel
        //            {
        //                NbCards = nbCards,
        //                ConfigResolution = ConfigResolution,
        //                Set = Set,
        //            };

        //            var result = InputOutputOrchestrator.ProcessInput(inputModel, Screenshot);
        //            return result;
        //        }
    }
}
