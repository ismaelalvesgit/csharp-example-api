using Example.Domain.Entitys;
using Example.Domain.Exceptions;
using Example.Domain.Interfaces.Repository;
using Example.Domain.Interfaces.Services;
using Example.Domain.Models;

namespace Example.Application.Services
{
    public class ProductService : ServiceBase<Product>, IProductService
    {
        readonly protected ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository repository, ICategoryRepository categoryRepository) : base(repository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task<int> InsertAsync(Product model)
        {
            var category = await _categoryRepository.AnyAsync(model.CategoryId);
            return category ? await base._repository.InsertAsync(model) : throw new NotFoundException("Category not exist...");
        }

        public override Task<Product?> FindByIdAsync(int id)
        {
            return base._repository.FindByIdAsync(id, new string[] { "Category" });
        }

        public override Task<Pagination<Product>> FindAllAsync(QueryData query)
        {
            var options = GetQueryOptions(query);
            options.Includes = new string[] { "Category" };
            return base._repository.FindAllAsync(query.Page, query.PageSize, options);
        }
    }
}