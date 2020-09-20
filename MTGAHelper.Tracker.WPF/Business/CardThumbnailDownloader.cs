using MTGAHelper.Entity;
using MTGAHelper.Entity.Config.App;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Tracker.WPF.Config;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MTGAHelper.Tracker.WPF.Business
{
    public class CardThumbnailDownloader
    {
        WebClient webClient = new System.Net.WebClient();
        Dictionary<int, Card> allCards;
        string folderData;

        public CardThumbnailDownloader(
            IDataPath dataPath,
            Dictionary<int, Card> allCards)
        {
            this.folderData = dataPath.FolderData;
            this.allCards = allCards;
        }

        public void CheckAndDownloadThumbnails(ICollection<int> grpIds)
        {
            foreach (var grpId in grpIds.Where(i => IsImageAvailable(i) == false))
            {
                DownloadImageFor(grpId);
            }
        }

        private bool IsImageAvailable(int grpId)
        {
            //try
            //{
            //    var thumbnailLocal = Utilities.GetThumbnailLocal(allCards[grpId].imageArtUrl);
            //    var found = File.Exists(thumbnailLocal);
            //    return found;
            //}
            //catch
            //{
            //    // Do as if the file exists if any error
            //    Log.Error("Error in IsImageAvailable for grpId {grpId}", grpId);
                return true;
            //}
        }

        private void DownloadImageFor(int grpId)
        {
            //try
            //{
            //    var url = DebugOrRelease.Server + new Lib.Util().GetThumbnailUrl(allCards[grpId].imageArtUrl);
            //    var imageBytes = webClient.DownloadData(url);

            //    var thumbnailLocal = Utilities.GetThumbnailLocal(allCards[grpId].imageArtUrl);
            //    File.WriteAllBytes(thumbnailLocal, imageBytes);
            //}
            //catch
            //{
            //    // Avoid crashing the tracker in any case
            //    Log.Error("Error in DownloadImageFor for grpId {grpId}", grpId);
            //}
        }
    }
}
