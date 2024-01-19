using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;
        private readonly ITokenProvider tokenProvider;
        public AuthController(IAuthService _authService, ITokenProvider _tokenProvider)
        {
            authService= _authService;
            tokenProvider= _tokenProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginRequestDto loginRequestDto = new();

            return View(loginRequestDto);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto responseDto = await authService.LoginAsync(model);
                if (responseDto != null && responseDto.IsSuccess)
                {

                    LoginResponseDto loginResponseDto = 
                        JsonConvert.DeserializeObject< LoginResponseDto >(Convert.ToString(responseDto.Result));

                    await SignInUser(loginResponseDto);

                    tokenProvider.SetToken(loginResponseDto.Token);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = responseDto.Message;                    
                    return View(model);
                }
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roleList = new List<SelectListItem>() {
                new() {Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new() {Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;

            RegistrationRequestDto model = new();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto responseDto = await authService.RegisterAsync(model);
                ResponseDto assignRole;
                if (responseDto != null && responseDto.IsSuccess)
                {
                    if (string.IsNullOrEmpty(model.Role))
                    {
                        model.Role = SD.RoleCustomer;
                    }
                    assignRole = await authService.AssignRoleAsync(model);
                    if (assignRole != null && assignRole.IsSuccess)
                    {
                        TempData["success"] = "Registration Successful";
                        return RedirectToAction("Login");
                    }                    
                }
                else
                {
                    TempData["error"] = responseDto.Message;
                    return View(model);
                }
            }

            var roleList = new List<SelectListItem>() {
                new() {Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new() {Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            tokenProvider.ClearToken();

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task SignInUser(LoginResponseDto model)
        {
            var handler=new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);
            if (jwt != null)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                    jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));

                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                  jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value));
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                  jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name).Value));
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.FamilyName,
                  jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName).Value));

                identity.AddClaim(new Claim(ClaimTypes.Name,
                 jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));

                identity.AddClaim(new Claim(ClaimTypes.Role,
               jwt.Claims.FirstOrDefault(c => c.Type == "role").Value));

                var principal = new ClaimsPrincipal(identity);

                //User.Claims.Where(x => x.Type == "family_name")

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
        }

    }
}
