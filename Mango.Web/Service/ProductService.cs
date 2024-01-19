using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService baseService;

        public ProductService(IBaseService _baseService)
        {
            baseService = _baseService;
        }
        public async Task<ResponseDto> CreateProductAsync(ProductDto productDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data= productDto,
                Url = $"{SD.ProductAPIBaseUrl}/api/Product"
            });
        }


        public async Task<ResponseDto> DeleteProductAsync(int id)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.ProductAPIBaseUrl}/api/Product/{id}"
            });
        }

        public async Task<ResponseDto> GetAllProductAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.ProductAPIBaseUrl}/api/Product"
            });
        }


        public async Task<ResponseDto> GetProductByIdAsync(int id)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url =$"{SD.ProductAPIBaseUrl}/api/Product/{id}"
            });
        }


        public async Task<ResponseDto> UpdateProductAsync(ProductDto productDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                Url = $"{SD.ProductAPIBaseUrl}/api/Product"
            });
        }
    }
}

