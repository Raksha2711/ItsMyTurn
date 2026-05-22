using System.Security.Claims;

namespace srv.slots.api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Returns the customer id from the JWT 'sub' claim. Throws if missing.</summary>
    public static uint GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? user.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(id) || !uint.TryParse(id, out var parsed))
            throw new UnauthorizedAccessException("User id claim missing or invalid.");
        return parsed;
    }
}
