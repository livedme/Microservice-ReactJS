using AutoMapper;
using Azure;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Mango.Services.ShoppingCartAPI.Controllers
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
                IEnumerable<Product> products = dbContext.Products.ToList();
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
                Product product = dbContext.Products.First(x => x.ProductId == id);
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
                Product product = mapper.Map<Product>(productDto);
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
                Product product = mapper.Map<Product>(productDto);
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
                Product product = dbContext.Products.First(x => x.ProductId == id);
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
