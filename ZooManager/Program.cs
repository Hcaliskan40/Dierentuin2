using Microsoft.EntityFrameworkCore;
using ZooManager.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=Dierentuin5;Trusted_Connection=True;MultipleActiveResultSets=true";

// ✅ Windows: LocalDB (SqlServer) - zoals opdracht
// ✅ Mac: SQLite (alleen om lokaal te kunnen runnen)
if (OperatingSystem.IsWindows())
{
    builder.Services.AddDbContext<ZooDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    // Mac/Linux fallback
    builder.Services.AddDbContext<ZooDbContext>(options =>
        options.UseSqlite("Data Source=zoo-dev.db"));
}

var app = builder.Build();

// ✅ Migrate + Seed (alleen als DB bereikbaar is)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();