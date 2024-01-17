using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController(IAuthService _authService) : ControllerBase
    {
        private readonly IAuthService authService = _authService;
        private readonly ResponseDto response = new();

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationRequestDto model)
        {
            var result = await authService.Register(model);
            if(!string.IsNullOrEmpty(result))
            {
                response.IsSuccess = false;
                response.Message = result;

                return BadRequest(response);
            }

            return Ok(response);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            var loginResponse = await authService.Login(model);
            if (loginResponse.User == null)
            {
                response.IsSuccess = false;
                response.Message = "User or password is incorrect";
                return BadRequest(response);
            }
            response.Result= loginResponse;

            return Ok(response);
        }
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                response.IsSuccess = false;
                response.Message = "Error Encountered";
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
