using Microsoft.EntityFrameworkCore;

namespace WeddingInvitation.Server.Data;

public class WeddingDbContext(DbContextOptions<WeddingDbContext> options) : DbContext(options)
{
    public DbSet<Wedding> Weddings => Set<Wedding>();
    public DbSet<Rsvp> Rsvps => Set<Rsvp>();
    public DbSet<GuestWish> GuestWishes => Set<GuestWish>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wedding>().HasIndex(w => w.Slug).IsUnique();
    }
}

public class Wedding
{
    public int Id { get; set; }
    public string Slug { get; set; } = "";
    public string GroomName { get; set; } = "";
    public string BrideName { get; set; } = "";
    public DateTime WeddingDate { get; set; }

    public List<Rsvp> Rsvps { get; set; } = [];
    public List<GuestWish> Wishes { get; set; } = [];
}

public class Rsvp
{
    public int Id { get; set; }
    public int WeddingId { get; set; }
    public string GuestName { get; set; } = "";
    public bool IsAttending { get; set; }
    public int GuestCount { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GuestWish
{
    public int Id { get; set; }
    public int WeddingId { get; set; }
    public string GuestName { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
