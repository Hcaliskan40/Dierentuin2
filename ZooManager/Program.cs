using Microsoft.EntityFrameworkCore;
using ZooManager.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC + Views
builder.Services.AddControllersWithViews();

// API Controllers (voor later)
builder.Services.AddControllers();

// DbContext (SQL Server LocalDB)
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=Dierentuin???;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ZooDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Seed database (optioneel, maar handig)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();
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

// MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API routes (attribute routing)
app.MapControllers();

app.Run();