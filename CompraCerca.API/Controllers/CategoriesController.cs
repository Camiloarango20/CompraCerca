using CompraCerca.API.Data;
using CompraCerca.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompraCerca.API.Controllers
{
    [Route("api/[controller]")] // La ruta base será: api/categories
    [ApiController]            // Habilita comportamientos automáticos de Web API (validación de modelos, etc.)
    public class CategoriesController : ControllerBase
    {
        private readonly CompraCercaDbContext _context;

        // Inyección de dependencias: Le pedimos al framework que nos entregue la instancia de la base de datos
        public CategoriesController(CompraCercaDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        // Obtiene todas las categorías de la base de datos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/categories/5
        // Obtiene una categoría por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(); // Retorna 404 Not Found
            }

            return category; // Retorna 200 OK con la categoría
        }

        // POST: api/categories
        // Crea una nueva categoría
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Retorna un código 201 Created y la url donde se puede consultar el recurso creado
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
    }
}