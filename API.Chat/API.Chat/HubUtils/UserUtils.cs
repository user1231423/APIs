using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.HubUtils
{
    /// <summary>
    /// User utils
    /// </summary>
    public static class UserUtils
    {
        /// <summary>
        /// Get current request user id
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetRequestUserId(HttpContext httpContext)
        {
            return httpContext.Request.Query["userId"].ToString();
        }
    }
}
