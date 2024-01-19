using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    // [Authorize(Roles =SD.RoleCustomer)]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        public ProductController(IProductService _productService)
        {
            productService = _productService;
        }
        // GET: CouponController
        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = new();

            ResponseDto?response=await productService.GetAllProductAsync();
            if(response!=null && response.IsSuccess)
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

        
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await productService.CreateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }
        // GET: CouponController/Edit/5
        public async Task<IActionResult> Edit(int id)
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

        // POST: CouponController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ResponseDto? response = await productService.UpdateProductAsync(model);
                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "product updated successfully";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = response?.Message;
                    }
                }

                return View(model);
            }
            catch
            {
                return View();
            }
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
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDto? response = await productService.DeleteProductAsync(id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Deleted get successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return RedirectToAction("Index");
        }

        // POST: CouponController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
