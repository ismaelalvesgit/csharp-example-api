using Example.Domain.Entitys;
using Example.Domain.Interfaces.Repository;

namespace Example.Data.Repositorys
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        { }
    }
}