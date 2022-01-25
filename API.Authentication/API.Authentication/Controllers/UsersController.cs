namespace API.Authentication.Controllers
{
    using API.Authentication.DTO.Users;
    using AutoMapper;
    using Business.Authentication.Attributes;
    using Business.Authentication.Interfaces;
    using Business.Authentication.Models;
    using Business.Authentication.Services;
    using Common.Encoding.Hash;
    using Common.ExceptionHandler.Exceptions;
    using Common.Pagination;
    using Common.Pagination.Models;
    using Data.Authentication.Globalization.Errors;
    using Data.Authentication.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// User controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// User service
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="authenticate"></param>
        /// <returns></returns>
        [HttpPost("Authenticate")]
        public async Task<ActionResult> Authenticate(AuthenticateRequest authenticate)
        {
            AuthenticateResponse authenticated = await _userService.AuthenticateAsync(authenticate);

            return Ok(authenticated.Token);
        }

        /// <summary>
        /// List users
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet("List")]
        //[Authorize]
        public async Task<ActionResult> List([FromQuery] PaginationParams pagination)
        {
            //List users
            PagedList<User> users = await _userService.ListAsync(pagination);

            //Add pagination header on response
            Response.Headers.Add("x-pagination", JsonConvert.SerializeObject(users.GetMetaData()));

            //Build DTO objects
            List<UserDTO> userDTOs = _mapper.Map<List<UserDTO>>(users);

            return Ok(userDTOs);
        }

        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            //Load user
            User user = await _userService.LoadAsync(id);

            //Build user DTO
            UserDTO userDTO = _mapper.Map<UserDTO>(user);

            return Ok(userDTO);
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(CreateUserDTO createUser)
        {
            //Build user object
            User user = _mapper.Map<User>(createUser);

            //Save user obj and return saved id
            return Ok(await _userService.SaveAsync(user));
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, UpdateUserDTO updateUser)
        {
            User oldUser = await _userService.LoadAsync(id);

            if (oldUser == null)
                throw new NotFoundException(Errors.UserNotFound);

            User user = _mapper.Map(updateUser, oldUser);

            //Save user obj and return saved id
            return Ok(await _userService.UpdateAsync(id, user));
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            //Delete user obj and return deleted id
            return Ok(await _userService.DeleteAsync(id));
        }
    }
}
