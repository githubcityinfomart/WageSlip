using System.ComponentModel.DataAnnotations;

namespace MyPaySlipLive.Models.AdminModel
{
    public class ManageUsersViewModel
    {
        public Guid? EmployeeId { get; set; }
        public string? SearchTerm { get; set; }
        public List<UserInfo> Users { get; set; }

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int CurrentPageSize { get; set; }


        public string? Token { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        //[MinLength(6, ErrorMessage = "New Password must be at least 6 characters long.")]
        //[DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        // [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        
        public string PhoneNumber { get; set; }
        public ManageUsersViewModel()
        {
            Users = new List<UserInfo>();

        }
    }

    public class UserInfo
    {
        public Guid Id { get; set; } // Employee Primary Id
        public string CompanyName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string Ecode { get; set; } = null!;

    }
}
