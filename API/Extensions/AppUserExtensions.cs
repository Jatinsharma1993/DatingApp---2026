using API.DTOs;
using API.Enitites;
using API.Interfaces;

namespace API.Extentions;

public static class AppUserExtensions
{
    public static UserDto ToDto(this AppUser user, ITokenService tokenService)
    {
        return new UserDto
        {
            Username = user.UserName,
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };
    }
}

