using CompraCerca.API.DTOs;
using CompraCerca.API.Interfaces;
using CompraCerca.API.Models;

namespace CompraCerca.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive
            });
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                IsActive = categoryDto.IsActive
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);

            return new CategoryResponseDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name,
                Description = createdCategory.Description,
                IsActive = createdCategory.IsActive
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryCreateDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            category.IsActive = categoryDto.IsActive;

            await _categoryRepository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            await _categoryRepository.DeleteAsync(category);
            return true;
        }
    }
}