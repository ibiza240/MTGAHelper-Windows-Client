using System;
using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.User;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using MTGAHelper.Tracker.WPF.Config;

namespace MTGAHelper.Tracker.WPF.IoC
{
    public class MapperProfileTrackerWpf : Profile
    {
        public MapperProfileTrackerWpf()
        {
            CreateMap<CardForDraftPickDto, CardDraftPickWpf>()
                .ForMember(i => i.RareDraftPickEnum, i => i.MapFrom(x => x.IsRareDraftPick));

            CreateMap<Entity.Card, CardWpf>()
                .ForMember(i => i.ArenaId, i => i.MapFrom(x => x.grpId))
                .ForMember(i => i.ColorIdentity, i => i.MapFrom(x => x.color_identity))
                .ForMember(i => i.ManaCost, i => i.MapFrom(x => x.mana_cost));

            CreateMap<CardWpf, CardVM>();

            CreateMap<CardWithAmountWpf, LibraryCardWithAmountVM>()
                .ForMember(i => i.ImageArtUrl, i => i.MapFrom(x => new Util().GetThumbnailLocal(x.ImageArtUrl)));

            CreateMap<CardDraftPickWpf, CardDraftPickVM>()
                //.ForMember(i => i.ColorGradient, i => i.MapFrom(x => x.Colors.Select;
                .ForMember(i => i.ImageCardUrl, i => i.MapFrom(x => "https://img.scryfall.com/cards" + x.ImageCardUrl))
                .ForMember(i => i.ImageArtUrl, i => i.MapFrom(x => new Util().GetThumbnailLocal(x.ImageArtUrl)
                ))
                .ForMember(i => i.CardVM, i => i.MapFrom(x => x));

            CreateMap<ConfigModelApp, OptionsWindowVM>();
                //.ForMember(i => i.ForceCardPopupSide, i => i.MapFrom(x =>  string.IsNullOrEmpty(x.ForceCardPopupSide) ? "On the left" : x.ForceCardPopupSide));
        }
    }
}
