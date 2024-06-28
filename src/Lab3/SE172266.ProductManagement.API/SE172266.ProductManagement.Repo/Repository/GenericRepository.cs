using Microsoft.EntityFrameworkCore;
using SE172266.ProductManagement.API.Model.ProductModel;
using SE172266.ProductManagement.Repo.Entities;
using SE172266.ProductManagement.Repo.Interfaces;
using SE172266.ProductManagement.Repo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SE172266.ProductManagement.Repo.Repository
{
    public class GenericRepository<T> where T : class, ISoftDelete
    {
        MyStoreDBContext _context;
        DbSet<T> _dbSet;

        public GenericRepository(MyStoreDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual ResponseSearchModel<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.Where(e => !e.IsDeleted);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            int total = query.Count();
            int totalPages = pageSize.HasValue && pageSize.Value > 0 ? (int)Math.Ceiling(total / (double)pageSize.Value) : 1;

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = Math.Max(pageIndex.Value - 1, 0);
                int validPageSize = Math.Max(pageSize.Value, 1);
                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return new ResponseSearchModel<T>
            {
                Entities = query.ToList(),
                PageIndex = pageIndex ?? 1,
                PageSize = pageSize ?? total,
                TotalPages = totalPages,
                TotalProducts = total
            };
        }

        public virtual T GetById(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null && !entity.IsDeleted)
            {
                return entity;
            }
            return null;
        }

        public virtual void Insert(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Delete(int id)
        {
            T entityToDelete = _dbSet.Find(id);

            if (entityToDelete != null)
            {
                if (_context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    _dbSet.Attach(entityToDelete);
                }
                _dbSet.Remove(entityToDelete);
            }
            else
            {
                throw new ArgumentException($"Entity with id {id} not found.");
            }
        }

        public virtual void Update(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual int Count(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Count();
        }
    }
}
