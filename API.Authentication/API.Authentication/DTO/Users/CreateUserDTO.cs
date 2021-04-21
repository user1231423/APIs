using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Authentication.DTO.Users
{
    public class CreateUserDTO
    {
        /// <summary>
        /// First name
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "Max first name length is 50")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [StringLength(50, ErrorMessage = "Max first name length is 50")]
        public string LastName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Must be a valid email")]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(64, ErrorMessage = "Max password length is 64")]
        public string Password { get; set; }
    }
}
