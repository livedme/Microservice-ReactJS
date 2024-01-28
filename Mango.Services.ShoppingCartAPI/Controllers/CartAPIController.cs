using AutoMapper;
using Azure;
using Mango.MessageBus;
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/Cart")]
    [ApiController]
    [Authorize]
    public class CartAPIController(AppDbContext _dbContext, IMapper _mapper, IProductService _productService, ICouponService _couponService, IMessageBug _messageBug, IConfiguration _configuration) : ControllerBase
    {
        private readonly AppDbContext dbContext = _dbContext;
        private readonly IProductService productService = _productService;
        private readonly ICouponService couponService = _couponService;
        private readonly IMessageBug messageBug = _messageBug;
        private readonly IConfiguration configuration= _configuration;

        private ResponseDto response = new();
        private IMapper mapper = _mapper;
        
        [HttpGet("GetCartByUserId/{userId}")]        
        public async Task<ResponseDto> GetCartByUserId(string userId)
        {
            try
            {
                CartDto cart = new CartDto()
                {
                    CartHeader = mapper.Map<CartHeaderDto>(dbContext.CartHeaders.First(x => x.UserId == userId))
                };

                  cart.CartDetails = mapper.Map<IEnumerable<CartDetailsDto>>(dbContext.CartDetails.Where(u=>u.CartHeaderId==
                  cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await productService.GetAllProductAsync();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(p => p.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * (item.Product==null?0: item.Product.Price));
                }

                // apply coupon if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto couponDto = await couponService.GetCouponByCodeAsync(cart.CartHeader.CouponCode);
                    if (couponDto != null && cart.CartHeader.CartTotal > couponDto.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= couponDto.DiscountAmount;
                        cart.CartHeader.Discount = couponDto.DiscountAmount;
                    }
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

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon(CartDto cartDto)
        {
            try
            {
                var cartHeader = await dbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartHeader.CouponCode = cartDto.CartHeader.CouponCode;
                dbContext.CartHeaders.Update(cartHeader);
                await dbContext.SaveChangesAsync();
                response.Result = true;
            }
            catch (Exception e)
            {
                response.Result = false;
                response.IsSuccess = false;
                response.Message = e.Message;
            }

            return response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon(CartDto cartDto)
        {
            try
            {
                var cartHeader = await dbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartHeader.CouponCode = "";
                dbContext.CartHeaders.Update(cartHeader);
                await dbContext.SaveChangesAsync();
                response.Result = true;
            }
            catch (Exception e)
            {
                response.Result = false;
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

        [HttpDelete]
        [Route("RemoveCartDetails/{id:int}")]
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

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest(CartDto cartDto)
        {
            try
            {
                await messageBug.PublishMessage(cartDto, configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                response.Result = true;
            }
            catch (Exception e)
            {
                response.Result = false;
                response.IsSuccess = false;
                response.Message = e.Message;
            }

            return response;
        }
    }
}
