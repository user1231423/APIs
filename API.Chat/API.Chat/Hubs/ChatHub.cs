using API.Chat.DTO.Message;
using API.Chat.HubUtils;
using Business.Chat;
using Business.Chat.Models;
using Business.Chat.Services;
using Data.Chat.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Chat.Hubs
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// Chat manager
        /// </summary>
        private readonly ChatService _chatManager;

        /// <summary>
        /// Conversation service
        /// </summary>
        private readonly ConversationService _conversationService;

        /// <summary>
        /// Message service
        /// </summary>
        private readonly MessageService _messageService;

        /// <summary>
        /// User service
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chatManager"></param>
        /// <param name="conversationService"></param>
        /// <param name="messageService"></param>
        /// <param name="userService"></param>
        public ChatHub(ChatService chatManager, ConversationService conversationService, MessageService messageService, UserService userService)
        {
            _chatManager = chatManager;
            _conversationService = conversationService;
            _messageService = messageService;
            _userService = userService;
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
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = ex.Message.ToString() });
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(SendMessageDTO message)
        {
            try
            {
                //Send message
                var messageSent = await _chatManager.SendMessage(new SendMessage()
                {
                    Body = message.Body,
                    ConversationId = message.ConversationId,
                    UserId = message.UserId
                });

                //Send object to users
                await Clients.Users(messageSent.UserIds).SendAsync("MessageReceived", messageSent.Message);
            }
            catch (Exception ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = ex.Message.ToString() });
            }
        }

        /// <summary>
        /// Update message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateMessage"></param>
        /// <returns></returns>
        public async Task UpdateMessage(int id, UpdateMessageDTO updateMessage)
        {
            try
            {
                var updatingUser = _userService.Load(updateMessage.UserId);

                if (updatingUser == null)
                    await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = "User not found" });

                //Load message
                var message = _messageService.Load(id);

                if(message == null)
                    await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = "Message not found" });

                if (!string.IsNullOrEmpty(updateMessage.Body))
                    message.Body = updateMessage.Body;

                await _messageService.Update(id, message);

                var conversation = _conversationService.Load(message.ConversationId, false, true, false, false);

                var usersToSendUpdatedMessage = conversation.Users.Where(x => x.UserId != updateMessage.UserId).Select(x => x.UserId.ToString()).ToList();

                //Send object to users
                await Clients.Users(usersToSendUpdatedMessage).SendAsync("MessageReceived", message);
            }
            catch (Exception ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = ex.Message.ToString() });
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="deleteMessage"></param>
        /// <returns></returns>
        public async Task DeleteMessage(DeleteMessageDTO deleteMessage)
        {
            try
            {
                //Load message
                var message = _messageService.Load(deleteMessage.Id);

                if (message == null)
                    await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = "Message not found" });

                await _messageService.Delete(deleteMessage.Id);

                var conversation = _conversationService.Load(message.ConversationId, false, true, false, false);

                var usersToSendUpdatedMessage = conversation.Users.Where(x => x.UserId != deleteMessage.UserId).Select(x => x.UserId.ToString()).ToList();
            }
            catch (Exception ex)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("ErrorReceived", new { message = ex.Message.ToString() });
            }
        }
    }
}
