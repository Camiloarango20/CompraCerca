using CompraCerca.API.DTOs;

namespace CompraCerca.API.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto);
        Task<bool> UpdateUserAsync(int id, UserCreateDto userDto);
        Task<bool> DeleteUserAsync(int id);
    }
}