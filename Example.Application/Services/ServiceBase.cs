using Example.Domain.Entitys;
using Example.Domain.Interfaces.Repository;
using Example.Domain.Interfaces.Services;
using Example.Domain.Models;

namespace Example.Application.Services;

public class ServiceBase<TEntity> : IServiceBase<TEntity> where TEntity : EntityBase
{
    protected readonly IRepositoryBase<TEntity> _repository;

    public ServiceBase(IRepositoryBase<TEntity> repository)
    {
        _repository = repository;
    }

    public QueryOptions GetQueryOptions(QueryData query)
    {
        var options = new QueryOptions()
        {
            OrderBy = query.OrderBy,
            OrderByDescending = query.OrderByDescending,
        };

        if (query.FilterBy is not null && query.FilterBy.Length > 0)
        {
            options.Where = query.FilterBy.ToList().ConvertAll(new Converter<string, WhereOptions>(x => new WhereOptions(x))).ToArray();
        }

        return options;
    }

    public virtual Task DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    public virtual Task DeleteAsync(TEntity model)
    {
        return _repository.DeleteAsync(model);
    }

    public virtual Task<Pagination<TEntity>> FindAllAsync(QueryData query)
    {
        var options = GetQueryOptions(query);
        return _repository.FindAllAsync(query.Page, query.PageSize, options);
    }

    public virtual Task<TEntity?> FindByIdAsync(int id)
    {

        return _repository.FindByIdAsync(id);
    }

    public virtual Task<int> InsertAsync(TEntity model)
    {
        return _repository.InsertAsync(model);
    }

    public virtual Task UpdateAsync(TEntity model)
    {
        return _repository.UpdateAsync(model);
    }
}
