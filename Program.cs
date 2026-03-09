using MedSync_API.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// 1. Cargar .env antes de cualquier configuración
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 2. Configurar servicios básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Necesario para la interfaz visual de Swagger [cite: 54]

// 3. Configuración de MySQL
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La variable DB_CONNECTION no se encontró en el .env");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// 4. Configurar el Middleware (Solo una vez)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Genera el JSON
    app.UseSwaggerUI(); // Genera la interfaz visual [cite: 54]
}

app.UseHttpsRedirection();
app.UseAuthorization(); // Importante para la seguridad [cite: 6]

app.MapControllers(); // Esto mapea tus controladores de la carpeta /Controllers 

// BORRA ESTA LÍNEA: app.MapHospitalEndpoints(); 
// (A menos que estés usando Minimal APIs, pero tú tienes controladores físicos)

app.Run();