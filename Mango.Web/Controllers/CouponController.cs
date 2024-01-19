using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
   // [Authorize(Roles =SD.RoleCustomer)]
    public class CouponController : Controller
    {
        private readonly ICouponService couponService;

        public CouponController(ICouponService _couponService)
        {
            couponService= _couponService;
        }
        // GET: CouponController
        public async Task<IActionResult> Index()
        {
            List<CouponDto>? list = new();

            ResponseDto?response=await couponService.GetAllCouponAsync();
            if(response!=null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
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
        public async Task<IActionResult> Create(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await couponService.CreateCouponAsync(model);
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CouponController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
        // GET: CouponController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }



        // GET: CouponController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDto? response = await couponService.DeleteCouponAsync(id);
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
