using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Authentication.DTO.Users
{
    public class UpdateUserDTO
    {
        /// <summary>
        /// First name
        /// </summary>
        [StringLength(50, ErrorMessage = "Max first name length is 50")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [StringLength(50, ErrorMessage = "Max last name length is 50")]
        public string LastName { get; set; }

        /// <summary>
        /// Status
        /// <para>1: Active</para>
        /// <para>0: Undefined</para>
        /// <para>-1: Disabled</para>
        /// </summary>
        [Range(-1, 1, ErrorMessage = "Status must be between -1 and 1")]
        public short Status { get; set; }
    }
}
