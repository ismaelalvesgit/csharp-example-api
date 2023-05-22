using Bogus;
using Example.Application.Dto;
using Example.Domain.Entitys;

namespace Example.unitTests.Helpers
{
    public static class CategoryGenerator
    {
        public static IEnumerable<Category> GenerateValidCategorys(int quantity)
        {
            var categorys = new List<Category>();
            
            for (int i = 0; i < quantity; i++)
            {
                var fake = new Faker();
                var category = new Category(
                    id: 0,
                    name: fake.Name.FullName(),
                    imageUrl: fake.Internet.Url(),
                    products: Array.Empty<Product>()
                );

                categorys.Add(category);
            }

            return categorys;
        }
    }
}
