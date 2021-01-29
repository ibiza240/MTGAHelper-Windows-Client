using System;
using System.Collections.Generic;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Lib.OutputLogParser.Models;
using static MTGAHelper.Lib.OutputLogParser.InMatchTracking.OwnedZone;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public enum OwnedZone
    {
        Unknown = ZoneSimpleEnum.Unknown,
        Battlefield = ZoneSimpleEnum.ZoneType_Battlefield,
        OppLibrary = ZoneSimpleEnum.ZoneType_Library,
        OppGraveyard = ZoneSimpleEnum.ZoneType_Graveyard,
        Exile = ZoneSimpleEnum.ZoneType_Exile,
        OppHand = ZoneSimpleEnum.ZoneType_Hand,
        Stack = ZoneSimpleEnum.ZoneType_Stack,
        OppCommand = ZoneSimpleEnum.ZoneType_Command,
        Limbo = ZoneSimpleEnum.ZoneType_Limbo,
        OppSideboard = ZoneSimpleEnum.ZoneType_Sideboard,
        OppRevealed = ZoneSimpleEnum.ZoneType_Revealed,
        Pending = ZoneSimpleEnum.ZoneType_Pending,
        OppPhasedOut = ZoneSimpleEnum.ZoneType_PhasedOut,

        MyLibrary,
        MyGraveyard,
        MyHand,
        MyCommand,
        MySideboard,
        MyRevealed,
        MyPhasedOut
    }

    public static class EnumExtensions
    {
        public static IEnumerable<OwnedZone> EveryZone()
        {
            yield return Unknown;
            yield return Battlefield;
            yield return OppLibrary;
            yield return OppGraveyard;
            yield return Exile;
            yield return OppHand;
            yield return Stack;
            yield return OppCommand;
            yield return Limbo;
            yield return OppSideboard;
            yield return OppRevealed;
            yield return Pending;
            yield return OppPhasedOut;
            yield return MyLibrary;
            yield return MyGraveyard;
            yield return MyHand;
            yield return MyCommand;
            yield return MySideboard;
            yield return MyRevealed;
            yield return MyPhasedOut;
        }

        public static OwnedZone ToOwnedZone(this ZoneSimpleEnum simpleZoneType, bool isMyZone)
        {
            if (isMyZone == false)
                return (OwnedZone)simpleZoneType;

            switch (simpleZoneType)
            {
                case ZoneSimpleEnum.ZoneType_Library:
                    return MyLibrary;
                case ZoneSimpleEnum.ZoneType_Graveyard:
                    return MyGraveyard;
                case ZoneSimpleEnum.ZoneType_Hand:
                    return MyHand;
                case ZoneSimpleEnum.ZoneType_Command:
                    return MyCommand;
                case ZoneSimpleEnum.ZoneType_Sideboard:
                    return MySideboard;
                case ZoneSimpleEnum.ZoneType_Revealed:
                    return MyRevealed;
                case ZoneSimpleEnum.ZoneType_PhasedOut:
                    return MyPhasedOut;

                case ZoneSimpleEnum.Unknown:
                case ZoneSimpleEnum.ZoneType_Battlefield:
                case ZoneSimpleEnum.ZoneType_Exile:
                case ZoneSimpleEnum.ZoneType_Stack:
                case ZoneSimpleEnum.ZoneType_Limbo:
                case ZoneSimpleEnum.ZoneType_Pending:
                    // shared zone, should not be called though
                    return (OwnedZone)simpleZoneType;
                default:
                    throw new ArgumentOutOfRangeException(nameof(simpleZoneType), simpleZoneType, null);
            }
        }

        public static bool IsOpponentZone(this OwnedZone zone)
        {
            return GetPlayerEnum(zone) == PlayerEnum.Opponent;
        }

        public static bool IsOneOfMyZones(this OwnedZone zone)
        {
            return GetPlayerEnum(zone) == PlayerEnum.Me;
        }

        public static PlayerEnum GetPlayerEnum(this OwnedZone zone)
        {
            switch (zone)
            {
                case MyLibrary:
                case MyGraveyard:
                case MyHand:
                case MyCommand:
                case MySideboard:
                case MyRevealed:
                case MyPhasedOut:
                    return PlayerEnum.Me;

                case OppHand:
                case OppLibrary:
                case OppGraveyard:
                case OppCommand:
                case OppSideboard:
                case OppRevealed:
                case OppPhasedOut:
                    return PlayerEnum.Opponent;

                case Unknown:
                case Battlefield:
                case Exile:
                case Stack:
                case Limbo:
                case Pending:
                    return PlayerEnum.Unknown;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
        }

        public static bool IsSharedZone(this OwnedZone zone)
        {
            switch (zone)
            {
                case Battlefield:
                case Exile:
                case Stack:
                case Limbo:
                case Pending:
                    return true;

                default:
                    return false;
            }
        }

        public static bool ShouldTrackOpponentCards(this OwnedZone zone)
        {
            return zone != Unknown && zone != Limbo;
        }

        public static string ShortString(this GameObjectType t)
        {
            switch (t)
            {
                case GameObjectType.Unknown:
                    return "?";
                case GameObjectType.GameObjectType_Card:
                    return "card";
                case GameObjectType.GameObjectType_Token:
                    return "token";
                case GameObjectType.GameObjectType_Ability:
                    return "ability";
                case GameObjectType.GameObjectType_Emblem:
                    return "emblem";
                case GameObjectType.GameObjectType_SplitCard:
                    return "splitcard";
                case GameObjectType.GameObjectType_SplitLeft:
                    return "left half";
                case GameObjectType.GameObjectType_SplitRight:
                    return "right half";
                case GameObjectType.GameObjectType_RevealedCard:
                    return "reveal";
                default:
                    throw new ArgumentOutOfRangeException(nameof(t), t, null);
            }
        }
    }
}