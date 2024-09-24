using ASG.Services;
using ASGBackend.Data;
using ASGBackend.Interfaces;
using ASGBackend.Services;
using ASGShared.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASGBackend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AIAgentService _aiAgentService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;

        public UserController(
            AIAgentService aiAgentService,
            IUserRepository userRepository,
            ILogger<UserController> logger,
            UserService userService)
        {
            _aiAgentService = aiAgentService;
            _userRepository = userRepository;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(user);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("registered/{email}")]
        public async Task<IActionResult> UserFullyRegistered(string email)
        {
            try
            {
                _logger.LogInformation($"Checking if user with email: {email} is fully registered");
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning($"User with email {email} not found");
                    return NotFound();
                }

                var isFullyRegistered = await _userService.ValidateFullyRegistered(user.Id);
                return Ok(isFullyRegistered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user is fully registered");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, updatedUser);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
