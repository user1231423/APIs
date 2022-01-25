using System.ComponentModel.DataAnnotations;

namespace API.Authentication.DTO.Users
{
    /// <summary>
    /// Create user
    /// </summary>
    public class CreateUserDTO
    {
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}
