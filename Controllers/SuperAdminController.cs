using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.BLL.Service;
using MyPaySlipLive.Extensions;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.AdminModel;
using NPOI.POIFS.Crypt.Dsig;

namespace MyPaySlipLive.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly INotyfService _notyf;
        private ICompany _company;
        private IBlog _blog;
        private int TotalPages;
        private int TotalRecords;
        //public int PageNumber = 1;
        //public int PageSize = 10;

        public SuperAdminController(ICompany company, INotyfService notyf, IBlog blog)
        {

            _company = company;
            _notyf = notyf;
            _blog = blog;
        }


        [HttpGet]
        public async Task<IActionResult> Dashboard(int pageNumber = 1)
        {
            int pageSize = TempData.ContainsKey("PageSize") ? (int)TempData["PageSize"]! : 10;


            var pagination = new Pagination()
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            TempData["PageNumber"] = pageNumber;
            TempData["PageSize"] = pageSize;
            var response = await GetCompanies(pagination);
            var model = new CompanyViewModel
            {
                CompanyList = JsonConvert.DeserializeObject<List<CompanyDto>>(response.Result)!,
                CompanyOBJ = new CompanyDto(),
                TotalPages = (int)Math.Ceiling((double)response.TotalRecords / pagination.PageSize),
                CurrentPage = pagination.PageNumber,
                CurrentPageSize = pageSize
            };

            return View(model);
        }


        //[HttpPost]
        //public async Task<List<CompanyDto>> GetCompanies(Pagination pagination)
        //{
        //    var companies = new List<CompanyDto>();
        //    var response = await _company.GetAll(pagination);
        //    companies = JsonConvert.DeserializeObject<List<CompanyDto>>(response.Result);
        //    return companies;
        //}


        [HttpPost]
        public async Task<Response> GetCompanies(Pagination pagination)
        {
            var companies = new List<CompanyDto>();
            var response = await _company.GetAll(pagination);
            return response;
        }



        [HttpPost]
        public async Task<IActionResult> ManageCompany(CompanyViewModel model)
        {
            int PageNumber = TempData.ContainsKey("PageNumber") ? (int)TempData["PageNumber"]! : 1;
            var companyModel = new CompanyDto();
            companyModel.Email = model.CompanyOBJ.Email; companyModel.Code = model.CompanyOBJ.Code;
            companyModel.Name = model.CompanyOBJ.Name; companyModel.Password = model.CompanyOBJ.Password;
            companyModel.IsActive = model.CompanyOBJ.IsActive;
            companyModel.Id = model.CompanyOBJ.Id == Guid.Empty ? Guid.Empty : model.CompanyOBJ.Id;
            if (model.CompanyOBJ.Id == Guid.Empty)
            {
                var response = await _company.AddUpdate(companyModel);
                if (response.Status == "OK")
                {
                    _notyf.Success($"{response.Message}");
                }
                else
                {
                    _notyf.Error($"{response.Message}");
                }

                return RedirectToAction("Dashboard", "SuperAdmin", new { PageNumber });
            }
            else
            {
                var response = await _company.AddUpdate(companyModel);
                if (response.Status == "OK")
                {
                    _notyf.Success($"{response.Message}");
                    return RedirectToAction("Dashboard", "SuperAdmin", new { PageNumber });
                }
                else
                {
                    _notyf.Error($"{response.Message}");
                    return RedirectToAction("EditCompany", "SuperAdmin", new { companyModel.Id });
                }
            }
        }





        public async Task<IActionResult> EditCompany(Guid id)
        {
            int pageNumber = TempData.ContainsKey("PageNumber") ? (int)TempData["PageNumber"]! : 1;
            int pageSize = TempData.ContainsKey("PageSize") ? (int)TempData["PageSize"]! : 10;

            var response = await _company.GetById(id);
            if (response.Result == null)
            {
                return NotFound();
            }
            var jsonResponse = JsonConvert.DeserializeObject<CompanyDto>(response.Result);
            var pagination = new Pagination()
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };
            var getAllCompanyList = await GetCompanies(pagination);
            var model = new CompanyViewModel
            {
                CompanyOBJ = new CompanyDto
                {
                    Id = jsonResponse!.Id,
                    Name = jsonResponse.Name,
                    Email = jsonResponse.Email,
                    CompanyCode = jsonResponse.CompanyCode,
                    Password = jsonResponse.Password,
                    IsActive = jsonResponse.IsActive
                },
                CompanyList = JsonConvert.DeserializeObject<List<CompanyDto>>(getAllCompanyList.Result)!,
                TotalPages = (int)Math.Ceiling((double)getAllCompanyList.TotalRecords / pagination.PageSize),
                CurrentPage = pagination.PageNumber,
                CurrentPageSize = pageSize

            };

            return View("Dashboard", model);
        }





        //--------------------- Blogs

        [HttpGet]
        public async Task<IActionResult> AddBlog(int pageNumber = 1)
        {
            try
            {
                int pageSize = TempData.ContainsKey("PageSize") ? (int)TempData["PageSize"]! : 10;


                var pagination = new Pagination()
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };

                TempData["PageNumber"] = pageNumber;
                TempData["PageSize"] = pageSize;
                var response = await GetBlogs(pagination);
                var model = new BlogViewModel
                {
                    BlogList = JsonConvert.DeserializeObject<List<BlogDto>>(response.Result)!,
                    BlogObJ = new BlogDto(),
                    TotalPages = (int)Math.Ceiling((double)response.TotalRecords / pagination.PageSize),
                    CurrentPage = pagination.PageNumber,
                    CurrentPageSize = pageSize
                };
                return View(model);
            }

            catch (Exception ex)
            {
                return View();
            }


        }

        private string StripHtmlButPreservePlainText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            input = System.Net.WebUtility.HtmlDecode(input);
            return System.Text.RegularExpressions.Regex.Replace(input, @"<[^>]*>", string.Empty);
        }



        [HttpPost]
        public async Task<IActionResult> ManageBlog(BlogViewModel model)
        {
            int PageNumber = TempData.ContainsKey("PageNumber") ? (int)TempData["PageNumber"]! : 1;
            if (string.IsNullOrEmpty(model.BlogObJ.Details))
            {
                _notyf.Error($"Please Enter Blog Detail!");
                return RedirectToAction("AddBlog", "SuperAdmin", new { PageNumber });
            }
            string sanitizedContent = StripHtmlButPreservePlainText(model.BlogObJ.Details).Trim();
            if (string.IsNullOrEmpty(sanitizedContent))
            {
                _notyf.Error($"Failed, Please Enter Blog Detail!");
                return RedirectToAction("AddBlog", "SuperAdmin", new { PageNumber });
            }

            var blogModel = new BlogDto();
            blogModel.Details = model.BlogObJ.Details;
            blogModel.Id = model.BlogObJ.Id == Guid.Empty ? Guid.Empty : model.BlogObJ.Id;
            if (model.BlogObJ.Id == Guid.Empty)
            {
                var response = await _blog.AddUpdateBlog(blogModel);
                if (response.Status == "OK")
                {
                    _notyf.Success($"{response.Message}");
                }
                else
                {
                    _notyf.Error($"{response.Message}");
                }

                return RedirectToAction("AddBlog", "SuperAdmin", new { PageNumber });
            }
            else
            {
                var response = await _blog.AddUpdateBlog(blogModel);
                if (response.Status == "OK")
                {
                    _notyf.Success($"{response.Message}");
                    return RedirectToAction("AddBlog", "SuperAdmin", new { PageNumber });
                }
                else
                {
                    _notyf.Error($"{response.Message}");
                    return RedirectToAction("EditBlog", "SuperAdmin", new { blogModel.Id });
                }
            }
        }


        [HttpPost]
        public async Task<Response> GetBlogs(Pagination pagination)
        {
            var response = await _blog.GetAllBlogsForSuperAdmin(pagination);
            return response;
        }

        public async Task<IActionResult> EditBlog(Guid id)
        {
            int pageNumber = TempData.ContainsKey("PageNumber") ? (int)TempData["PageNumber"]! : 1;
            int pageSize = TempData.ContainsKey("PageSize") ? (int)TempData["PageSize"]! : 10;

            var response = await _blog.GetBlogById(id);
            if (response.Result == null)
            {
                return NotFound();
            }
            var jsonResponse = JsonConvert.DeserializeObject<BlogDto>(response.Result);
            var pagination = new Pagination()
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };
            var getAllBlogList = await GetBlogs(pagination);
            var model = new BlogViewModel
            {
                BlogObJ = new BlogDto
                {
                    Id = jsonResponse!.Id,
                    Details = jsonResponse.Details,
                    Date = jsonResponse.Date
                },
                BlogList = JsonConvert.DeserializeObject<List<BlogDto>>(getAllBlogList.Result)!,
                TotalPages = (int)Math.Ceiling((double)getAllBlogList.TotalRecords / pagination.PageSize),
                CurrentPage = pagination.PageNumber,
                CurrentPageSize = pageSize

            };

            return View("AddBlog", model);
        }


        public async Task<IActionResult> DeleteBlog(BlogViewModel blog)
        {

            int pageSize = TempData.ContainsKey("PageSize") ? (int)TempData["PageSize"]! : 10;
            Guid id = (Guid)blog.BlogId!;
            var response = await _blog.DeleteBlog(id);
            if (response.Status == "OK")
            {
                _notyf.Success($"{response.Message}");
            }
            else
            {
                _notyf.Success($"{response.Message}");
            }

            return RedirectToAction("AddBlog", "SuperAdmin", new { pageNumber = blog.CurrentPage });



        }



        //--------------------- DownLoad paySlip



        public async Task<IActionResult> PaySlipDashBoard()
        {
            var data = new ExportSlipViewModel();
            var getAllData = await _company.GetAllCompaniesAndCompanyCode();
            if (getAllData.Status == "OK")
            {
                data = JsonConvert.DeserializeObject<ExportSlipViewModel>(getAllData.Result);
            }
            return View(data);
        }

        public async Task<IActionResult> GetDataByFilter(string companyCode, string? selectedCompany, string? SelectedMonth, string? ecode)
        {
            var data = new ExportSlipViewModel();
            var getDataByFilter = await _company.GetByFilter(companyCode, selectedCompany, SelectedMonth, ecode);
            if (getDataByFilter.Status == "OK")
            {
                data = JsonConvert.DeserializeObject<ExportSlipViewModel>(getDataByFilter.Result);
            }
            else
            {
                _notyf.Error($"{getDataByFilter.Message}");
            }
            var getAllData = await _company.GetAllCompaniesAndCompanyCode();
            if (getAllData.Status == "OK")
            {
                var allData = JsonConvert.DeserializeObject<ExportSlipViewModel>(getAllData.Result);


                data.CompanyCodes = allData.CompanyCodes ?? new List<GetComapanyCode>();
                data.CompanyNames = allData.CompanyNames ?? new List<GetCompanyName>();
                data.UserDetail = data.UserDetail ?? new List<UserDetailViewModel>();
            }

            return View("PaySlipDashBoard", data);

        }
    }
}
