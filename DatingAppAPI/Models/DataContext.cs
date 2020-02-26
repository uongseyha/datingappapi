using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppAPI.Models
{
    public class DataContext: DbContext
    {
        //private const string connectionString = "data source=.\\SQLEXPRESS; initial catalog=DatingAppDB;integrated security=SSPI;";

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(connectionString);
        //}

        public DataContext(DbContextOptions<DataContext> option) : base(option)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(k => new { k.LikeeId, k.LikerId });

            builder.Entity<Like>()
                .HasOne(k => k.Likee)
                .WithMany(k => k.Likers)
                .HasForeignKey(k => k.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                            .HasOne(k => k.Liker)
                            .WithMany(k => k.Likees)
                            .HasForeignKey(k => k.LikerId)
                            .OnDelete(DeleteBehavior.Restrict);



        }

    }
}
