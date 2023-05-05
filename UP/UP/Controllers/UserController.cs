using Microsoft.AspNetCore.Mvc;
using UP.DTO;

namespace UP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase 
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        
        [HttpPut, Route("editUserLogin")]
        public async Task<ActionResult> EditUserLogin([FromBody] DTO.EditUserLoginRequest request)
        {
            _logger.LogInformation($"Edit: login: (" + request.Login + ")");
            var ur = new Repositories.UserRepository();
            try
            {
                if (request.Login.Length < 4)
                {
                    return BadRequest("Логин должен быть больше 4 символов");
                }
                if (ur.IsLoginUnique(request.Login))
                {
                    return BadRequest("Этот логин уже используется");
                }
                if (request.Login != null)
                {
                    ur.ChangeUserLogin(request.Id, request.Login);
                }
                return Ok("User edited");
            }
            catch(Exception)
            {
                return BadRequest("Не удалось изменить логин");
            }
        }
        
        [HttpPut, Route("editUserPassword")]
        public async Task<ActionResult> EditUser([FromBody] DTO.EditUserPasswordRequest request)
        {
            _logger.LogInformation($"Edit: password: (" + request.Password + ")\npasswordRep: (" + request.PasswordRepeat + ")");
            var ur = new Repositories.UserRepository();
            try
            {
                if (request.Password.Length == 0)
                {
                    return BadRequest("Введите пароль");
                }
                if (request.Password.Length < 4)
                {
                    return BadRequest("Пароль должен быть больше 4 символов");
                }
                if (request.Password != null)
                {
                    if (request.Password == request.PasswordRepeat)
                    {
                        ur.ChangeUserPassword(request.Id, request.Password);
                    }
                    else
                    {
                        return BadRequest("Пароли не совпадают");
                    }
                }
                return Ok("Пароль успешно изменен");
            }
            catch(Exception)
            {
                return BadRequest("Не удалось изменить пароль");
            }
        }
        
        [HttpPut, Route("editUserEmail")]
        public async Task<ActionResult> EditUserEmail([FromBody] DTO.EditUserEmailRequest request)
        {
            _logger.LogInformation($"Edit: email: (" + request.Email + ")");
            var ur = new Repositories.UserRepository();
            try
            {
                if (request.Email.Length < 4)
                {
                    return BadRequest("Email должен быть больше 4 символов");
                }
                if (ur.IsLoginUnique(request.Email))
                {
                    return BadRequest("Этот Email уже используется");
                }
                if (request.Email != null)
                {
                    ur.ChangeUserEmail(request.Id, request.Email);
                }
                return Ok("Email edited");
            }
            catch(Exception)
            {
                return BadRequest("Не удалось изменить Email");
            }
        }
        
        [HttpPut, Route("editUser")]
        public async Task<ActionResult> EditUser([FromBody] DTO.EditUserRequest request)
        {
            _logger.LogInformation($"Edit: login: (" + request.Login + ")\npassword: (" + request.Password + ")\npasswordRep: (" + request.PasswordRepeat + ")\nemail: " + request.Email);
            var ur = new Repositories.UserRepository();
            try
            {
                if (request.Login.Length < 4)
                {
                    return BadRequest("Логин должен быть больше 4 символов");
                }
                if (request.Password.Length < 4)
                {
                    return BadRequest("Пароль должен быть больше 4 символов");
                }
                if (request.Password != null)
                {
                    if (request.Password == request.PasswordRepeat)
                    {
                        ur.ChangeUserPassword(request.Id, request.Password);
                    }
                    else
                    {
                        return BadRequest("Passwords are doesn't match");
                    }
                }
                if (request.Email.Length < 4)
                {
                    return BadRequest("Email должен быть больше 4 символов");
                }
                if (request.Email != null)
                {
                    ur.ChangeUserEmail(request.Id, request.Email);
                }
                if (ur.IsLoginUnique(request.Login))
                {
                    return BadRequest("Этот логин уже используется");
                }
                if (request.Login != null)
                {
                    ur.ChangeUserLogin(request.Id, request.Login);
                }
                return Ok("User edited");
            }
            catch(Exception)
            {
                return BadRequest("Status not changed");
            }
        }
        
        [HttpGet, Route("getUserPreviousPasswordsList")]
        public async Task<ActionResult> GetUserPreviousPasswordsList(int id)
        {
            var ur = new Repositories.UserRepository();
            try
            {
                var passwords = ur.GetUserPasswordsHistory(id);
                if (passwords.Count == 0)
                    return Ok("There is no passwords");
                return Ok(passwords);
            }
            catch(Exception)
            {
                return BadRequest("Can't get user's previous passwords");
            }
        }
        
        [HttpGet, Route("getUserLoginHistory")]
        public async Task<IActionResult> GetUserLoginHistory(int id)
        {
            var ur = new Repositories.UserRepository();
            try
            {
                var loginHistories = ur.GetUserLoginHistory(id);
                return Ok(loginHistories);
            }
            catch(Exception)
            {
                return BadRequest("Can't return user's login history");
            }
        }

        [HttpDelete, Route("deleteAccount")]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            try
            {
                var ur = new Repositories.UserRepository();
                ur.DeleteUser(id);
                return Ok("Account deleted successfully");
            }
            catch(Exception)
            {
                return BadRequest("Account not deleted");
            }
        }
        
        [HttpPut, Route("changeLogin")]
        public async Task<ActionResult> ChangeUserName(int id, string newLogin)
        {
            var ur = new Repositories.UserRepository();
            try
            {
                ur.ChangeUserName(id, newLogin);
                return Ok("Login changed");
            }
            catch(Exception)
            {
                return BadRequest("Status not changed");
            }
        }
        
        [HttpPut, Route("changePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var ur = new Repositories.UserRepository();
            var user = ur.GetUserById(request.Id);
            try
            {
                if (request.Password != request.PasswordRepeat)
                {
                    return UnprocessableEntity("Passwords doesn't match");
                }
                if (request.Password.Length == 0)
                {
                    return UnprocessableEntity("Fill in the fields");
                }
                if (request.Password.Length > 32)
                {
                    return UnprocessableEntity("Password must be less than 32 symbols");
                }
                if (request.Password.Length < 4)
                {
                    return UnprocessableEntity("Password must be above than 3 symbols");
                }
                ur.ChangePassword(request.Id, request.Password);
                return Ok("Password changed");
            }
            catch(Exception)
            {
                return BadRequest("Password not changed");
            }
        }
    }
}