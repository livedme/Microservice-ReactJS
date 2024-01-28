using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IProductService _productService, IShoppingCartService _shoppingCartService) : Controller
    {
        private readonly IProductService productService = _productService;
        private readonly IShoppingCartService shoppingCartService = _shoppingCartService;
        private readonly ILogger<HomeController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = new();

            ResponseDto? response = await productService.GetAllProductAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
                // TempData["success"] = "Data get successfully";
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }
        // GET: CouponController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ResponseDto? response = await productService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }



        // GET: CouponController/Delete/5
        [HttpPost]
        public async Task<IActionResult> Details(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetailsDto = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId
            };
            cartDto.CartDetails = new List<CartDetailsDto>()
            {
                cartDetailsDto
            };


            ResponseDto? response = await shoppingCartService.UpSertCartAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added to shopping cart";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return RedirectToAction("Index");
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
