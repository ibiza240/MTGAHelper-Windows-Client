using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Interop;
using MTGAHelper.Tracker.WPF.Tools;
using Serilog;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class ScreenshotTaker
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        //[DllImport("user32.dll")]
        //public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        //[DllImport("user32.dll")]
        //public static extern IntPtr GetClientRect(IntPtr hWnd, out RECT lpRect);

        //[DllImport("user32.dll")]
        //public static extern IntPtr GetWindowDC(IntPtr hWnd);
        //[DllImport("user32.dll")]
        //public static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hSrc);

        //[DllImport("gdi32.dll")]
        //public static extern IntPtr CreateCompatibleDC(IntPtr hWnd);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr CreateCompatibleBitmap(IntPtr hWnd, int width, int height);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr SelectObject(IntPtr hdcDest, IntPtr hBitmap);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr BitBlt(IntPtr hdcDest, int i1, int i2, int width, int height, IntPtr hdcSrc, int i3, int i4, CopyPixelOperation srcCopy);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr DeleteDC(IntPtr hWnd);
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr DeleteObject(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hWnd, out WINDOWINFO pwi);

        public static Bitmap TakeScreenshot(string processName)
        {
            if (processName == null)
            {
                // Just take a whole screen screenshot if no process provided
                return TakeScreenshotFullscreen(processName);
            }

            var screenSize = GetScreenSize();

            Process proc = Process.GetProcessesByName(processName)[0];
            IntPtr hwnd = proc.MainWindowHandle;
            var rc = GetWindowRectLooped(hwnd);

            // Take safer method if Fullscreen
            if (rc.Width == screenSize.Width && rc.Height == screenSize.Height)
                return TakeScreenshotFullscreen(processName);
            else
                return TakeScreenshotWindow(processName);
        }

        private static Bitmap TakeScreenshotWindow(string processName)
        {
            Log.Information("TakeScreenshotWindow {procName}", processName ?? "[no process]");

            Process proc = Process.GetProcessesByName(processName)[0];
            IntPtr hwnd = proc.MainWindowHandle;

            // Focus on the application
            Utilities.SetForegroundWindow(proc.MainWindowHandle);
            Utilities.ShowWindow(proc.MainWindowHandle, 9);
            // Need some amount of delay to let window bring the window forward
            Thread.Sleep(500);

            // Get Raw screenshot of window
            var rc = GetWindowRectLooped(hwnd);

            Log.Information("TakeScreenshotWindow WindowSize {rc}", rc.Size);

            //Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
            //using (Graphics gfxBmp = Graphics.FromImage(bmp))
            //{
            //    IntPtr hdcBitmap = gfxBmp.GetHdc();
            //    PrintWindow(hwnd, hdcBitmap, 0);
            //    gfxBmp.ReleaseHdc(hdcBitmap);
            //}
            WINDOWINFO wi;
            var  windowRect = GetWindowRectLooped(hwnd);
            GetWindowInfo(hwnd, out wi);

            var captionHeight = wi.rcClient.Top - wi.rcWindow.Top;
            var rect = new RECT(windowRect.Left + (int)wi.cxWindowBorders, windowRect.Top + captionHeight,
                windowRect.Right - (int)wi.cxWindowBorders, windowRect.Bottom - (int)wi.cyWindowBorders);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height); // fix black -> transparent
                g.CopyFromScreen(rect.Location, Point.Empty, rect.Size, CopyPixelOperation.SourceCopy);
            }
            //bmp.Save(@"C:\Users\BL\source\repos\test.bmp");
            return bmp;

            //// Adjust to client size
            //RECT rcSize;
            //GetClientRect(hwnd, out rcSize);

            //// Calculate border at the sides of the raw screenshot
            //var border = (rc.Width - rcSize.Width) / 2;
            //var y = rc.Height - border - rcSize.Height;
            //Bitmap windowContent = new Bitmap(rcSize.Width, rcSize.Height, PixelFormat.Format24bppRgb);
            //using (Graphics gfxBmp = Graphics.FromImage(windowContent))
            //{
            //    gfxBmp.DrawImage(bmp, new Point(-border, -y));
            //}

            ////windowContent.Save(@"C:\Users\BL\source\repos\test.bmp");
            //return windowContent;
        }

        static Bitmap TakeScreenshotFullscreen(string procName)
        {
            Log.Information("TakeScreenshotFullscreen {procName}", procName ?? "[no process]");

            if (procName != null)
            {
                Process proc = Process.GetProcessesByName(procName)[0];
                IntPtr hwnd = proc.MainWindowHandle;

                // Focus on the application
                Utilities.SetForegroundWindow(hwnd);
                Utilities.ShowWindow(hwnd, 9);

                // Need some amount of delay to let window bring the window forward
                Thread.Sleep(500);
            }

            var screenSize = GetScreenSize();

            Log.Information("TakeScreenshotFullscreen ScreenSize {rc}", screenSize);

            // Create a bitmap
            var b = new Bitmap(screenSize.Width, screenSize.Height, PixelFormat.Format24bppRgb);

            // Create a graphics object from the bitmap
            using (Graphics g = Graphics.FromImage(b))
            {
                // Copy the screen to the graphics object
                g.CopyFromScreen(0, 0, 0, 0, screenSize, CopyPixelOperation.SourceCopy);
            }

            // Return the bitmap
            return b;
        }

        private static Size GetScreenSize()
        {
            // This block is required to work with scales other than 100% (in Display Properties)
            System.Windows.Media.Matrix toDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
            {
                toDevice = source.CompositionTarget.TransformToDevice;
            }
            var screenWidth = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenWidth * toDevice.M11);
            var screenHeight = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenHeight * toDevice.M22);

            //var screenSize = new Size((int)System.Windows.SystemParameters.PrimaryScreenWidth, (int)System.Windows.SystemParameters.PrimaryScreenHeight);
            var screenSize = new Size(screenWidth, screenHeight);
            return screenSize;
        }

        static RECT GetWindowRectLooped(IntPtr hwnd)
        {
            RECT rect;
            var retry = 0;
            const int maxRetry = 10;
            bool ok;
            // Sometimes it gives an error, try a few times
            do
            {
                ok = GetWindowRect(hwnd, out rect);
                retry++;

                if (ok == false)
                    Thread.Sleep(500);
            }
            while (ok == false && retry <= maxRetry);

            if (retry > 1)
                Debugger.Break();

            // Final check for error
            if (ok == false)
                throw new Exception("Cannot get game window size");

            return rect;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWINFO
    {
        public uint cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public uint dwStyle;
        public uint dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        public WINDOWINFO(Boolean? filler) : this() // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        {
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;

        public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
        {
        }
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            _Left = Left;
            _Top = Top;
            _Right = Right;
            _Bottom = Bottom;
        }

        public int X
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Y
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        public int Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Height
        {
            get { return _Bottom - _Top; }
            set { _Bottom = value + _Top; }
        }
        public int Width
        {
            get { return _Right - _Left; }
            set { _Right = value + _Left; }
        }
        public Point Location
        {
            get { return new Point(Left, Top); }
            set
            {
                _Left = value.X;
                _Top = value.Y;
            }
        }
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                _Right = value.Width + _Left;
                _Bottom = value.Height + _Top;
            }
        }

        public static implicit operator Rectangle(RECT Rectangle)
        {
            return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
        }
        public static implicit operator RECT(Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }
        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }
        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override string ToString()
        {
            return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
        }

        public override bool Equals(object Object)
        {
            if (Object is RECT)
            {
                return Equals((RECT)Object);
            }
            else if (Object is Rectangle)
            {
                return Equals(new RECT((Rectangle)Object));
            }

            return false;
        }
    }
}
