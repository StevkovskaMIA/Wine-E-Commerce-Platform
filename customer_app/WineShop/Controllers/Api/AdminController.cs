using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WineShop.Domain.DomainModels;
using WineShop.Domain.Identity;
using WineShop.Services.Interface;

namespace WineShop.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<EShopApplicationUser> userManager;

        public AdminController(IOrderService orderService, UserManager<EShopApplicationUser> userManager)
        {
            this._orderService = orderService;
            this.userManager = userManager;
        }


        [HttpGet("[action]")]
        
        public List<Order> GetAllActiveOrders()
        {
            return this._orderService.GetAllOrders();
        }


        [HttpPost("[action]")]

        public Order GetDetailsForOrder(BaseEntity model)
        {
            return this._orderService.GetOrderDetails(model);
        }


        [HttpPost("[action]")]
        public bool ImportAllUsers(List<UserRegistrationDto> model)
        {
            bool status = true;
            foreach (var item in model) {

                var userCheck = userManager.FindByEmailAsync(item.Email).Result;
                if (userCheck == null)
                {
                    var user = new EShopApplicationUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var result = userManager.CreateAsync(user, item.Password).Result;
                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }
            return status;
        }
    }
}
