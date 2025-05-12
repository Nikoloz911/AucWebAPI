using AucWebAPI.Core;
using AucWebAPI.Models;
using System.Security.Claims;

namespace AucWebAPI.JWT;
public interface IJWTService
{
    Token GetToken(User user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
