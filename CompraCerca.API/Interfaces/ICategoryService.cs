using CompraCerca.API.DTOs;

namespace CompraCerca.API.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto categoryDto);
        Task<bool> UpdateCategoryAsync(int id, CategoryCreateDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}