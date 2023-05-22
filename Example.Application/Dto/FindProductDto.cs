using Example.Domain.Entitys;

namespace Example.Application.Dto
{
    public class FindProductDto : EntityBase
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public float Quantity { get; set; }
        public FindCategoryDto? Category { get; set; }
    }
}
