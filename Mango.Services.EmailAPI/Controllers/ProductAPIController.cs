using AutoMapper;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.EmailAPI.Controllers
{
    [Route("api/Product")]
    [ApiController]
    //[Authorize]
    public class ProductAPIController(AppDbContext _dbContext, IMapper _mapper) : ControllerBase
    {
        private readonly AppDbContext dbContext = _dbContext;
        private ResponseDto response = new();
        private IMapper mapper = _mapper;

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<EmailLogger> products = dbContext.Products.ToList();
                response.Result = mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;
            }

            return response;
        }
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                EmailLogger product = dbContext.Products.First(x => x.ProductId == id);
                response.Result = mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;

            }

            return response;
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ResponseDto Post([FromBody] ProductDto productDto)
        {
            try
            {
                EmailLogger product = mapper.Map<EmailLogger>(productDto);
                dbContext.Products.Add(product);
                dbContext.SaveChanges();

                response.Result = mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;

            }

            return response;
        }
        [HttpPut]
        // [Authorize(Roles = "Admin")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                EmailLogger product = mapper.Map<EmailLogger>(productDto);
                dbContext.Products.Update(product);
                dbContext.SaveChanges();

                response.Result = mapper.Map<ProductDto>(product);
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;

            }

            return response;

        }
        [HttpDelete]
        [Route("{id:int}")]
        // [Authorize(Roles = "Admin")]
        public ResponseDto Delete(int id)
        {
            try
            {
                EmailLogger product = dbContext.Products.First(x => x.ProductId == id);
                dbContext.Products.Remove(product);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;

            }

            return response;
        }
    }
}
