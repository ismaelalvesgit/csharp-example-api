using Example.Domain.Entitys;

namespace Example.Application.Dto
{
    public class FindCategoryDto : EntityBase
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
    }
}
