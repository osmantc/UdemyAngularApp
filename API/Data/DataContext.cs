using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DataContext() => this.ChangeTracker.LazyLoadingEnabled = false;

        public virtual DbSet<AppUser> AppUsers { get; set; }

        

    }
}