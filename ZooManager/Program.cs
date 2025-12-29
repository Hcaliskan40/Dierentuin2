using Microsoft.EntityFrameworkCore;
using ZooManager.Data;
using ZooManager.Seed;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ZooDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ZooDb")));

var app = builder.Build();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ZooManager.Data.ZooDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.Run();