using GrannysKitchen.Models.DBModels;
using GrannysKitchen.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.Data
{
    public class GrannysKitchenDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public GrannysKitchenDbContext(DbContextOptions<GrannysKitchenDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
           // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("GrannysKitchen_APIContext"));
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<ChefUsers> ChefUsers { get; set; }
        public DbSet<ResetPasswordTokens> ResetPasswordTokens { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<FoodItems> FoodItems { get; set; }
        public DbSet<ChefMenu> ChefMenu { get; set; }
        public DbSet<Orders> Orders { get; set; }
    }
}



