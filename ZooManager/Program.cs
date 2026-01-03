using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("DefaultConnection missing.");

builder.Services.AddDbContext<ZooDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ZooLogicService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();
    db.Database.Migrate();
    // DbSeeder.Seed(db); // alleen als jullie seeding willen gebruiken
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