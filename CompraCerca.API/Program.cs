using CompraCerca.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar CompraCercaDbContext en el contenedor de Inyección de Dependencias
builder.Services.AddDbContext<CompraCercaDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Registrar los controladores
builder.Services.AddControllers();

// 4. Registrar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();