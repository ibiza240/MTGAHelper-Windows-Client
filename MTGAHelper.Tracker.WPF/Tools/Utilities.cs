using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using MTGAHelper.Lib;

namespace MTGAHelper.Tracker.WPF.Tools
{
    public static class Utilities
    {
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);

        private static readonly Util EntityUtil = new Util();

        internal static string GetThumbnailLocal(string imageArtUrl)
        {
            string thumbnailUrl = EntityUtil.GetThumbnailUrl(imageArtUrl).Replace("/images", "Assets");
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MTGAHelper", thumbnailUrl);
            return path;
        }

        /// <summary>
        /// Extension method for copying the properties of one object to another object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyPropertiesTo<T>(this T source, T destination)
        {
            // Iterate the Properties of the destination instance and  
            // populate them from their source counterparts  
            var destinationProperties = destination.GetType().GetProperties();
            foreach (PropertyInfo destinationPi in destinationProperties)
            {
                PropertyInfo sourcePi = source.GetType().GetProperty(destinationPi.Name);
                if (destinationPi.CanWrite)
                    destinationPi.SetValue(destination, sourcePi?.GetValue(source, null), null);
                else
                {
                    Debug.WriteLine(destinationPi.Name);
                }
            }
        }

        /// <summary>
        /// Extension method for copying the properties of one object to another object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyPropertiesFrom<T>(this T destination, T source)
        {
            // Iterate the Properties of the destination instance and  
            // populate them from their source counterparts  
            var destinationProperties = destination.GetType().GetProperties();
            foreach (PropertyInfo destinationPi in destinationProperties)
            {
                PropertyInfo sourcePi = source.GetType().GetProperty(destinationPi.Name);
                if (destinationPi.CanWrite)
                    destinationPi.SetValue(destination, sourcePi?.GetValue(source, null), null);
                else
                {
                    Debug.WriteLine(destinationPi.Name);
                }
            }
        }

        public static string GetRealTypeName(this Type t)
        {
            if (!t.IsGenericType)
                return t.Name;

            var sb = new StringBuilder();
            sb.Append(t.Name.Substring(0, t.Name.IndexOf('`')));
            sb.Append('<');
            var appendComma = false;
            foreach (Type arg in t.GetGenericArguments())
            {
                if (appendComma) sb.Append(',');
                sb.Append(GetRealTypeName(arg));
                appendComma = true;
            }
            sb.Append('>');
            return sb.ToString();
        }

        public static string GetRealTypeNameXML(this Type t)
        {
            if (!t.IsGenericType)
                return t.Name;

            var sb = new StringBuilder();
            sb.Append(t.Name.Substring(0, t.Name.IndexOf('`')));
            sb.Append('[');
            var appendComma = false;
            foreach (Type arg in t.GetGenericArguments())
            {
                if (appendComma) sb.Append(',');
                sb.Append(GetRealTypeNameXML(arg));
                appendComma = true;
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
