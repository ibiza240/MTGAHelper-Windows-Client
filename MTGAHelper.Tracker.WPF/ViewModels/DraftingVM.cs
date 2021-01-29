using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Tools;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class DraftInfoBuffered
    {
        public DraftPickProgress DraftProgress { get; set; } = new DraftPickProgress();

        public ICollection<CardDraftPickWpf> CardsDraftBuffered { get; set; } = new CardDraftPickWpf[0];
    }

    public class DraftingVM : BasicModel
    {
        #region Dependancy Injection Initializer

        /// <summary>
        /// Initialize the class using dependency injection
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="allCards"></param>
        /// <param name="api"></param>
        public DraftingVM(IMapper mapper, ICollection<Card> allCards, ServerApiCaller api, CardThumbnailDownloader cardThumbnailDownloader)
        {
            this.mapper = mapper;
            AllCards = allCards;
            Api = api;
            this.cardThumbnailDownloader = cardThumbnailDownloader;
            CardsThatDidNotWheelVM = new CardsListVM(DisplayType.None, CardsListOrder.ManaCost, mapper, cardThumbnailDownloader);
        }

        public void SetCardsPerPack(int cardsPerPack)
        {
            CardsPerPack = cardsPerPack;
        }

        #endregion

        #region Constants

        /// <summary>
        /// Number of drafters (bots) in the draft pod
        /// </summary>
        private const int POD_SIZE = 8;

        #endregion

        public DraftInfoBuffered DraftInfoBuffered = new DraftInfoBuffered();

        #region Public Properties

        public int CardsPerPack { get; private set; }

        /// <summary>
        /// Whether to show the card list that did not wheel
        /// </summary>
        public bool ShowCardListThatDidNotWheel
        {
            get => _ShowCardListThatDidNotWheel;
            set => SetField(ref _ShowCardListThatDidNotWheel, value, nameof(ShowCardListThatDidNotWheel));
        }

        public bool ShowLinkCardListThatDidNotWheel => IsVisibleWhenCardsDidNotWheel && CardsDraft.Count > 0;

        /// <summary>
        /// Whether a message is visible when no cards wheeled
        /// </summary>
        public bool IsVisibleWhenCardsDidNotWheel => CardsThatDidNotWheel.Count > 0;

        /// <summary>
        /// Card wheeling message string
        /// </summary>
        public string CardsWheelingMessage => CardsThatDidNotWheel.Count > 0 ? $"{CardsThatDidNotWheel.Count} cards didn't wheel from this pack" :
            (/*CurrentDraftPickProgress.PickNumber >= POD_SIZE ? "This pack won't wheel" :*/ $"{(NumCardsWheeling == 0 ? "No" : NumCardsWheeling.ToString())} card{(NumCardsWheeling == 1 ? " is" : "s are")} wheeling in this pack");

        /// <summary>
        /// List of cards that did not wheel
        /// </summary>
        public CardsListVM CardsThatDidNotWheelVM { get; set; }

        public ServerApiCaller Api { get; }

        /// <summary>
        /// True while draft picking, False else (eg. looking at card pool after picking)
        /// </summary>
        public bool ShowGlobalMTGAHelperSays
        {
            get => _ShowGlobalMTGAHelperSays;
            set => SetField(ref _ShowGlobalMTGAHelperSays, value, nameof(ShowGlobalMTGAHelperSays));
        }

        public ICollection<CardDraftPickVM> CardsDraft { get; set; } = new CardDraftPickVM[0];

        public ICollection<int> CardsThatDidNotWheel { get; set; } = new int[0];

        public string CardChosenThatDidNotWheel { get; private set; }

        public double RareDraftOpacity => CardsDraft.Any(x => x.RareDraftPickEnum == RaredraftPickReasonEnum.None && Math.Abs(x.RatingValue) > float.Epsilon) ? 1.0d : 0.5d;

        public IList<DraftingVMPxpx> PxpxItems { get; set; }

        public DraftingVMPxpx PxpxItemSelected
        {
            get => _PxpxItemSelected;
            set => SetField(ref _PxpxItemSelected, value, nameof(PxpxItemSelected));
        }

        public int PxpxCardsNumber => CardsPerPack - (PxpxItemSelected?.PickNumber ?? 0);

        public List<DraftPickProgress> DraftPicksHistory { get; private set; } = new List<DraftPickProgress>();

        public Dictionary<string, Dictionary<string, CustomDraftRating>> CustomRatingsBySetThenCardName { get; internal set; }

        public string LimitedRatingsSource { get; internal set; }

        #endregion

        #region Private Backing Fields

        /// <summary>backing field</summary>
        private bool _ShowCardListThatDidNotWheel;

        /// <summary>backing field</summary>
        private bool _ShowGlobalMTGAHelperSays;

        #endregion

        #region Private Fields

        private int NumCardsWheeling => Math.Max(0, CardsPerPack - (PxpxItemSelected?.PickNumber ?? 0) - POD_SIZE);

        public ICollection<CardDraftPickWpf> DraftRatings { get; internal set; }

        private readonly ICollection<Card> AllCards;
        private readonly CardThumbnailDownloader cardThumbnailDownloader;
        private bool UpdateCardsDraftBuffered;

        private DraftingVMPxpx _PxpxItemSelected;

        private readonly object LockCardsDraft = new object();

        readonly IMapper mapper;

        #endregion

        #region Internal Methods

        internal void ResetDraftPicks(string set)
        {
            CardsPerPack = set switch
            {
                "ELD" => 14,
                "IKO" => 15,
                _ => 15,
            };

            PxpxItems = GeneratePxpxList();
            PxpxItemSelected = PxpxItems.First();

            CardsDraft = new CardDraftPickVM[0];
            CardChosenThatDidNotWheel = default;
            CardsThatDidNotWheel = new int[0];
            DraftPicksHistory = new List<DraftPickProgress>();
            DraftInfoBuffered = new DraftInfoBuffered();
            ShowGlobalMTGAHelperSays = true;

            UpdateCardsDraftBuffered = true;

            OnPropertyChanged(nameof(ShowGlobalMTGAHelperSays));
            OnPropertyChanged(nameof(PxpxItems));
        }

        private List<DraftingVMPxpx> GeneratePxpxList()
        {
            const int packs = 3;
            var itemsPxpx = new List<DraftingVMPxpx>(packs * CardsPerPack);
            for (int iPack = 0; iPack < packs; iPack++)
            {
                for (int iPick = 0; iPick < CardsPerPack; iPick++)
                {
                    itemsPxpx.Add(new DraftingVMPxpx
                    {
                        PackNumber = iPack,
                        PickNumber = iPick,
                    });
                }
            }

            return itemsPxpx;
        }

        internal void SetCardsDraftBuffered(DraftPickProgress draftPick, ICollection<CardDraftPickWpf> ratingsInfo)
        {
            lock (LockCardsDraft)
            {
                // PickedCards are not ordered correctly in QuickDraft
                var previousPick = DraftPicksHistory.FirstOrDefault(i =>
                    i.PackNumber == draftPick.PackNumber && i.PickNumber == draftPick.PickNumber - 1);
                if (previousPick != null)
                {
                    // Find the cards that was picked by difference
                    var diffs = draftPick.PickedCards.Except(previousPick.PickedCards).ToList();
                    if (diffs.Count == 1)
                    {
                        draftPick.PickedCards = previousPick.PickedCards.Append(diffs.Single()).ToList();
                    }
                }

                // Manage the packs history, this is all for the "previously seen cards that didn't wheel" feature
                // (this if condition is only there to manage the case where the ratings source was changed, thus triggering an update here
                //  and we don't want that to affect the history)
                //
                // With Human draft, it's also to prevent affecting the history when running the DraftHelper more than once for the cards displayed
                if (draftPick.PackNumber != DraftInfoBuffered.DraftProgress.PackNumber || draftPick.PickNumber != DraftInfoBuffered.DraftProgress.PickNumber)
                {
                    CardsThatDidNotWheel = new int[0];

                    // Reset the progress history if it is a new draft
                    if (draftPick.DraftId != default && DraftPicksHistory.Count > 0 && draftPick.DraftId != DraftPicksHistory[0].DraftId)
                    {
                        DraftPicksHistory = new List<DraftPickProgress> { draftPick };
                    }

                    // Try to find the original pack that wheeled to extract the cards that didn't wheel
                    var originalPack = DraftPicksHistory.FirstOrDefault(i =>
                        i.PackNumber == draftPick.PackNumber && i.PickNumber == draftPick.PickNumber - POD_SIZE);

                    if (originalPack != null)
                    {
                        CardsThatDidNotWheel = originalPack.DraftPack
                            .Where(i => draftPick.DraftPack.Contains(i) == false).ToArray();

                        var packFollowing = DraftPicksHistory.FirstOrDefault(i =>
                            i.PackNumber == originalPack.PackNumber && i.PickNumber == originalPack.PickNumber + 1);
                        if (packFollowing != null && packFollowing.PickedCards.Any())
                            CardChosenThatDidNotWheel = mapper.Map<Card>(packFollowing.PickedCards.Last()).name;
                    }

                    var nextPxpx = PxpxItems?.FirstOrDefault(i => i.PackNumber == draftPick.PackNumber && i.PickNumber == draftPick.PickNumber);
                    if (nextPxpx != null)
                    {
                        PxpxItemSelected = nextPxpx;
                        OnPropertyChanged(nameof(PxpxItemSelected));
                    }
                }

                if (draftPick.PackNumber != DraftInfoBuffered.DraftProgress.PackNumber || draftPick.PickNumber != DraftInfoBuffered.DraftProgress.PickNumber
                    || (draftPick.PackNumber == 0 && draftPick.PickNumber == 0))
                {
                    // This was in the if above initially, but it's also required to run for the P1p1
                    DraftPicksHistory.Add(draftPick);
                }

                RefreshCardList(draftPick, ratingsInfo);
            }
        }

        void RefreshCardList(DraftPickProgress draftPick, ICollection<CardDraftPickWpf> ratingsInfo)
        {
            DraftInfoBuffered.DraftProgress = draftPick;
            DraftInfoBuffered.CardsDraftBuffered = ratingsInfo;
            UpdateCardsDraftBuffered = true;
        }

        internal void ToggleShowHideCardListPopupThatDidNotWheel()
        {
            ShowCardListThatDidNotWheel = !ShowCardListThatDidNotWheel;
        }

        internal void RefreshCardsDraft()
        {
            OnPropertyChanged(nameof(CardsDraft));
        }

        internal void SetCardsDraftFromBuffered()
        {
            if (UpdateCardsDraftBuffered == false)
                return;

            lock (LockCardsDraft)
            {
                CardsDraft = mapper.Map<ICollection<CardDraftPickVM>>(DraftInfoBuffered.CardsDraftBuffered);
                cardThumbnailDownloader.CheckAndDownloadThumbnails(CardsDraft.Select(i => i.ArenaId).ToArray());

                UpdateCardsDraftBuffered = false;
                OnPropertyChanged(nameof(CardsDraft));
                OnPropertyChanged(nameof(CardsWheelingMessage));
                OnPropertyChanged(nameof(ShowGlobalMTGAHelperSays));
                OnPropertyChanged(nameof(IsVisibleWhenCardsDidNotWheel));
                OnPropertyChanged(nameof(ShowLinkCardListThatDidNotWheel));

                // Setup the card list popup content
                var cardsWheeled = AllCards
                    .Where(i => CardsThatDidNotWheel.Contains(i.grpId))
                    .Select(mapper.Map<CardWpf>)
                    .ToArray();

                CardsThatDidNotWheelVM.SetCards(CardChosenThatDidNotWheel, cardsWheeled);

                OnPropertyChanged(nameof(CardsThatDidNotWheelVM));
            }
        }

        #endregion

        //int nbCardsWheeling => Math.Max(0, CardsDraftByTier.Values.Sum(i => i.Count) - POD_SIZE);

        //public Dictionary<float, ICollection<CardDraftPickVM>> CardsDraftByTier { get; set; } = new Dictionary<float, ICollection<CardDraftPickVM>>();
    }
}
