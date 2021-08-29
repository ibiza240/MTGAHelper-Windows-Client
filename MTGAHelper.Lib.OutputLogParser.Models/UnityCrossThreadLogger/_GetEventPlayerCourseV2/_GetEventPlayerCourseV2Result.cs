//using System.Collections.Generic;
//using MTGAHelper.Entity.OutputLogParsing;
//using Serilog;

//namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
//{
//    public class GetEventPlayerCourseV2Result : MtgaOutputLogPartResultBase<PayloadRaw<GetEventPlayerCourseV2Raw>>, IResultCardPool
//    {
//        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetEventPlayerCourseV2;
//        public List<int> CardPool
//        {
//            get
//            {
////                if (Raw == null)
////                {
////                    // I don't know how that can be posible but
////                    // this happened to me while debugging, looking at the log file however, everything looked good:
////                    /*
////[UnityCrossThreadLogger]<== Event.GetPlayerCourseV2 {"id":289,"payload":{"Id":"25e5478a-6b06-4ea4-8cfa-e3b096336d49","InternalEventName":"QuickDraft_WAR_20200515","PlayerId":null,"EventType":0,"EventSubType":0,"ModuleInstanceData":{"HasPaidEntry":"Gold","DraftInfo":{"DraftId":"933E5CE627155485:QuickDraft_WAR_20200515:Draft"},"DraftComplete":true,"HasGranted":true,"DeckSelected":true},"CurrentEventState":"ReadyToMatch","CurrentModule":"TransitionToMatches","CardPool":[69634,69662,69491,69608,69522,69484,69484,69615,69690,69560,69574,69602,69592,69514,69627,69500,69697,69694,69694,69531,69618,69643,69602,69649,69634,69632,69480,69610,69473,69672,69624,69458,69484,69519,69460,69524,69574,69602,69511,69488,69459,69497],"CourseDeck":{"commandZoneGRPIds":[],"attributes":{},"companionGRPId":0,"mainDeck":[69662,1,69608,1,69522,1,69615,1,69602,3,69592,0,69694,1,69484,3,69560,0,69500,1,69610,1,69632,1,69618,1,69627,1,69480,1,69649,0,73123,6,69531,0,69672,1,69624,1,69519,1,69524,1,69488,1,69690,1,73126,5,73135,6],"sideboard":[69592,1,69514,1,69634,2,69491,1,69560,1,69574,2,69694,1,69697,1,69649,1,69643,1,69531,1,69459,1,69497,1,69473,1,69458,1,69460,1,69511,1],"isValid":false,"isCompanionValid":false,"lockedForUse":false,"lockedForEdit":false,"resourceId":"00000000-0000-0000-0000-000000000000","cardSkins":[{"grpId":69488,"ccv":"SG"}],"id":"b16dd7c8-0e81-49e6-902c-7dfe34e08113","name":"Draft Deck","description":"","format":"Draft","deckTileId":69662,"cardBack":"CardBack_Default","lastUpdated":"2020-05-22T01:18:40.706048"},"PreviousOpponents":[]}}
////                    */

////                    Log.Warning("Weirdly, the PayloadRaw of the GetEventPlayerCourseV2Result is NULL");
////                    System.Diagnostics.Debugger.Break();
////                    return new List<int>();
////                }

//                // From debugging all that, I think it's the serialization that creates a new object instance with default constructor at first
//                return Raw?.payload?.CardPool ?? new List<int>();
//            }
//        }
//    }
//}