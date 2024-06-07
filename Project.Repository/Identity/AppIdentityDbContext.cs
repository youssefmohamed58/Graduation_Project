using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Project.Core.Entities;

namespace Project.Repository.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> Options) : base(Options)
        {

        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Projection> Projections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the one-to-many relationship between AppUser and Projection
            modelBuilder.Entity<AppUser>()
                .HasMany(user => user.Projections)
                .WithOne(projection => projection.AppUser)
                .HasForeignKey(projection => projection.AppUserId)
                .OnDelete(DeleteBehavior.Cascade); // Configure cascade delete

            base.OnModelCreating(modelBuilder);
        }
    }
}
