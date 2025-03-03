using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.repositories;
using SalonDeBelleza.src.services;


var builder = WebApplication.CreateBuilder(args);

// Configurar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configurar servicios básicos
builder.Services.AddControllers(); // Habilita controladores para APIs
builder.Services.AddEndpointsApiExplorer(); // Habilita documentación de API
builder.Services.AddSwaggerGen(); // Habilita Swagger para pruebas
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.RootDirectory = "/src/views"; // Establece la nueva ruta de las vistas
});

// Configurar el contexto de la base de datos con MySql.EntityFrameworkCore
var connectionString = "server=bqoiit78a116zt8g5ovu-mysql.services.clever-cloud.com;port=3306;database=bqoiit78a116zt8g5ovu;user=uaarc4dw9qk5anxf;password=v9NDoyB9DEaRNsY9U71B";
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));

// Registrar el servicio de base de datos
builder.Services.AddSingleton<DatabaseService>(provider =>
    new DatabaseService(connectionString));

// Registrar el repositorio y el servicio
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();


var app = builder.Build();

// Configurar el servidor para que escuche en Railway/Heroku
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

// Middleware para manejar solicitudes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSession();
app.UseStaticFiles(); // Para archivos en wwwroot
app.UseRouting(); // Habilita enrutamiento
app.UseAuthorization(); // Se activará cuando se implemente autenticación
// Mapear rutas
app.MapControllers(); // API Controllers
app.MapRazorPages(); // Razor Pages

app.Run();