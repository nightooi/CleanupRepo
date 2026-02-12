using events.infra.Model;
using Microsoft.EntityFrameworkCore;

namespace events.infra;

public class EventsDbContext : DbContext
{
    public EventsDbContext(DbContextOptions<EventsDbContext> options) : base(options) {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Feature> Features => Set<Feature>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Event
        modelBuilder.Entity<Event>(e =>
        {

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
             .HasColumnName("id");

            e.Property(x => x.Name)
             .HasColumnName("name")
             .IsRequired();

            e.Property(x => x.EventStart)
             .HasColumnName("event_start")
             .IsRequired();

            e.Property(x => x.EventEnd)
             .HasColumnName("event_end")
             .IsRequired();

            e.Property(x => x.Covers)
             .HasColumnName("covers")
             .HasColumnType("text[]");

            e.HasOne(x => x.Category)
            .WithMany(x => x.Events)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        });

        modelBuilder.Entity<Category>(c =>
        {
            c.ToTable("categories");
            c.HasKey(x => x.Id);

            c.Property(x => x.Id)
             .HasColumnName("id");

            c.Property(x => x.Name)
             .HasColumnName("name")
             .IsRequired();

            c.HasMany(x => x.Events)
            .WithOne(x => x.Category)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        });

        modelBuilder.Entity<Feature>(f =>
        {
            f.ToTable("features");
            f.HasKey(x => x.Id);

            f.Property(x => x.Id)
             .HasColumnName("id");

            f.Property(x => x.Name)
             .HasColumnName("name")
             .IsRequired();

            f.HasMany(x => x.Events)
            .WithMany(x => x.Features);
            
        });
    }
}
