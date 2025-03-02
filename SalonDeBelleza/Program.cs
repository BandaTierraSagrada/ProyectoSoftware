using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using SalonDeBelleza.src.models;

var builder = WebApplication.CreateBuilder(args);

// Obtener la conexión de la variable de entorno de Heroku
var connectionString = Environment.GetEnvironmentVariable("MYSQL_ADDON_URI") ??
                        "server=bqoiit78a116zt8g5ovu-mysql.services.clever-cloud.com:3306;database=bqoiit78a116zt8g5ovu;user=uaarc4dw9qk5anxf;password=v9NDoyB9DEaRNsY9U71B";

// Reemplazar el esquema `mysql://usuario:password@host:puerto/db` de Clever Cloud
if (connectionString.StartsWith("mysql://"))
{
    var uri = new Uri(connectionString);
    var host = uri.Host;
    var dbport = uri.Port;
    var userInfo = uri.UserInfo.Split(':');
    var user = userInfo[0];
    var password = userInfo[1];
    var database = uri.AbsolutePath.TrimStart('/');

    connectionString = $"server={host};port={dbport};database={database};user={user};password={password}";
}

// Configurar Entity Framework con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Agregar servicios
//builder.Services.AddScoped<UsuariosRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configurar puerto de Heroku
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
