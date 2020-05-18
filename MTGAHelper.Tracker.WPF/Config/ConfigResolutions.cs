using MTGAHelper.Tracker.DraftHelper.Shared.Config;
using System.Collections.Generic;

namespace MTGAHelper.Tracker.WPF.Config
{
    public class ConfigResolutions : IConfigResolutions
    {
        public ICollection<ConfigResolution> ResolutionSettings { get; set; } = new ConfigResolution[]
        {
            //// Ratio 1.25 (5:4)
            //new ConfigResolution
            //{
            //        Resolution = new Size(1280, 1024),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(147, 107),
            //            FirstCardArtLocation = new System.Drawing.Point(3, 198),
            //            LastCardArtLocation = new System.Drawing.Point(747, 706),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(2560, 2048),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(260, 189),
            //            FirstCardArtLocation = new System.Drawing.Point(160, 472),
            //            LastCardArtLocation = new System.Drawing.Point(1468, 1367),
            //        },
            //},

            //// Ratio 1.33 (4:3)
            //new ConfigResolution
            //{
            //        Resolution = new Size(1024, 768),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(111, 81),
            //            FirstCardArtLocation = new System.Drawing.Point(34, 148),
            //            LastCardArtLocation = new System.Drawing.Point(592, 530),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1152, 864),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(104, 76),
            //            FirstCardArtLocation = new System.Drawing.Point(128, 211),
            //            LastCardArtLocation = new System.Drawing.Point(651, 569),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1280, 960),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(130, 94),
            //            FirstCardArtLocation = new System.Drawing.Point(80, 204),
            //            LastCardArtLocation = new System.Drawing.Point(734, 651),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1400, 1050),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(151, 110),
            //            FirstCardArtLocation = new System.Drawing.Point(47, 203),
            //            LastCardArtLocation = new System.Drawing.Point(809, 464),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1600, 1200),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(156, 113),
            //            FirstCardArtLocation = new System.Drawing.Point(128, 269),
            //            LastCardArtLocation = new System.Drawing.Point(912, 805),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(2048, 1536),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(208, 152),
            //            FirstCardArtLocation = new System.Drawing.Point(128, 326),
            //            LastCardArtLocation = new System.Drawing.Point(1174, 1042),
            //        },
            //},

            //// Ratio 1.6  (16:10)
            //new ConfigResolution
            //{
            //        Resolution = new Size(1280, 800),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(111, 80),
            //            FirstCardArtLocation = new System.Drawing.Point(162, 165),
            //            LastCardArtLocation = new System.Drawing.Point(720, 546),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1440, 900),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(130, 94),
            //            FirstCardArtLocation = new System.Drawing.Point(160, 174),
            //            LastCardArtLocation = new System.Drawing.Point(814, 621),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1680, 1050),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(151, 109),
            //            FirstCardArtLocation = new System.Drawing.Point(187, 204),
            //            LastCardArtLocation = new System.Drawing.Point(950, 725),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1920, 1200),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(174, 125),
            //            FirstCardArtLocation = new System.Drawing.Point(213, 233),
            //            LastCardArtLocation = new System.Drawing.Point(1085, 828),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(2560, 1600),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(230, 167),
            //            FirstCardArtLocation = new System.Drawing.Point(285, 309),
            //            LastCardArtLocation = new System.Drawing.Point(1446, 1104),
            //        },
            //},

            //// Ratio 1.66 (5:3)
            //new ConfigResolution
            //{
            //        Resolution = new Size(1280, 768),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(111, 80),
            //            FirstCardArtLocation = new System.Drawing.Point(162, 149),
            //            LastCardArtLocation = new System.Drawing.Point(720, 530),
            //        },
            //},

            //// Ratio 1.77 (16:9)
            //new ConfigResolution
            //{
            //        Resolution = new Size(768, 432),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(62, 45),
            //            FirstCardArtLocation = new System.Drawing.Point(115, 84),
            //            LastCardArtLocation = new System.Drawing.Point(429, 298),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1024, 576),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(83, 61),
            //            FirstCardArtLocation = new System.Drawing.Point(154, 111),
            //            LastCardArtLocation = new System.Drawing.Point(572, 397),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1024, 768),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(83, 61),
            //            FirstCardArtLocation = new System.Drawing.Point(154, 207),
            //            LastCardArtLocation = new System.Drawing.Point(572, 493),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1152, 648),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(93, 68),
            //            FirstCardArtLocation = new System.Drawing.Point(173, 125),
            //            LastCardArtLocation = new System.Drawing.Point(643, 447),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1152, 864),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(93, 68),
            //            FirstCardArtLocation = new System.Drawing.Point(173, 233),
            //            LastCardArtLocation = new System.Drawing.Point(643, 555),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1280, 720),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(104, 76),
            //            FirstCardArtLocation = new System.Drawing.Point(192, 139),
            //            LastCardArtLocation = new System.Drawing.Point(715, 497),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1280, 800),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(104, 76),
            //            FirstCardArtLocation = new System.Drawing.Point(192, 179),
            //            LastCardArtLocation = new System.Drawing.Point(715, 537),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1280, 960),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(104, 76),
            //            FirstCardArtLocation = new System.Drawing.Point(192, 259),
            //            LastCardArtLocation = new System.Drawing.Point(715, 617),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1366, 768),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(110, 80),
            //            FirstCardArtLocation = new System.Drawing.Point(206, 149),
            //            LastCardArtLocation = new System.Drawing.Point(763, 530),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1440, 900),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(117, 84),
            //            FirstCardArtLocation = new System.Drawing.Point(216, 202),
            //            LastCardArtLocation = new System.Drawing.Point(805, 604),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1536, 864),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(124, 91),
            //            FirstCardArtLocation = new System.Drawing.Point(231, 167),
            //            LastCardArtLocation = new System.Drawing.Point(858, 596),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1600, 900),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(130, 94),
            //            FirstCardArtLocation = new System.Drawing.Point(240, 174),
            //            LastCardArtLocation = new System.Drawing.Point(894, 621),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1600, 1200),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(130, 94),
            //            FirstCardArtLocation = new System.Drawing.Point(240, 324),
            //            LastCardArtLocation = new System.Drawing.Point(894, 771),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1664, 936),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(134, 98),
            //            FirstCardArtLocation = new System.Drawing.Point(250, 181),
            //            LastCardArtLocation = new System.Drawing.Point(929, 646),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1680, 1050),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(136, 99),
            //            FirstCardArtLocation = new System.Drawing.Point(252, 235),
            //            LastCardArtLocation = new System.Drawing.Point(938, 705),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1792, 1008),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(145, 106),
            //            FirstCardArtLocation = new System.Drawing.Point(269, 195),
            //            LastCardArtLocation = new System.Drawing.Point(1001, 695),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1792, 1344),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(156, 113),
            //            FirstCardArtLocation = new System.Drawing.Point(224, 341),
            //            LastCardArtLocation = new System.Drawing.Point(1008, 877),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1920, 1080),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(156, 113),
            //            FirstCardArtLocation = new System.Drawing.Point(289, 209),
            //            LastCardArtLocation = new System.Drawing.Point(1072, 745),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(1920, 1200),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(156, 113),
            //            FirstCardArtLocation = new System.Drawing.Point(289, 269),
            //            LastCardArtLocation = new System.Drawing.Point(1072, 805),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(2048, 1536),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(167, 120),
            //            FirstCardArtLocation = new System.Drawing.Point(307, 415),
            //            LastCardArtLocation = new System.Drawing.Point(1144, 987),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(2560, 1440),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(206, 150),
            //            FirstCardArtLocation = new System.Drawing.Point(385, 279),
            //            LastCardArtLocation = new System.Drawing.Point(1430, 993),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(2560, 1600),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(206, 150),
            //            FirstCardArtLocation = new System.Drawing.Point(385, 359),
            //            LastCardArtLocation = new System.Drawing.Point(1430, 1074),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(2560, 2048),
            //        IsPanoramic = true,
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(208, 152),
            //            FirstCardArtLocation = new System.Drawing.Point(384, 582),
            //            LastCardArtLocation = new System.Drawing.Point(1430, 1298),
            //        },
            //},
            //new ConfigResolution
            //{
            //        Resolution = new Size(3840, 2160),
            //        Template = new ConfigResolutionTemplate
            //        {
            //            ArtSize = new Size(310, 226),
            //            FirstCardArtLocation = new System.Drawing.Point(577, 418),
            //            LastCardArtLocation = new System.Drawing.Point(2145, 1490),
            //        },
            //},
        };

    }
}
