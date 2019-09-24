using System;
using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.User;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MTGAHelper.Tracker.WPF.IoC
{
    public class MapperProfileTrackerWpf : Profile
    {
        public MapperProfileTrackerWpf()
        {
            CreateMap<CardForDraftPickDto, CardDraftPick>()
                .ForMember(i => i.RareDraftPickEnum, i => i.MapFrom(x => x.IsRareDraftPick));

            CreateMap<Entity.Card, Card>()
                .ForMember(i => i.ArenaId, i => i.MapFrom(x => x.grpId))
                .ForMember(i => i.ManaCost, i => i.MapFrom(x => x.mana_cost));

            CreateMap<Card, CardVM>();

            CreateMap<CardWithAmount, LibraryCardWithAmountVM>()
                .ForMember(i => i.ImageArtUrl, i => i.MapFrom(x => new Util().GetThumbnailLocal(x.ImageArtUrl)));

            CreateMap<CardDraftPick, CardDraftPickVM>()
                //.ForMember(i => i.ColorGradient, i => i.MapFrom(x => x.Colors.Select;
                .ForMember(i => i.ImageCardUrl, i => i.MapFrom(x => "https://img.scryfall.com/cards" + x.ImageCardUrl))
                .ForMember(i => i.ImageArtUrl, i => i.MapFrom(x => new Util().GetThumbnailLocal(x.ImageArtUrl)
                ))
                .ForMember(i => i.CardVM, i => i.MapFrom(x => x));
        }
    }
}
