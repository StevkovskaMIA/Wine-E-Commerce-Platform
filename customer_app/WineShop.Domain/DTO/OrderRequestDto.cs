using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineShop.Domain.DTO
{
    public class OrderRequestDto
    {
        // "pickup" или "delivery"
        public string DeliveryType { get; set; } = "pickup";

        // Само кога DeliveryType == "delivery"
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
    }
}