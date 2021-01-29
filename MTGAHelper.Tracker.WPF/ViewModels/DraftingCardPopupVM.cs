using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Tools;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class DraftingCardPopupVM : BasicModel
    {
        public CardDraftPickVM Card
        {
            get => _Card;
            set => SetField(ref _Card, value, nameof(Card));
        }

        public bool ShowGlobalMTGAHelperSays
        {
            get => _ShowGlobalMTGAHelperSays;
            set => SetField(ref _ShowGlobalMTGAHelperSays, value, nameof(ShowGlobalMTGAHelperSays));
        }

        public string RatingsSource
        {
            get => _RatingsSource;
            set => SetField(ref _RatingsSource, value, nameof(RatingsSource));
        }

        public bool ShowRatingsSource
        {
            get => _ShowRatingsSource;
            set => SetField(ref _ShowRatingsSource, value, nameof(ShowRatingsSource));
        }

        public bool ShowCustomRating
        {
            get => _ShowCustomRating;
            set => SetField(ref _ShowCustomRating, value, nameof(ShowCustomRating));
        }

        public bool IsPopupVisible
        {
            get => _IsPopupVisible;
            set => SetField(ref _IsPopupVisible, value, nameof(IsPopupVisible));
        }

        public bool IsMouseInPopup
        {
            get => _IsMouseInPopup;
            set => SetField(ref _IsMouseInPopup, value, nameof(IsMouseInPopup));
        }

        public string CustomRatingDescription
        {
            get => _CustomRatingDescription;
            set => SetField(ref _CustomRatingDescription, value, nameof(CustomRatingDescription));
        }

        public int CustomRatingSelected
        {
            get => _CustomRatingSelected;
            set => SetField(ref _CustomRatingSelected, value, nameof(CustomRatingSelected));
        }

        public ICollection<CardDraftPickWpf> RatingsToShow
        {
            get => _RatingsToShow;
            set => SetField(ref _RatingsToShow, value, nameof(RatingsToShow));
        }

        public ICollection<int> CustomRatings { get; } = new[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10
        };

        private CardDraftPickVM _Card = new CardDraftPickVM();

        private bool _ShowGlobalMTGAHelperSays;

        private string _RatingsSource;

        private bool _ShowRatingsSource;

        private bool _ShowCustomRating;

        private bool _IsPopupVisible;

        private bool _IsMouseInPopup;

        private string _CustomRatingDescription;

        private int _CustomRatingSelected;

        private ICollection<CardDraftPickWpf> _RatingsToShow;

        public void SetDraftCard(CardDraftPickVM cardVM, bool showGlobalMTGAHelperSays, bool showAllRatings, ICollection<CardDraftPickWpf> draftRatings)
        {
            Card = cardVM;
            RatingsToShow = new CardDraftPickWpf[] { cardVM };

            if (showAllRatings)
            {
                RatingsToShow = RatingsToShow.Union(draftRatings
                    .Where(i => i.Name == cardVM.Name)
                    .Where(i => i.Set == cardVM.Set)
                    .Where(i => i.RatingSource != RatingsSource)
                    .Where(i => i.RatingValue != default)
                ).ToArray();
            }

            CustomRatingSelected = Card.CustomRatingValue ?? 0;
            CustomRatingDescription = Card.CustomRatingDescription;
            ShowGlobalMTGAHelperSays = showGlobalMTGAHelperSays;
            ShowCustomRating = cardVM.CustomRatingValue != null || cardVM.CustomRatingDescription != null;
            OnPropertyChanged(nameof(Card));
        }

        public void SetPopupRatingsSource(bool showRatingsSource, string source)
        {
            ShowRatingsSource = showRatingsSource;
            RatingsSource = source;
        }
    }
}
