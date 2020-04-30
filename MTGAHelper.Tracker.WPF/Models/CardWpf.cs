using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Models
{
    public class CardWithAmountWpf : CardWpf
    {
        public virtual int Amount { get; set; }
    }

    public class CardWpf : ObservableObject
    {
        public int ArenaId { get; set; }

        public string Name { get; set; }

        public string Rarity { get; set; }

        public string ImageCardUrl { get; set; }

        public string ImageArtUrl { get; set; }

        public ICollection<string> Colors { get; set; } = new string[0];

        public ICollection<string> ColorIdentity { get; set; } = new string[0];

        public string ManaCost { get; set; }

        public int Cmc { get; set; }

        public string Type { get; set; }

        private static readonly Regex RegexCmcImages = new Regex(@"{([^}]+)}", RegexOptions.Compiled);

        public IEnumerable<string> CmcImages
        {
            get
            {
                MatchCollection matches = RegexCmcImages.Matches(ManaCost ?? "");

                if (matches.Count == 0) return new string[0];

                var ret = matches.Cast<Match>()
                    .Select(i => i.Value.Replace("{", "").Replace("}", "").Replace("/", ""))
                    .Select(i => $"https://www.mtgahelper.com/images/manaIcons/{i}.png")
                    .ToArray();

                return ret;
            }
        }
    }
}
