using MTGAHelper.Entity;
//using Serilog;
using System.Collections.Generic;

namespace MTGAHelper.Lib.CollectionDecksCompare
{
    public class CardRequiredInfo
    {
        public static readonly Dictionary<RarityEnum, UserWeightDto> DEFAULT_WEIGHTS = new Dictionary<RarityEnum, UserWeightDto>
        // Default weights but can be changed
        {
            {  RarityEnum.Mythic, new UserWeightDto(150, 1) },
            {  RarityEnum.RareLand, new UserWeightDto(0, 0) },
            {  RarityEnum.RareNonLand, new UserWeightDto(120, 1) },
            {  RarityEnum.Uncommon, new UserWeightDto(30, 1) },
            {  RarityEnum.Common, new UserWeightDto(40, 1) },
            {  RarityEnum.Other, new UserWeightDto(0, 0) },
        };

        public Card Card { get; private set; }
        public bool IsSideboard { get; private set; }
        public string DeckId { get; private set; }
        public string DeckName { get; private set; }
        public int NbRequired { get; private set; }
        public int NbOwned { get; private set; }
        public int NbMissing { get; private set; } // { return Math.Max(0, NbRequired - NbOwned); } }
        public float MissingWeight { get; private set; }
        public bool IsForAverageArchetypeOthersInMain { get; private set; }

        //public void SetMissingWeight(float newWeight) => MissingWeight = newWeight;

        public CardRequiredInfo(Card card, bool isForAverageArchetypeOthersInMain, bool isSideboard, IDeck deck, int nbRequired, int nbOwned, int nbMissing, float priorityFactor,
            string userId, Dictionary<RarityEnum, UserWeightDto> weights)
        {
            var weightsToUse = weights ?? DEFAULT_WEIGHTS;

            //if (deck.Name.StartsWith("Jod"))
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
            //if (card.name == "Duress") System.Diagnostics.Debugger.Break();

            Card = card;
            IsSideboard = isSideboard;
            DeckId = deck.Id;
            DeckName = deck.Name;
            NbRequired = nbRequired;
            NbOwned = nbOwned;
            NbMissing = nbMissing;
            IsForAverageArchetypeOthersInMain = isForAverageArchetypeOthersInMain;

            var cardWeight = -10000f;
            var rarity = card.GetRarityEnum(true);
            if (weightsToUse.ContainsKey(rarity))
            {
                cardWeight = isSideboard ? weightsToUse[rarity].Sideboard : weightsToUse[rarity].Main;
            }
            else
            {
                ////System.Diagnostics.Debugger.Break();
                //Log.Error("User {userId} CardRequiredInfo problem with {cardName}", userId, card.name);
                ////var ex = new CardRequiredInfoWeightException("CardRequiredInfo problem with {cardName}");
                ////ex.Data.Add("cardName", card.name);
                ////throw ex;
            }

            MissingWeight = cardWeight * NbMissing * priorityFactor;
        }
    }
}
