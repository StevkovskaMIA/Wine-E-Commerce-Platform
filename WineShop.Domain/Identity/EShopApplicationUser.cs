﻿using Microsoft.AspNetCore.Identity;
using WineShop.Domain.DomainModels;

namespace WineShop.Domain.Identity
{
    public class EShopApplicationUser : IdentityUser
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Address { get; set; }

        public virtual ShoppingCart UserCart { get; set; }

        public virtual ICollection<Order> Orders { get; set; }


    }
}
