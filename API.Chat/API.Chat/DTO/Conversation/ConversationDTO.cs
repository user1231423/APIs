using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.DTO.Conversation
{
    public class ConversationDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// <para>1: Private</para>
        /// <para>2: Group</para>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Date started
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Update date
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}
