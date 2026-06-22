using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WineShop.Domain;
using WineShop.Domain.DTO;
using WineShop.Service.Interface;
using WineShop.Services.Interface;


namespace WineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ITastingPackageService _tastingPackageService;


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ITastingPackageService tastingPackageService,IProductService productService)
        {
            _logger = logger;
            _productService = productService;
            _tastingPackageService = tastingPackageService;

        }

        public IActionResult Index()
        {
            var model = new HomePageViewModel
            {
                Products = _productService.GetAllProducts()
                                         .Take(4)
                                         .ToList(),

                TastingPackages = _tastingPackageService.GetAllTastingPackages()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
