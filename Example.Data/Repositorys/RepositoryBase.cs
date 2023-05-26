using Example.Data.Helpers;
using Example.Domain.Entitys;
using Example.Domain.Exceptions;
using Example.Domain.Interfaces.Repository;
using Example.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.Data.Repositorys;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    protected readonly AppDbContext _context;

    public RepositoryBase(AppDbContext appDbContext) : base()
    {
        _context = appDbContext;
    }

    public virtual async Task<bool> AnyAsync(int id)
    {
        return await _context.Set<TEntity>().AnyAsync(x => x.Id == id);
    }

    public virtual async Task DeleteAsync(int id)
    {
        var model = await FindByIdAsync(id);
        if (model != null)
        {
            await DeleteAsync(model);
        }
    }

    public virtual async Task DeleteAsync(TEntity model)
    {
        _context.Set<TEntity>().Remove(model);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<Pagination<TEntity>> FindAllAsync(int page, int pageSize)
    {
        var list = await _context.Set<TEntity>()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize).ToListAsync();

        var totalItems = await _context.Set<TEntity>().CountAsync();

        return new Pagination<TEntity>(list, totalItems, page, pageSize);
    }

    public virtual async Task<Pagination<TEntity>> FindAllAsync(int page, int pageSize, QueryOptions options)
    {
        var query = _context.Set<TEntity>().AsQueryable();
        var queryTotal = _context.Set<TEntity>().AsQueryable();

        if (options.Includes is not null)
        {
            foreach (var include in options.Includes)
            {
                query = query.Include(include);
            }
        }

        if (options.Where is not null)
        {
            foreach (var where in options.Where)
            {
                if (!string.IsNullOrEmpty(where.FilterBy) && query != null && query.PropertyExists(where.FilterBy) && !string.IsNullOrEmpty(where.Value))
                {
                    query = query.WhereByOperation(where.FilterBy, where.Value, where.Operation);
                    queryTotal = queryTotal?.WhereByOperation(where.FilterBy, where.Value, where.Operation);
                }
            }
        }

        if (options.OrderByDescending == true)
        {
            if (!string.IsNullOrEmpty(options.OrderBy) && query.PropertyExists(options.OrderBy))
            {
                query = query.OrderByPropertyDescending(options.OrderBy);
            }
            else
            {
                query = query.OrderByDescending(o => o.Id);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(options.OrderBy) && query.PropertyExists(options.OrderBy))
            {
                query = query.OrderByProperty(options.OrderBy);
            }
            else
            {
                query = query.OrderBy(o => o.Id);
            }
        }

        query = query.Skip((page - 1) * pageSize);
        query = query.Take(pageSize);

        var list = await query.ToListAsync();

        var totalItems = await queryTotal.CountAsync();

        return new Pagination<TEntity>(list, totalItems, page, pageSize);
    }

    public virtual async Task<TEntity?> FindByIdAsync(int id, string[] includes)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.SingleOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<TEntity?> FindByIdAsync(int id)
    {
        return await _context.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<int> InsertAsync(TEntity model)
    {
        var id = _context.Set<TEntity>().Add(model).Entity.Id;
        await _context.SaveChangesAsync();
        return id;
    }

    public virtual async Task UpdateAsync(TEntity model)
    {
        var entity = await _context.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == model.Id) ?? throw new NotFoundException("Id not found");
        model.Id = entity.Id;
        model.CreatedAt = entity.CreatedAt;
        _context.Set<TEntity>().Update(model);
        await _context.SaveChangesAsync();
    }
}
