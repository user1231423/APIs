using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Authentication.DTO.Users
{
    public class UserDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

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
        public string Email { get; set; }

        /// <summary>
        /// Status
        /// <para>1: Active</para>
        /// <para>0: Undefined</para>
        /// <para>-1: Disabled</para>
        /// </summary>
        public short Status { get; set; }

        /// <summary>
        /// Create date
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Update date
        /// </summary>
        public DateTime? UpdateDate { get; set; }
    }
}
