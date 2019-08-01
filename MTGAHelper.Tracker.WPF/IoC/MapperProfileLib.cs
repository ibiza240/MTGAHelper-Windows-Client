using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Web.Models.Response.User;

namespace MTGAHelper.Tracker.WPF.IoC
{
    public class MapperProfileTrackerWpf : Profile
    {
        public MapperProfileTrackerWpf()
        {
            CreateMap<CardForDraftPickDto, CardDraftPick>();
        }
    }
}
