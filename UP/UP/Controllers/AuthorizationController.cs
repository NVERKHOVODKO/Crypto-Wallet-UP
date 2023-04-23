using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UP.DTO;
using System.Web;
using System.Web.Http.Cors; 

namespace UP.Controllers
{
    [Route("[controller]")]
    //[System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(ILogger<AuthorizationController> logger)
        {
            _logger = logger;
        }
        
        [HttpPost]
        [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]//разрешаю использовать
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request) {
            _logger.LogInformation($"Index request at {DateTime.Now:hh:mm:ss}");
            try
            {
                var ur = new Repositories.UserRepository();
                var user = ur.Login(request.Login, request.Password);
                if (user != null) {
                    ur.SaveAccountLoginHistory(user.Id);
                    if (user.IsBlocked)
                    {
                        _logger.LogInformation($"Account is blocked");
                        return BadRequest("Your account is blocked: " + ur.GetUserBlockingReason(user.Id));
                    }
                    if (user.IsDeleted)
                    {
                        _logger.LogInformation($"There is no such user");
                        return NotFound("There is no such user. Try again");
                    }
                    _logger.LogInformation($"User login: {user.Login}", user.Login);
                    return Ok(user);
                } else {
                    _logger.LogInformation($"There is no such user. Try again");
                    return NotFound("There is no such user. Try again");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error");
                _logger.LogInformation($"Unknown error");
                return BadRequest("Error");
            }
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterRequest request) {
            try
            {
                if (request.Password.Length < 4)
                {
                    return UnprocessableEntity("Passwords must be more that 4 symbols");
                }
                if (request.Password != request.PasswordRepeat)
                {
                    return UnprocessableEntity("Passwords doesn't match");
                }
                var ur = new Repositories.UserRepository();
                if (ur.IsLoginUnique(request.Login))
                {
                    return UnprocessableEntity("Username already used");
                }
                var ar = new Repositories.AuthorizationRepository();
                if (!ar.IsValidEmail(request.Email))
                {
                    return UnprocessableEntity("Email isn't correct");
                }
                ur.WriteNewUserToDatabase(request.Login, request.Password, request.Email);
                _logger.LogInformation($"User created successfully");
                return Ok("User created successfully");
            }
            catch (Exception)
            {
                _logger.LogInformation($"User not created");
                return UnprocessableEntity("User not created");
            }
        }
    }
}