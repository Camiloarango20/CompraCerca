using System.Text.Json;
using CompraCerca.API.DTOs;
using Xunit;

namespace CompraCerca.API.Tests;

public class AuthResponseContractTests
{
    [Fact]
    public void AuthResponseDto_ExposesExpiration()
    {
        var property = typeof(AuthResponseDto).GetProperty("Expiration");

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property!.PropertyType);
    }

    [Fact]
    public void AuthResponseDto_SerializesExpirationInWebCamelCase()
    {
        var dto = new AuthResponseDto
        {
            Token = "token",
            Email = "user@example.com",
            UserName = "Test User",
            Role = "User"
        };

        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.Contains("\"expiration\"", json);
    }

    [Fact]
    public void AuthResponseDto_KeepsExistingFields()
    {
        foreach (var name in new[] { "Token", "Email", "UserName", "Role" })
        {
            Assert.NotNull(typeof(AuthResponseDto).GetProperty(name));
        }
    }
}
