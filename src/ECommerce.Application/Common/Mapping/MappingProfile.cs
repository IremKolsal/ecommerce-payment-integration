using AutoMapper;
using ECommerce.Application.Abstractions.Models;
using ECommerce.Application.Commands.CompleteOrder; 
using ECommerce.Application.Commands.CreateOrder;  
using ECommerce.Application.Queries.GetProducts;     
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductInfo, GetProductsQueryResult>()
            .ConstructUsing(src => new GetProductsQueryResult(
                src.Id, src.Name, src.Description, src.Price, src.Currency, src.Category, src.Stock
            ));

        CreateMap<Order, CreateOrderCommandResult>()
            .ForCtorParam("Id", opt => opt.MapFrom(s => s.Id))
            .ForCtorParam("OrderId", opt => opt.MapFrom(s => s.ExternalOrderId))
            .ForCtorParam("Status", opt => opt.MapFrom(s => s.Status))
            .ForCtorParam("TotalAmount", opt => opt.MapFrom(s => s.TotalAmount));

        CreateMap<Order, CompleteOrderCommandResult>()
            .ForCtorParam("OrderId", opt => opt.MapFrom(s => s.ExternalOrderId))
            .ForCtorParam("Status", opt => opt.MapFrom(s => s.Status));
    }
}
