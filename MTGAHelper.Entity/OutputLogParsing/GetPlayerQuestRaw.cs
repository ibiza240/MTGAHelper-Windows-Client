using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class LocParams
    {
        public int number1 { get; set; }
        public int number2 { get; set; }
        public int number3 { get; set; }
    }

    public class ChestDescription
    {
        public string image1 { get; set; }
        public object image2 { get; set; }
        public object image3 { get; set; }
        public string prefab { get; set; }
        public object referenceId { get; set; }
        public string headerLocKey { get; set; }
        public object descriptionLocKey { get; set; }
        public string quantity { get; set; }
        public LocParams locParams { get; set; }
        public DateTime availableDate { get; set; }
    }

    public class GetPlayerQuestRaw
    {
        public string questId { get; set; }
        public int goal { get; set; }
        public string locKey { get; set; }
        public string tileResourceId { get; set; }
        public string treasureResourceId { get; set; }
        public string questTrack { get; set; }
        public bool isNewQuest { get; set; }
        public int endingProgress { get; set; }
        public int startingProgress { get; set; }
        public bool canSwap { get; set; }
        public object inventoryUpdate { get; set; }
        public ChestDescription chestDescription { get; set; }
        public int hoursWaitAfterComplete { get; set; }
    }
}
