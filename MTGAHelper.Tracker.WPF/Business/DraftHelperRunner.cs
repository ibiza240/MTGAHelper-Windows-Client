using MTGAHelper.Tracker.WPF.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MTGAHelper.Tracker.WPF.Business
{
    public enum DraftHelperRunnerValidationResultEnum
    {
        Unknown,
        Success,
        InputConfigMissing,
        MtgaProgramNotRunning,
    }

    public class DraftHelperRunner
    {
        private string folderCommunication;
        private string filepathInputConfig;
        private string processName;

        public DraftHelperRunner()
        {
        }

        public DraftHelperRunnerValidationResultEnum Validate(string folderCommunication, string processName)
        {
            //this.folderCommunication = folderCommunication.Replace("%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            //this.filepathInputConfig = Path.Combine(folderCommunication, "inputConfig.json");
            //this.processName = processName;

            //if (File.Exists(filepathInputConfig) == false)
            //    return DraftHelperRunnerValidationResultEnum.InputConfigMissing;

            //else if (Process.GetProcessesByName(processName).Any() == false)
            //    return DraftHelperRunnerValidationResultEnum.MtgaProgramNotRunning;

            //else
                return DraftHelperRunnerValidationResultEnum.Success;
        }

        public void Run(string ratingsSource, string folderDraftHelper)
        {
            //// Use same ratings source as in Options
            //var inputConfigFileContent = File.ReadAllText(filepathInputConfig);
            //var inputConfigModel = JsonConvert.DeserializeObject<InputModel>(inputConfigFileContent);
            //if (inputConfigModel.RatingsSource != ratingsSource)
            //{
            //    inputConfigModel.RatingsSource = ratingsSource;
            //    File.WriteAllText(filepathInputConfig, JsonConvert.SerializeObject(inputConfigModel));
            //}

            //var screenshot = ScreenshotTaker.TakeScreenshot(processName);
            //screenshot.Save(Path.Combine(folderCommunication, "screenshot.bmp"));
            //File.Copy(filepathInputConfig, Path.Combine(folderCommunication, "input.json"), true);

            //var filepathConsole = Path.Combine(folderDraftHelper, "MTGAHelper.Tracker.DraftHelper.ConsoleNet.exe");
            //Log.Information($"Starting console at {filepathConsole}");
            //var process = new Process
            //{
            //    StartInfo = new ProcessStartInfo
            //    {
            //        FileName = filepathConsole,
            //        UseShellExecute = false,
            //        CreateNoWindow = false,
            //        WorkingDirectory = Path.GetDirectoryName(filepathConsole)
            //    }
            //};

            //process.Start();
            //process.WaitForExit();
        }
    }
}
