using CompraCerca.API.Data;
using CompraCerca.API.DTOs;
using CompraCerca.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompraCerca.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CompraCercaDbContext _context;

        public ProductsController(CompraCercaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    UserId = p.UserId,
                    UserName = $"{p.User.FirstName} {p.User.LastName}",
                    City = p.City,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductResponseDto
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                UserId = product.UserId,
                UserName = $"{product.User.FirstName} {product.User.LastName}",
                City = product.City,
                IsActive = product.IsActive
            };

            return Ok(productDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> PostProduct(ProductCreateDto productDto)
        {
            var category = await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null)
            {
                return BadRequest($"La categoría con ID {productDto.CategoryId} no existe.");
            }

            var user = await _context.Users.FindAsync(productDto.UserId);
            if (user == null)
            {
                return BadRequest($"El usuario con ID {productDto.UserId} no existe.");
            }

            var product = new Product
            {
                Title = productDto.Title,
                Description = productDto.Description,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId,
                UserId = productDto.UserId,
                City = productDto.City,
                IsActive = productDto.IsActive
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var responseDto = new ProductResponseDto
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                UserId = product.UserId,
                UserName = $"{user.FirstName} {user.LastName}",
                City = product.City,
                IsActive = product.IsActive
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductCreateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest($"La categoría con ID {productDto.CategoryId} no existe.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == productDto.UserId);
            if (!userExists)
            {
                return BadRequest($"El usuario con ID {productDto.UserId} no existe.");
            }

            product.Title = productDto.Title;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;
            product.UserId = productDto.UserId;
            product.City = productDto.City;
            product.IsActive = productDto.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}