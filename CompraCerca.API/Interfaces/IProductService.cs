using CompraCerca.API.DTOs;

namespace CompraCerca.API.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();

        Task<ProductResponseDto?> GetProductByIdAsync(int id);

        Task<(ProductResponseDto? Product, string? ErrorMessage)>
            CreateProductAsync(ProductCreateDto productDto, int userId);

        Task<(bool Success, string? ErrorMessage)>
            UpdateProductAsync(
                int id,
                ProductCreateDto productDto,
                int userId,
                bool isAdmin);

        Task<(bool Success, string? ErrorMessage)>
            DeleteProductAsync(
                int id,
                int userId,
                bool isAdmin);
    }
}