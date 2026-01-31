using System.Security.Claims;
using CasinoAppBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace CasinoAppBackend.Controllers
{
    // [Produces("application/json")] default since it derives from ControllerBase
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private ApplicationUser? _appUser;

        protected ApplicationUser? AppUser
        {
            get
            {
                if (User != null && User.Claims != null && User.Claims.Any())
                {

                    var claimsTypes = User.Claims.Select(x => x.Type);
                    if (!claimsTypes.Contains(ClaimTypes.NameIdentifier))
                    {
                        return null;
                    }

                    // User is an instance of ClaimsPrincipal
                    var userClaimsId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    _ = int.TryParse(userClaimsId, out int id);

                    _appUser = new ApplicationUser
                    {
                        Id = id,
                        Username = User.FindFirst(ClaimTypes.Name)?.Value,
                        Email = User.FindFirst(ClaimTypes.Email)?.Value,
                        Role = User.FindFirst(ClaimTypes.Role)?.Value
                    };
                    return _appUser;
                }
                return null;
            }
        }
    }
}
