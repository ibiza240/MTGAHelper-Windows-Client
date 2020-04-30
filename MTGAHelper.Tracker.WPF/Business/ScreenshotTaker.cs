using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using MTGAHelper.Tracker.WPF.Tools;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class ScreenshotTaker
    {
        /// <summary>
        /// Method for taking the screenshot of a process
        /// </summary>
        /// <param name="procName"></param>
        /// <returns></returns>
        public static Bitmap TakeScreenshot(string procName)
        {
            Process proc;

            // Attempt to get the process by name
            try
            {
                proc = Process.GetProcessesByName(procName)[0];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }

            // Focus on the application
            Utilities.SetForegroundWindow(proc.MainWindowHandle);
            Utilities.ShowWindow(proc.MainWindowHandle, 9);

            // Need some amount of delay to let window bring the window forward
            Thread.Sleep(500);

            // Create a window rect
            var rect = new Utilities.Rect();

            // Get the window rect
            IntPtr error = Utilities.GetWindowRect(proc.MainWindowHandle, ref rect);

            // Prevent infinite loops
            var retry = 0;
            const int maxRetry = 5;

            // Sometimes it gives an error, try a few times
            while (error == (IntPtr) 0 && retry < maxRetry)
            {
                error = Utilities.GetWindowRect(proc.MainWindowHandle, ref rect);
                retry++;
            }

            // Final check for error
            if (error == (IntPtr) 0)
                return null;

            // Get the dimensions on the process window
            int width = (rect.right - rect.left);
            int height = (rect.bottom - rect.top);

            // Create a bitmap
            var b = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap
            using Graphics g = Graphics.FromImage(b);

            // Copy the screen to the graphics object
            g.CopyFromScreen(rect.left, rect.top, 0,0, b.Size, CopyPixelOperation.SourceCopy);

            // Return the bitmap
            return b;
        }
    }
}
