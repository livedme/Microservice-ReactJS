using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;
using System.Net.Http.Headers;
using Todo.Blazor.Helpers;

namespace Todo.Blazor.Auth
{
    public class JWTAuthenticationProvider: AuthenticationStateProvider, ILoginService
    {
        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;
        private static readonly string TokenKey = "TOKENKEY";
        private AuthenticationState Anonymous => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        public JWTAuthenticationProvider(IJSRuntime js, HttpClient httpClient)
        {
            this.js = js;
            this.httpClient = httpClient;
        
           // js.SetInLocalStorage(TOKENKEY,"");
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await js.GetToken(TokenKey);

                if (string.IsNullOrEmpty(token))
                {
                    return Anonymous;
                }
                return BuildAuthenticationState(token);
            }
            catch
            {
                return Anonymous;
            }
            
        }

        public async Task Login(string token)
        {
            await js.SetToken(TokenKey, token);
            var authState = BuildAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
            await js.RemoveToken(TokenKey);
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(jwt))
            {
                var keys = jwt.Split('.');
                if (keys.Length > 1)
                {
                    var payload = jwt.Split('.')[1];
                    var jsonBytes = ParseBase64WithoutPadding(payload);
                    var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                    keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

                    if (roles != null)
                    {
                        if (roles.ToString().Trim().StartsWith("["))
                        {
                            var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                            foreach (var parsedRole in parsedRoles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                        }

                        keyValuePairs.Remove(ClaimTypes.Role);
                    }

                    claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
                }
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

    }
}
