using AutoMapper;
using Example.Application.Dto;
using Example.Domain.Entitys;

namespace Example.Application.Mappers
{
    public class CategoryMapper : Profile
    {
        public CategoryMapper() {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
        }
    }
}
