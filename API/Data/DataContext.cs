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
        // public virtual DbSet<Photo> Photos { get; set; } //FIXME: tek başına bir photos nesnesi yoktur, mutlaka appuser ile alakalıdır.
        //FIXME: o yüzden direkt db'ye eklenmeyeceğine göre (relationla appUser üzerinden eklenebilir sadece) Photos için dbset'e gerek yoktur.
    }
}