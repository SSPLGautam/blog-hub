using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data
{
    public class AppDbContext: IdentityDbContext<IdentityUser>
    {
        public  AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
       
        public DbSet<Post>Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                  new Category { Id=1,Name="Technology"},
                  new Category { Id = 2, Name = "Health" },
                  new Category { Id = 3, Name = "LifeStyle" }

                );
            modelBuilder.Entity<Post>().HasData( 
                 new Post { 
                     Id =1,
                     Title ="Health Post 1",
                     Content = "Content of Health Post 1",
                     Authore = "Yash Yadav",
                     PublishedDate = new DateTime(2023,1,1),
                     CategoryId = 2,
                     FeatureImagePath = "images/Health1.jpg",




                 },
                     new Post
                     {
                         Id = 2,
                         Title = "Teach Post 1",
                         Content = "Content of Teach Post 1",
                         Authore = "Ravi ",
                         PublishedDate = new DateTime(2024, 6, 1),
                         CategoryId = 1,
                         FeatureImagePath = "images/Teach.jpg",




                     },
                      new Post
                      {
                          Id = 3,
                          Title = "LifeStyle Post 1",
                          Content = "Content of LifeStyle Post 1",
                          Authore = "joi Davel ",
                          PublishedDate = new DateTime(2025, 12, 9),
                          CategoryId = 3,
                          FeatureImagePath = "images/download.jpg",




                      }

                );
                
                
                
                
                
        }
    }
}
 