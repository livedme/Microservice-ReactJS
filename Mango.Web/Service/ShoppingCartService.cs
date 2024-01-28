using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IBaseService baseService;

        public ShoppingCartService(IBaseService _baseService)
        {
            baseService = _baseService;
        }

        public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShoppingCartAPIBaseUrl}/api/cart/ApplyCoupon"
            });
        }


        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,                
                Url = $"{SD.ShoppingCartAPIBaseUrl}/api/cart/GetCartByUserId/{userId}"
            });
        }


        public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.ShoppingCartAPIBaseUrl}/api/cart/RemoveCartDetails/{cartDetailsId}"
            });
        }


        public async Task<ResponseDto?> UpSertCartAsync(CartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShoppingCartAPIBaseUrl}/api/cart/CartUpsert"
            });
        }
        public async Task<ResponseDto?> EmailCartRequest(CartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = $"{SD.ShoppingCartAPIBaseUrl}/api/cart/EmailCartRequest"
            });
        }
    }
}

