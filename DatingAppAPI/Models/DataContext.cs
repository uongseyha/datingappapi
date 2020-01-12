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
    }
}
