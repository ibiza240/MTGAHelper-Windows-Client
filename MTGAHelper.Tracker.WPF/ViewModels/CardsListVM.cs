using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Utility;

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

        public bool ShowDrawPctAndAmount { get; }
        public bool ShowAmount { get; }
        public CardsListOrder CardsListOrder { get; set; }

        public string CardChosen { get; set; } = "TEST";
        public int CardCount => stats.CardsLeftInDeck;
        public int LandCount => stats.LandsLeftInDeck;
        public int TotalLandsInitial => stats.TotalLandsInitial;
        public float DrawLandPct => stats.DrawLandPct;

        readonly Stats stats = new Stats();
        readonly Util util = new Util();
        readonly BorderGradientCalculator gradientCalculator = new BorderGradientCalculator();

        public CardsListVM(bool showDrawPctAndAmount, bool showAmount, CardsListOrder cardsListOrder)
        {
            ShowDrawPctAndAmount = showDrawPctAndAmount;
            ShowAmount = showAmount;
            CardsListOrder = cardsListOrder;
        }

        internal void SetCards(string cardChosen, ICollection<CardWpf> cards)
        {
            CardChosen = cardChosen;

            var data = cards.Select(i => ConvertCard(i.ArenaId, 1, 1f / cards.Count));
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

        public void ConvertCardList(IReadOnlyCollection<CardDrawInfo> cardsRaw)
        {
            var cardsQuery = cardsRaw
                .Select(c => ConvertCard(c.GrpId, c.Amount, c.DrawChance));

            var cards = OrderByCardListOrder(cardsQuery).ToArray();

            if (Cards != null)
            {
                UpdateCards(cards);
                if (CardsListOrder == CardsListOrder.DrawChance)
                    Cards.Sort(c => c.OrderByDescending(i => i.DrawPercent));
                stats.Refresh(Cards);
                NotifyStatsChanged();
                return;
            }

            if (cardsRaw.Any() == false)
                return;

            Cards = new ObservableCollection<LibraryCardWithAmountVM>(cards.Select(AddBorderColor).Select(c =>
            {
                // set IsAmountChanged = false
                c.Amount = c.Amount;
                return c;
            }));
            stats.Reset(Cards);

            RaisePropertyChangedEvent(string.Empty);
        }

        IOrderedEnumerable<LibraryCardWithAmountVM> OrderByCardListOrder(IEnumerable<LibraryCardWithAmountVM> cardsQuery)
        {
            return CardsListOrder switch
            {
                CardsListOrder.Cmc => cardsQuery.OrderBy(i => i.Type.Contains("Land") ? 0 : 1).ThenBy(i => i.Cmc),
                CardsListOrder.DrawChance => cardsQuery.OrderByDescending(i => i.DrawPercent),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        bool ShouldComeBeforeByCardListOrder(LibraryCardWithAmountVM left, LibraryCardWithAmountVM right)
        {
            return CardsListOrder switch
            {
                CardsListOrder.Cmc => left.Type.Contains("Land") || left.Cmc < right.Cmc,
                CardsListOrder.DrawChance => left.DrawPercent > right.DrawPercent,
                _ => throw new ArgumentOutOfRangeException()
            };
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
                {
                    Cards[i].DrawPercent = 0f;
                    Cards[i].Amount = 0;
                    continue;
                }

                Cards[i].DrawPercent = c.DrawPercent;
                Cards[i].Amount = c.Amount;

                dictCards.Remove(grpId);
            }

            // exit early if done
            if (dictCards.Count <= 0)
                return;

            // Add cards that are not yet in the collection
            var cardsToAddUnordered = dictCards
                .Where(kvp => kvp.Value.Amount > 0)
                .Select(kvp => AddBorderColor(kvp.Value));
            var cardsToAdd = OrderByCardListOrder(cardsToAddUnordered).ToArray();

            var insertAt = 0;
            foreach (var cardToAdd in cardsToAdd)
            {
                // Find where to insert based on CMC
                while (insertAt < Cards.Count && ShouldComeBeforeByCardListOrder(Cards[insertAt], cardToAdd))
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

        LibraryCardWithAmountVM ConvertCard(int grpId, int amount, float drawChance)
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
                DrawPercent = drawChance,
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
