using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.DTO.Message
{
    public class SendMessageDTO
    {
        /// <summary>
        /// Message Body
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// User sending message
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Conversation Id
        /// </summary>
        [Required]
        public int ConversationId { get; set; }
    }
}
