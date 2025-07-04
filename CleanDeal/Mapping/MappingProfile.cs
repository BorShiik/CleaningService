using AutoMapper;
using CleanDeal.DTO.DTOs;
using CleanDeal.DTO.ViewModel;
using CleanDeal.Model.Models;

namespace CleanDeal.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile() {
            CreateMap<CleaningOrder, CleaningOrderDTO>()
                .ForMember(d => d.ServiceNames,
                           o => o.MapFrom(s => string.Join(", ", s.ServiceItems.Any()
                                ? s.ServiceItems.Select(si => si.ServiceType.Name)
                                : new[] { s.ServiceType.Name })))
                .ForMember(d => d.PaymentAmount,
                           o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : (decimal?)null))
                .ForMember(d => d.TipAmount,
                           o => o.MapFrom(s => s.Payment != null ? s.Payment.Tip : (decimal?)null))
                .ForMember(d => d.TotalPrice,
                           o => o.MapFrom(s => s.TotalPrice != null ? s.TotalPrice : (decimal?)null))
                .ForMember(d => d.LoyaltyPoints,
                           o => o.MapFrom(s => s.Payment != null ? (int)(s.Payment.Amount / 10) : 0))
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
                .ForMember(d => d.ServiceTypeIds,
                           o => o.MapFrom(s => s.ServiceItems.Any()
                               ? s.ServiceItems.Select(si => si.ServiceTypeId)
                               : new[] { s.ServiceTypeId }))
                .ForMember(d => d.UserEmail,
                           o => o.MapFrom(s => s.User.Email));

            CreateMap<CleaningOrder, CleanerAvailableOrderDTO>()
                 .ForMember(d => d.ServiceNames,
                           o => o.MapFrom(s => string.Join(", ", s.ServiceItems.Any()
                                ? s.ServiceItems.Select(si => si.ServiceType.Name)
                                : new[] { s.ServiceType.Name })));

            CreateMap<CleaningOrder, CleanerMyOrderDTO>()
                .ForMember(d => d.ServiceNames,
                           o => o.MapFrom(s => string.Join(", ", s.ServiceItems.Any()
                                ? s.ServiceItems.Select(si => si.ServiceType.Name)
                                : new[] { s.ServiceType.Name })))
                .ForMember(d => d.Status, o => o.MapFrom(s =>
                    s.Status == OrderStatus.Finished ? "Ukończone" : "W trakcie"))
                .ForMember(d => d.CanComplete, o => o.MapFrom(s => s.Status == OrderStatus.InProcess));

            CreateMap<Payment, PaymentDTO>();
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>()
                .ForMember(d => d.Image, opt => opt.Ignore())
                .ForMember(d => d.ImageMimeType, opt => opt.Ignore());

            CreateMap<ProductOrderItem, ProductOrderItemDTO>()
               .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
               .ForMember(d => d.Price, o => o.MapFrom(s => s.Product.Price));

            CreateMap<ProductOrder, ProductOrderDTO>()
                .ForMember(d => d.PaymentAmount,
                           o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : (decimal?)null))
                .ForMember(d => d.LoyaltyPoints,
                           o => o.MapFrom(s => s.Payment != null ? (int)(s.Payment.Amount / 10) : 0));

            CreateMap<ProductOrderCreateViewModel, ProductOrder>()
                .ForMember(o => o.User, opt => opt.Ignore())
                .ForMember(o => o.Payment, opt => opt.Ignore())
                .ForMember(o => o.Items, opt => opt.Ignore());

            CreateMap<ChatMessage, ChatMessageDTO>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.Receiver, opt => opt.MapFrom(src => src.Receiver != null ? src.Receiver : null))
                .ForMember(d => d.IsAdmin, o => o.Ignore());

            CreateMap<Review, ReviewDTO>()
                .ForMember(d => d.UserFullName,
                           o => o.MapFrom(s => s.CleaningOrder.User.FullName))
                .ForMember(d => d.UserID,
                           o => o.MapFrom(s => s.CleaningOrder.User.Id));

            CreateMap<OrderCreateViewModel, CleaningOrder>()
                .ForMember(co => co.ServiceType, opt => opt.Ignore())
                .ForMember(co => co.User, opt => opt.Ignore())
                .ForMember(co => co.ServiceItems, opt => opt.Ignore())
                .ForMember(co => co.Payment, opt => opt.Ignore())
                .ForMember(co => co.Review, opt => opt.Ignore())
                .ForMember(co => co.ChatMessages, opt => opt.Ignore());
        }
    }
}
