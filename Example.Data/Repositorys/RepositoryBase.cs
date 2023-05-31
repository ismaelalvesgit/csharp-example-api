using Example.Data.Helpers;
using Example.Domain.Entitys;
using Example.Domain.Exceptions;
using Example.Domain.Interfaces.Repository;
using Example.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.Data.Repositorys
{
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

        private static IQueryable<TEntity> IncludesQuery(IQueryable<TEntity> query,  string[]? includes) 
        {
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        private static IQueryable<TEntity> OrderByQuery(IQueryable<TEntity> query, QueryOptions options)
        {
            if (!string.IsNullOrEmpty(options.OrderBy) && query.PropertyExists(options.OrderBy))
            {
                IQueryable<TEntity>? orderBy;
                if (options.OrderByDescending == true) 
                {
                    orderBy = query.OrderByPropertyDescending(options.OrderBy);
                }
                else
                {
                    orderBy = query.OrderByProperty(options.OrderBy);
                }
                if (orderBy != null)
                    query = orderBy;
            }
            else
            {
                query = query.OrderBy(o => o.Id);
            }

            return query;
        }

        private static IQueryable<TEntity> WhereByQuery(IQueryable<TEntity> query, WhereOptions[]? wheres)
        {
            if (wheres is not null)
            {
                foreach (var where in wheres)
                {
                    if (!string.IsNullOrEmpty(where.FilterBy) && query != null && query.PropertyExists(where.FilterBy) && !string.IsNullOrEmpty(where.Value))
                    {
                        IQueryable<TEntity>? whereBy = query.WhereByOperation(where.FilterBy, where.Value, where.Operation);
                        if (whereBy != null)
                            query = whereBy;
                    }
                }
            }

            return query;
        }

        public virtual async Task<Pagination<TEntity>> FindAllAsync(int page, int pageSize, QueryOptions options)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            var queryTotal = _context.Set<TEntity>().AsQueryable();

            query = IncludesQuery(query, options.Includes);
            query = WhereByQuery(query, options.Where);
            queryTotal = WhereByQuery(queryTotal, options.Where);
            query = OrderByQuery(query, options);

            query = query.Skip((page - 1) * pageSize);
            query = query.Take(pageSize);

            var list = await query.ToListAsync();

            var totalItems = await queryTotal.CountAsync();

            return new Pagination<TEntity>(list, totalItems, page, pageSize);
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

        public virtual async Task<TEntity?> FindByIdAsync(int id, string[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            query = IncludesQuery(query, includes);

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
}