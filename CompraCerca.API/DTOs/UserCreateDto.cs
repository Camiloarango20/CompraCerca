
using System.ComponentModel.DataAnnotations;

namespace CompraCerca.API.DTOs
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(12, ErrorMessage = "La contraseña debe tener al menos 12 caracteres.")]
        [StringLength(72, ErrorMessage = "La contraseña no puede superar 72 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
    }
}
