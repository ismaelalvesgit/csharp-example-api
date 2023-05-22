using Example.Domain.Entitys;
using Example.Domain.Models;

namespace Example.Domain.Interfaces.Repository;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    Task<int> InsertAsync(TEntity model);
    Task UpdateAsync(TEntity model);
    Task DeleteAsync(int id);
    Task DeleteAsync(TEntity model);
    Task<bool> AnyAsync(int id);
    Task<TEntity?> FindByIdAsync(int id);
    Task<TEntity?> FindByIdAsync(int id, string[] includes);
    Task<Pagination<TEntity>> FindAllAsync(int page, int pageSize);
    Task<Pagination<TEntity>> FindAllAsync(int page, int pageSize, QueryOptions options);
}
