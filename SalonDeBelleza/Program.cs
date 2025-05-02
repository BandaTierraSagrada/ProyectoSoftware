using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalonDeBelleza.src.config;
using SalonDeBelleza.src.Controllers;
using SalonDeBelleza.src.repositories;
using SalonDeBelleza.src.services;


var builder = WebApplication.CreateBuilder(args);
var mySetting = builder.Configuration.GetSection("SMTP");
// Configurar servicios
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.RootDirectory = "/src/views";
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Configurar sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configurar MySQL con Pomelo
var connectionString = "server=bqoiit78a116zt8g5ovu-mysql.services.clever-cloud.com;port=3306;database=bqoiit78a116zt8g5ovu;user=uaarc4dw9qk5anxf;password=v9NDoyB9DEaRNsY9U71B";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Registrar dependencias
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<CitaRepository>();
builder.Services.AddScoped<BotService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<WhatsAppService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");
// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    /*app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); // <-- Configura el endpoint de Swagger
        c.RoutePrefix = string.Empty; // <-- Esto hace que Swagger UI esté disponible en la raíz
    });*/
}



app.UseSession(); // Habilitar sesiones
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();
app.MapRazorPages();

app.Run();
