using System.Net;
using System.Text.Json;
using CompraCerca.API.DTOs;

namespace CompraCerca.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pasa la petición al siguiente componente del pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Registra el error completo en la consola/logs del servidor
                _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);

                // Maneja la respuesta HTTP hacia el cliente
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // En entorno de Desarrollo (Development) mostramos el detalle del error.
            // En Producción ocultamos el detalle técnico para proteger el servidor.
            var response = new ErrorResponseDto
            {
                StatusCode = context.Response.StatusCode,
                Message = "Ocurrió un error interno en el servidor.",
                Details = _env.IsDevelopment() ? exception.Message : null
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(json);
        }
    }
}