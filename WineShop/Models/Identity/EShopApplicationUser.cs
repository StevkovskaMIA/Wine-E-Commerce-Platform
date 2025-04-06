using Microsoft.AspNetCore.Identity;
using WineShop.Models.Domain;

namespace WineShop.Models.Identity
{
    public class EShopApplicationUser : IdentityUser
    {

        public String? FirstName { get; set; }
        public String? LastName { get; set; }

        public String? Address { get; set; }

        public virtual ShoppingCart UserCart { get; set; }



    }
}
