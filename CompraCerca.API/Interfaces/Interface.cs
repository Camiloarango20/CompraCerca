using CompraCerca.API.DTOs;

namespace CompraCerca.API.Interfaces
{
    public interface IAuthService
    {
        Task<(AuthResponseDto? Response, string? ErrorMessage)> RegisterAsync(UserCreateDto userDto);
        Task<(AuthResponseDto? Response, string? ErrorMessage)> LoginAsync(UserLoginDto loginDto);
    }
}