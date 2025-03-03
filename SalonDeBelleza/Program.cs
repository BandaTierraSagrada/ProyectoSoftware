using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;

var builder = WebApplication.CreateBuilder(args);

// Configurar el contexto de la base de datos para MySQL
var connectionString = "server=bqoiit78a116zt8g5ovu-mysql.services.clever-cloud.com;port=3306;database=bqoiit78a116zt8g5ovu;user=uaarc4dw9qk5anxf;password=v9NDoyB9DEaRNsY9U71B";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)))); // Cambia la versión según tu servidor MySQL

// Configurar Identity
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.RootDirectory = "/src/views"; // Establece la nueva ruta de las vistas
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de sesión de 30 min
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware para manejar solicitudes
app.UseSession(); // Habilitar sesiones
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();