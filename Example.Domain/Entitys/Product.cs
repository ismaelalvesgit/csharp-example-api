namespace Example.Domain.Entitys
{
    public class Product : EntityBase
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public float Quantity { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public Product()
        { }

        public Product(int id, string name, float quantity, int categoryId, string? description, string? imageUrl)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Quantity = quantity;
            CategoryId = categoryId;
            Price = decimal.Zero;
        }

        public void UpdateAmount(decimal price)
        {
            Price = price;
        }
    }
}