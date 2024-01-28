﻿using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IShoppingCartService
    {
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);         
        Task<ResponseDto?> UpSertCartAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto);
        Task<ResponseDto?> EmailCartRequest(CartDto cartDto);
        
    }
}
