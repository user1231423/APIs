using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Files.DTO
{
    public class FileDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Original name
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// Content type
        /// </summary>
        public string ContentType { get; set; }
    }
}
