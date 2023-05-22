using AutoMapper;
using Example.Application.Dto;
using Example.Domain.Entitys;

namespace Example.Application.Mappers
{
    public class ProductMapper : Profile
    {
        public ProductMapper() {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
