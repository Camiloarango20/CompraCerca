using CompraCerca.API.Data;
using CompraCerca.API.Interfaces;
using CompraCerca.API.Middlewares;
using CompraCerca.API.Repositories;
using CompraCerca.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

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
// 1.1 CONFIGURACIÓN DE CORS (Agregado para Vercel)
// =====================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercelFrontend", policy =>
    {
        policy.WithOrigins(
                "https://compracerca-web.vercel.app", // Tu frontend en Vercel
                "http://localhost:5173",              // Para pruebas locales en Vite
                "http://localhost:3000"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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
// 3. REPOSITORIOS
// =====================================================
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// =====================================================
// 4. SERVICIOS
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
        Description = "Pega solo el token JWT del login.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
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
// 10. SEGURIDAD & CORS
// =====================================================
// IMPORTANTE: UseCors debe ir ANTES de UseAuthentication y UseAuthorization
app.UseCors("AllowVercelFrontend");

app.UseAuthentication();
app.UseAuthorization();

// =====================================================
// 11. RUTAS Y EJECUCIÓN
// =====================================================
app.MapControllers();

app.Run();