using System.Collections.Generic;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.CacheLoaders;
using MTGAHelper.Lib.EventsSchedule;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.ClientToMatch;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Lib.Ioc;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch;
using MTGAHelper.Lib.OutputLogProgress;
using MTGAHelper.Web.UI.Shared;
using SimpleInjector;

namespace MTGAHelper.Lib.OutputLogParser.IoC
{
    public static class SimpleInjectorRegistrations
    {
        public static Container RegisterServicesLibOutputLogParser(this Container container)
        {
            container.RegisterSingleton<IEventTypeCache, SimpleEventCache>();

            container.RegisterSingleton<AutoMapperEventNameToTypeConverter>();
            container.RegisterSingleton<DeckListConverter>();
            container.Collection.Append<AutoMapper.Profile, MapperProfileLibCardConvert>(Lifestyle.Singleton);
            container.Collection.Append<AutoMapper.Profile, MapperProfileLibOutputLogParser>(Lifestyle.Singleton);

            container.Register<ReaderMtgaOutputLog>();
            container.Register<MtgaOutputLogResultsPreparer2>();
            container.Register<ReaderMtgaOutputLogUnityCrossThreadLogger>();
            container.Register<GetDecksListV3Converter>();
            container.Register<GetCombinedRankInfoConverter>();
            container.Register<GetPlayerV3CardsConverter>();
            container.Register<GetPlayerInventoryConverter>();
            container.Register<DeckSubmitConverter>();
            container.Register<MatchCreatedConverter>();
            //container.Register<DuelSceneGameStopConverter>();
            //container.Register<DuelSceneSideboardingStartConverter>();
            //container.Register<DuelSceneSideboardingStopConverter>();
            container.Register<AuthenticateResponseConverter>();
            container.Register<MythicRatingUpdatedConverter>();
            container.Register<RankUpdatedConverter>();
            container.Register<InventoryUpdatedConverter>();
            container.Register<ReaderMtgaOutputLogGre>();
            container.Register<ReaderMtgaOutputLogGreMatchToClient>();
            container.Register<ReaderDetailedLogs>();
            container.Register<ReaderAccountsClient>();
            container.Register<IntermissionReqConverter>();
            container.Register<ConnectRespConverter>();
            container.Register<MulliganReqConverter>();
            container.Register<GameStateMessageConverter>();
            container.Register<GetActiveEventsV2Converter>();
            container.Register<GetEventPlayerCourseV2Converter>();
            container.Register<GetEventPlayerCoursesV2Converter>();
            container.Register<CompleteDraftConverter>();
            container.Register<SubmitDeckReqConverter>();
            container.Register<LogInfoRequestConverter>();
            container.Register<GetPreconDecksV3Converter>();
            container.Register<DraftStatusConverter>();
            container.Register<DraftMakePickConverter>();
            container.Register<CrackBoostersConverter>();
            container.Register<CompleteVaultConverter>();
            container.Register<StateChangedConverter>();
            container.Register<DieRollResultsRespConverter>();
            container.Register<GetPlayerProgressConverter>();
            container.Register<GetSeasonAndRankDetailConverter>();
            container.Register<EventClaimPrizeConverter>();
            container.Register<PayEntryConverter>();
            container.Register<ProgressionGetAllTracksConverter>();
            container.Register<GetPlayerQuestsConverter>();
            container.Register<ClientToMatchConverterGeneric>();
            container.Register<ClientToMatchConverter<PayloadSubmitDeckResp>>();
            container.Register<ClientToMatchConverter<PayloadEnterSideboardingReq>>();
            container.Register<PostMatchUpdateConverter>();
            container.Register<JoinPodmakingConverter>();
            container.Register<MakeHumanDraftPickConverter>();
            container.Register<SelectNReqConverter>();
            container.Register<GroupReqConverter>();
            container.Register<ReaderMessageSummarized>();
            container.Register<ZipDeflator>();
            container.Register<OutputLogMessagesBatcher>();

            return container;
        }

        public static Container RegisterFileLoaders(this Container container)
        {
            container.RegisterSingleton<ICacheLoader<Dictionary<int, Set>>, CacheLoaderSets>();
            container.RegisterSingleton<ICacheLoader<Dictionary<string, DraftRatings>>, CacheLoaderDraftRatings>();
            container.RegisterSingleton<ICacheLoader<Dictionary<int, Card>>, CacheLoaderAllCards>();
            container.RegisterSingleton<IPossibleDateFormats, DateFormatsFromFile>();

            return container;
        }

        public static Container RegisterServicesTracker(this Container container)
        {
            container.Register<InGameTracker2>();
            container.RegisterSingleton<DraftPicksCalculator>();

            return container;
        }
    }
}
