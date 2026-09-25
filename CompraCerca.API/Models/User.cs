using CompraCerca.API.Models;

public class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string Role { get; set; } = "User";

    public ICollection<Product> Products { get; set; } = new List<Product>();



}