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
               .ForMember(d => d.ServiceTypeName, o => o.MapFrom(s => s.ServiceType.Name))
               .ForMember(d => d.PaymentAmount, o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : (decimal?)null))
               .ForMember(d => d.HasReview, o => o.MapFrom(s => s.Review != null))
               .ForMember(d => d.ReviewRating, o => o.MapFrom(s => s.Review != null ? (int?)s.Review.Rating : null));

            CreateMap<CleaningOrder, OrderCreateViewModel>()
                .ForMember(d => d.ServiceTypeOptions, o => o.Ignore());

            CreateMap<Payment, PaymentDTO>();

            CreateMap<ChatMessage, ChatMessageDTO>().ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName));

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
