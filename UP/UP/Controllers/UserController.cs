using Microsoft.AspNetCore.Mvc;
using UP.DTO;

namespace UP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase 
    {
        [HttpPut, Route("editUser")]
        public async Task<ActionResult> EditUser(int id, Models.User user)
        {
            var ur = new Repositories.UserRepository();
            try
            {
                ur.EditUser(id, user);
                return Ok("Status changed");
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