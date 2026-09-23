using System.ComponentModel.DataAnnotations;

namespace CompraCerca.API.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "El título del producto es obligatorio.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 99999999.99, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un ID de categoría válido.")]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un ID de usuario válido.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public string City { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}