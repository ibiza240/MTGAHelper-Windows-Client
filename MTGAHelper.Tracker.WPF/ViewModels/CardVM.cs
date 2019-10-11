using MTGAHelper.Tracker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardVM : CardWpf
    {
        Dictionary<string, Color> DictColorToHex = new Dictionary<string, Color>
        {
            { "W", Color.FromRgb(255,255,255) },
            { "U", Color.FromRgb(0,0,255) },
            { "B", Color.FromRgb(0,0,0) },
            { "R", Color.FromRgb(255,0,0) },
            { "G", Color.FromRgb(34,139,34) },
        };

        ICollection<string> ColorsOfTheCard => Colors != null && Colors.Any() ? Colors : ColorIdentity;

        public ObservableProperty<GradientStopCollection> ColorGradient { get; set; } = new ObservableProperty<GradientStopCollection>(new GradientStopCollection());

        public void SetColorBorder()
        {
            ColorGradient.Value = CreateGradient();
        }

        private GradientStopCollection CreateGradient()
        {
            if (ColorsOfTheCard.Count == 1)
                return CreateGradientMono(ColorsOfTheCard.First());
            else if (ColorsOfTheCard.Count == 2)
                return CreateGradientDual();
            else if (ColorsOfTheCard.Count == 3)
                return CreateGradientTriple();
            else if (ColorsOfTheCard.Count > 3)
                return CreateGradientGold();

            // Colorless or default if any problem
            return CreateGradientColorless();
        }

        private GradientStopCollection CreateGradientMono(string color)
        {
            return new GradientStopCollection(new[] {
                new GradientStop(DictColorToHex[color], 0d),
                new GradientStop(DictColorToHex[color], 1d),
            });
        }

        private GradientStopCollection CreateGradientDual()
        {
            return new GradientStopCollection(new[] {
                new GradientStop(DictColorToHex[ColorsOfTheCard.First()], 0d),
                new GradientStop(DictColorToHex[ColorsOfTheCard.Last()], 1d),
            });
        }

        private GradientStopCollection CreateGradientTriple()
        {
            var c1 = ColorsOfTheCard.First();
            var c2 = ColorsOfTheCard.Skip(1).First();
            var c3 = ColorsOfTheCard.Last();

            return new GradientStopCollection(new[] {
                new GradientStop(DictColorToHex[c1], 0d),
                new GradientStop(DictColorToHex[c1], 0.1d),
                new GradientStop(DictColorToHex[c2], 0.25),
                new GradientStop(DictColorToHex[c2], 0.75),
                new GradientStop(DictColorToHex[c3], 0.9d),
                new GradientStop(DictColorToHex[c3], 1d),
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
