using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyPaySlipLive.Models.AccountModel;
using System.Security.Claims;
using System.Security.Principal;
using MyPaySlipLive.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.Enum;
using Newtonsoft.Json;
using MyPaySlipLive.CommonMethods;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http.Extensions;
namespace MyPaySlipLive.Controllers
{

    public class AccountController : Controller
    {
        private CommonExtensions _common;
        private IUser _userLogin; //Interface
        private readonly INotyfService _notyf;
        public AccountController(CommonExtensions login, IUser userLogin, INotyfService notyf)
        {
            _common = login;
            _userLogin = userLogin;
            _notyf = notyf;
        }


        [HttpGet("Login/{companyCode?}")]
        public IActionResult Login(string companyCode)
        {
            var model = new LoginDto();
            if (!string.IsNullOrWhiteSpace(companyCode))
            {
                model.CompanyCode = companyCode;
            }
            return View(model);
        }


        [HttpPost("Login/{companyCode?}")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _userLogin.Login(model);
                if (response.Status == "OK")
                {
                    bool IsLogin = await _common.SignInUserAsync(model, response);
                    if (IsLogin)
                    {
                        var token = JsonConvert.DeserializeObject<string>(response.Result);
                        var Role = await CommonFunctions.DecodeJwt(token!);
                        var firstRole = Role.FirstOrDefault();

                        if (firstRole == "SuperAdmin")
                        {
                            _notyf.Success("You Are Logging in as SuperAdmin");
                            return RedirectToAction("Dashboard", "SuperAdmin");

                        }
                        else if (firstRole == "Admin")
                        {
                            _notyf.Success("You Are Logging in as Admin");
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        else if (firstRole == "User")
                        {
                            _notyf.Success("You Are Logging in as User");
                            return RedirectToAction("Dashboard", "User");
                        }
                        else
                        {
                            _notyf.Error($"Invalid Attempt, Please try Again!");

                        }


                    }
                    else
                    {
                        _notyf.Error($"{response.Message}");
                    }
                }
                else
                {
                    _notyf.Error($"{response.Message}");

                }


            }

            return View(model);
        }

        [HttpGet("/SuperAdmin")]
        public IActionResult SuperAdminLogin()
        {
             return View();
        }





        [HttpGet]

        public async Task<IActionResult> Logout()
        {
            bool isLogout = await _common.Logout();
            if (isLogout)
            {
                return RedirectToAction("Home", "Home");
            }
            else
            {
                return RedirectToAction("Home", "Home");

            }

        }





        //-----------Not Using
        public async Task<bool> SignInUserAsync(string username, string role, bool rememberMe)
        {
            try
            {
                if (HttpContext == null)
                {
                    throw new InvalidOperationException("HttpContext is not available.");
                }
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)

        };
                //claims.Add(new Claim("Name", username));
                //claims.Add(new Claim("Role", role));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
