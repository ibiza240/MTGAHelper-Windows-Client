using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MTGAHelper.Tracker.WPF.Tools
{
    /// <summary>
    /// A configurable of type string that gets represented by the controls as a file path
    /// </summary>
    public class ConfigurablePath : Configurable<string>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="t"></param>
        /// <param name="path"></param>
        public ConfigurablePath(BrowseType t, string path) : base(path)
        {
            DialogType = t;
        }

        #endregion

        #region Public Enumerations

        /// <summary>
        /// Types of ConfigurablePath used for setting the browse command to the correct dialog
        /// </summary>
        public enum BrowseType
        {
            OpenFile,
            OpenFolder,
            Save
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The filter to be used in the browse dialog
        /// </summary>
        public string FileFilter { get; set; } = "All Files|*.*";

        /// <summary>
        /// The default file name to be used in the browse dialog
        /// </summary>
        public string DefaultFileName { get; set; }

        /// <summary>
        /// The type of browse dialog
        /// </summary>
        public BrowseType DialogType { get; set; }

        #endregion

        #region Browse Command

        public ICommand BrowseCmd
        {
            get { return _BrowseCmd ??= new RelayCommand(param => Browse(), param => Can_Browse()); }
        }

        private ICommand _BrowseCmd;

        private bool Can_Browse()
        {
            return true;
        }

        private void Browse()
        {
            try
            {
                // Get the default file name
                string fileName = DefaultFileName;
                var path = "";

                // Attempt to get the name from the current user string
                try
                {
                    fileName = Path.GetFileNameWithoutExtension(ViewValueString);
                }
                catch
                {
                    // ignored
                }

                // Attempt to get the path from the current user string
                try
                {
                    path = Path.GetDirectoryName(ViewValueString);
                }
                catch
                {
                    //ignored
                }

                switch (DialogType)
                {
                    case BrowseType.OpenFile:
                    {
                        // Open the dialog to select the file
                        var openFileDialog = new OpenFileDialog
                        {
                            InitialDirectory = path ?? "",
                            Multiselect = false,
                            Filter = FileFilter,
                            RestoreDirectory = true,
                        };

                        // Show and wait for result
                        if (openFileDialog.ShowDialog() == true)
                        {
                            ViewValueString = openFileDialog.FileName;
                        }

                        break;
                    }

                    case BrowseType.OpenFolder:
                    {
                        var dialog = new CommonOpenFileDialog
                        {
                            InitialDirectory = path,
                            IsFolderPicker = true,
                            RestoreDirectory = true
                        };

                        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            ViewValueString = dialog.FileName + @"\";
                        }

                        break;
                    }

                    default:
                    {
                        // Create the save dialog
                        var saveFileDialog = new SaveFileDialog
                        {
                            InitialDirectory = path ?? "",
                            Filter = FileFilter,
                            FilterIndex = 1,
                            RestoreDirectory = true,
                            FileName = fileName ?? ""
                        };

                        // Show and wait for result
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            ViewValueString = saveFileDialog.FileName;
                        }

                        break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion
    }
}
