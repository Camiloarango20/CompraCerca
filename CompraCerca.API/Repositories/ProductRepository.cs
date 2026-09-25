using CompraCerca.API.Data;
using CompraCerca.API.DTOs;
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

        public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsQueryable();

            // 1. Aplicar filtros dinámicos si se especifican
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string search = filter.Search.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(search) ||
                                         p.Description.ToLower().Contains(search));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            // 2. Contar el total de registros que coinciden con los filtros
            int totalCount = await query.CountAsync();

            // 3. Aplicar paginación (Skip y Take)
            var items = await query
                .OrderByDescending(p => p.Id) // Se ordena por Id en lugar de CreatedAt
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}