using System;
using System.Security.Claims;

namespace API.Extentions;

public static class ClaimPrincipleExtension
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get user from token");
        return username;
    }
}
