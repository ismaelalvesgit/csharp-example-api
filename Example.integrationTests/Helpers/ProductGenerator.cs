using Bogus;
using Example.Application.Dto;
using Example.Domain.Entitys;

namespace Example.integrationTests.Helpers
{
    public static class ProductGenerator
    {
        public static IEnumerable<Product> GenerateValidProducts(int quantity, int categoryId)
        {
            var products = new List<Product>();
            
            for (int i = 0; i < quantity; i++)
            {
                var fake = new Faker();
                var product = new Product(
                    id: 0,
                    name: fake.Name.FullName(),
                    quantity: fake.Random.Number(1, 10),
                    categoryId: categoryId,
                    description: fake.Commerce.Locale,
                    imageUrl: fake.Internet.Url()
                 );
                
                product.UpdateAmount(fake.Random.Number(1, 10));

                products.Add(product);
            }

            return products;
        }

        public static IEnumerable<ProductDto> GenerateValidProductsDto(int quantity, int categoryId)
        {
            var products = new List<ProductDto>();

            for (int i = 0; i < quantity; i++)
            {
                var fake = new Faker();
                var product = new ProductDto()
                {
                    Name = fake.Name.FullName(),
                    ImageUrl = fake.Internet.Url(),
                    CategoryId = categoryId,
                    Description = fake.Commerce.Locale,
                    Price = fake.Random.Number(10, 1000),
                    Quantity = fake.Random.Number(1, 10)
                };

                products.Add(product);
            }

            return products;
        }
    }
}
