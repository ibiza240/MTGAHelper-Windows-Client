using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class CardSkin
    {
        public int grpId { get; set; }
        public string ccv { get; set; }
    }

    public class CourseDeckRaw
    {
        public int commandZoneGRPId { get; set; }
        public bool isValid { get; set; }
        public bool lockedForUse { get; set; }
        public bool lockedForEdit { get; set; }
        public string resourceId { get; set; }
        //public List<CardSkin> cardSkins { get; set; }
        //public string id { get; set; }
        //public string name { get; set; }
        //public string description { get; set; }
        //public string format { get; set; }
        //public int deckTileId { get; set; }
        //public List<int> mainDeck { get; set; }
        //public List<int> sideboard { get; set; }
        //public object cardBack { get; set; }
        //public DateTime lastUpdated { get; set; }




        public List<CardSkin> cardSkins { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string format { get; set; }
        public int? deckTileId { get; set; }
        public List<int> mainDeck { get; set; }
        public List<int> sideboard { get; set; }
        public string cardBack { get; set; }
        public DateTime lastUpdated { get; set; }
    }

}
