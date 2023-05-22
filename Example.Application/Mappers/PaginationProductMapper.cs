using AutoMapper;
using Example.Application.Dto;
using Example.Domain.Entitys;
using Example.Domain.Models;

namespace Example.Application.Mappers
{
    public class PaginationProductMapper : Profile
    {
        public PaginationProductMapper() {
            CreateMap<Pagination<Product>, Pagination<FindProductDto>>();
            CreateMap<Pagination<FindProductDto>, Pagination<Product>>();
        }
    }
}
