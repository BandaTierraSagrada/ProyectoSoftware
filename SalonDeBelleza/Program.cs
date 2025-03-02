using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.services;


var builder = WebApplication.CreateBuilder(args);

// Configurar servicios básicos
builder.Services.AddControllers(); // Habilita controladores para APIs
builder.Services.AddEndpointsApiExplorer(); // Habilita documentación de API
builder.Services.AddSwaggerGen(); // Habilita Swagger para pruebas
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.RootDirectory = "/src/views"; // Establece la nueva ruta de las vistas
});

// Configurar el contexto de la base de datos con MySql.EntityFrameworkCore
var connectionString = "server=j6jbbxae5c8vg4m1.cbetxkdyhwsb.us-east-1.rds.amazonaws.com;port=3306;database=r7ul5s0h1znsh05k;user=b5p1cxxztq4ff0c5;password=u8xnmhd7bdge29t5";
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));

// Registrar el servicio de base de datos
builder.Services.AddSingleton<DatabaseService>(provider =>
    new DatabaseService(connectionString));

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

app.UseStaticFiles(); // Para archivos en wwwroot
app.UseRouting(); // Habilita enrutamiento
app.UseAuthorization(); // Se activará cuando se implemente autenticación

// Mapear rutas
app.MapControllers(); // API Controllers
app.MapRazorPages(); // Razor Pages

app.Run();