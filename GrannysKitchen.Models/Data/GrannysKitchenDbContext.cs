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
    }
}
