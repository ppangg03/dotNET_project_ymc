using backend_yourmycelebrity.Data;
using backend_yourmycelebrity.Models;
using backend_yourmycelebrity.Repositories.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace backend_yourmycelebrity.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        } 
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T?> GetByID(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T?> Update(int id,T entity)
        {
            var existing = await _context.Set<T>().FindAsync(id);
            if (existing == null)
            {
                return null;
            }
            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<T> Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if(entity == null)
            {
                return false;
            }
            _context.Set<T>().Remove(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }
        public Task<List<T>> GetAllWithInclude(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();
            query = includes.Aggregate(query, (current,include) =>  current.Include(include));
            return query.ToListAsync();
        }
        public void AddRange(IEnumerable<T> model)
        {
            _context.Set<T>().AddRange(model);
            _context.SaveChanges();
        }
        public IEnumerable<T> GetList(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }
        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _context.Set<T>().Where(predicate));
        }
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }
        public int Count()
        {
            return _context.Set<T>().Count();
        }
        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }
        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }
        //private bool TodoItemExists(int id)
        //{
        //    return _context.Set<T>.Any(e => e.Id == id);
        //}
        //public async Task<T> GetByIDWithInclude(int id, params Expression<Func<T, object>>[] includes)
        //{
        //    IQueryable<T> query = _context.Set<T>();
        //    query = includes.Aggregate(query, (current, include) => current.Include(include));
        //    return  query.FirstOrDefault(id);
        //}
    }
}
