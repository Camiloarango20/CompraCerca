using CompraCerca.API.DTOs;
using CompraCerca.API.Interfaces;
using CompraCerca.API.Models;

namespace CompraCerca.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                City = u.City,
                IsActive = u.IsActive
            });
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                City = user.City,
                IsActive = user.IsActive
            };
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto)
        {
            // Aplicamos hash con BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = passwordHash,
                City = userDto.City,
                IsActive = userDto.IsActive
            };

            var createdUser = await _userRepository.CreateAsync(user);

            return new UserResponseDto
            {
                Id = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email,
                City = createdUser.City,
                IsActive = createdUser.IsActive
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UserCreateDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            user.City = userDto.City;
            user.IsActive = userDto.IsActive;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}