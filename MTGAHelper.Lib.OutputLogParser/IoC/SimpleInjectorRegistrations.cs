using MTGAHelper.Lib.IoC;
using MTGAHelper.Lib.OutputLogParser.EventsSchedule;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch;
using MTGAHelper.Lib.OutputLogParser.OutputLogProgress;
using MTGAHelper.Lib.OutputLogParser.Readers;
using MTGAHelper.Lib.OutputLogParser.Readers.GreMessageType;
using MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger;
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
            container.RegisterSingleton<CourseDeckCardsConverter>();
            container.Collection.Append<AutoMapper.Profile, MapperProfileLibCardConvert>(Lifestyle.Singleton);
            container.Collection.Append<AutoMapper.Profile, MapperProfileLibOutputLogParser>(Lifestyle.Singleton);

            container.Register<ReaderMtgaOutputLog>();
            //container.Register<ReaderMtgaOutputLogGre>();
            container.Register<ReaderDetailedLogs>();
            container.Register<ReaderMessageSummarized>();
            container.Register<ReaderAccountsClient>();
            container.Register<ReaderAccountsAccountClient>();
            container.Register<ReaderMtgaOutputLogUnityCrossThreadLogger>();
            container.Collection.Register<IMessageReaderUnityCrossThreadLogger>(typeof(IMessageReaderUnityCrossThreadLogger).Assembly);
            container.Collection.Register<IMessageReaderRequestToServer>(typeof(IMessageReaderRequestToServer).Assembly);

            container.Register<AuthenticateResponseConverter>();

            // === GreMessageType readers === //
            container.Register<DieRollResultsRespConverter>();
            container.Register<GameStateMessageConverter>();
            container.Register<GreConnectRespConverter>();
            container.Register<GroupReqConverter>();
            container.Register<IntermissionReqConverter>();
            container.Register<MulliganReqConverter>();
            container.Register<QueuedGameStateMessageConverter>();
            container.Register<SelectNReqConverter>();
            container.Register<SubmitDeckReqConverter>();

            // === ClientToMatch readers === //
            container.Register<ClientToMatchConverter<PayloadSubmitDeckResp>>();
            container.Register<ClientToMatchConverter<PayloadEnterSideboardingReq>>();

            container.Register<LogSplitter>();
            container.Register<MtgaOutputLogResultsPreparer>();
            container.Register<OutputLogMessagesBatcher>();
            container.Register<ZipDeflator>();

            return container;
        }

        public static Container RegisterFileLoaders(this Container container)
        {
            container.RegisterFileLoadersShared();
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
