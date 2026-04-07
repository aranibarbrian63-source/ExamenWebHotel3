using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using examenwed3.Data;
using examenwed3.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la base de datos REAL (SQLite) - Se guarda en un archivo local
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=HotelReservas.db"));

// 2. Configurar Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Servicios necesarios
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();