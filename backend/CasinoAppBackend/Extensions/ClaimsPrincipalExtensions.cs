using System.Security.Claims;

namespace CasinoAppBackend.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the authenticated user's ID (GUID) from the JWT claims.
        /// </summary>
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? throw new Exception("User ID claim is missing.");

            return Guid.Parse(id);
        }
    }
}
