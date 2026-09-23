using CompraCerca.API.DTOs;
using CompraCerca.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompraCerca.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> PostProduct(ProductCreateDto productDto)
        {
            var (product, errorMessage) = await _productService.CreateProductAsync(productDto);
            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }

            return CreatedAtAction(nameof(GetProduct), new { id = product!.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductCreateDto productDto)
        {
            var (success, errorMessage) = await _productService.UpdateProductAsync(id, productDto);
            if (!success)
            {
                if (errorMessage == "El producto especificado no existe.")
                {
                    return NotFound();
                }
                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}