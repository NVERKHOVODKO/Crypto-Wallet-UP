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
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request) {
            _logger.LogInformation($"Index request at {DateTime.Now:hh:mm:ss}");
            _logger.LogInformation($"Login: " + request.Login);
            _logger.LogInformation($"Password: " + request.Password);
            try
            {
                var ur = new Repositories.UserRepository();
                var user = ur.Login(request.Login, request.Password);
                if (user != null) {
                    ur.SaveAccountLoginHistory(user.Id);
                    if (user.IsBlocked)
                    {
                        _logger.LogInformation($"Account is blocked");
                        return BadRequest("Ваш аккаунт заблокирован: " + ur.GetUserBlockingReason(user.Id));
                    }
                    if (user.IsDeleted)
                    {
                        _logger.LogInformation($"There is no such user");
                        return NotFound("Пользователь удален");
                    }
                    _logger.LogInformation($"User login: {user.Login}", user.Login);
                    return Ok(user);
                } else {
                    _logger.LogInformation($"There is no such user. Try again");
                    return NotFound("Пользователь не найден");
                }
            }
            catch (Exception exception)
            {
                _logger.LogInformation($"Unknown error");
                return BadRequest("Пользователь не найден");
            }
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterRequest request) {
            try
            {
                _logger.LogInformation($"Login: " + request.Login + "\nPassword: " + request.Password + "\nPasswordRep: " + request.PasswordRepeat);
                if (request.Password.Length < 4)
                {
                    _logger.LogInformation($"Passwords must be more that 4 symbols");
                    return UnprocessableEntity("Пароль должен быть больше 4 символов");
                }
                if (request.Password != request.PasswordRepeat)
                {
                    _logger.LogInformation($"Passwords doesn't match");
                    return UnprocessableEntity("Пароли не совпадают");
                }
                var ur = new Repositories.UserRepository();
                if (ur.IsLoginUnique(request.Login))
                {
                    _logger.LogInformation($"Username already used");
                    return UnprocessableEntity("Имя пользователя уже занято");
                }
                var ar = new Repositories.AuthorizationRepository();
                ur.WriteNewUserToDatabase(request.Login, request.Password);
                _logger.LogInformation($"User created successfully");
                _logger.LogInformation($"User created successfully");
                return Ok("Аккаунт успешно создан");
            }
            catch (Exception)
            {
                _logger.LogInformation($"User not created");
                return UnprocessableEntity("Не удалось создать пользователя");
            }
        }
    }
}