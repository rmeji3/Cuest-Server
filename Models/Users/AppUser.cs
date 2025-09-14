using Microsoft.AspNetCore.Identity;

namespace Cuest.Models.AppUsers
{
    public class AppUser : IdentityUser
    {
        public string? DisplayName { get; set; }
    }
}
