using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService baseService;

        public CouponService(IBaseService _baseService)
        {
            baseService = _baseService;
        }
        public async Task<ResponseDto> CreateCouponAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data=couponDto,
                Url = $"{SD.CouponAPIBaseUrl}/api/CouponAPI"
            });
        }

        public async Task<ResponseDto> DeleteCouponAsync(int id)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.CouponAPIBaseUrl}/api/CouponAPI/{id}"
            });
        }

        public async Task<ResponseDto> GetAllCouponAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CouponAPIBaseUrl}/api/CouponAPI"
            });
        }

        public async Task<ResponseDto> GetCouponAsync(string code)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.CouponAPIBaseUrl}/api/CouponAPI/GetByCode/{code}"
            });
        }

        public async Task<ResponseDto> GetCouponByIdAsync(int id)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url =$"{SD.CouponAPIBaseUrl}/api/coupon/{id}"
            });
        }

        public async Task<ResponseDto> UpdateCouponAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = couponDto,
                Url = $"{SD.CouponAPIBaseUrl}/api/coupon"
            });
        }
    }
}

