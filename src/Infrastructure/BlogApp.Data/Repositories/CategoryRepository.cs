using System;
using System.Collections.Generic;
using System.Text;
using BlogApp.Core.Data.Repositories;
using BlogApp.Core.Services;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;


namespace BlogApp.Data.Repositories
{

    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

    }
}