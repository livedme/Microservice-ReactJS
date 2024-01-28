
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CouponService(IHttpClientFactory _httpClientFactory)
        {
            httpClientFactory = _httpClientFactory;
        }
        public async Task<CouponDto> GetCouponByCodeAsync(string code)
        {
            var client = httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/Coupon/GetByCode/{code}");
            if (response!=null && response.IsSuccessStatusCode)
            {
                var apiContent = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                if (resp!=null && resp.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
                }
            }
            return new CouponDto();
        }

        //public async Task<ResponseDto> GetProductByIdAsync(int id)
        //{
        //    return await baseService.SendAsync(new RequestDto()
        //    {
        //        ApiType = SD.ApiType.GET,
        //        Url =$"{SD.ProductAPIBaseUrl}/api/Product/{id}"
        //    });
        //}


    }
}

