using CompraCerca.API.DTOs;
using CompraCerca.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompraCerca.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories (Público)
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/categories/5 (Público)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // POST: api/categories (Solo Administradores)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> PostCategory(CategoryCreateDto categoryDto)
        {
            var (createdCategory, errorMessage) = await _categoryService.CreateCategoryAsync(categoryDto);

            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }

            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory!.Id }, createdCategory);
        }

        // PUT: api/categories/5 (Solo Administradores)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryCreateDto categoryDto)
        {
            var (success, errorMessage) = await _categoryService.UpdateCategoryAsync(id, categoryDto);

            if (!success)
            {
                if (errorMessage == "La categoría especificada no existe.")
                {
                    return NotFound();
                }

                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        // DELETE: api/categories/5 (Solo Administradores)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}