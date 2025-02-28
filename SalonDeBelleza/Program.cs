using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

//  Configurar servicios básicos
builder.Services.AddRazorPages(); // Habilita Razor Pages
builder.Services.AddControllers(); // Habilita controladores para APIs
builder.Services.AddEndpointsApiExplorer(); // Habilita documentación de API
builder.Services.AddSwaggerGen(); // Habilita Swagger para pruebas
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.RootDirectory = "/src/views"; // Establece la nueva ruta de las vistas
});

var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "public")),
    RequestPath = ""
});

//  Configurar el servidor para que escuche en Railway/Heroku
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

//  Middleware para manejar solicitudes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // Para archivos en wwwroot
app.UseRouting(); // Habilita enrutamiento
app.UseAuthorization(); // Se activará cuando se implemente autenticación

//Mapear rutas
app.MapControllers(); // API Controllers
app.MapRazorPages(); // Razor Pages

app.Run();
