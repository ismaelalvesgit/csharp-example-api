using Example.Domain.Entitys;
using Example.Domain.Interfaces.Repository;

namespace Example.Data.Repositorys
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        { }
    }
}