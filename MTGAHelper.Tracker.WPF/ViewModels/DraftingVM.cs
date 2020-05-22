using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Tracker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="all"></param>
        internal void Init(ICollection<Card> all)
        {
            AllCards = all;
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
        public CardsListVM CardsThatDidNotWheelVM { get; set; } = new CardsListVM(DisplayType.None, CardsListOrder.ManaCost);

        /// <summary>
        /// True while draft picking, False else (eg. looking at card pool after picking)
        /// </summary>
        public bool ShowGlobalMTGAHelperSays
        {
            get => _ShowGlobalMTGAHelperSays;
            set => SetField(ref _ShowGlobalMTGAHelperSays, value, nameof(ShowGlobalMTGAHelperSays));
        }

        public ICollection<CardDraftPickVM> CardsDraft { get; set; } = new CardDraftPickVM[0];

        public bool IsHumanDraft
        {
            get => _IsHumanDraft;
            set => SetField(ref _IsHumanDraft, value, nameof(IsHumanDraft));
        }

        internal void DraftHumanPickCard(int arenaId)
        {
            var pxpxIndex = CardsPerPack * PxpxItemSelected.PackNumber + PxpxItemSelected.PickNumber;
            if (pxpxIndex < PxpxItems.Count - 1)
            {
                // Increment the pick pack
                var nextPxpx = PxpxItems[pxpxIndex + 1];
                DraftProgressHuman.PackNumber = nextPxpx.PackNumber;
                DraftProgressHuman.PickNumber = nextPxpx.PickNumber;
                PxpxItemSelected = nextPxpx;
                OnPropertyChanged(nameof(PxpxItemSelected));
            }

            // Save the pick
            DraftProgressHuman.PickedCards.Add(arenaId);

            // Clear cards list
            RefreshCardList(new DraftPickProgress(new int[0]), new CardDraftPickWpf[0]);
            ShowCardListThatDidNotWheel = false;
            OnPropertyChanged(nameof(ShowCardListThatDidNotWheel));
        }

        public ICollection<int> CardsThatDidNotWheel { get; set; } = new int[0];

        public string CardChosenThatDidNotWheel { get; private set; }

        public double RareDraftOpacity => CardsDraft.Any(x => x.RareDraftPickEnum == RaredraftPickReasonEnum.None && Math.Abs(x.RatingValue) > float.Epsilon) ? 1.0d : 0.5d;

        public IList<DraftingVMPxpx> PxpxItems { get; set; }

        public DraftingVMPxpx PxpxItemSelected { get; set; }

        public DraftPickProgress DraftProgressHuman { get; set; }

        public int PxpxCardsNumber => CardsPerPack - (PxpxItemSelected?.PickNumber ?? 0);

        public List<DraftPickProgress> DraftPicksHistory { get; private set; } = new List<DraftPickProgress>();

        public Dictionary<string, Dictionary<string, CustomDraftRating>> CustomRatingsBySetThenCardName { get; internal set; }
        
        public string LimitedRatingsSource { get; internal set; }

        #endregion

        #region Private Backing Fields

        /// <summary>
        /// Whether to show the card list that did not wheel
        /// </summary>
        private bool _ShowCardListThatDidNotWheel;

        /// <summary>
        /// True while draft picking, False else (eg. looking at card pool after picking)
        /// </summary>
        private bool _ShowGlobalMTGAHelperSays;

        private bool _IsHumanDraft;

        #endregion

        #region Private Fields

        private int NumCardsWheeling => Math.Max(0, CardsPerPack - (PxpxItemSelected?.PickNumber ?? 0) - POD_SIZE);

        private ICollection<Card> AllCards = new Card[0];

        private bool UpdateCardsDraftBuffered;

        private readonly object LockCardsDraft = new object();

        #endregion

        #region Internal Methods

        internal void ResetDraftPicks(string set, bool isHumanDraft, string humanDraftId = null)
        {
            Dictionary<string, int> cardsPerPackBySet = new Dictionary<string, int>
            {
                { "IKO", 15 },
                { "ELD", 14 },
            };

            CardsPerPack = cardsPerPackBySet.ContainsKey(set) ? cardsPerPackBySet[set] : 15;

            var itemsPxpx = new List<DraftingVMPxpx>();
            for (int iPack = 0; iPack < 3; iPack++)
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

            PxpxItems = itemsPxpx;
            PxpxItemSelected = PxpxItems.First();

            CardsDraft = new CardDraftPickVM[0];
            CardChosenThatDidNotWheel = default;
            CardsThatDidNotWheel = new int[0];
            DraftPicksHistory = new List<DraftPickProgress>();
            DraftInfoBuffered = new DraftInfoBuffered();
            ShowGlobalMTGAHelperSays = true;
            IsHumanDraft = isHumanDraft;

            if (isHumanDraft)
            {
                DraftProgressHuman = new DraftPickProgress()
                {
                    DraftId = humanDraftId
                };
            }

            UpdateCardsDraftBuffered = true;

            OnPropertyChanged(nameof(ShowGlobalMTGAHelperSays));
            OnPropertyChanged(nameof(PxpxItems));
            OnPropertyChanged(nameof(PxpxItemSelected));
        }

        internal void SetCardsDraftBuffered(DraftPickProgress draftPick, ICollection<CardDraftPickWpf> ratingsInfo, bool isHuman)
        {
            lock (LockCardsDraft)
            {
                if (isHuman)
                {
                    draftPick.PackNumber = DraftProgressHuman.PackNumber;
                    draftPick.PickNumber = DraftProgressHuman.PickNumber;
                    draftPick.DraftId = DraftProgressHuman.DraftId;
                    draftPick.PickedCards = DraftProgressHuman.PickedCards.ToList();
                }
                else
                {
                    // PickedCards are not ordered correctly in QuickDraft
                    var previousPick = DraftPicksHistory.FirstOrDefault(i => i.PackNumber == draftPick.PackNumber && i.PickNumber == draftPick.PickNumber - 1);
                    if (previousPick != null)
                    {
                        // Find the cards that was picked by difference
                        var diffs = draftPick.PickedCards.ToList();
                        var previousPicks = previousPick.PickedCards.ToList();
                        previousPicks.ForEach(l => diffs.Remove(l));
                        if (diffs.Count == 1)
                        {
                            draftPick.PickedCards = previousPicks.Append(diffs.Single()).ToList();
                        }
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

                    // !!! Human drafting cards wheeling management is too messy for now
                    // !!! So only compute it for quick draft
                    if (isHuman == false)
                    {
                        // Try to find the original pack that wheeled to extract the cards that didn't wheel
                        var originalPack = DraftPicksHistory.FirstOrDefault(i => i.PackNumber == draftPick.PackNumber && i.PickNumber == draftPick.PickNumber - POD_SIZE);

                        if (originalPack != null)
                        {
                            CardsThatDidNotWheel = originalPack.DraftPack.Where(i => draftPick.DraftPack.Contains(i) == false).ToArray();

                            var packFollowing = DraftPicksHistory.FirstOrDefault(i => i.PackNumber == originalPack.PackNumber && i.PickNumber == originalPack.PickNumber + 1);
                            if (packFollowing != null)
                                CardChosenThatDidNotWheel = Mapper.Map<Card>(packFollowing.PickedCards.Last()).name;
                        }
                    }

                    var nextPxpx = PxpxItems.FirstOrDefault(i => i.PackNumber == draftPick.PackNumber && i.PickNumber == draftPick.PickNumber);
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
                CardsDraft = Mapper.Map<ICollection<CardDraftPickVM>>(DraftInfoBuffered.CardsDraftBuffered);

                UpdateCardsDraftBuffered = false;
                OnPropertyChanged(nameof(CardsDraft));
                OnPropertyChanged(nameof(CardsWheelingMessage));
                OnPropertyChanged(nameof(ShowGlobalMTGAHelperSays));
                OnPropertyChanged(nameof(IsVisibleWhenCardsDidNotWheel));
                OnPropertyChanged(nameof(ShowLinkCardListThatDidNotWheel));

                // Setup the card list popup content
                var cardsWheeled = AllCards
                    .Where(i => CardsThatDidNotWheel.Contains(i.grpId))
                    .Select(Mapper.Map<CardWpf>)
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
