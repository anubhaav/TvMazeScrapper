using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvMazeScrapper.Models;

namespace TvMazeScrapper.Data
{
    public class ShowCastDbContext : DbContext
    {
        public DbSet<ShowModel> ShowDb { get; set; }
        public DbSet<CastModel> CastDb { get; set; }
        public DbSet<PageNumberModel> PageNumberDb { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=showcast.db");


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShowModel>()
                .HasIndex(show => show.ShowId)
                .IsUnique();

            modelBuilder.Entity<ShowModel>()
             .HasIndex(show => show.Id);

            modelBuilder.Entity<ShowModel>()
               .HasKey(show => show.ShowId);

            modelBuilder.Entity<CastModel>()
               .HasKey(cast => cast.CastId);

            modelBuilder.Entity<CastModel>()
               .HasIndex(cast => cast.Birthday);

          

            modelBuilder.Entity<CastModel>()
               .HasIndex(cast => cast.CastId)
               .IsUnique();

            modelBuilder.Entity<ShowModel>()
             .HasMany<CastModel>(show => show.Cast)
             .WithOne()
             .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
