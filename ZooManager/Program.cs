using Microsoft.EntityFrameworkCore;
using ZooManager.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC + API
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// DbContext (SQL Server LocalDB)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ZooDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Eerst migrations toepassen, dan seeden
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