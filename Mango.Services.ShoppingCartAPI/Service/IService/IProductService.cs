﻿
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductAsync();         
      //  Task<ResponseDto> GetProductByIdAsync(int id);
    }
}