using MTGAHelper.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public enum GameOutcomeEnum
    {
        Unknown,
        Victory,
        Defeat,
        Draw,
    }

    public enum FirstTurnEnum
    {
        Unknown,
        Play,
        Draw,
    }

    public class MatchResult
    {
        public string MatchId { get; set; }

        public string EventName { get; set; }
        public string EventType { get; set; }
        public DateTime StartDateTime { get; set; }
        public long SecondsCount => Games.Any() == false ? 0 :
            (long)(Games.Last().StartDateTime.AddSeconds(Games.Last().SecondsCount) - Games.First().StartDateTime).TotalSeconds;
        public ICollection<GameDetail> Games { get; set; } = new List<GameDetail>();

        public GameOutcomeEnum Outcome
        {
            get
            {
                var wins = Games.Count(i => i.Outcome == GameOutcomeEnum.Victory);
                var defeats = Games.Count(i => i.Outcome == GameOutcomeEnum.Defeat);

                if (wins == 0 && defeats == 0)
                    return Games.All(i => i.Outcome == GameOutcomeEnum.Draw) ? GameOutcomeEnum.Draw : GameOutcomeEnum.Unknown;

                return wins > defeats ? GameOutcomeEnum.Victory : wins == defeats ? GameOutcomeEnum.Draw : GameOutcomeEnum.Defeat;
            }
        }

        public MatchOpponentInfo Opponent { get; set; }

        public ConfigModelRawDeck DeckUsed { get; set; }
        //public ConfigModelRawDeck DeckUsed => Games.Any() ? Games.First().DeckUsed : new ConfigModelRawDeck { Name = "Unknown" };

        //public string DeckColors { get; set; }
        //public string OpponentDeckColors { get; set; }

        public ICollection<int> GetOpponentCardsSeen()
        {
            return Games.SelectMany(i => i.OpponentCardsSeen.Keys).ToArray();
        }

        public static MatchResult CreateDefault()
        {
            return new MatchResult
            {
                Opponent = new MatchOpponentInfo
                {
                    ScreenName = "N/A",
                    RankingClass = "Beginner",
                },
            };
        }
    }


    public class MatchOpponentInfo
    {
        public string ScreenName { get; set; }
        public bool IsWotc { get; set; }
        public string RankingClass { get; set; }
        public int RankingTier { get; set; }
        public double MythicPercentile { get; set; }
        public int MythicLeaderboardPlace { get; set; }
    }
}
