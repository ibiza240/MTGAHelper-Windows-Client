// using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace MTGAHelper.Tracker.WPF.Business
{

    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    public class StartupShortcutManager
    {
        public bool ManageRunOnStartup(bool runOnStartup)
        {
            //try
            //{
            var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "MTGAHelper Tracker.lnk");
            var shortcutExists = File.Exists(shortcutPath);

            if (runOnStartup)
            {
                if (shortcutExists == false)
                    CreateShortcut(shortcutPath);
            }
            else if (shortcutExists)
            {
                System.IO.File.Delete(shortcutPath);
            }

            return true;
            //}
            //catch (Exception ex)
            //{
            //    Log.Write(Serilog.Events.LogEventLevel.Error, ex, "Could not change the startup config:");
            //    return false;
            //}
        }

        void CreateShortcut(string shortcutPath)
        {
             var appLocation = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");

            // var shell = new WshShell();
            // var shortcut = shell.CreateShortcut(shortcutPath) as IWshShortcut;
            // shortcut.TargetPath = appLocation;
            // shortcut.WorkingDirectory = Path.GetDirectoryName(appLocation);
            // shortcut.Save();

            IShellLink link = (IShellLink)new ShellLink();

            // setup shortcut information
            link.SetDescription("MTGAHelper Tracker");
            link.SetPath(appLocation);

            // save it
            IPersistFile file = (IPersistFile)link;
            file.Save(shortcutPath, false);
        }
    }
}
