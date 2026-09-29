using CompraCerca.API.DTOs;

namespace CompraCerca.API.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<(CategoryResponseDto? Category, string? ErrorMessage)> CreateCategoryAsync(CategoryCreateDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateCategoryAsync(int id, CategoryCreateDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}