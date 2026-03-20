using BlogApp.Data;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using BlogApp.Core.Data.Repositories;
using System.Diagnostics;

namespace BlogApp.Data.Repositories
{
    public    class GenericRepository<T>:IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(AppDbContext context)
        {
            this._context = context;
         
            _dbSet  = _context.Set<T>();    
        }     
        public  IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }
                
        public async Task AddAsync(T entity)
        {     await _dbSet.AddAsync(entity);

        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity) { 
            _dbSet.Remove(entity);  

        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

      
      
    }
}
