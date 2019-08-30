using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class MtgaDeck
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public int DeckTileId { get; set; }
        //public IList<int> mainDeck { get; set; }
        //public IList<int> sideboard { get; set; }
        //public dynamic cardBack { get; set; }
        public string LastUpdated { get; set; }
    }

}
