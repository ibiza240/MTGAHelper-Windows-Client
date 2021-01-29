using AutoMapper;
using MTGAHelper.Lib;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public enum DisplayType
    {
        Percent,
        CountOnly,
        None
    }

    public enum CardsListOrder
    {
        ManaCost,
        DrawChance,
    }

    public class CardsListVM : BasicModel
    {
        public event EventHandler CardsUpdated;

        private readonly IMapper mapper;
        private readonly CardThumbnailDownloader CardThumbnailDownloader;

        public CardsListVM(DisplayType display, CardsListOrder cardsListOrder, IMapper mapper, CardThumbnailDownloader cardThumbnailDownloader)
        {
            CardsListOrder = cardsListOrder;
            Display = display;
            this.mapper = mapper;
            CardThumbnailDownloader = cardThumbnailDownloader;
        }

        public ObservableCollection<LibraryCardWithAmountVM> Cards { get; set; }

        public bool ShowImage
        {
            get => _ShowCardImage;
            set => SetField(ref _ShowCardImage, value, nameof(ShowImage));
        }

        public bool ShowDrawPercent => Display == DisplayType.Percent;
        public bool ShowCardCountOnly => Display == DisplayType.CountOnly;
        public CardsListOrder CardsListOrder { get; set; }
        public string CardChosen { get; set; } = "TEST";
        public int CardCount => Stats.CardsLeftInDeck;
        public int LandCount => Stats.LandsLeftInDeck;
        public int TotalLandsInitial => Stats.TotalLandsInitial;
        public float DrawLandPct => Stats.DrawLandPct;

        private bool _ShowCardImage = true;
        private DisplayType Display { get; }
        private readonly Stats Stats = new Stats();
        private readonly BorderGradientCalculator GradientCalculator = new BorderGradientCalculator();

        public void ResetCards()
        {
            Cards = null;
            Stats.Reset();
            OnPropertyChanged(string.Empty);
        }

        public void ConvertCardList(IReadOnlyCollection<CardDrawInfo> cardsRaw)
        {
            var cardsQuery = cardsRaw
                .Select(c => ConvertCard(c.GrpId, c.Amount, c.DrawChance));

            var cards = OrderByCardListOrder(cardsQuery).ToArray();

            if (Cards != null)
            {
                //var countChanged = cards.Sum(i => i.Amount) != Cards.Sum(i => i.Amount);
                var countChanged = cards.Count() != Cards.Count();

                UpdateCards(cards);
                if (CardsListOrder == CardsListOrder.DrawChance)
                    Cards.Sort(c => c.OrderByDescending(i => i.DrawPercent));
                Stats.Refresh(Cards);
                NotifyStatsChanged();

                if (countChanged)
                    CardsUpdated?.Invoke(this, null);

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
            Stats.Reset(Cards);

            OnPropertyChanged(string.Empty);
        }

        private IOrderedEnumerable<LibraryCardWithAmountVM> OrderByCardListOrder(IEnumerable<LibraryCardWithAmountVM> cardsQuery)
        {
            return CardsListOrder switch
            {
                CardsListOrder.ManaCost => cardsQuery.OrderBy(i => i.Type.Contains("Land") ? 1 : 0).ThenBy(i => i.Cmc),
                CardsListOrder.DrawChance => cardsQuery.OrderByDescending(i => i.DrawPercent).ThenBy(i => i.Type.Contains("Land") ? 1 : 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool ShouldComeBeforeByCardListOrder(LibraryCardWithAmountVM left, LibraryCardWithAmountVM right)
        {
            return CardsListOrder switch
            {
                CardsListOrder.ManaCost => left.Type.Contains("Land") || left.Cmc < right.Cmc,
                CardsListOrder.DrawChance => left.DrawPercent > right.DrawPercent,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void UpdateCards(ICollection<LibraryCardWithAmountVM> newCards)
        {
            // Remove any card with 0 amount
            for (int i = Cards.Count - 1; i >= 0; i--)
            {
                if (Cards[i].Amount == 0)
                    Cards.RemoveAt(i);
            }

            var dictCards = newCards.ToDictionary(i => i.ArenaId, i => i);

            // Modify card counts for cards already in the collection
            for (int i = Cards.Count - 1; i >= 0; i--)
            {
                int grpId = Cards[i].ArenaId;

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

        private LibraryCardWithAmountVM AddBorderColor(LibraryCardWithAmountVM card)
        {
            card.BorderGradient = GradientCalculator.CalculateBorderGradient(card);
            return card;
        }

        private LibraryCardWithAmountVM ConvertCard(int grpId, int amount, float drawChance)
        {
            var card = mapper.Map<Entity.Card>(grpId);

            var ret = new LibraryCardWithAmountVM
            {
                ArenaId = grpId,
                Amount = amount,
                Colors = card.colors,
                ColorIdentity = card.color_identity,
                ImageArtUrl = Utilities.GetThumbnailLocal(card.imageArtUrl),
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

        private void NotifyStatsChanged()
        {
            OnPropertyChanged(nameof(CardCount));
            OnPropertyChanged(nameof(LandCount));
            OnPropertyChanged(nameof(DrawLandPct));
        }

        internal void SetCards(string cardChosen, ICollection<CardWpf> cards)
        {
            CardChosen = cardChosen;

            var data = cards.Select(i => ConvertCard(i.ArenaId, 1, 1f / cards.Count));
            Cards = new ObservableCollection<LibraryCardWithAmountVM>(data.Select(AddBorderColor));

            OnPropertyChanged(nameof(CardChosen));
            OnPropertyChanged(nameof(Cards));
        }
    }
}
