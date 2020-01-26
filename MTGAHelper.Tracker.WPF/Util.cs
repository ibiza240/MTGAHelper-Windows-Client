using System;
using System.IO;

namespace MTGAHelper.Tracker.WPF
{
    class Util
    {
        readonly Entity.Util util = new Entity.Util();

        internal string GetThumbnailLocal(string imageArtUrl)
        {
            var thumbnailUrl = util.GetThumbnailUrl(imageArtUrl).Replace("/images", "Assets");
            var ret =
                Path.GetFullPath(
#if DEBUG == false || DEBUGWITHSERVER
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper") + "\\" +
#endif
                        thumbnailUrl);

            return ret;
        }
    }
}
