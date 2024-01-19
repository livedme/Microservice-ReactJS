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
    [Route("api/Cart")]
    [ApiController]
    //[Authorize]
    public class CartAPIController(AppDbContext _dbContext, IMapper _mapper) : ControllerBase
    {
        private readonly AppDbContext dbContext = _dbContext;
        private ResponseDto response = new();
        private IMapper mapper = _mapper;
        [HttpGet("GetCart/{userId}")]        
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new CartDto()
                {
                    CartHeader = mapper.Map<CartHeaderDto>(dbContext.CartHeaders.First(x => x.UserId == userId))
                };

                  cart.CartDetails = mapper.Map<IEnumerable<CartDetailsDto>>(dbContext.CartDetails.Where(u=>u.CartHeaderId==
                  cart.CartHeader.CartHeaderId));

                foreach(var item in cart.CartDetails)
                {
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                response.Result = cart;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;

            }

            return response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeader = await dbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeader == null)
                {
                    CartHeader newCartHeader=mapper.Map<CartHeader>(cartDto.CartHeader);   
                    dbContext.CartHeaders.Add(newCartHeader);
                    await dbContext.SaveChangesAsync();

                    cartDto.CartDetails.First().CartHeaderId =newCartHeader.CartHeaderId;
                    dbContext.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await dbContext.SaveChangesAsync();
                }
                else {
                    var cartdetails =await dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeader.CartHeaderId);

                    if (cartdetails == null)
                    {
                        cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                        dbContext.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        cartDto.CartDetails.First().Count += cartdetails.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartdetails.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId= cartdetails.CartDetailsId;

                        dbContext.CartDetails.Update(mapper.Map<CartDetails>(cartDto.CartDetails.First()));

                        await dbContext.SaveChangesAsync();

                    }
                }

                response.Result = cartDto;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;

            }

            return response;
        }

        [HttpDelete("RemoveCartDetails")]
        public async Task<ResponseDto> RemoveCartDetails(int id)
        {
            try
            {
                CartDetails cartDetails = dbContext.CartDetails.First(u => u.CartDetailsId == id);

                int totalCartItem = dbContext.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                dbContext.CartDetails.Remove(cartDetails);

                if (totalCartItem == 1)
                {
                    var cartHeader=await dbContext.CartHeaders.FirstOrDefaultAsync(u=>u.CartHeaderId==cartDetails.CartHeaderId);
                    if(cartHeader != null)
                    {
                        dbContext.CartHeaders.Remove(cartHeader);
                    }
                }
                await dbContext.SaveChangesAsync();

                response.Result = true;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = e.Message;

            }

            return response;
        }
        //[HttpGet]
        //public ResponseDto Get()
        //{
        //    try
        //    {
        //        IEnumerable<CartHeaderDto> products = dbContext.Products.ToList();
        //        response.Result = mapper.Map<IEnumerable<ProductDto>>(products);
        //    }
        //    catch (Exception e)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = e.Message;
        //    }

        //    return response;
        //}
        //[HttpGet]
        //[Route("{id:int}")]
        //public ResponseDto Get(int id)
        //{
        //    try
        //    {
        //        CartHeaderDto product = dbContext.Products.First(x => x.ProductId == id);
        //        response.Result = mapper.Map<ProductDto>(product);
        //    }
        //    catch (Exception e)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = e.Message;

        //    }

        //    return response;
        //}

        //[HttpPost]
        ////[Authorize(Roles = "Admin")]
        //public ResponseDto Post([FromBody] ProductDto productDto)
        //{
        //    try
        //    {
        //        CartHeaderDto product = mapper.Map<CartHeaderDto>(productDto);
        //        dbContext.Products.Add(product);
        //        dbContext.SaveChanges();

        //        response.Result = mapper.Map<ProductDto>(product);
        //    }
        //    catch (Exception e)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = e.Message;

        //    }

        //    return response;
        //}
        //[HttpPut]
        //// [Authorize(Roles = "Admin")]
        //public ResponseDto Put([FromBody] ProductDto productDto)
        //{
        //    try
        //    {
        //        CartHeaderDto product = mapper.Map<CartHeaderDto>(productDto);
        //        dbContext.Products.Update(product);
        //        dbContext.SaveChanges();

        //        response.Result = mapper.Map<ProductDto>(product);
        //    }
        //    catch (Exception e)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = e.Message;

        //    }

        //    return response;

        //}
        //[HttpDelete]
        //[Route("{id:int}")]
        //// [Authorize(Roles = "Admin")]
        //public ResponseDto Delete(int id)
        //{
        //    try
        //    {
        //        CartHeaderDto product = dbContext.Products.First(x => x.ProductId == id);
        //        dbContext.Products.Remove(product);
        //        dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = e.Message;

        //    }

        //    return response;
        //}
    }
}
