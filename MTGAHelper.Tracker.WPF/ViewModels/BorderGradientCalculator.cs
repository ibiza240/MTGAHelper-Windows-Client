using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MTGAHelper.Tracker.WPF.Models;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class BorderGradientCalculator
    {
        private readonly Dictionary<string, Color> DictColorToHex = new Dictionary<string, Color>
        {
            { "W", Color.FromRgb(255,255,255) },
            { "U", Color.FromRgb(0,0,255) },
            { "B", Color.FromRgb(0,0,0) },
            { "R", Color.FromRgb(255,0,0) },
            { "G", Color.FromRgb(34,139,34) },
        };

        public GradientStopCollection CalculateBorderGradient(CardWpf theCard)
        {
            var colorsOfTheCard = theCard.Colors != null && theCard.Colors.Any()
                ? theCard.Colors
                : theCard.ColorIdentity;

            var gradient = CreateGradient(colorsOfTheCard.Select(c => DictColorToHex[c]).ToArray());

            return gradient;
        }

        private GradientStopCollection CreateGradient(IReadOnlyList<Color> colorsOfTheCard)
        {
            switch (colorsOfTheCard.Count)
            {
                case 1:
                    return CreateGradientMono(colorsOfTheCard[0]);
                case 2:
                    return CreateGradientDual(colorsOfTheCard[0], colorsOfTheCard[1]);
                case 3:
                    return CreateGradientTriple(colorsOfTheCard[0], colorsOfTheCard[1], colorsOfTheCard[2]);
                case 4:
                case 5:
                    return CreateGradientGold();
                default:
                    // Colorless or default if any problem
                    return CreateGradientColorless();
            }
        }

        private GradientStopCollection CreateGradientMono(Color color)
        {
            return new GradientStopCollection(new[] {
                new GradientStop(color, 0d),
                new GradientStop(color, 1d),
            });
        }

        private GradientStopCollection CreateGradientDual(Color c1, Color c2)
        {
            return new GradientStopCollection(new[] {
                new GradientStop(c1, 0d),
                new GradientStop(c2, 1d),
            });
        }

        private GradientStopCollection CreateGradientTriple(Color c1, Color c2, Color c3)
        {
            return new GradientStopCollection(new[] {
                new GradientStop(c1, 0d),
                new GradientStop(c1, 0.1d),
                new GradientStop(c2, 0.25),
                new GradientStop(c2, 0.75),
                new GradientStop(c3, 0.9d),
                new GradientStop(c3, 1d),
            });
        }

        private GradientStopCollection CreateGradientGold()
        {
            return new GradientStopCollection(new[] {
                new GradientStop(Color.FromRgb(255,215,0), 0d),
                new GradientStop(Color.FromRgb(255,215,0), 1d),
            });
        }

        private GradientStopCollection CreateGradientColorless()
        {
            return new GradientStopCollection(new[] {
                new GradientStop(Color.FromRgb(128,128,128), 0d),
                new GradientStop(Color.FromRgb(128,128,128), 1d),
            });
        }
    }
}
