using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SalonDeBelleza.src.models; // Asegúrate de que este using apunte a la carpeta donde está tu modelo y DbContext

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios básicos
builder.Services.AddControllers(); // Habilita controladores para APIs
builder.Services.AddEndpointsApiExplorer(); // Habilita documentación de API
builder.Services.AddSwaggerGen(); // Habilita Swagger para pruebas
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.RootDirectory = "/src/views"; // Establece la nueva ruta de las vistas
});

// Configurar el contexto de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21)), // Asegúrate de usar la versión correcta de MySQL
        mySqlOptions => mySqlOptions.EnableRetryOnFailure() // Habilita la resiliencia a errores transitorios
    ));

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