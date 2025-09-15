using Microsoft.EntityFrameworkCore;
using RickAndMortyApp.Data.Entities;

namespace RickAndMortyApp.Data
{
    public class RickAndMortyContext : DbContext
    {
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Episode> Episodes => Set<Episode>();
        public DbSet<CharacterEpisode> CharacterEpisodes => Set<CharacterEpisode>();

        public RickAndMortyContext(DbContextOptions<RickAndMortyContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CharacterEpisode>()
                .HasKey(ce => new { ce.CharacterId, ce.EpisodeId });

            modelBuilder.Entity<CharacterEpisode>()
                .HasOne(ce => ce.Character)
                .WithMany(c => c.CharacterEpisodes)
                .HasForeignKey(ce => ce.CharacterId);

            modelBuilder.Entity<CharacterEpisode>()
                .HasOne(ce => ce.Episode)
                .WithMany(e => e.CharacterEpisodes)
                .HasForeignKey(ce => ce.EpisodeId);

            modelBuilder.Entity<Character>()
                .HasOne(c => c.Origin)
                .WithMany(l => l.OriginCharacters)
                .HasForeignKey(c => c.OriginId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Character>()
                .HasOne(c => c.Location)
                .WithMany(l => l.Residents)
                .HasForeignKey(c => c.CurrentLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
