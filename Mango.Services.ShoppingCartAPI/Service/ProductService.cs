
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ProductService(IHttpClientFactory _httpClientFactory)
        {
            httpClientFactory = _httpClientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductAsync()
        {
            var client = httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent=await response.Content.ReadAsStringAsync();
            var resp=JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDto>();
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

