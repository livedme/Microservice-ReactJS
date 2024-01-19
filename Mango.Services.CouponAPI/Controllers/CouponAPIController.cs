using AutoMapper;
using Azure;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/Coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController(AppDbContext _dbContext, IMapper _mapper) : ControllerBase
    {
        private readonly AppDbContext dbContext = _dbContext;
        private ResponseDto response = new();
        private IMapper mapper = _mapper;

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> coupons = dbContext.Coupons.ToList();
                response.Result = mapper.Map<IEnumerable<CouponDto>>(coupons);
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
        [Authorize(Roles ="Admin")]
        public ResponseDto Get(int id)
        {
            {
                try
                {
                    Coupon obj = dbContext.Coupons.First(x => x.CouponId == id);
                    response.Result = mapper.Map<CouponDto>(obj);
                }
                catch (Exception e)
                {
                    response.IsSuccess = false;
                    response.Message = e.Message;

                }

                return response;
            }
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            {
                try
                {
                    Coupon obj = dbContext.Coupons.First(x => x.CouponCode.ToLower() == code.ToLower());

                    response.Result = mapper.Map<CouponDto>(obj);
                }
                catch (Exception e)
                {
                    response.IsSuccess = false;
                    response.Message = e.Message;

                }

                return response;
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            {
                try
                {
                    Coupon coupon = mapper.Map<Coupon>(couponDto);
                    dbContext.Coupons.Add(coupon);
                    dbContext.SaveChanges();

                    response.Result = mapper.Map<CouponDto>(coupon);
                }
                catch (Exception e)
                {
                    response.IsSuccess = false;
                    response.Message = e.Message;

                }

                return response;
            }
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            {
                try
                {
                    Coupon coupon = mapper.Map<Coupon>(couponDto);
                    dbContext.Coupons.Update(coupon);
                    dbContext.SaveChanges();

                    response.Result = mapper.Map<CouponDto>(coupon);
                }
                catch (Exception e)
                {
                    response.IsSuccess = false;
                    response.Message = e.Message;

                }

                return response;
            }
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon coupon = dbContext.Coupons.First(x => x.CouponId == id);
                dbContext.Coupons.Remove(coupon);
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
