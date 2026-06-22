using Microsoft.AspNetCore.Identity;

namespace EShopAdminApplication.Models
{
    public class EShopApplicationUser
    {

        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }

    
    }
}
