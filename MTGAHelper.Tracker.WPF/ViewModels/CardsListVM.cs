﻿using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardNotificationEventArgs : EventArgs
    {
        public int Index { get; set; }

        public CardNotificationEventArgs()
        {
        }

        public CardNotificationEventArgs(int index)
        {
            Index = index;
        }
    }

    public class CardsListVM : ObservableObject
    {
        public delegate void CardNotificationHandler(object sender, CardNotificationEventArgs e);

        public ObservableCollection<LibraryCardWithAmountVM> Cards { get; set; }

        public bool ShowDrawPct { get; set; } = true;

        public CardsListVM()
        {
        }

        public CardsListVM(bool showDrawPct)
        {
            ShowDrawPct = showDrawPct;
        }

        public void ResetCards()
        {
            Cards = null;
            RaisePropertyChangedEvent(nameof(Cards));
            RaisePropertyChangedEvent(nameof(ShowDrawPct));
        }

        public void ConvertCardList(Dictionary<int, int> cardsRaw)
        {
            var totalCards = cardsRaw.Sum(i => i.Value);

            var cards = cardsRaw
                .Select(i => ConvertCard(i.Key, i.Value, totalCards))
                .OrderBy(i => i.Type.Contains("Land") ? 0 : 1)
                .ThenBy(i => i.Cmc)
                //.ThenBy(i => i.Colors);
                //.ToArray()
                ;

            if (Cards == null)
            {
                Cards = new ObservableCollection<LibraryCardWithAmountVM>(cards);
                foreach (var c in Cards)
                    c.RefreshBindings();

                RaisePropertyChangedEvent(nameof(Cards));
            }
            else
            {
                // Remove any card with 0 amount
                for (int i = Cards.Count - 1; i >= 0; i--)
                {
                    if (Cards[i].Amount == 0)
                        Cards.RemoveAt(i);
                }

                // Uglier but...Update values instead of re-binding full data for better performance
                var dictCards = cards.ToDictionary(i => i.ArenaId, i => i);

                // Modify or remove cards
                for (int i = Cards.Count - 1; i >= 0; i--)
                {
                    var grpId = Cards[i].ArenaId;
                    if (dictCards.ContainsKey(grpId))//&& dictCards[grpId].Amount != 0)
                    {
                        if (Cards[i].DrawPercent.Value != dictCards[grpId].DrawPercent.Value)
                            Cards[i].DrawPercent.Value = dictCards[grpId].DrawPercent.Value;

                        if (Cards[i].Amount != dictCards[grpId].Amount)
                        {
                            // Update the numbers
                            Cards[i].Amount = dictCards[grpId].Amount;
                            Cards[i].RefreshBindings();
                        }
                    }
                    //else
                    //{
                    //    // Remove the entry
                    //    Cards.RemoveAt(i);
                    //}
                }

                // Add cards
                var cardsToAdd = cardsRaw
                    .Where(i => i.Value > 0)
                    .Where(i => Cards.Any(x => x.ArenaId == i.Key) == false)
                    .ToArray();
                if (cardsToAdd.Length > 0)
                {
                    //var newCards = new List<LibraryCardWithAmountVM>(Cards);
                    Action<int, LibraryCardWithAmountVM> InsertRow = (i, cardToAdd) =>
                    {
                        Cards.Insert(i, cardToAdd);
                        cardToAdd.RefreshBindings();
                    };

                    foreach (var c in cardsToAdd)
                    {
                        var cardToAdd = ConvertCard(c.Key, c.Value, totalCards);

                        if (Cards.Count == 0)
                        {
                            InsertRow(0, cardToAdd);
                        }
                        else
                        {
                            // Find where to insert based on CMC
                            bool inserted = false;
                            for (int i = 0; i < Cards.Count; i++)
                            {
                                if (Cards[i].Cmc >= cardToAdd.Cmc)
                                {
                                    InsertRow(i, cardToAdd);
                                    inserted = true;
                                    break;
                                }
                            }

                            if (inserted == false)
                                InsertRow(Cards.Count, cardToAdd);
                        }
                    }

                    //Cards = newCards;
                    //RaisePropertyChangedEvent(nameof(Cards));
                }
            }
        }

        LibraryCardWithAmountVM ConvertCard(int grpId, int amount, int totalCards)
        {
            //if (grpId == 69714) System.Diagnostics.Debugger.Break();

            var card = Mapper.Map<Entity.Card>(grpId);
            var cardVM = Mapper.Map<CardVM>(Mapper.Map<CardWpf>(card));
            var util = new Entity.Util();

            //if (card.name == "Swamp") System.Diagnostics.Debugger.Break();

            var ret = new LibraryCardWithAmountVM
            {
                ArenaId = grpId,
                Amount = amount,
                CardVM = cardVM,
                Colors = card.colors,
                ColorIdentity = card.color_identity,
                ImageArtUrl = util.GetThumbnailUrl(card.imageArtUrl),
                ImageCardUrl = "https://img.scryfall.com/cards" + card.imageCardUrl,
                Name = card.name,
                Rarity = card.rarity,
                DrawPercent = new ObservableProperty<float>((float)amount / totalCards),
                Cmc = card.cmc,
                ManaCost = card.mana_cost,
                Type = card.type,
            };

            return ret;
        }
    }

}
