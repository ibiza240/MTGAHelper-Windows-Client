//using MTGAHelper.Tracker.DraftHelper.Shared;
//using MTGAHelper.Tracker.DraftHelper.Shared.Services;

//namespace MTGAHelper.Tracker.WPF.Business
//{
//    class SupporterChecker : ISupporterChecker
//    {
//        readonly IEmailProvider emailProvider;
//        readonly ServerApiCaller serverApiCaller;

//        public SupporterChecker(
//            IEmailProvider emailProvider,
//            ServerApiCaller serverApiCaller)
//        {
//            this.emailProvider = emailProvider;
//            this.serverApiCaller = serverApiCaller;
//        }
//        public bool IsSupporter()
//        {
//            return serverApiCaller.IsSupporter(emailProvider.Email);
//        }
//    }
//}
