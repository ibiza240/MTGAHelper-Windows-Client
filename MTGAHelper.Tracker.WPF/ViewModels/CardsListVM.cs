using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public enum CardsListOrder
    {
        Cmc,
        DrawChance,
    }

    public class CardsListVM : ObservableObject
    {
        public ObservableCollection<LibraryCardWithAmountVM> Cards { get; set; }

        public bool ShowDrawPctAndAmount { get; set; } = true;
        public bool ShowAmount { get; set; } = true;
        public CardsListOrder CardsListOrder { get; set; } = CardsListOrder.Cmc;

        public string CardChosen { get; set; } = "TEST";
        public int CardCount => stats.CardsLeftInDeck;
        public int LandCount => stats.LandsLeftInDeck;
        public int TotalLandsInitial => stats.TotalLandsInitial;
        public float DrawLandPct => stats.DrawLandPct;

        readonly Stats stats = new Stats();
        readonly Util util = new Util();
        readonly BorderGradientCalculator gradientCalculator = new BorderGradientCalculator();

        public CardsListVM()
        {
        }

        public CardsListVM(bool showDrawPctAndAmount, bool showAmount, CardsListOrder cardsListOrder)
        {
            ShowDrawPctAndAmount = showDrawPctAndAmount;
            ShowAmount = showAmount;
            CardsListOrder = cardsListOrder;
        }

        internal void SetCards(string cardChosen, ICollection<CardWpf> cards)
        {
            CardChosen = cardChosen;

            var data = cards.Select(i => ConvertCard(i.ArenaId, 1, cards.Count));
            Cards = new ObservableCollection<LibraryCardWithAmountVM>(data.Select(AddBorderColor));

            RaisePropertyChangedEvent(nameof(CardChosen));
            RaisePropertyChangedEvent(nameof(Cards));
        }

        public void ResetCards()
        {
            Cards = null;
            stats.Reset();
            RaisePropertyChangedEvent(string.Empty);
        }

        public void ConvertCardList(IReadOnlyDictionary<int, int> cardsRaw)
        {
            var totalCards = cardsRaw.Sum(i => i.Value);

            var cardsQuery = cardsRaw
                .Select(i => ConvertCard(i.Key, i.Value, totalCards));

            switch (CardsListOrder)
            {
                case CardsListOrder.Cmc:
                    cardsQuery = cardsQuery
                        .OrderBy(i => i.Type.Contains("Land") ? 0 : 1)
                        .ThenBy(i => i.Cmc);
                    break;
                case CardsListOrder.DrawChance:
                    cardsQuery = cardsQuery
                        .OrderByDescending(i => i.DrawPercent)
                        .ToArray();
                    break;
            }

            var cards = cardsQuery.ToArray();

            if (Cards != null)
            {
                UpdateCards(cards);
                stats.Refresh(Cards);
                NotifyStatsChanged();
                return;
            }

            if (cardsRaw.Any() == false)
                return;

            Cards = new ObservableCollection<LibraryCardWithAmountVM>(cards.Select(AddBorderColor));
            stats.Reset(Cards);

            RaisePropertyChangedEvent(string.Empty);
        }

        void UpdateCards(ICollection<LibraryCardWithAmountVM> newCards)
        {
            // Remove any card with 0 amount
            for (var i = Cards.Count - 1; i >= 0; i--)
            {
                if (Cards[i].Amount == 0)
                    Cards.RemoveAt(i);
            }

            var dictCards = newCards.ToDictionary(i => i.ArenaId, i => i);

            // Modify card counts for cards already in the collection
            for (var i = Cards.Count - 1; i >= 0; i--)
            {
                var grpId = Cards[i].ArenaId;

                if (!dictCards.TryGetValue(grpId, out var c))
                    continue;

                Cards[i].DrawPercent = c.DrawPercent;
                Cards[i].Amount = c.Amount;

                dictCards.Remove(grpId);
            }

            // exit early if done
            if (dictCards.Count <= 0)
                return;

            // Add cards that are not yet in the collection
            var cardsToAdd = dictCards
                .Where(kvp => kvp.Value.Amount > 0)
                .Select(kvp => AddBorderColor(kvp.Value))
                .OrderBy(c => c.Cmc)
                .ToArray();

            var insertAt = 0;
            foreach (var cardToAdd in cardsToAdd)
            {
                // Find where to insert based on CMC
                while (insertAt < Cards.Count && Cards[insertAt].Cmc < cardToAdd.Cmc)
                    insertAt++;

                Cards.Insert(insertAt, cardToAdd);
                insertAt++;
            }
        }

        LibraryCardWithAmountVM AddBorderColor(LibraryCardWithAmountVM card)
        {
            card.BorderGradient = gradientCalculator.CalculateBorderGradient(card);
            return card;
        }

        LibraryCardWithAmountVM ConvertCard(int grpId, int amount, int totalCards)
        {
            var card = Mapper.Map<Entity.Card>(grpId);

            var ret = new LibraryCardWithAmountVM
            {
                ArenaId = grpId,
                Amount = amount,
                Colors = card.colors,
                ColorIdentity = card.color_identity,
                ImageArtUrl = util.GetThumbnailLocal(card.imageArtUrl),
                ImageCardUrl = card.imageCardUrl,
                Name = card.name,
                Rarity = card.rarity,
                DrawPercent = (float)amount / totalCards,
                Cmc = card.cmc,
                ManaCost = card.mana_cost,
                Type = card.type,
            };

            return ret;
        }

        void NotifyStatsChanged()
        {
            RaisePropertyChangedEvent(nameof(CardCount));
            RaisePropertyChangedEvent(nameof(LandCount));
            RaisePropertyChangedEvent(nameof(DrawLandPct));
        }
    }
}
