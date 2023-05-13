
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using User_Story.Models;

namespace User_Story.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users{ get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("Roles");  
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Files>().ToTable("Files");
            modelBuilder.Entity<Topic>().ToTable("Topics");
            modelBuilder.Entity<Message>().ToTable("Messages").HasOne(x => x.User).WithMany(x => x.Messages).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<News>().ToTable("News");
        }
    }

}
