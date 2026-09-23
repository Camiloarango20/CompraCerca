using CompraCerca.API.DTOs;
using CompraCerca.API.Interfaces;
using CompraCerca.API.Models;

namespace CompraCerca.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => MapToResponseDto(p));
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return MapToResponseDto(product);
        }

        public async Task<(ProductResponseDto? Product, string? ErrorMessage)> CreateProductAsync(ProductCreateDto productDto)
        {
            var categoryExists = await _categoryRepository.ExistsAsync(productDto.CategoryId);
            if (!categoryExists)
            {
                return (null, $"La categoría con ID {productDto.CategoryId} no existe.");
            }

            var userExists = await _userRepository.ExistsAsync(productDto.UserId);
            if (!userExists)
            {
                return (null, $"El usuario con ID {productDto.UserId} no existe.");
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

            var createdProduct = await _productRepository.CreateAsync(product);
            return (MapToResponseDto(createdProduct), null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateProductAsync(int id, ProductCreateDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return (false, "El producto especificado no existe.");
            }

            var categoryExists = await _categoryRepository.ExistsAsync(productDto.CategoryId);
            if (!categoryExists)
            {
                return (false, $"La categoría con ID {productDto.CategoryId} no existe.");
            }

            var userExists = await _userRepository.ExistsAsync(productDto.UserId);
            if (!userExists)
            {
                return (false, $"El usuario con ID {productDto.UserId} no existe.");
            }

            product.Title = productDto.Title;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;
            product.UserId = productDto.UserId;
            product.City = productDto.City;
            product.IsActive = productDto.IsActive;

            await _productRepository.UpdateAsync(product);
            return (true, null);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            await _productRepository.DeleteAsync(product);
            return true;
        }

        private static ProductResponseDto MapToResponseDto(Product p)
        {
            return new ProductResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty,
                UserId = p.UserId,
                UserName = p.User != null ? $"{p.User.FirstName} {p.User.LastName}" : string.Empty,
                City = p.City,
                IsActive = p.IsActive
            };
        }
    }
}