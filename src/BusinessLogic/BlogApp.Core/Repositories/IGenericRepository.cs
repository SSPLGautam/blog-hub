using BlogApp.Models;

namespace BlogApp.Core.Repositories
{
    public  interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();

        Task<T> GetByIdAsync(int id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task SaveAsync();
    }
}
