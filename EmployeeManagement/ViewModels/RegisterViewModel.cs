using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Remote(action: "EmailInUse", controller:"Account")]
        [Required]
        [EmailAddress]
        [ValidateEmailDomain(allowedDomain:"gmail.com",
            ErrorMessage = "Email Domain must be gamil.com")]
        public string Email{ get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage ="Password do not match")]
        public string ConfirmPassword{ get; set; }


        public string City { get; set; }
    }
}
