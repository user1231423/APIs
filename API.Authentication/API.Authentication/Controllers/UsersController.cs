namespace API.Authentication.Controllers
{
    using API.Authentication.DTO.Users;
    using Business.Authentication.Middleware;
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

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// User service
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="authenticate"></param>
        /// <returns></returns>
        [HttpPost("Authenticate")]
        public ActionResult Authenticate(AuthenticateRequest authenticate)
        {
            try
            {
                var authenticated = _userService.Authenticate(authenticate);

                return Ok(new { authenticated.Token });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// List users
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet("List")]
        [ServiceFilter(typeof(Authorize))]
        public ActionResult List([FromQuery] PaginationParams pagination)
        {
            try
            {
                //List users
                var users = _userService.List(pagination);

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
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            try
            {
                //Load user
                var user = _userService.Load(id);

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
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Post(CreateUserDTO createUser)
        {
            try
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
                return Ok(await _userService.Save(user));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
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
            try
            {
                var user = new User()
                {
                    FirstName = updateUser.FirstName,
                    LastName = updateUser.LastName,
                    Status = updateUser.Status
                };

                //Save user obj and return saved id
                return Ok(await _userService.Update(id, user));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //Save user obj and return saved id
                return Ok(await _userService.Delete(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
