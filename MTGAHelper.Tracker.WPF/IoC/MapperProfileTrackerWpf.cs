using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.User;
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

            CreateMap<CardWithAmountWpf, LibraryCardWithAmountVM>();
            //.ForMember(i => i.ImageArtUrl, i => i.MapFrom(x => new Util().GetThumbnailLocal(x.ImageArtUrl)));

            CreateMap<CardDraftPickWpf, CardDraftPickVM>()
                .ForMember(i => i.BorderGradient, i => i.Ignore());
            CreateMap<ConfigModelApp, OptionsWindowVM>();
            //.ForMember(i => i.ForceCardPopupSide, i => i.MapFrom(x =>  string.IsNullOrEmpty(x.ForceCardPopupSide) ? "On the left" : x.ForceCardPopupSide));
        }
    }
}
