namespace API.Enitites;

public class AppUser
{
    public int Id { get; set; }

    public required string UserName { get; set; }

    public required byte[] PaswwordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
}
