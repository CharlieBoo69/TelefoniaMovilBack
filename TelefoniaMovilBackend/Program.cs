using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TelefoniaMovilBackend.Data;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);

//Configura la cadena de conexión MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configura la cadena de conexión SQLSERVER
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//options.UseSqlServer(connectionString));



// Configura la autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

            // Mapea NameIdentifier para que ASP.NET Core lo reconozca
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

// Configura sesiones

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true;                // Seguridad: cookies accesibles solo por HTTP(TRUE)
    options.Cookie.IsEssential = true;             // Esencial para el funcionamiento
});

// Agrega el servicio de autorización y define la política para el rol de administrador
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("role", "admin"));
});

builder.Services.AddControllers();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.WithOrigins("http://localhost:8080") // Cambia esto por la URL de tu frontend...https://funny-chaja-cfbdb1.netlify.app
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()); //  permite enviar cookies
});

    

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin");


app.UseRouting();

// Habilita sesiones
app.UseSession();

// Habilita autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapea los endpoints solo para controladores
app.MapControllers();

app.Run();
