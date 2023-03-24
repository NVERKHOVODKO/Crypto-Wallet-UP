using Microsoft.AspNetCore.Mvc;

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
                return Ok(ur.GetUserPasswordsHistory(id));
            }
            catch(Exception)
            {
                return BadRequest("Can't get user's previous passwords");
            }
        }
        
        [HttpPut, Route("setStatusDel")]
        public async Task<ActionResult> SetStatusDel(int id, bool status)
        {
            try
            {
                var ur = new Repositories.UserRepository();
                ur.SetUserStatusDel(id, status);
                return Ok("Status changed");
            }
            catch(Exception)
            {
                return BadRequest("Status not changed");
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
        
        [HttpPut, Route("setStatusBlock")]
        public async Task<ActionResult> SetStatusBlock(int id, bool status)
        {
            try
            {
                var ur = new Repositories.UserRepository();
                ur.SetUserStatusBlock(id, status);
                return Ok("Status changed");
            }
            catch(Exception)
            {
                return BadRequest("Status not changed");
            }
        }
        
        [HttpPut, Route("changeLogin")]
        public async Task<ActionResult> ChangeUserName(int id, String newLogin)
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
        public async Task<ActionResult> ChangePassword(int id, String password, String passwordRepeat)
        {
            var ur = new Repositories.UserRepository();
            var user = ur.GetUserById(id);
            try
            {
                if (password != passwordRepeat)
                {
                    return UnprocessableEntity("Passwords doesn't match");
                }
                if (password.Length == 0)
                {
                    return UnprocessableEntity("Fill in the fields");
                }
                if (password.Length > 32)
                {
                    return UnprocessableEntity("Password must be less than 32 symbols");
                }
                if (password.Length < 4)
                {
                    return UnprocessableEntity("Password must be above than 3 symbols");
                }
                ur.ChangePassword(id, password);
                return Ok("Password changed");
            }
            catch(Exception)
            {
                return BadRequest("Password not changed");
            }
        }
        
        [HttpGet, Route("getUserList")]
        public async Task<ActionResult> GetUserList()
        {
            var ur = new Repositories.UserRepository();
            try
            {
                return Ok(ur.GetUserList());
            }
            catch(Exception)
            {
                return BadRequest("Unable to return userList");
            }
        }
    }
}