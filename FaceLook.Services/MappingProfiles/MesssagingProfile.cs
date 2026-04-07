using AutoMapper;
using FaceLook.Data.Entities;
using FaceLook.Web.ViewModels;

namespace FaceLook.Services.MappingProfiles
{
    public class MesssagingProfile : Profile
    {
        public MesssagingProfile()
        {
            CreateMap<Message, MessageViewModel>()
                .ForMember(dest => dest.SenderEmail, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.Email : string.Empty));

            CreateMap<Chat, ChatViewModel>();
        }
    }
}
