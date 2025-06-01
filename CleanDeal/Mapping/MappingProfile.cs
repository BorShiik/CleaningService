using AutoMapper;
using CleanDeal.DTOs;
using CleanDeal.Models;
using CleanDeal.ViewModel;

namespace CleanDeal.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        /* ---------------------------- DTO dla list / szczegółów ---------------------------- */
        CreateMap<CleaningOrder, CleaningOrderDTO>()
            .ForMember(d => d.ServiceTypeName, o => o.MapFrom(s => s.ServiceType.Name))
            .ForMember(d => d.PaymentAmount, o => o.MapFrom(s => s.Payment != null ? s.Payment.Amount : (decimal?)null))
            .ForMember(d => d.HasReview, o => o.MapFrom(s => s.Review != null))
            .ForMember(d => d.ReviewRating, o => o.MapFrom(s => s.Review != null ? (int?)s.Review.Rating : null))
            .ForMember(d => d.Status,
                           o => o.MapFrom(s =>
                                  s.Status == OrderStatus.WaitingForCleaner ? "Oczekuje"
                                    : s.Status == OrderStatus.InProcess ? "W toku"
                                    : "Ukończone"))
            .ForMember(d => d.OrderTotal, o => o.MapFrom(s => s.TotalPrice))
            .ForMember(d => d.IsCompleted, o => o.MapFrom(s => s.Status == OrderStatus.Finished));

        /* -------------------------- VM ⇄ Encja (create / edit) -------------------------- */
        CreateMap<CleaningOrder, OrderCreateViewModel>()
            .ForMember(d => d.SelectedServiceTypeIds, o => o.MapFrom(s => s.Items.Select(i => i.ServiceTypeId)))
            .ForMember(d => d.ServiceTypeOptions, o => o.Ignore())
            .ForMember(d => d.PriceMap, o => o.Ignore())
            .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.TotalPrice));

        CreateMap<OrderCreateViewModel, CleaningOrder>()
            .ForMember(o => o.Id, x => x.Ignore())
            .ForMember(o => o.Status, x => x.Ignore())
            .ForMember(o => o.UserId, x => x.Ignore())
            .ForMember(o => o.Items, x => x.Ignore())   // w repozytorium
            .ForMember(o => o.Payment, x => x.Ignore())
            .ForMember(o => o.Review, x => x.Ignore())
            .ForMember(o => o.ChatMessages, x => x.Ignore());
    }
}
