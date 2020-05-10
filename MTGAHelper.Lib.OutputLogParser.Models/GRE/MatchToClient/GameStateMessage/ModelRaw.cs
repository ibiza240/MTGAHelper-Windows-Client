using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.GameStateMessage.Raw
{
    public class GameStateMessageRaw : GreMatchToClientSubMessageBase
    {
        public GameStateMessage gameStateMessage { get; set; }
        public Prompt prompt { get; set; }
        public ActionsAvailableReq actionsAvailableReq { get; set; }
        public NonDecisionPlayerPrompt nonDecisionPlayerPrompt { get; set; }
    }

    public class ActionsAvailableReq
    {
        public List<Action3> actions { get; set; }
        public List<InactiveAction> inactiveActions { get; set; }
    }

    public class Result
    {
        public string scope { get; set; }
        public string result { get; set; }
        public int winningTeamId { get; set; }
        public string reason { get; set; }
    }

    public class GameInfo
    {
        public string matchID { get; set; }
        public int gameNumber { get; set; }
        public string stage { get; set; }
        public string type { get; set; }
        public string variant { get; set; }
        public string matchState { get; set; }
        public string matchWinCondition { get; set; }
        public int maxTimeoutCount { get; set; }
        public int maxPipCount { get; set; }
        public int timeoutDurationSec { get; set; }
        public List<Result> results { get; set; }
        public string superFormat { get; set; }
        public string mulliganType { get; set; }
    }

    public class Power
    {
        public int value { get; set; }
    }

    public class Toughness
    {
        public int value { get; set; }
    }

    public class GameObject
    {
        public int instanceId { get; set; }
        public int grpId { get; set; }
        public string type { get; set; }
        public string visibility { get; set; }
        public int ownerSeatId { get; set; }
        public int controllerSeatId { get; set; }
        public List<string> cardTypes { get; set; }
        public List<string> subtypes { get; set; }
        public int name { get; set; }
        public List<int> abilities { get; set; }
        public int overlayGrpId { get; set; }
        public int? zoneId { get; set; }
        public bool? isTapped { get; set; }
        public bool? isFacedown { get; set; }
        public List<string> color { get; set; }
        public Power power { get; set; }
        public Toughness toughness { get; set; }
        public List<int?> viewers { get; set; }
    }

    public class Detail
    {
        public string key { get; set; }
        public string type { get; set; }
        public List<int> valueInt32 { get; set; }
        public List<string> valueString { get; set; }
    }

    public class Annotation
    {
        public int id { get; set; }
        public long affectorId { get; set; }
        public List<int> affectedIds { get; set; }
        public List<string> type { get; set; }
        public List<Detail> details { get; set; }
        public bool allowRedaction { get; set; }

        public override string ToString()
        {
            return string.Join(", ", type);
        }
    }

    public class ManaCost
    {
        public List<string> color { get; set; }
        public int count { get; set; }
        public int costId { get; set; }
    }

    public class Action2
    {
        public string actionType { get; set; }
        public int instanceId { get; set; }
        public List<ManaCost> manaCost { get; set; }
        public int? abilityGrpId { get; set; }
        public int? sourceId { get; set; }
    }

    public class Action
    {
        public int seatId { get; set; }
        public Action2 action { get; set; }
    }

    public class Zone
    {
        public int zoneId { get; set; }
        public string type { get; set; }
        public string visibility { get; set; }
        public List<int> objectInstanceIds { get; set; }
        public int? ownerSeatId { get; set; }
        public List<int?> viewers { get; set; }

        public override string ToString()
        {
            return $"{zoneId} {type} - Player {ownerSeatId}";
        }
    }

    public class Timer
    {
        public int timerId { get; set; }
        public string type { get; set; }
        public int durationSec { get; set; }
        public string behavior { get; set; }
        public int warningThresholdSec { get; set; }
        public int? elapsedSec { get; set; }
        public bool? running { get; set; }
        public int? elapsedMs { get; set; }
    }

    public class TurnInfo
    {
        public string phase { get; set; }
        public int turnNumber { get; set; }
        public int activePlayer { get; set; }
        public int priorityPlayer { get; set; }
        public int decisionPlayer { get; set; }
        public string nextPhase { get; set; }
        public string nextStep { get; set; }
        public string step { get; set; }
    }

    public class Player
    {
        public int lifeTotal { get; set; }
        public int systemSeatNumber { get; set; }
        public int maxHandSize { get; set; }
        public int mulliganCount { get; set; }
        public int turnNumber { get; set; }
        public int teamId { get; set; }
        public List<int> timerIds { get; set; }
        public int controllerSeatId { get; set; }
        public string controllerType { get; set; }
        public int pipCount { get; set; }
        public string pendingMessageType { get; set; }
    }

    public class GameStateMessage
    {
        public string type { get; set; }
        public int gameStateId { get; set; }
        public GameInfo gameInfo { get; set; }
        public List<Player> players { get; set; }
        public List<GameObject> gameObjects { get; set; }
        public List<Annotation> annotations { get; set; }
        public int prevGameStateId { get; set; }
        public string update { get; set; }
        public List<Action> actions { get; set; }
        public List<Zone> zones { get; set; }
        public List<int> diffDeletedInstanceIds { get; set; }
        public List<Timer> timers { get; set; }
        public TurnInfo turnInfo { get; set; }
        public int? pendingMessageCount { get; set; }
    }

    public class Parameter
    {
        public string parameterName { get; set; }
        public string type { get; set; }
        public int numberValue { get; set; }
    }

    public class Prompt
    {
        public int promptId { get; set; }
        public List<Parameter> parameters { get; set; }
    }

    public class ManaCost2
    {
        public List<string> color { get; set; }
        public int count { get; set; }
        public int costId { get; set; }
    }

    public class Action3
    {
        public string actionType { get; set; }
        public int grpId { get; set; }
        public int instanceId { get; set; }
        public string grouping { get; set; }
        public List<ManaCost2> manaCost { get; set; }
        public bool shouldStop { get; set; }
    }

    public class InactiveAction
    {
        public string actionType { get; set; }
        public int grpId { get; set; }
        public int instanceId { get; set; }
        public string grouping { get; set; }
        public int sourceId { get; set; }
    }

    public class Parameter2
    {
        public string parameterName { get; set; }
        public string type { get; set; }
        public int numberValue { get; set; }
    }

    public class NonDecisionPlayerPrompt
    {
        public int promptId { get; set; }
        public List<Parameter2> parameters { get; set; }
    }
}
