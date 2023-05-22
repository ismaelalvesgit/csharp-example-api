using AutoMapper;
using Example.Application.Dto;
using Example.Domain.Entitys;

namespace Example.Application.Mappers
{
    public class FindCategoryMapper : Profile
    {
        public FindCategoryMapper() {
            CreateMap<Category, FindCategoryDto>();
            CreateMap<FindCategoryDto, Category>();
        }
    }
}
