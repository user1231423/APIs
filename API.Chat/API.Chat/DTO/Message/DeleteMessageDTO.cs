using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.DTO.Message
{
    public class DeleteMessageDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [Required]
        public int UserId { get; set; }
    }
}
