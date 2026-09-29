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

        public async Task<(CategoryResponseDto? Category, string? ErrorMessage)> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            bool exists = await _categoryRepository.ExistsByNameAsync(categoryDto.Name);
            if (exists)
            {
                return (null, $"Ya existe una categoría con el nombre '{categoryDto.Name}'.");
            }

            var category = new Category
            {
                Name = categoryDto.Name.Trim(),
                Description = categoryDto.Description,
                IsActive = categoryDto.IsActive
            };

            var createdCategory = await _categoryRepository.CreateAsync(category);

            var response = new CategoryResponseDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name,
                Description = createdCategory.Description,
                IsActive = createdCategory.IsActive
            };

            return (response, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateCategoryAsync(int id, CategoryCreateDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return (false, "La categoría especificada no existe.");
            }

            // Si se cambia el nombre, verificar que no colisione con otra categoría existente
            if (!category.Name.Equals(categoryDto.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                bool exists = await _categoryRepository.ExistsByNameAsync(categoryDto.Name);
                if (exists)
                {
                    return (false, $"Ya existe otra categoría con el nombre '{categoryDto.Name}'.");
                }
            }

            category.Name = categoryDto.Name.Trim();
            category.Description = categoryDto.Description;
            category.IsActive = categoryDto.IsActive;

            await _categoryRepository.UpdateAsync(category);
            return (true, null);
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