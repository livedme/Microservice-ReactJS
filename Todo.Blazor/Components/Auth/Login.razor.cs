using BlazorApp2.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Todo.Blazor.Helpers;
using Todo.Blazor.Models;
using Todo.Blazor.Service.IService;

namespace Todo.Blazor.Components.Auth
{    
    public class LoginBase : ComponentBase
    {
        [Inject]
        protected NavigationManager navManager { get; set; }
        [Inject]
        protected IAuthService AuthService { get; set; }
       
        [Inject]
        protected AuthenticationStateProvider AuthStateProvider { get; set; }        

        [CascadingParameter]
        protected Task<AuthenticationState> authenticationStateTask { get; set; }
      
        [SupplyParameterFromForm(FormName ="Login")] 
        public LoginRequestDto? model { get; set; }=new LoginRequestDto();
        public EditContext editContext; 
        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
          //  editContext = new(model);
        }
        protected async void LoginUser()
        {
            ResponseDto response = await AuthService.LoginAsync(model);
            if (response != null && response.IsSuccess)
            {
                var loginInfo = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

                if (loginInfo != null)
                {
                    var authProvider = (JWTAuthenticationProvider)AuthStateProvider;
                    await authProvider.UpdateAuthenticationState(loginInfo.Token);

                    //IJSRuntimeExtensions

                    navManager.NavigateTo("/", forceLoad: true);
                }
            }
            else
            {
                //  TempData["error"] = response?.Message;
            }

        }
        protected override void OnAfterRender(bool firstRender)
        {
            //if (firstRender) TranslationSetting.SetText("");
        }
    }
}
