
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponByCodeAsync(string code);         
    }
}
