using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Please enter an Email Address!")]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "Please enter a Password!")]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

    }
}