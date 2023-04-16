using Microsoft.AspNetCore.Mvc;
using UP.DTO;

namespace UP.Controllers
{
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase {
        
        [HttpPost, Route("authorization")]
        public async Task<ActionResult> Login([FromBody] AuthenticationRequest request) {
            try
            {
                var ur = new Repositories.UserRepository();
                var user = ur.Login(request.Login, request.Password);
                if (user != null) {
                    ur.SaveAccountLoginHistory(user.Id);
                    if (user.IsBlocked)
                    {
                        return BadRequest("Your account is blocked: " + ur.GetUserBlockingReason(user.Id));
                    }
                    if (user.IsDeleted)
                    {
                        return NotFound("There is no such user. Try again");
                    }
                    return Ok(user);
                } else {
                    return NotFound("There is no such user. Try again");
                }
            }
            catch (Exception exception)
            {
                return BadRequest("Error");
            }
        }

        [HttpPost, Route("register")]
        public async Task<ActionResult> RegisterNewUser([FromBody] RegisterRequest request) {
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
                if (ar.IsValidEmail(request.Email))
                {
                    return UnprocessableEntity("Email isn't correct");
                }
                ur.WriteNewUserToDatabase(request.Login, request.Password, request.Email);
                return Ok("User created successfully");
            }
            catch (Exception)
            {
                return UnprocessableEntity("User not created");
            }
        }
    }
}