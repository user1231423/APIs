using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.DTO.Message
{
    public class UpdateMessageDTO
    {
        /// <summary>
        /// Message Body
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Body length must be at least 1")]
        public string Body { get; set; }

        /// <summary>
        /// User updating
        /// </summary>
        [Required]
        public int UserId { get; set; }
    }
}
