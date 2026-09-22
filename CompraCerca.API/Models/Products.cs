using System.ComponentModel.DataAnnotations.Schema;

namespace CompraCerca.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Le indicamos explícitamente a SQL Server: 18 dígitos totales, 2 decimales
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}