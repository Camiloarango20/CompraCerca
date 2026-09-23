namespace CompraCerca.API.DTOs
{
    // DTO utilizado para responder al cliente con la información limpia
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}