using AutoMapper;
using ECommerce.Application.Abstractions.Models;
using ECommerce.Infrastructure.Balance.Models;

namespace ECommerce.Infrastructure.Balance.Mapping;

public sealed class BalanceMappingProfile : Profile
{
    public BalanceMappingProfile()
    {
        // External DTO -> Gateway read-model (Info)
        CreateMap<ProductDto, ProductInfo>()
            .ConstructUsing(s => new ProductInfo(
                s.Id, s.Name, s.Description, s.Price, s.Currency, s.Category, s.Stock
            ));

        CreateMap<PreOrderDetail, PreorderInfo>()
            .ForCtorParam("ExternalOrderId", o => o.MapFrom(s => s.OrderId))
            .ForCtorParam("Amount", o => o.MapFrom(s => s.Amount))
            .ForCtorParam("Status", o => o.MapFrom(s => s.Status));

        CreateMap<CompletedOrder, CompletionInfo>()
            .ForCtorParam("ExternalOrderId", o => o.MapFrom(s => s.OrderId))
            .ForCtorParam("Status", o => o.MapFrom(s => s.Status))
            .ForCtorParam("CompletedAt", o => o.MapFrom(s => s.CompletedAt));
    }
}
