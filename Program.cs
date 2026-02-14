using Microsoft.EntityFrameworkCore;
using hotzerver.pilkki.calendar.Data;
using hotzerver.pilkki.calendar.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddDbContext<PilkkiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<HolidayService>();
builder.Services.AddScoped<WeekendService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PilkkiDbContext>();
    db.Database.EnsureCreated();
    SeedData.EnsureSeeded(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<hotzerver.pilkki.calendar.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
