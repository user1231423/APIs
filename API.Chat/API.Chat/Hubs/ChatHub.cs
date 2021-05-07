using API.Chat.HubUtils;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.Hubs
{
    public class ChatHub : Hub
    {
        public ChatHub()
        {
        }

        /// <summary>
        /// Hub connection test method
        /// </summary>
        /// <returns></returns>
        public async Task TestConnection()
        {
            try
            {
                await Clients.User(UserUtils.GetRequestUserId(Context.GetHttpContext())).SendAsync("TestMessageReceived", "Connection established");
            }
            catch (Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = e.Message.ToString()});
            }
        }

        /// <summary>
        /// Event triggered when a connection to the socket is established
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            try
            {
                await Task.FromResult(0);
            }
            catch (Exception e)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = e.Message.ToString() });
            }
        }

        /// <summary>
        /// Event triggered when socket is disconected
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception e)
        {
            try
            {
                await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = e.Message.ToString() });
            }
        }
    }
}
