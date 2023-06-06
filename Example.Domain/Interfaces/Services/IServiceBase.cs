using Example.Domain.Entitys;
using Example.Domain.Models;

namespace Example.Domain.Interfaces.Services
{
    public interface IServiceBase<TEntity> where TEntity : EntityBase
    {
        Task<int> InsertAsync(TEntity model);
        Task UpdateAsync(TEntity model);
        Task DeleteAsync(int id);
        Task DeleteAsync(TEntity model);
        Task<TEntity?> FindByIdAsync(int id);
        Task<Pagination<TEntity>> FindAllAsync();
        Task<Pagination<TEntity>> FindAllAsync(QueryData query);
        QueryOptions GetQueryOptions(QueryData query);
    }
}