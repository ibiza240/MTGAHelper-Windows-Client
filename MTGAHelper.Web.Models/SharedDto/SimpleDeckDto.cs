using MTGAHelper.Web.UI.Model.Response.Dto;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.SharedDto
{
    public class SimpleDeckDto
    {
        public string Name { get; set; } = "N/A";
        public ICollection<DeckCardDto> Cards { get; set; } = new DeckCardDto[0];

        public string Colors { get; set; } = "";
        public string DeckImage { get; set; } = "";

        //internal string GetColors()
        //{
        //    var order = new Dictionary<char, int> {
        //        { 'W', 1 },
        //        { 'U', 2 },
        //        { 'B', 3 },
        //        { 'R', 4 },
        //        { 'G', 5 },
        //    };

        //    var colors = Main.Union(Sideboard).SelectMany(i => i.Color.ToArray()).Distinct();
        //    var colorsFinal = colors.OrderBy(i => order[i]);
        //    return string.Join("", colorsFinal);
        //}
    }
}
