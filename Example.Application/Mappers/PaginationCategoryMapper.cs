using AutoMapper;
using Example.Application.Dto;
using Example.Domain.Entitys;
using Example.Domain.Models;

namespace Example.Application.Mappers
{
    public class PaginationCategoryMapper : Profile
    {
        public PaginationCategoryMapper()
        {
            CreateMap<Pagination<Category>, Pagination<FindCategoryDto>>();
            CreateMap<Pagination<FindCategoryDto>, Pagination<Category>>();
        }
    }
}
