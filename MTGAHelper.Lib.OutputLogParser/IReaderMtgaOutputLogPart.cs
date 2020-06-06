using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser
{
    public interface IReaderMtgaOutputLogPart
    {
        ICollection<IMtgaOutputLogPartResult> ParsePart(string part);
        string LogTextKey { get; }
    }

    public interface IReaderMtgaOutputLogJsonBase
    {
        bool IsJson { get; }
    }

    public interface IReaderMtgaOutputLogJson : IReaderMtgaOutputLogJsonBase
    {
        IMtgaOutputLogPartResult ParseJson(string json);
    }

    public interface IReaderMtgaOutputLogJson<T> : IReaderMtgaOutputLogJson
    {
        //IMtgaOutputLogPartResult<T> ParseJson(string json);
    }

    public interface IReaderMtgaOutputLogJsonMulti : IReaderMtgaOutputLogJsonBase
    {
        ICollection<IMtgaOutputLogPartResult> ParseJsonMulti(string json);
    }

    //public enum ReaderMtgaOutputLogPartTypeEnum
    //{
    //    Unknown,
    //    Ignored,
    //    GetDeckList,
    //    GetCombinedRankInfo,
    //    GetPlayerCards,
    //    GetPlayerInventory,
    //    DeckSubmit,
    //    MatchGameRoomStateChangedEvent,
    //    MatchCreated,
    //    ConnectResp,
    //    GameStateMessage,
    //    RankUpdated,
    //    InventoryUpdated,
    //    IntermissionReq,
    //    MulliganReq,
    //    GetActiveEvents,
    //    SubmitDeckReq,
    //    DuelSceneGameStop,
    //    DuelSceneSideboardingStart,
    //    DuelSceneSideboardingStop,
    //    ClientConnected,
    //    MythicRatingUpdated,
    //    AuthenticateResponse,
    //    GetEventPlayerCourseV2,
    //    LogInfoRequest,
    //    CompleteDraft,
    //    DraftMakePick,
    //    DraftStatus,
    //}
}
