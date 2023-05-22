using AutoMapper;
using Example.Application.Dto;
using Example.Domain.Entitys;

namespace Example.Application.Mappers
{
    public class FindProductMapper : Profile
    {
        public FindProductMapper() {
            CreateMap<Product, FindProductDto>();
            CreateMap<FindProductDto, Product>();
        }
    }
}
