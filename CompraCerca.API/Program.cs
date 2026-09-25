using System.Text;
using CompraCerca.API.Data;
using CompraCerca.API.Interfaces;
using CompraCerca.API.Middlewares;
using CompraCerca.API.Repositories;
using CompraCerca.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// 1. CADENA DE CONEXIÓN A SQL SERVER
// =====================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
}

builder.Services.AddDbContext<CompraCercaDbContext>(options =>
    options.UseSqlServer(connectionString));

// =====================================================
// 2. CONFIGURACIÓN DE AUTENTICACIÓN JWT
// =====================================================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// =====================================================
// 3. REPOSITORIOS (INYECCIÓN DE DEPENDENCIAS)
// =====================================================
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// =====================================================
// 4. SERVICIOS (INYECCIÓN DE DEPENDENCIAS)
// =====================================================
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// =====================================================
// 5. CONTROLADORES
// =====================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// =====================================================
// 6. SWAGGER + JWT BEARER
// =====================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CompraCerca API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingresa únicamente el Token JWT obtenido en el Login.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// =====================================================
// 7. CONSTRUIR APLICACIÓN
// =====================================================
var app = builder.Build();

// =====================================================
// 8. MIDDLEWARE GLOBAL DE EXCEPCIONES
// =====================================================
app.UseMiddleware<ExceptionMiddleware>();

// =====================================================
// 9. PIPELINE Y SWAGGER
// =====================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// =====================================================
// 10. SEGURIDAD (AUTHENTICATION ANTES DE AUTHORIZATION)
// =====================================================
app.UseAuthentication();
app.UseAuthorization();

// =====================================================
// 11. MAPEO DE RUTAS Y EJECUCIÓN
// =====================================================
app.MapControllers();

app.Run();