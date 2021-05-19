namespace API.Chat.Controllers
{
    using API.Chat.DTO.Message;
    using API.Chat.Hubs;
    using Business.Chat.Filters;
    using Business.Chat.Services;
    using Common.Pagination.Models;
    using Data.Chat.Globalization.Errors;
    using Data.Chat.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        /// <summary>
        /// User service
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Conversation service
        /// </summary>
        private readonly ConversationService _conversationService;

        /// <summary>
        /// Message service
        /// </summary>
        private readonly MessageService _messageService;

        /// <summary>
        /// User conversation service
        /// </summary>
        private readonly UserConversationService _userConversationService;

        /// <summary>
        /// Chat hub
        /// </summary>
        private readonly IHubContext<ChatHub> _chatHub;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="conversationService"></param>
        /// <param name="messageService"></param>
        public MessagesController(UserService userService, ConversationService conversationService, MessageService messageService, UserConversationService userConversationService, IHubContext<ChatHub> hub)
        {
            _userService = userService;
            _conversationService = conversationService;
            _messageService = messageService;
            _userConversationService = userConversationService;
            _chatHub = hub;
        }

        /// <summary>
        /// List user user conversation messages
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="conversationId"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListUserConversationMessages([FromQuery] int userId, [FromQuery] int conversationId, [FromQuery] PaginationParams pagination)
        {
            var user = _userService.Load(userId);

            if(user == null)
                return NotFound(new { message = Errors.UserNotFound });

            var userConversation = _userConversationService.Filter(new UserConversationFilter() {
                ConversationId = conversationId,
                IncludeConversation = false,
                IncludeUser = false,
                UserId = userId,
                Status = 1
            }, new PaginationParams() { CurrentPage = 1, PageSize = 1 }).FirstOrDefault();

            if(userConversation == null)
                return NotFound(new { message = Errors.UserConversationNotFoundOrIsNotActive });

            var conversationMessages = _messageService.Filter(new MessageFilter()
            {
                ConversationId = conversationId
            }, pagination);

            var messageDTOs = conversationMessages.Select(x => new MessageDTO() {
                Body = x.Body,
                ConversationId = x.ConversationId,
                CreateDate = x.CreateDate,
                Id = x.Id,
                UpdateDate = x.UpdateDate,
                UserId = x.UserId
            }).ToList();

            return Ok(messageDTOs);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SendMessage(SendMessageDTO sendMessage)
        {
            //Find message author
            var author = _userService.Load(sendMessage.UserId);

            if (author == null)
                return NotFound(new { message = Errors.UserNotFound });

            //Load conversation
            var conversation = _conversationService.Load(sendMessage.ConversationId, false, true, false, false);

            if (conversation == null)
                return NotFound(new { message = Errors.ConversationNotFound });

            //Find user conversation
            var userConversation = conversation.Users.Where(x => x.UserId == sendMessage.UserId).FirstOrDefault();

            if (userConversation == null)
                return NotFound(new { message = Errors.UserConversationNotFound });

            if (userConversation.Status == -1)
                return BadRequest(new { message = Errors.UserWasKicked });

            if (userConversation.Status == -2)
                return BadRequest(new { message = Errors.UserLeftTheConversation });

            //Create message
            var message = new Message()
            {
                Body = sendMessage.Body,
                Conversation = conversation,
                ConversationId = conversation.Id,
                CreateDate = DateTime.Now,
                UpdateDate = null,
                User = author,
                UserId = author.Id
            };

            //Add message to conversation
            conversation.Messages.Add(message);

            await _conversationService.Update(conversation.Id, conversation);

            var usersToSendCreatedMessage = conversation.Users.Where(x => x.UserId != message.UserId).Select(x => x.UserId.ToString()).ToList();

            //Send object to users
            await _chatHub.Clients.Users(usersToSendCreatedMessage).SendAsync("MessageReceived", message);

            return Ok(message);
        }

        /// <summary>
        /// Update message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateMessage"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMessage(int id, UpdateMessageDTO updateMessage)
        {
            var updatingUser = _userService.Load(updateMessage.UserId);

            if (updatingUser == null)
                return NotFound(new { message = Errors.UserNotFound });

            //Load message
            var message = _messageService.Load(id);

            if (message == null)
                return NotFound(new { message = Errors.MessageNotFound });

            if (!string.IsNullOrEmpty(updateMessage.Body))
                message.Body = updateMessage.Body;

            await _messageService.Update(id, message);

            var conversation = _conversationService.Load(message.ConversationId, false, true, false, false);

            var usersToSendUpdatedMessage = conversation.Users.Where(x => x.UserId != updateMessage.UserId).Select(x => x.UserId.ToString()).ToList();

            //Send object to users
            await _chatHub.Clients.Users(usersToSendUpdatedMessage).SendAsync("MessageUpdated", message);

            return Ok(message);
        }

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            //Load message
            var message = _messageService.Load(id);

            if (message == null)
                return NotFound(new { message = Errors.MessageNotFound });

            await _messageService.Delete(id);

            var conversation = _conversationService.Load(message.ConversationId, false, true, false, false);

            var usersToSendDeletedMessage = conversation.Users.Where(x => x.UserId != message.UserId).Select(x => x.UserId.ToString()).ToList();

            //Send object to users
            await _chatHub.Clients.Users(usersToSendDeletedMessage).SendAsync("MessageDeleted", message);

            return Ok(message);
        }
    }
}
