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
                .ForMember(d => d.ServiceTypeName,
                           o => o.MapFrom(s => s.ServiceType.Name))
                .ForMember(d => d.PaymentAmount,
                           o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : (decimal?)null))
                .ForMember(d => d.TotalPrice,
                           o => o.MapFrom(s => s.TotalPrice != null ? s.TotalPrice : (decimal?)null))
                .ForMember(d => d.HasReview,
                           o => o.MapFrom(s => s.Review != null))
                .ForMember(d => d.ReviewRating,
                           o => o.MapFrom(s => s.Review != null ? (int?)s.Review.Rating : null))
                .ForMember(d => d.Status,
                           o => o.MapFrom(s =>
                                  s.Status == OrderStatus.WaitingForCleaner ? "Oczekuje"
                                    : s.Status == OrderStatus.InProcess ? "W toku"
                                    : "Ukończone"));

            CreateMap<CleaningOrder, OrderCreateViewModel>()
                .ForMember(d => d.ServiceTypeOptions, o => o.Ignore());

            CreateMap<CleaningOrder, CleanerAvailableOrderDTO>()
                 .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.ServiceType.Name));

            CreateMap<CleaningOrder, CleanerMyOrderDTO>()
                .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.ServiceType.Name))
                .ForMember(d => d.Status, o => o.MapFrom(s =>
                    s.Status == OrderStatus.Finished ? "Ukończone" : "W trakcie"))
                .ForMember(d => d.CanComplete, o => o.MapFrom(s => s.Status == OrderStatus.InProcess));

            CreateMap<Payment, PaymentDTO>();

            CreateMap<ChatMessage, ChatMessageDTO>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.UserName))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver != null ? src.Receiver.UserName : null));

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
