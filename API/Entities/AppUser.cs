using API.Entities;

namespace API.Enitites;

public class AppUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? ImageUrl { get; set; }
    public required byte[] PaswwordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
    //Navigation Property
    public Member Member { get; set; } = null!;
}
