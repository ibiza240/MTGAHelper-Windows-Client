using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.Config;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;

namespace MTGAHelper.Entity.IoC
{
    public class MapperProfileEntity : Profile
    {
        public MapperProfileEntity(CacheSingleton<Dictionary<int, Card>> cacheCards)
        {
            var dictAllCards = cacheCards.Get();

            // grpId to Card
            CreateMap<int, Card>().ConvertUsing(i => dictAllCards.ContainsKey(i) ? dictAllCards[i] :
                new Card
                {
                    grpId = 0,
                    name = "Unknown",
                    imageCardUrl = "https://cdn11.bigcommerce.com/s-0kvv9/images/stencil/1280x1280/products/266486/371622/classicmtgsleeves__43072.1532006814.jpg?c=2&imbypass=on"
                });

            //CreateMap<MtgaDeck, ConfigModelRawDeck>();

            CreateMap<KeyValuePair<int, int>, CardWithAmount>()
                .ConvertUsing(i => new CardWithAmount(dictAllCards[i.Key], i.Value));

            CreateMap<QuestUpdate, PlayerQuest>();
            CreateMap<TrackDiff, PlayerProgress>();
            CreateMap<DraftMakePickRaw, DraftPickProgress>();

            CreateMap<ConfigModelRawDeck, ConfigModelDeck>()
                .ForMember(i => i.ScraperTypeId, i => i.MapFrom(x => "userdeck-mtgadeck"))
                .ForMember(i => i.UrlDeckList, i => i.MapFrom(x => (string)null));


            //public class ConfigModelRawDeck
            //{
            //    public string Id { get; set; }
            //    public int DeckTileId { get; set; }
            //    public DateTime LastUpdated { get; set; }
            //    public string Name { get; set; }
            //    public string Format { get; set; }
            //    public string ArchetypeId { get; set; }
            //    public Dictionary<int, int> CardsMain { get; set; } = new Dictionary<int, int>();
            //    public Dictionary<int, int> CardsSideboard { get; set; } = new Dictionary<int, int>();
            //}

            //public class ConfigModelDeck : ConfigModelRawDeck, IConfigModel
            //{
            //    public const string SOURCE_SYSTEM = "automatic";
            //    public const string SOURCE_USERCUSTOM = "usercustom";
            //    public DateTime DateScrapedUtc { get; set; }
            //    public DateTime DateCreatedUtc { get; set; }
            //    public string ScraperTypeId { get; set; }
            //    public int? ScraperTypeOrderIndex { get; set; }
            //    public string Url { get; set; }
            //    public string UrlDeckList { get; set; }

            //    [JsonIgnore]
            //    public IDeck Deck { get; set; }

        }
    }
}
