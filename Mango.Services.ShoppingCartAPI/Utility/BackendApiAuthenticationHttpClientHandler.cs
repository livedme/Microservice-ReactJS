using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Mango.Services.ShoppingCartAPI.Utility
{
    
    public class BackendApiAuthenticationHttpClientHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor= _httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));  
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
