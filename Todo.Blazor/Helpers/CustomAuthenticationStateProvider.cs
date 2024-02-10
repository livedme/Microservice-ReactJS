using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Todo.Blazor.Models;

namespace Todo.Blazor.Helpers
{
    public class CustomAuthenticationStateProvider() : AuthenticationStateProvider
    {
         //LocalStorageSetting localStorageService = new LocalStorageSetting();

        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var stringToken = await LocalStorageSetting.GetTokenAsync();

            if (string.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(anonymous));

            var deserializeToken = Serializations.DeserializeJsonString<LoginResponseDto>(stringToken);
            if (deserializeToken == null) return await Task.FromResult(new AuthenticationState(anonymous));

            return await Task.FromResult(new AuthenticationState(SetClaimsPrincipal(deserializeToken)));

        }
        public async Task UpdateAuthenticationState(LoginResponseDto loginResponseDto)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (loginResponseDto != null && loginResponseDto.Token != null)
            {
                var serializeToken = Serializations.SerializeObj(loginResponseDto);
                await localStorageService.SetTokenAsync(serializeToken);
                claimsPrincipal = SetClaimsPrincipal(loginResponseDto);
            }
            else
            {
                await localStorageService.RemoveTokenAsync();
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

        }

        private ClaimsPrincipal SetClaimsPrincipal(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.NameId,
                 jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));


            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));



            var principal = new ClaimsPrincipal(identity);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return principal;
        }
    }
}
