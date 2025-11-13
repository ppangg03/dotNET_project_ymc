using System.Linq.Expressions;

namespace backend_yourmycelebrity.Repositories.Interface
{
    public interface IGenericRepository<T>  where T : class
    {
        //basic
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByID(int id);
        Task<T> Add(T entity);
        Task<T?> Update(int id,T entity);
        Task<bool> Delete(int id);
        


        //Query
        Task<List<T>> GetAllWithInclude(params Expression<Func<T, object>>[] includes);
        //Task<IQueryable<T>> GetAllWithInclude();
        //Task<T> GetByIDWithInclude(int id, params Expression<Func<T, object>>[] includes );

        void AddRange(IEnumerable<T> entity);

        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetList(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        int Count();
        Task<int> CountAsync();
        //void Update(T entity);
        void Remove(T entity);
        void Dispose();
    }
}
