	using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using MyPaySlipLive.BLL.Interface;
using MyPaySlipLive.DAL;
using MyPaySlipLive.Models;
using MyPaySlipLive.Models.AdminModel;
using MyPaySlipLive.Models.Static;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Text.RegularExpressions;


namespace MyPaySlipLive.Controllers
{


	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{

		private IEmployee _employee;
		private IUser _user;

		private readonly INotyfService _notyf;

		public AdminController(IEmployee employee, INotyfService notyf, IUser user)
		{
			_employee = employee;
			_notyf = notyf;
			_user = user;
		}

		private readonly List<string> expectedHeadings = new List<string>
	{
		"ecode", "name", "wdays", "leaves", "comm", "advance", "tax", "month", "year", "company",
		"location", "sex", "chq", "bank", "account", "pfno", "esino", "category", "salbasis",
		"basic", "hra", "ca", "allow", "washing", "total", "ebasic", "ehra", "eca", "eallow",
		"reimb", "etotal", "pf", "esi", "tded", "net", "fpf", "epf", "pfemp", "esie", "gross", "remark", "remarks","remarks1", "sapcode",
		"ttl", "nhlv", "others", "conv", "tds", "transferredtoac", "netinr","mobilenumber" , "otherdeduction"
	};

		private readonly List<string> necessaryHeadings = new List<string>
		{
			"ecode", "name",  "company"
		};
		//----------------------------------Company Login 


		[HttpGet]
		public IActionResult CompanyLogin()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CompanyLogin(CompanyLoginViewModel model)
		{
			if (ModelState.IsValid)
			{

				if (model.CompanyCode == "123" && model.Username == "user" && model.Password == "password")
				{
					TempData["Message"] = "Login successful!";
					return RedirectToAction("ManageCompany");
				}

				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			}

			return View(model);
		}


		public IActionResult ForgotPassword()
		{
			TempData["Message"] = "Password reset functionality is not implemented yet.";
			return RedirectToAction("CompanyLogin");
		}




		//----------------------- Salary Upload 

		[HttpGet]
		public IActionResult SalaryUploader()
		{
			//var model = new SalaryUploadViewModel
			//{

			//    UploadedFiles = new List<UploadedFile>
			//    {
			//        new UploadedFile { FileName = "JanuarySalary.xlsx", UploadedDate = DateTime.Now.AddDays(-10) },
			//        new UploadedFile { FileName = "FebruarySalary.xlsx", UploadedDate = DateTime.Now.AddDays(-5) },
			//        new UploadedFile { FileName = "MarchSalary.xlsx", UploadedDate = DateTime.Now.AddDays(-15) },
			//        new UploadedFile { FileName = "AprilSalary.xlsx", UploadedDate = DateTime.Now.AddDays(-25) }
			//    }
			//};

			return View();
		}





		[HttpPost]
		public async Task<IActionResult> UploadExcelSheet(string SelectedMonth, IFormFile file)
		{
			try
			{
				if (string.IsNullOrEmpty(SelectedMonth))
				{
					TempData["ErrorMessage"] = "Please select a Month.";
					return RedirectToAction("SalaryUploader");
				}

				if (file == null || file.Length <= 0)
				{
					TempData["ErrorMessage"] = "Please upload a valid Excel file.";
					return RedirectToAction("SalaryUploader");
				}
				if (!int.TryParse(SelectedMonth, out var selectedMonthInt))
				{
					TempData["ErrorMessage"] = "Invalid month selection.";
					return RedirectToAction("SalaryUploader");
				}


				using (var stream = new MemoryStream())
				{
					await file.CopyToAsync(stream);
					stream.Position = 0;
					IWorkbook workbook;
					if (file.FileName.EndsWith(".xls"))
					{
						workbook = new HSSFWorkbook(stream); // For .xls files
					}
					else if (file.FileName.EndsWith(".xlsx"))
					{
						workbook = new XSSFWorkbook(stream); // For .xlsx files
					}
					else
					{
						TempData["ErrorMessage"] = "Unsupported file format. Please upload a .xls or .xlsx file.";
						return RedirectToAction("SalaryUploader");
					}

					var sheet = workbook.GetSheetAt(0); // Get the first sheet
					var rowCount = sheet.LastRowNum + 1; // Number of rows
					var colCount = sheet.GetRow(0).LastCellNum; // Number of columns

					// Define the expected headings (including the new fields)


					// Store the actual headings and their respective column index in a dictionary
					Dictionary<string, int> columnMapping = new Dictionary<string, int>();
					for (int col = 0; col < colCount; col++)
					{
						var heading = sheet.GetRow(0).GetCell(col).ToString().ToLower().Trim();
						heading = Regex.Replace(heading, @"[_.;]", ""); // Remove special characters
						if (expectedHeadings.Contains(heading))
						{
							columnMapping[heading] = col;
						}
					}

					// Check if any expected heading is missing
					var missingColumns = necessaryHeadings.Except(columnMapping.Keys).ToList();
					if (missingColumns.Any())
					{
						TempData["ErrorMessage"] = "Excel file has incorrect or missing headings: " + string.Join(", ", missingColumns);
						return RedirectToAction("SalaryUploader");
					}

					List<EmployeeDto> employeeList = new List<EmployeeDto>();
					HashSet<int> distinctMonthsInSheet = new HashSet<int>();

					// Process each row
					for (int row = 1; row <= sheet.LastRowNum; row++)
					{
						var rowData = sheet.GetRow(row);
						if (rowData != null && rowData.Cells.Any(cell => cell.CellType != CellType.Blank))
						{
							if (DateTime.TryParse(GetCellValue(rowData, columnMapping, "month"), out var dateValue))
							{
								distinctMonthsInSheet.Add(dateValue.Month);
								if (distinctMonthsInSheet.Count > 1)
								{
									TempData["ErrorMessage"] = "Excel sheet contains data for multiple months. Please ensure it contains data only for the selected month.";
									return RedirectToAction("SalaryUploader");
								}
								if (dateValue.Month != selectedMonthInt)
								{
									string selectedMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonthInt);
									TempData["ErrorMessage"] = $"Excel sheet contains data for multiple months. Expected month: {selectedMonthName}, but found {dateValue.ToString("MMMM")}. Please correct the file and try again.";
									return RedirectToAction("SalaryUploader");
								}


								if (dateValue.Month == selectedMonthInt)
								{
									if (!string.IsNullOrWhiteSpace(GetCellValue(rowData, columnMapping, "ecode")) &&
	!string.IsNullOrWhiteSpace(GetCellValue(rowData, columnMapping, "name")) &&
	!string.IsNullOrWhiteSpace(GetCellValue(rowData, columnMapping, "company")))
									{
										var employee = new EmployeeDto
										{
											Ecode = GetCellValue(rowData, columnMapping, "ecode"),
											Name = GetCellValue(rowData, columnMapping, "name"),
											Wdays = GetCellValue(rowData, columnMapping, "wdays"),
											Leaves = GetCellValue(rowData, columnMapping, "leaves"),
											Comm = GetCellValue(rowData, columnMapping, "comm"),
											Advance = GetCellValue(rowData, columnMapping, "advance"),
											Tax = GetCellValue(rowData, columnMapping, "tax"),
											//Date = DateTime.TryParse(GetCellValue(rowData, columnMapping, "month"), out var dateValue) ? dateValue : default,
											Date = dateValue,
											Company = GetCellValue(rowData, columnMapping, "company"),
											Location = GetCellValue(rowData, columnMapping, "location"),
											Sex = GetCellValue(rowData, columnMapping, "sex"),
											Chq = GetCellValue(rowData, columnMapping, "chq"),
											Bank = GetCellValue(rowData, columnMapping, "bank"),
											Account = ParseNumber(GetCellValue(rowData, columnMapping, "account")),
											PfNumber = ParseNumber(GetCellValue(rowData, columnMapping, "pfno")),
											EsiNumber = GetCellValue(rowData, columnMapping, "esino"),
											Category = GetCellValue(rowData, columnMapping, "category"),
											SalBasis = GetCellValue(rowData, columnMapping, "salbasis"),
											Basic = GetCellValue(rowData, columnMapping, "basic"),
											Hra = GetCellValue(rowData, columnMapping, "hra"),
											Ca = GetCellValue(rowData, columnMapping, "ca"),
											Allow = GetCellValue(rowData, columnMapping, "allow"),
											Washing = GetCellValue(rowData, columnMapping, "washing"),
											Total = GetCellValue(rowData, columnMapping, "total"),
											Ebasic = GetCellValue(rowData, columnMapping, "ebasic"), // Employer Basic
											Ehra = GetCellValue(rowData, columnMapping, "ehra"), // Employer HRA
											Eca = GetCellValue(rowData, columnMapping, "eca"), // Employer City Allowance
											Eallow = GetCellValue(rowData, columnMapping, "eallow"),// Employer Other Allowance
											ReImb = GetCellValue(rowData, columnMapping, "reimb"), // Reimbursement
											Etotal = GetCellValue(rowData, columnMapping, "etotal"),// Employer Total
											Pf = GetCellValue(rowData, columnMapping, "pf"), // PF
											Esi = ParseNumber(GetCellValue(rowData, columnMapping, "esi")), // ESI
											Tded = GetCellValue(rowData, columnMapping, "tded"), // Total Deductions
											Net = GetCellValue(rowData, columnMapping, "net"), // Net Salary
											Fpf = GetCellValue(rowData, columnMapping, "fpf"), // Final PF
											Epf = GetCellValue(rowData, columnMapping, "epf"), // Employer PF
											PfEmp = GetCellValue(rowData, columnMapping, "pfemp"), // PF Employee
											Esie = GetCellValue(rowData, columnMapping, "esie"), // Employer ESI
											Gross = GetCellValue(rowData, columnMapping, "gross"), // Gross Salary
											Remark = GetCellValue(rowData, columnMapping, "remarks1"),
											// Remarks
											// Additional fields
											SapCode = GetCellValue(rowData, columnMapping, "sapcode"),
											Ttl = GetCellValue(rowData, columnMapping, "ttl"),
											Nhlv = GetCellValue(rowData, columnMapping, "nhlv"),
											Others = GetCellValue(rowData, columnMapping, "others"),
											Conv = GetCellValue(rowData, columnMapping, "conv"),
											Tds = GetCellValue(rowData, columnMapping, "tds"),
											MobileNumber = ParseNumber(GetCellValue(rowData, columnMapping, "mobilenumber")),
											TransferredToAc = ParseNumber(GetCellValue(rowData, columnMapping, "transferredtoac")),



										};


										employeeList.Add(employee);
									}
								}
							}


						}
					}

					var employeeData = new AddUpdateEmployeeDto
					{
						EmployeeData = employeeList
					};

					if (employeeList.Count > 0)
					{
						var companyCode = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyCode")?.Value;
						var response = await _employee.AddEmployeeDetails(employeeData, companyCode!);
						if (response.Status == "OK")
						{
							_notyf.Success("Sheet Uploaded");
							return View("SalaryUploader");
						}
						else
						{
							_notyf.Error($"{response.Message}");
						}
					}
					else
					{
						_notyf.Error("Data in Sheet is Empty Or Month is not found, Add Data And try Again!");
					}
					return View("SalaryUploader");
				}
			}
			catch (Exception ex)
			{
				_notyf.Error($"{ex.Message}");
				return View("SalaryUploader");
			}

		}


