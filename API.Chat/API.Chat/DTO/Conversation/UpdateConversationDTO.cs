using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.DTO.Conversation
{
    public class UpdateConversationDTO
    {
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "Max conversation name lenght is 55")]
        public string Name { get; set; }
    }
}
