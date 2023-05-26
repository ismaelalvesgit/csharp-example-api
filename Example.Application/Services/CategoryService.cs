using Example.Domain.Entitys;
using Example.Domain.Interfaces.Repository;
using Example.Domain.Interfaces.Services;

namespace Example.Application.Services;

public class CategoryService : ServiceBase<Category>, ICategoryService
{
    public CategoryService(ICategoryRepository repository) : base(repository)
    { }
}