		private string ParseNumber(string esiString)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(esiString))
				{
					// Parse as double to handle scientific notation
					double esiDouble = double.Parse(esiString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);

					// Convert to long
					return Convert.ToString(esiDouble);
				}

				return "0";
			}
			catch
			{
				return esiString;
			}

		}


		private string GetCellValue(IRow row, Dictionary<string, int> columnMapping, string columnName)
		{
			if (columnMapping.TryGetValue(columnName, out var colIndex))
			{
				return row.GetCell(colIndex)?.ToString() ?? string.Empty;
			}
			return string.Empty;
		}

		[HttpGet]
		public IActionResult DownloadExcel()
		{
			// Create a new Excel workbook
			IWorkbook workbook = new XSSFWorkbook();
			ISheet sheet = workbook.CreateSheet("Sheet1");

			// Create a header row
			IRow headerRow = sheet.CreateRow(0);
			headerRow.CreateCell(0).SetCellValue("Ecode");
			headerRow.CreateCell(1).SetCellValue("Name");
			headerRow.CreateCell(2).SetCellValue("Wdays");
			headerRow.CreateCell(3).SetCellValue("Leaves");
			headerRow.CreateCell(4).SetCellValue("Comm");
			headerRow.CreateCell(5).SetCellValue("Advance");
			headerRow.CreateCell(6).SetCellValue("Tax");
			headerRow.CreateCell(7).SetCellValue("Month");
			headerRow.CreateCell(8).SetCellValue("Company");
			headerRow.CreateCell(9).SetCellValue("Location");
			headerRow.CreateCell(10).SetCellValue("Sex");
			headerRow.CreateCell(11).SetCellValue("Chq");
			headerRow.CreateCell(12).SetCellValue("Bank");
			headerRow.CreateCell(13).SetCellValue("Account");
			headerRow.CreateCell(14).SetCellValue("PF_No");
			headerRow.CreateCell(15).SetCellValue("Esi_no");
			headerRow.CreateCell(16).SetCellValue("Category");
			headerRow.CreateCell(17).SetCellValue("Sal_basis");
			headerRow.CreateCell(18).SetCellValue("Basic");
			headerRow.CreateCell(19).SetCellValue("HRA");
			headerRow.CreateCell(20).SetCellValue("CA");
			headerRow.CreateCell(21).SetCellValue("Allow");
			headerRow.CreateCell(22).SetCellValue("Washing");
			headerRow.CreateCell(23).SetCellValue("Total");
			headerRow.CreateCell(24).SetCellValue("EBasic");
			headerRow.CreateCell(25).SetCellValue("EHRA");
			headerRow.CreateCell(26).SetCellValue("ECA");
			headerRow.CreateCell(27).SetCellValue("EAllow");
			headerRow.CreateCell(28).SetCellValue("Reimb");
			headerRow.CreateCell(29).SetCellValue("Etotal");
			headerRow.CreateCell(30).SetCellValue("PF");
			headerRow.CreateCell(31).SetCellValue("ESI");
			headerRow.CreateCell(32).SetCellValue("Tded");
			headerRow.CreateCell(33).SetCellValue("Net");
			headerRow.CreateCell(34).SetCellValue("FPF");
			headerRow.CreateCell(35).SetCellValue("EPF");
			headerRow.CreateCell(36).SetCellValue("Pf_EMP");
			headerRow.CreateCell(37).SetCellValue("ESIE");
			headerRow.CreateCell(38).SetCellValue("Gross");
 			headerRow.CreateCell(39).SetCellValue("Remarks1");
<<<<<<< HEAD

            //Add some sample data
            IRow row1 = sheet.CreateRow(1);
            row1.CreateCell(0).SetCellValue(1000000);
            row1.CreateCell(1).SetCellValue("Charanjit Singh Demo");
            row1.CreateCell(2).SetCellValue(29);
            row1.CreateCell(3).SetCellValue(1);
            row1.CreateCell(4).SetCellValue(1300);
            row1.CreateCell(5).SetCellValue(300);
            row1.CreateCell(6).SetCellValue(""); // Empty field
            row1.CreateCell(7).SetCellValue(""); // Placeholder for the date
            row1.CreateCell(8).SetCellValue("Vibracoustic India Private Limited Moulding");
            row1.CreateCell(9).SetCellValue("Mohali");
            row1.CreateCell(10).SetCellValue("M");
            row1.CreateCell(11).SetCellValue(627555);
            row1.CreateCell(12).SetCellValue("PNB");
            row1.CreateCell(13).SetCellValue(3883000110269669);
            row1.CreateCell(14).SetCellValue(101763455824);
            row1.CreateCell(15).SetCellValue(1216384345);
            row1.CreateCell(16).SetCellValue("NON ITI");
            row1.CreateCell(17).SetCellValue("Moulding");
            row1.CreateCell(18).SetCellValue(11500);
            row1.CreateCell(19).SetCellValue(2994);
            row1.CreateCell(20).SetCellValue(""); // Empty field
            row1.CreateCell(21).SetCellValue(""); // Empty field
            row1.CreateCell(22).SetCellValue(""); // Empty field
            row1.CreateCell(23).SetCellValue(14494);
            row1.CreateCell(24).SetCellValue(11500);
            row1.CreateCell(25).SetCellValue(2994);
            row1.CreateCell(26).SetCellValue(0);
            row1.CreateCell(27).SetCellValue(0);
            row1.CreateCell(28).SetCellValue(1300);
            row1.CreateCell(29).SetCellValue(15794);
            row1.CreateCell(30).SetCellValue(1380);
            row1.CreateCell(31).SetCellValue(119);
            row1.CreateCell(32).SetCellValue(1804);
            row1.CreateCell(33).SetCellValue(13990);
            row1.CreateCell(34).SetCellValue(958);
            row1.CreateCell(35).SetCellValue(422);
            row1.CreateCell(36).SetCellValue(1495);
            row1.CreateCell(37).SetCellValue(514);
            row1.CreateCell(38).SetCellValue(17803);
            row1.CreateCell(39).SetCellValue("Submit your Pic to HR");
            DateTime dateValue = new DateTime(2024, 11, 30);
            row1.CreateCell(7).SetCellValue(dateValue);

            // Apply a custom format to display "November 2024" by default
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat dateFormat = workbook.CreateDataFormat();
            dateStyle.DataFormat = dateFormat.GetFormat("MMMM-yyyy"); // "November 2024"
            row1.GetCell(7).CellStyle = dateStyle;
            sheet.AutoSizeColumn(7);

            // Add some sample data
            //IRow row1 = sheet.CreateRow(1);
            //row1.CreateCell(0).SetCellValue(1);
            //row1.CreateCell(1).SetCellValue("John Doe");
            //row1.CreateCell(2).SetCellValue(30);

            //IRow row2 = sheet.CreateRow(2);
            //row2.CreateCell(0).SetCellValue(2);
            //row2.CreateCell(1).SetCellValue("Jane Smith");
            //row2.CreateCell(2).SetCellValue(25);


=======


          

            //Add some sample data
            IRow row1 = sheet.CreateRow(1);
            row1.CreateCell(0).SetCellValue(1000000);
            row1.CreateCell(1).SetCellValue("Charanjit Singh Demo");
            row1.CreateCell(2).SetCellValue(29);
            row1.CreateCell(3).SetCellValue(1);
            row1.CreateCell(4).SetCellValue(1300);
            row1.CreateCell(5).SetCellValue(300);
            row1.CreateCell(6).SetCellValue(""); // Empty field
            row1.CreateCell(7).SetCellValue(""); // Placeholder for the date
            row1.CreateCell(8).SetCellValue("Vibracoustic India Private Limited Moulding");
            row1.CreateCell(9).SetCellValue("Mohali");
            row1.CreateCell(10).SetCellValue("M");
            row1.CreateCell(11).SetCellValue(627555);
            row1.CreateCell(12).SetCellValue("PNB");
            row1.CreateCell(13).SetCellValue(3883000110269669);
            row1.CreateCell(14).SetCellValue(101763455824);
            row1.CreateCell(15).SetCellValue(1216384345);
            row1.CreateCell(16).SetCellValue("NON ITI");
            row1.CreateCell(17).SetCellValue("Moulding");
            row1.CreateCell(18).SetCellValue(11500);
            row1.CreateCell(19).SetCellValue(2994);
            row1.CreateCell(20).SetCellValue(""); // Empty field
            row1.CreateCell(21).SetCellValue(""); // Empty field
            row1.CreateCell(22).SetCellValue(""); // Empty field
            row1.CreateCell(23).SetCellValue(14494);
            row1.CreateCell(24).SetCellValue(11500);
            row1.CreateCell(25).SetCellValue(2994);
            row1.CreateCell(26).SetCellValue(0);
            row1.CreateCell(27).SetCellValue(0);
            row1.CreateCell(28).SetCellValue(1300);
            row1.CreateCell(29).SetCellValue(15794);
            row1.CreateCell(30).SetCellValue(1380);
            row1.CreateCell(31).SetCellValue(119);
            row1.CreateCell(32).SetCellValue(1804);
            row1.CreateCell(33).SetCellValue(13990);
            row1.CreateCell(34).SetCellValue(958);
            row1.CreateCell(35).SetCellValue(422);
            row1.CreateCell(36).SetCellValue(1495);
            row1.CreateCell(37).SetCellValue(514);
            row1.CreateCell(38).SetCellValue(17803);
            row1.CreateCell(39).SetCellValue("Submit your Pic to HR");
            DateTime dateValue = new DateTime(2024, 11, 30);
            row1.CreateCell(7).SetCellValue(dateValue);

            // Apply a custom format to display "November 2024" by default
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat dateFormat = workbook.CreateDataFormat();
            dateStyle.DataFormat = dateFormat.GetFormat("MMMM-yyyy"); // "November 2024"
            row1.GetCell(7).CellStyle = dateStyle;
            sheet.AutoSizeColumn(7);
            //IRow row2 = sheet.CreateRow(2);
            //row2.CreateCell(0).SetCellValue(2);
            //row2.CreateCell(1).SetCellValue("Jane Smith");
            //row2.CreateCell(2).SetCellValue(25);


>>>>>>> 015c68a791fdcb46e0daaee858d7e4494ebb48f8
            using (var stream = new MemoryStream())
			{
				workbook.Write(stream);
				var content = stream.ToArray();

				// Return the file as a download
				return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SampleData.xlsx");
			}
		}
		//----------------------- Manage Users



		//[HttpGet]
		//public IActionResult ManageUsers(string searchTerm)
		//{
		//    var model = new ManageUsersViewModel
		//    {
		//        SearchTerm = searchTerm,
		//        Users = GetUsers().Where(u => string.IsNullOrEmpty(searchTerm) || u.CompanyName.Contains(searchTerm) || u.Username.Contains(searchTerm)).ToList()
		//    };
		//    return View(model);
		//}
		[Authorize]
		[HttpGet]

		public async Task<IActionResult> Dashboard(int pageNumber = 1)
		{

			var companyCode = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyCode")?.Value;
			//string search = string.Empty;
			string search = TempData["SearchTerm"] as string ?? string.Empty;
			int pageSize = TempData.ContainsKey("PageSizeofUsers") ? (int)TempData["PageSizeofUsers"]! : 50;
			TempData["PageNumberofUsers"] = pageNumber;
			TempData["PageSizeofUsers"] = pageSize;
			ViewBag.PageSize = pageSize;
			var getAllCompanyEmployeeDto = new GetAllCompanyEmployeeDto()
			{
				CompanyCode = companyCode!,
				Pagination = new Pagination
				{
					PageSize = pageSize,
					PageNumber = pageNumber
				}
			};
			var users = new List<UserInfo>();
			var response = await _employee.GetAllByCompany(getAllCompanyEmployeeDto, search);
			users = JsonConvert.DeserializeObject<List<UserInfo>>(response.Result)!;
			//var users = await GetEmployeesByCompany(companyCode!, search);
			var model = new ManageUsersViewModel
			{
				Users = users,
				TotalPages = (int)Math.Ceiling((double)response.TotalRecords / getAllCompanyEmployeeDto.Pagination.PageSize),
				CurrentPage = getAllCompanyEmployeeDto.Pagination.PageNumber,
				CurrentPageSize = pageSize
			};

			return View(model);


		}
		[HttpPost]
		public IActionResult ChangePageSize(int pageSize)
		{
			if (pageSize > 0)
			{
				// Store the page size in TempData or other storage
				TempData["PageSizeofUsers"] = pageSize;
				return Json(new { success = true, message = "Invalid page size." });
			}
			else
			{
				return Json(new { success = false, message = "Invalid page size." });
			}
		}
		[HttpPost]
		private async Task<List<UserInfo>> GetEmployeesByCompany(string companyCode, string search)
		{
			var getAllCompanyEmployeeDto = new GetAllCompanyEmployeeDto()
			{
				CompanyCode = companyCode,
				Pagination = new Pagination
				{
					PageSize = 10,
					PageNumber = 1
				}
			};
			var response = await _employee.GetAllByCompany(getAllCompanyEmployeeDto, search);
			return JsonConvert.DeserializeObject<List<UserInfo>>(response.Result)!;
		}

		[HttpPost]
		public IActionResult Search(string searchTerm)
		{

			TempData["SearchTerm"] = searchTerm;
			return RedirectToAction("Dashboard");
		}




		//------------------------------- Reset Password 



		[HttpGet]
		public IActionResult ResetPassword()
		{

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ManageUsersViewModel model)
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
			resetPass.Token = Token;
			resetPass.PhoneNumber = model.PhoneNumber;
			resetPass.NewPassword = model.NewPassword;
			var response = await _user.ResetPassword(resetPass);
			if (response.Status == "OK")
			{
				_notyf.Success("Password Reset Successful");
			}
			else
			{
				_notyf.Error("Password Reset Failed, Try Different Password");
			}
			return RedirectToAction("ResetPassword");
		}



		[HttpPost]
		public async Task<IActionResult> ResetPasswordOfEmployees(ManageUsersViewModel model)
		{
			var resetPass = new ResetPasswordDto();
			resetPass.EmployeeId = (Guid)model.EmployeeId!;
			var response = await _user.ResetToDefaultPassword(resetPass);
			if (response.Status == "OK")
			{
				_notyf.Success("Password Reset To Default");
			}
			else
			{
				_notyf.Error("Password Reset Failed");
			}
			return RedirectToAction("Dashboard");

		}


	}
}