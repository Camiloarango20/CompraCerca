using CompraCerca.API.DTOs;

namespace CompraCerca.API.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<(ProductResponseDto? Product, string? ErrorMessage)> CreateProductAsync(ProductCreateDto productDto);
        Task<(bool Success, string? ErrorMessage)> UpdateProductAsync(int id, ProductCreateDto productDto);
        Task<bool> DeleteProductAsync(int id);
    }
}