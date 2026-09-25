using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CompraCerca.API.DTOs;
using CompraCerca.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompraCerca.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Products
        // Público: cualquier persona puede consultar productos
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(products);
        }

        // GET: api/Products/5
        // Público
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Products
        // Requiere autenticación
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> PostProduct(
            ProductCreateDto productDto)
        {
            var userId = GetAuthenticatedUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var (product, errorMessage) =
                await _productService.CreateProductAsync(
                    productDto,
                    userId.Value);

            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product!.Id },
                product);
        }

        // PUT: api/Products/5
        // Requiere autenticación
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(
            int id,
            ProductCreateDto productDto)
        {
            var userId = GetAuthenticatedUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");

            var (success, errorMessage) =
                await _productService.UpdateProductAsync(
                    id,
                    productDto,
                    userId.Value,
                    isAdmin);

            if (!success)
            {
                if (errorMessage ==
                    "El producto especificado no existe.")
                {
                    return NotFound();
                }

                if (errorMessage ==
                    "No tienes permiso para modificar este producto.")
                {
                    return Forbid();
                }

                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        // Requiere autenticación
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = GetAuthenticatedUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");

            var (success, errorMessage) =
                await _productService.DeleteProductAsync(
                    id,
                    userId.Value,
                    isAdmin);

            if (!success)
            {
                if (errorMessage ==
                    "El producto especificado no existe.")
                {
                    return NotFound();
                }

                if (errorMessage ==
                    "No tienes permiso para eliminar este producto.")
                {
                    return Forbid();
                }

                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        // Obtener el ID del usuario desde el JWT
        private int? GetAuthenticatedUserId()
        {
            var claim =
                User.FindFirst(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null ||
                !int.TryParse(claim.Value, out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}