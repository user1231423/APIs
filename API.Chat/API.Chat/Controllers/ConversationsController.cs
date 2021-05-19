namespace API.Chat.Controllers
{
    using API.Chat.DTO.Conversation;
    using API.Chat.Hubs;
    using Business.Chat.Filters;
    using Business.Chat.Services;
    using Common.ExceptionHandler.Exceptions;
    using Common.Pagination;
    using Common.Pagination.Models;
    using Data.Chat.Globalization.Errors;
    using Data.Chat.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Conversations controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        /// <summary>
        /// User conversation service
        /// </summary>
        private readonly UserConversationService _userConversationService;

        /// <summary>
        /// User service
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Conversation service
        /// </summary>
        private readonly ConversationService _conversationService;

        /// <summary>
        /// Chat hub
        /// </summary>
        private readonly IHubContext<ChatHub> _chatHub;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userConversationService"></param>
        public ConversationsController(UserConversationService userConversationService, UserService userService, ConversationService conversationService, IHubContext<ChatHub> hub)
        {
            _userConversationService = userConversationService;
            _userService = userService;
            _conversationService = conversationService;
            _chatHub = hub;
        }

        /// <summary>
        /// List user conversations
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListUserConversations([FromQuery] int userId, [FromQuery] PaginationParams pagination)
        {
            var user = _userService.Load(userId);

            if (user == null)
                return NotFound(new { message = Errors.UserNotFound });

            var userConversationFilter = new UserConversationFilter()
            {
                UserId = userId,
                Status = 1,
                IncludeConversation = true,
                IncludeUser = true
            };

            var userConversations = _userConversationService.Filter(userConversationFilter, pagination);

            Response.Headers.Add("x-pagination", JsonConvert.SerializeObject(userConversations.GetMetaData()));

            var conversationDTOs = userConversations?.Select(x => new ConversationDTO()
            {
                Id = x.Conversation.Id,
                CreateDate = x.Conversation.CreateDate,
                Name = Regex.Replace(x.Conversation.Name, "(?<!(" + user.FullName + ", |, " + user.FullName + ").*)(" + user.FullName + ", |, " + user.FullName + ")", "", RegexOptions.IgnoreCase).ToString().TrimStart(),
                Type = x.Conversation.Type,
                UpdateDate = x.Conversation.UpdateDate
            });

            return Ok(conversationDTOs);
        }

        /// <summary>
        /// Create conversation
        /// </summary>
        /// <param name="createConversation"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateConversation(CreateConversationDTO createConversation)
        {
            if (createConversation.Users.Count != createConversation.Users.Distinct().Count())
                return BadRequest(new { message = Errors.DuplicateUserIds });

            //Load users by ids
            var users = _userService.LoadByIds(createConversation.Users);

            if (users.Count != createConversation.Users.Count)
                return BadRequest(new { message = Errors.InvalidUserIds });

            var conversation = new Conversation()
            {
                CreateDate = DateTime.Now,
                Type = createConversation.Type,
                Users = users.Select(x => new UserConversation()
                {
                    CreateDate = DateTime.Now,
                    NotificationStatus = 1,
                    Status = 0,
                    User = x
                }).ToList(),
                Name = string.IsNullOrEmpty(createConversation.Name) ? BuildConversationName(users.Select(x => x.FullName).ToList()) : createConversation.Name
            };

            return Ok(_conversationService.Save(conversation));
        }

        /// <summary>
        /// Update conversation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateConversation"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult UpdateConversation(int id, UpdateConversationDTO updateConversation)
        {
            //Load conversation
            var conversation = _conversationService.Load(id, false, false, false, false);

            if (conversation == null)
                return NotFound( new { message = Errors.ConversationNotFound });

            conversation.Name = updateConversation.Name;

            return Ok(_conversationService.Update(id, conversation));
        }

        /// <summary>
        /// Delete conversation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult DeleteConversation(int id)
        {
            return Ok(_conversationService.Delete(id));
        }

        /// <summary>
        /// Build conversation name
        /// </summary>
        /// <param name="usersFullNames"></param>
        /// <returns></returns>
        private static string BuildConversationName(List<string> usersFullNames)
        {
            var conversationName = "";

            usersFullNames.ForEach(name =>
            {
                if (name == usersFullNames.LastOrDefault())
                    conversationName += name;
                else
                    conversationName += name + ", ";
            });

            return conversationName;
        }
    }
}
