using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTGAHelper.Tracker.WPF
{
    class Util
    {
        internal string GetThumbnailLocal(string imageArtUrl)
        {
            var thumbnailUrl = new Entity.Util().GetThumbnailUrl(imageArtUrl);
            var ret =
                Path.GetFullPath(
#if DEBUG == false
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTGAHelper") + "\\" +
#endif
                        thumbnailUrl
                    .Replace("/images", "Assets"));

            return ret;
        }
    }
}
