using Microsoft.EntityFrameworkCore;
using WeddingInvitation.Server.Data;
using WeddingInvitation.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WeddingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Wedding") ?? "Data Source=wedding.db"));

var app = builder.Build();

// Tạo database + seed thiệp cưới mặc định
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeddingDbContext>();
    db.Database.EnsureCreated();
    if (!db.Weddings.Any(w => w.Slug == "ngoc-nam-lan-anh"))
    {
        db.Weddings.Add(new Wedding
        {
            Slug = "ngoc-nam-lan-anh",
            GroomName = "Ngọc Nam",
            BrideName = "Lan Anh",
            WeddingDate = new DateTime(2026, 10, 18, 10, 30, 0)
        });
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

// Host Blazor WebAssembly Client (hosted model)
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// ---- API ---------------------------------------------------------------

app.MapGet("/api/weddings/{slug}", async (string slug, WeddingDbContext db) =>
{
    var wedding = await db.Weddings.AsNoTracking().FirstOrDefaultAsync(w => w.Slug == slug);
    return wedding is null
        ? Results.NotFound()
        : Results.Ok(new WeddingDto(wedding.Slug, wedding.GroomName, wedding.BrideName, wedding.WeddingDate));
});

app.MapPost("/api/weddings/{slug}/rsvp", async (string slug, RsvpRequest request, WeddingDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.GuestName)) return Results.BadRequest();
    if (ProfanityFilter.ContainsProfanity(request.GuestName) || ProfanityFilter.ContainsProfanity(request.Message))
        return Results.BadRequest("Nội dung có từ ngữ chưa phù hợp.");

    var wedding = await db.Weddings.FirstOrDefaultAsync(w => w.Slug == slug);
    if (wedding is null) return Results.NotFound();

    db.Rsvps.Add(new Rsvp
    {
        WeddingId = wedding.Id,
        GuestName = request.GuestName.Trim(),
        IsAttending = request.IsAttending,
        GuestCount = Math.Clamp(request.GuestCount, 1, 20),
        Message = request.Message?.Trim(),
        CreatedAt = DateTime.UtcNow
    });
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/api/weddings/{slug}/wishes", async (string slug, WeddingDbContext db) =>
{
    var wedding = await db.Weddings.AsNoTracking().FirstOrDefaultAsync(w => w.Slug == slug);
    if (wedding is null) return Results.NotFound();

    var wishes = await db.GuestWishes.AsNoTracking()
        .Where(w => w.WeddingId == wedding.Id)
        .OrderByDescending(w => w.CreatedAt)
        .Take(200)
        .Select(w => new GuestWishDto { GuestName = w.GuestName, Content = w.Content, CreatedAt = w.CreatedAt })
        .ToListAsync();

    return Results.Ok(wishes);
});

app.MapPost("/api/weddings/{slug}/wishes", async (string slug, GuestWishDto wish, WeddingDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(wish.GuestName) || string.IsNullOrWhiteSpace(wish.Content))
        return Results.BadRequest();
    if (ProfanityFilter.ContainsProfanity(wish.GuestName) || ProfanityFilter.ContainsProfanity(wish.Content))
        return Results.BadRequest("Nội dung có từ ngữ chưa phù hợp.");

    var wedding = await db.Weddings.FirstOrDefaultAsync(w => w.Slug == slug);
    if (wedding is null) return Results.NotFound();

    db.GuestWishes.Add(new GuestWish
    {
        WeddingId = wedding.Id,
        GuestName = wish.GuestName.Trim(),
        Content = wish.Content.Trim(),
        CreatedAt = DateTime.UtcNow
    });
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Mọi route còn lại trả về SPA
app.MapFallbackToFile("index.html");

app.Run();
