using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService(IBaseService _baseService) : IAuthService
    {
        private readonly IBaseService baseService = _baseService;

        public async Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = $"{SD.AuthAPIBaseUrl}/api/auth/Login"
            },withBearer:false);
        }

        public async Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = $"{SD.AuthAPIBaseUrl}/api/auth/Register"
            }, withBearer: false);
        }
        public async Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = $"{SD.AuthAPIBaseUrl}/api/auth/AssignRole"
            });
        }

    }
}

