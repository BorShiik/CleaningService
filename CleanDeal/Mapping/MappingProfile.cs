using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.ViewModel;

namespace CleanDeal.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile() {
            CreateMap<CleaningOrder, CleaningOrderDTO>()
               .ForMember(dto => dto.ServiceTypeName, opt => opt.MapFrom(src => src.ServiceType.Name))
               .ForMember(dto => dto.UserEmail, opt => opt.MapFrom(src => src.User.Email))
               .ForMember(dto => dto.IsPaid, opt => opt.MapFrom(src => src.Payment != null));

            CreateMap<Payment, PaymentDTO>();

            CreateMap<ChatMessage, ChatMessageDTO>()
                .ForMember(dto => dto.SenderName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<Review, ReviewDTO>();

            CreateMap<OrderCreateViewModel, CleaningOrder>()
                .ForMember(co => co.ServiceType, opt => opt.Ignore())  
                .ForMember(co => co.User, opt => opt.Ignore())        
                .ForMember(co => co.Payment, opt => opt.Ignore())
                .ForMember(co => co.Review, opt => opt.Ignore())
                .ForMember(co => co.ChatMessages, opt => opt.Ignore());
        }
    }
}
