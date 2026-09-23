using System.ComponentModel.DataAnnotations;

namespace CompraCerca.API.DTOs
{
    // DTO utilizado para recibir los datos de creación o actualización desde el cliente
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}