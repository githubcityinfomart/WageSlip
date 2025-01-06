using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.Extensions;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.AccountModel;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MyPaySlipLive.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private CommonExtensions _common;
        private IBlog _blog;
        public HomeController(ILogger<HomeController> logger, CommonExtensions login, IBlog blog)
        {
            _logger = logger;
            _common = login;
            _blog = blog;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Home(int pageNumber = 1, int pageSize = 5)
        {
            var data = new List<BlogDto>();
            try
            {
                var pagination = new Pagination()
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };
                var response = await _blog.GetAllBlogsForSuperAdmin(pagination);
                if (response.Result != null)
                {
                    data = JsonConvert.DeserializeObject<List<BlogDto>>(response.Result);
                }
              
            }
            catch  {   data = new List<BlogDto>();  }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_NewsPartial", data);
            }

            return View(data);
        }



        //[HttpGet("/PfForms")]
        //public IActionResult PfForms()
        //{
        //    return View();
        //}

        //[HttpGet("/Insurance")]
        //public IActionResult Insurance()
        //{
        //    return View();
        //}

        //[HttpGet("/OutSourced")]
        //public IActionResult OutSourced()
        //{
        //    return View();
        //}

        //[HttpGet("/Esic")]
        //public IActionResult Esic()
        //{
        //    return View();
        //}

        //[HttpGet("/Faq")]
        //public IActionResult Faq()
        //{
        //    return View();
        //}

        public IActionResult Index()
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (User != null || User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Dashboard", "SuperAdmin");
                }
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("Dashboard", "User");
                }
            }
            return View();
        }



        [HttpGet("NotFound")]
        public IActionResult NotFound()
        {
            return View();
        }


        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
