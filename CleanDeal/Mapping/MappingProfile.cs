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
                .ForMember(d => d.Cleaner,
                           o => o.MapFrom(s => s.Cleaner))
                .ForMember(d => d.ReviewRating,
                           o => o.MapFrom(s => s.Review != null ? (int?)s.Review.Rating : null))
                .ForMember(d => d.Status,
                           o => o.MapFrom(s =>
                                  s.Status == OrderStatus.WaitingForCleaner ? "Oczekuje"
                                    : (s.Status == OrderStatus.InProcess ? "W toku"
                                    : "Ukończone")));

            CreateMap<CleaningOrder, OrderCreateViewModel>()
                .ForMember(d => d.ServiceTypeOptions, o => o.Ignore())
                .ForMember(d => d.UserEmail,
                           o => o.MapFrom(s => s.User.Email));

            CreateMap<CleaningOrder, CleanerAvailableOrderDTO>()
                 .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.ServiceType.Name));

            CreateMap<CleaningOrder, CleanerMyOrderDTO>()
                .ForMember(d => d.ServiceName, o => o.MapFrom(s => s.ServiceType.Name))
                .ForMember(d => d.Status, o => o.MapFrom(s =>
                    s.Status == OrderStatus.Finished ? "Ukończone" : "W trakcie"))
                .ForMember(d => d.CanComplete, o => o.MapFrom(s => s.Status == OrderStatus.InProcess));

            CreateMap<Payment, PaymentDTO>();
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();

            CreateMap<ProductOrderItem, ProductOrderItemDTO>()
               .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
               .ForMember(d => d.Price, o => o.MapFrom(s => s.Product.Price));

            CreateMap<ProductOrder, ProductOrderDTO>()
                .ForMember(d => d.PaymentAmount,
                           o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : (decimal?)null));

            CreateMap<ProductOrderCreateViewModel, ProductOrder>()
                .ForMember(o => o.User, opt => opt.Ignore())
                .ForMember(o => o.Payment, opt => opt.Ignore())
                .ForMember(o => o.Items, opt => opt.Ignore());

            CreateMap<ChatMessage, ChatMessageDTO>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src => src.Receiver != null ? src.Receiver.FullName : null));

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
