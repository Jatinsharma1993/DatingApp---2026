using System;
using API.Enitites;

namespace API.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}
