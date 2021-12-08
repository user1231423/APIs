namespace API.Authentication.Controllers
{
    using API.Authentication.DTO.Users;
    using Business.Authentication.Attributes;
    using Business.Authentication.Interfaces;
    using Business.Authentication.Models;
    using Business.Authentication.Services;
    using Common.Encoding.Hash;
    using Common.Pagination;
    using Common.Pagination.Models;
    using Data.Authentication.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
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
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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
        [Authorize]
        public async Task<ActionResult> List([FromQuery] PaginationParams pagination)
        {
            //List users
            var users = await _userService.ListAsync(pagination);

            //Add pagination header on response
            Response.Headers.Add("x-pagination", JsonConvert.SerializeObject(users.GetMetaData()));

            //Build DTO objects
            var userDTOs = users.Select(x => new UserDTO()
            {
                CreateDate = x.CreateDate,
                Email = x.Email,
                FirstName = x.FirstName,
                Id = x.Id,
                LastName = x.LastName,
                Status = x.Status,
                UpdateDate = x.UpdateDate
            }).ToList();

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
            var user = await _userService.LoadAsync(id);

            //Build user DTO
            var userDTO = new UserDTO()
            {
                CreateDate = user.CreateDate,
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.Id,
                LastName = user.LastName,
                Status = user.Status,
                UpdateDate = user.UpdateDate
            };

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
            var user = new User()
            {
                CreateDate = DateTime.Now,
                Email = createUser.Email,
                FirstName = createUser.FirstName,
                LastName = createUser.LastName,
                Password = createUser.Password.ToSHA256(),
                Status = 1
            };

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
            var user = new User()
            {
                FirstName = updateUser.FirstName,
                LastName = updateUser.LastName,
                Status = updateUser.Status
            };

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
