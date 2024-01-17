using AutoMapper;
using Azure;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
