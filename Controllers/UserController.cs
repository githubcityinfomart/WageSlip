using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
<<<<<<< HEAD
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.AdminModel;
=======
using PaySlip.BLL.Interface;
using PaySlip.DAL;
using PaySlip.Models;
using PaySlip.Models.AdminModel;
>>>>>>> 015c68a791fdcb46e0daaee858d7e4494ebb48f8

namespace MyPaySlipLive.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUser _user;
        private readonly INotyfService _notyf;
      
        public UserController(IUser user, INotyfService notyf )
        {
            _user = user;
            _notyf = notyf;
           
        }



        public async Task<IActionResult> Dashboard()
        {
         
            var companyCode = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyCode")?.Value;
            var employeeCode = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "EmployeeCode")?.Value;

            var response = await _user.GetByCode(employeeCode!, companyCode!);
            var data = JsonConvert.DeserializeObject<List<UserDetailViewModel>>(response.Result)!;
            data = data.OrderByDescending(data => int.Parse( data.Year!.Trim())).ThenByDescending(data => int.Parse(data.Month!.Trim())).ToList();
            return View(data);
        }

        [HttpGet]
        public IActionResult ResetMyPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetMyPassword(UserPasswordResetModel model)
        {
            //Phone Number Verification 

            if (model.PhoneNumber == null)
            {
                _notyf.Error("Please Enter Phone Number.");
                return View(model);
            }

            if (model.PhoneNumber.Length != 10 ||
        !long.TryParse(model.PhoneNumber, out long phoneNumber) ||
        phoneNumber < 5000000000 ||
        phoneNumber > 9999999999)
            {
                _notyf.Error("Phone number must be a 10-digit number starting from 5 to 9.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                _notyf.Error("Password Do Not Match");
                return View(model);
            }
            string Token = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")!.Value;
            var resetPass = new ResetPasswordDto();
            resetPass.PhoneNumber = model.PhoneNumber;
            resetPass.Token = Token;
            resetPass.NewPassword = model.NewPassword;
            var response = await _user.ResetPassword(resetPass);
            if (response.Status == "OK")
            {
                _notyf.Success("Password Change Successful");
            }
            else
            {
                _notyf.Error("Password Change Failed, Try Different Password");
            }
            return RedirectToAction("ResetMyPassword");
        }
    }
}
