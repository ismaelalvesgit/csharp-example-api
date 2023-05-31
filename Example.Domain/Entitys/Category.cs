using System.Collections.ObjectModel;

namespace Example.Domain.Entitys
{
    public class Category : EntityBase
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Product>? Products { get; set; }

        public Category()
        { }

        public Category(int id, string name, string? imageUrl, ICollection<Product>? products)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            Products = products ?? new Collection<Product>();
        }
    }
}