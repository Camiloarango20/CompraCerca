using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CompraCerca.API.DTOs;
using CompraCerca.API.Interfaces;
using CompraCerca.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace CompraCerca.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<(AuthResponseDto? Response, string? ErrorMessage)> RegisterAsync(UserCreateDto userDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return (null, "El correo electrónico ya se encuentra registrado.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = passwordHash,
                City = userDto.City,
                IsActive = userDto.IsActive,
                // Si no se especifica rol en el DTO, asigna "User" por defecto
                Role = string.IsNullOrWhiteSpace(userDto.Role) ? "User" : userDto.Role
            };

            var createdUser = await _userRepository.CreateAsync(user);
            string token = GenerateJwtToken(createdUser);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = createdUser.Email,
                UserName = $"{createdUser.FirstName} {createdUser.LastName}",
                Role = createdUser.Role
            };

            return (response, null);
        }

        public async Task<(AuthResponseDto? Response, string? ErrorMessage)> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return (null, "Credenciales inválidas.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return (null, "Credenciales inválidas.");
            }

            string token = GenerateJwtToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                UserName = $"{user.FirstName} {user.LastName}",
                Role = user.Role
            };

            return (response, null);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                // CLAIM ESENCIAL PARA EL CONTROL DE ROLES EN ASP.NET CORE:
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}