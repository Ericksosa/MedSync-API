using MedSync_API.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// 1. Cargar .env antes de cualquier configuración
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 2. Configurar servicios básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Necesario para la interfaz visual de Swagger 

// 3. Configuración de MySQL
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La variable DB_CONNECTION no se encontró en el .env");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// 4. Configurar el Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Genera el JSON
    app.UseSwaggerUI(); // Genera la interfaz visual
}

app.UseHttpsRedirection();
app.UseAuthorization(); // Importante para la seguridad

app.MapControllers(); // Esto mapea tus controladores de la carpeta /Controllers 

app.Run();
app.Run();