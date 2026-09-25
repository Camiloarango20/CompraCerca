using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CompraCerca.API.Data;
using CompraCerca.API.DTOs;
using CompraCerca.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CompraCerca.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CompraCercaDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            CompraCercaDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(
            UserCreateDto userDto)
        {
            // 1. Verificar si el correo ya existe
            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == userDto.Email);

            if (existingUser)
            {
                return BadRequest(
                    "El correo electrónico ya está registrado.");
            }

            // 2. Hashear la contraseña
            string passwordHash =
                BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            // 3. Crear el usuario
            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = passwordHash,
                City = userDto.City,

                // Un usuario registrado públicamente siempre será User
                IsActive = true,
                Role = "User"
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            // 4. Crear respuesta
            var responseDto = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                City = user.City,
                IsActive = user.IsActive,
                Role = user.Role
            };

            return CreatedAtAction(
                nameof(UsersController.GetUser),
                "Users",
                new { id = user.Id },
                responseDto);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // 1. Buscar usuario
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            // 2. Verificar si está activo
            if (!user.IsActive)
            {
                return Unauthorized("El usuario está inactivo.");
            }

            // 3. Verificar contraseña
            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    loginDto.Password,
                    user.PasswordHash);

            if (!isPasswordValid)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            // 4. Generar JWT
            string token = GenerateJwtToken(user);

            // 5. Devolver token y datos del usuario
            return Ok(new
            {
                token,

                user = new UserResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    City = user.City,
                    IsActive = user.IsActive,
                    Role = user.Role
                }
            });
        }

        // Generar JWT
        private string GenerateJwtToken(User user)
        {
            var jwtSettings =
                _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSettings["Key"]!));

            var claims = new[]
            {
                // ID del usuario
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()),

                // Correo
                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email),

                // Nombre
                new Claim(
                    "firstName",
                    user.FirstName),

                // Apellido
                new Claim(
                    "lastName",
                    user.LastName),

                // ROL DEL USUARIO
                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}