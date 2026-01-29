using AutoMapper;
using FaceLook.Data.Entities;
using FaceLook.ViewModels;

namespace FaceLook.Services.MappingProfiles
{
    public class MesssagingProfile : Profile
    {
        public MesssagingProfile()
        {
            CreateMap<Message, MessageViewModel>();
        }
    }
}
