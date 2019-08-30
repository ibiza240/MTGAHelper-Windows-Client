using System;
using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.User;
using System.Linq;

namespace MTGAHelper.Tracker.WPF.IoC
{
    public class MapperProfileTrackerWpf : Profile
    {
        public MapperProfileTrackerWpf()
        {
            CreateMap<CardForDraftPickDto, CardDraftPick>()
                .ForMember(i => i.RareDraftPickEnum, i => i.MapFrom(x => x.IsRareDraftPick));

            CreateMap<Card, CardVM>();

            CreateMap<CardDraftPick, CardDraftPickVM>()
                //.ForMember(i => i.ColorGradient, i => i.MapFrom(x => x.Colors.Select;
                .ForMember(i => i.ImageCardUrl, i => i.MapFrom(x => "https://img.scryfall.com/cards" + x.ImageCardUrl))
                .ForMember(i => i.ImageArtUrl, i => i.MapFrom(x => new Entity.Util().GetThumbnailUrl(x.ImageArtUrl)))
                .ForMember(i => i.CardVM, i => i.MapFrom(x => x));
        }
    }
}
