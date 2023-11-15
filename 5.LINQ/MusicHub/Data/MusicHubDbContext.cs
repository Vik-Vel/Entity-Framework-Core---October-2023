using MusicHub.Data.Models;

namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }
        DbSet<Album> Albums { get; set; }
        DbSet<Performer> Performers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Song> Songs { get; set; }
        DbSet<Writer> Writers { get; set; }

        DbSet<SongPerformer> SongPerformers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }



       


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SongPerformer>()
                .HasKey(sp => new { sp.SongId, sp.PerformerId });
        }

        //With Fluent API
        //Delete all attributes from classes
        //protected override void OnModelCreating2(ModelBuilder builder)
        //{
        //   //Sets primary key

        //   builder.Entity<Album>()
        //       .HasKey(a => a.Id);

        //   builder.Entity<Album>()
        //       .Property(a => a.Name)
        //           .IsRequired(true)
        //           .HasMaxLength(40);

        //   builder.Entity<Album>()
        //       .Property(a => a.ReleaseDate)
        //       .IsRequired(true);


        //   builder.Entity<Album>()
        //       .HasOne(a => a.Producer)
        //       .WithMany(p => p.Albums)
        //       .HasForeignKey(a => a.ProducerId);

        //   builder.Entity<Album>()
        //       .HasMany(a => a.Songs)
        //       .WithOne(s => s.Album);

        //   builder.Entity<Producer>()
        //       .HasKey(p => p.Id);

        //   builder.Entity<Producer>()
        //       .HasMany(p => p.Albums)
        //       .WithOne(a => a.Producer)
        //       .HasForeignKey(a => a.ProducerId);

        //   builder.Entity<SongPerformer>()
        //       .HasKey(sp => new { sp.SongId, sp.PerformerId });
        //}
    }
}
