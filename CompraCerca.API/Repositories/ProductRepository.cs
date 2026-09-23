using CompraCerca.API.Data;
using CompraCerca.API.Interfaces;
using CompraCerca.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CompraCerca.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CompraCercaDbContext _context;

        public ProductRepository(CompraCercaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Cargar datos de navegación para el DTO de respuesta
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            await _context.Entry(product).Reference(p => p.User).LoadAsync();

            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}