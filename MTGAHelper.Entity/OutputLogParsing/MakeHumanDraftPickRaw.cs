using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class MakeHumanDraftPickRaw
    {
        public string id { get; set; }
        public string request { get; set; }

        public int CardId
        {
            get
            {
                var requestObj = JsonConvert.DeserializeObject<MakeHumanDraftPickInnerRaw>(request);
                return requestObj.@params.cardId;
            }
        }
    }

    public class MakeHumanDraftPickInnerRaw
    {
        public MakeHumanDraftPickInnerParamsRaw @params { get; set; }
    }

    public class MakeHumanDraftPickInnerParamsRaw
    {
        public Guid draftId { get; set; }
        public int cardId { get; set; }
        public int packNumber { get; set; }
        public int pickNumber { get; set; }
    }
}
