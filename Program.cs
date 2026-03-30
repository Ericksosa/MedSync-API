using System.Text;
using DotNetEnv;
using MedSync_API.Data;
using MedSync_API.Models;
using MedSync_API.Repositories;
using MedSync_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// ─── 1. Cargar variables de entorno desde .env ────────────────────────────────
// Debe cargarse antes de cualquier configuración para que JWT_SECRET y DB_CONNECTION
// estén disponibles en IConfiguration.
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ─── 2. Servicios básicos de la API ──────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ─── 3. Swagger con soporte para JWT (RNF04) ─────────────────────────────────
// Permite enviar el token Bearer directamente desde la interfaz de Swagger.
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "MedSync API",
        Version = "v1",
        Description = "API REST para gestión de citas médicas, expedientes y pagos."
    });

    // Define el esquema de seguridad Bearer para que Swagger muestre el botón 'Authorize'
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Ingrese el token JWT con el prefijo 'Bearer '. Ejemplo: Bearer eyJhbG..."
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ─── 4. Configuración de MySQL con Entity Framework Core ──────────────────────
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("La variable DB_CONNECTION no se encontró en el .env");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ─── 5. ASP.NET Identity (gestión de usuarios y roles, RNF06) ────────────────
// Identity proporciona hash de contraseñas con BCrypt, gestión de roles
// y las tablas AspNetUsers, AspNetRoles, AspNetUserRoles en la base de datos.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opciones =>
{
    // Requisitos mínimos de contraseña para cumplir con la política de seguridad
    opciones.Password.RequireDigit           = true;
    opciones.Password.RequiredLength         = 8;
    opciones.Password.RequireUppercase       = true;
    opciones.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ─── 6. Autenticación JWT Bearer (RNF04, RNF05) ───────────────────────────────
// Todas las rutas con [Authorize] verifican el token JWT en el encabezado Authorization.
var jwtSecret = builder.Configuration["JWT:Secret"]
    ?? throw new InvalidOperationException("JWT:Secret no está configurado en appsettings.json o .env");

builder.Services.AddAuthentication(opciones =>
{
    opciones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opciones.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opciones =>
{
    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = builder.Configuration["JWT:Issuer"],
        ValidAudience            = builder.Configuration["JWT:Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

// ─── 7. Registro de Repositorios (inyección de dependencias) ─────────────────
// Los repositorios son la única capa que accede al ApplicationDbContext.
builder.Services.AddScoped<IHospitalRepository,           HospitalRepository>();
builder.Services.AddScoped<IDoctorRepository,             DoctorRepository>();
builder.Services.AddScoped<IPatientRepository,            PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository,        AppointmentRepository>();
builder.Services.AddScoped<IPaymentRepository,            PaymentRepository>();
builder.Services.AddScoped<ISpecialtyRepository,          SpecialtyRepository>();
builder.Services.AddScoped<IMedicalRecordRepository,      MedicalRecordRepository>();
builder.Services.AddScoped<IDiagnosisRepository,          DiagnosisRepository>();
builder.Services.AddScoped<IPrescriptionRepository,       PrescriptionRepository>();
builder.Services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();

// ─── 8. Registro de Servicios (inyección de dependencias) ────────────────────
// Los servicios encapsulan toda la lógica de negocio y son inyectados en los controladores.
builder.Services.AddScoped<IHospitalService,      HospitalService>();
builder.Services.AddScoped<IDoctorService,        DoctorService>();
builder.Services.AddScoped<IPatientService,       PatientService>();
builder.Services.AddScoped<IAppointmentService,   AppointmentService>();
builder.Services.AddScoped<IPaymentService,       PaymentService>();
builder.Services.AddScoped<ISpecialtyService,     SpecialtyService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IReportService,        ReportService>();
builder.Services.AddScoped<IAuthService,          AuthService>();

// ─── 9. Construcción de la aplicación ────────────────────────────────────────
var app = builder.Build();

// ─── 10. Migración automática, seed de roles y usuario administrador ─────────
using (var scope = app.Services.CreateScope())
{
    var db          = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Aplica las migraciones pendientes en la base de datos
    db.Database.Migrate();

    // Crea los roles base del sistema si aún no existen (RF02)
    string[] roles = ["Paciente", "Doctor", "Administrador"];
    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
            await roleManager.CreateAsync(new IdentityRole(rol));
    }

    // Seed del primer usuario Administrador si no existe ninguno.
    // Credenciales leídas desde .env: ADMIN_EMAIL y ADMIN_PASSWORD.
    // Si las variables no están definidas, se usan valores por defecto de desarrollo.
    var adminEmail    = Environment.GetEnvironmentVariable("ADMIN_EMAIL")    ?? "admin@medsync.com";
    var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Admin1234!";

    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email    = adminEmail,
            Nombre   = "Admin",
            Apellido = "Sistema"
        };
        var resultado = await userManager.CreateAsync(adminUser, adminPassword);
        if (resultado.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Administrador");
    }
}

// ─── 11. Pipeline de Middleware ───────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    // Swagger solo disponible en entorno de desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// El orden es crítico: Authentication debe ir antes que Authorization
app.UseAuthentication();
app.UseAuthorization();

// Mapea todos los controladores de la carpeta /Controllers
app.MapControllers();

app.Run();
