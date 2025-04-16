using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObjects.Models.Dtos
{
    public class SignUpUserDto
    {
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string Username { get; set; } = string.Empty!;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty!;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [MaxLength(50)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string ConfirmPassword { get; set; } = string.Empty!;


    }

    public class EditUserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50)]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = null!;
        public string CurrentPassword { get; set; } = string.Empty;

        [MaxLength(50)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? NewPassword { get; set; } 

        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [MaxLength(50)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? ConfirmNewPassword { get; set; } 
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = null!;
        public string? ProfilePictureBase64 { get; set; } 
    }
}