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
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger, UserService userService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null || user.Id == new Guid())
            {
                return Ok(new User());
            }

            return Ok(user);
        }

        [HttpGet("registered/{email}")]
        public async Task<IActionResult> EmailFullyRegistered(string email)
        {
            try
            {
                _logger.LogInformation($"Checking if user with email: {email} is fully registered");

                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning($"User with email {email} not found");
                    return Ok(false);
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
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User updatedUser)
        {
            try
            {
                if (id != updatedUser.Id)
                {
                    return BadRequest("User ID mismatch");
                }

                var user = await _userService.UpdateUserAsync(id, updatedUser);
                if (user == null)
                {
                    return Ok(new User());
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
        public async Task<IActionResult> DeleteUser(Guid id)
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

        [HttpGet("{userId}/preferences")]
        public async Task<IActionResult> GetUserPreferences(Guid userId)
        {
            var preferences = await _userService.GetUserPreferencesAsync(userId);

            if (preferences == null)
            {
                return Ok(new UserPreferences());
            }

            return Ok(preferences);
        }

        [HttpPut("{userId}/preferences")]
        public async Task<IActionResult> UpdateUserPreferences(Guid userId, [FromBody] UserPreferences updatedPreferences)
        {
            var result = await _userService.UpdateUserPreferencesAsync(userId, updatedPreferences);
            if (result == null)
            {
                return Ok(new UserPreferences());
            }
            return Ok(result);
        }
    }
}
