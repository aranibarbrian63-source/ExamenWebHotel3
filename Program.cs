using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using examenwed3.Data;
using examenwed3.Models;

var builder = WebApplication.CreateBuilder(args);

// --- CORRECCIÓN CLAVE ---
// Eliminamos la variable 'connectionString' que causaba el crash
// Configuramos el Contexto UNIFICADO directamente con SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=HotelReservas.db"));

// 2. Configurar Identity con Roles
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

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- INICIO: Inicialización de Roles, Admin y Hoteles ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Asegurar que la base de datos y tablas existan antes de insertar datos
        context.Database.EnsureCreated();

        // A. Crear Roles si no existen
        string[] roleNames = { "Administrador", "Cliente" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // B. Crear Usuario Administrador inicial
        var adminEmail = "admin@hotel.com";
        var adminPass = "Admin123!";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                NombreCompleto = "Administrador del Sistema"
            };

            var result = await userManager.CreateAsync(newAdmin, adminPass);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Administrador");
            }
        }

        // C. AGREGAR HOTELES AUTOMÁTICAMENTE
        if (!context.Hoteles.Any())
        {
            context.Hoteles.AddRange(
                new Hotel
                {
                    Nombre = "Gran Hotel Cochabamba",
                    Direccion = "Calle Aroma #123",
                    PrecioPorNoche = 350,
                    Descripcion = "5 estrellas con piscina y Wi-Fi"
                },
                new Hotel
                {
                    Nombre = "Residencial El Sol",
                    Direccion = "Av. Heroínas",
                    PrecioPorNoche = 120,
                    Descripcion = "Económico y céntrico"
                },
                new Hotel
                {
                    Nombre = "Boutique Jardín Secreto",
                    Direccion = "Calle Junín #45",
                    PrecioPorNoche = 500,
                    Descripcion = "Ambiente exclusivo y privado"
                }
            );
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos.");
    }
}
// --- FIN: Inicialización ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();