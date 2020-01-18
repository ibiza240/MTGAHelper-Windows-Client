using MTGAHelper.Web.UI.Model.Response;

namespace MTGAHelper.Web.Models.Response.User
{
    public class UserMasteryPassResponse : IResponse
    {
        public string DateEnd { get; set; }

        public int CurrentLevel { get; set; }
        public int CurrentXp { get; set; }
        public int DailyQuestsAvailable { get; set; }
        public int DailyWinsCompleted { get; set; }
        public int WeeklyWinsCompleted { get; set; }

        public int NbDaysLeft { get; set; }
        public int NbWeeksLeft { get; set; }
        public int FinalLevel { get; set; }

        public int XpWorthDailyQuestsToday { get; set; }
        public int XpWorthDailyQuestsFuture { get; set; }

        public int ExpectedDailyWins { get; set; }
        public int XpWorthDailyWinsToday { get; set; }
        public int XpWorthDailyWinsFuture { get; set; }

        public int ExpectedWeeklyWins { get; set; }
        public int XpWorthWeeklyWinsToday { get; set; }
        public int XpWorthWeeklyWinsFuture { get; set; }

        public int XpWorthTotal => XpWorthDailyQuestsToday + XpWorthDailyQuestsFuture +
            XpWorthDailyWinsToday + XpWorthDailyWinsFuture +
            XpWorthWeeklyWinsToday + XpWorthWeeklyWinsFuture;
    }
}
