using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.HubUtils
{
    /// <summary>
    /// Custom implementation os UserIdProvider
    /// </summary>
    public class UserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// Gets connected user id
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext().Request.Query["userId"].ToString();
        }
    }
}
