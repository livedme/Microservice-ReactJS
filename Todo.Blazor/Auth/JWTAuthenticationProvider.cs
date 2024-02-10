using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Todo.Blazor.Helpers;

namespace BlazorApp2.Models
{
    public class JWTAuthenticationProvider(IJSRuntime _js) : AuthenticationStateProvider
    {
        private readonly IJSRuntime js = _js;
        private readonly ClaimsPrincipal anonymouse = new (new ClaimsIdentity());
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Constants.JWTToken))
                    return await Task.FromResult(new AuthenticationState(anonymouse));

                var getUserClaims = DecrytToken(Constants.JWTToken);
                if (getUserClaims == null)
                    return await Task.FromResult(new AuthenticationState(anonymouse));

                var claimsPrincipal = SetClaimPrincipal(getUserClaims);
                if (claimsPrincipal == null)
                    return await Task.FromResult(new AuthenticationState(anonymouse));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AuthenticationState(anonymouse));
            }
        }

        public async Task UpdateAuthenticationState(string jwtToken)
        {
            var claimsPrincipal = new ClaimsPrincipal();

            if (string.IsNullOrEmpty(jwtToken))
            {
                Constants.JWTToken = null!;
                await js.RemoveToken("");
            }
            else
            {
                await js.SetToken("", jwtToken);

                Constants.JWTToken = jwtToken;
                var getUserClaims = DecrytToken(Constants.JWTToken);
                claimsPrincipal = SetClaimPrincipal(getUserClaims);
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
        {
            if (claims.email is null) return new ClaimsPrincipal();

            return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name,claims.name!),
                new(ClaimTypes.Email,claims.email!),
                new(ClaimTypes.NameIdentifier,claims.id!),
                new(ClaimTypes.Role,claims.roles!),
                new(ClaimTypes.GivenName,claims.DisplayName!)
            },"JwtAuth"));
        }
        private static CustomUserClaims DecrytToken(string jwtToken)
        {
            if(string.IsNullOrEmpty(jwtToken)) return new CustomUserClaims();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var name = token.Claims.FirstOrDefault(x => x.Type == "name");
            var email = token.Claims.FirstOrDefault(x => x.Type == "email");
            var displayName = token.Claims.FirstOrDefault(x => x.Type == "given_name");
            var id = token.Claims.FirstOrDefault(x => x.Type == "nameid");
            var roles = token.Claims.FirstOrDefault(x => x.Type == "role");
            return new CustomUserClaims(name!.Value, email!.Value, displayName!.Value,
                id!.Value, roles!.Value);
        }
    }
}
