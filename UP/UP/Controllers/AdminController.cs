using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UP.Repositories;

namespace UP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController: ControllerBase
    {
        [HttpPost, Route("blockUser")]
        public async Task<ActionResult> BlockUser(int id, String reason)
        {
            var ar = new AdminRepository();
            try
            {
                ar.BlockUser(id, reason);
                return Ok("User blocked");
            }
            catch(Exception)
            {
                return BadRequest("User not blocked");
            }
        }
        
        [HttpPost, Route("deleteUser")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var ar = new AdminRepository();
            try
            {
                ar.DeleteUser(id);
                return Ok("User deleted");
            }
            catch(Exception)
            {
                return BadRequest("User not deleted");
            }
        }
        
        [HttpPut, Route("setStatusDel")]
        public async Task<IActionResult> SetStatusDel(int id, bool status)
        {
            try
            {
                var ur = new UserRepository();
                ur.SetUserStatusDel(id, status);
                return Ok("Status changed");
            }
            catch(Exception)
            {
                return BadRequest("Status not changed");
            }
        }

        [HttpPut, Route("setStatusBlock")]
        public async Task<IActionResult> SetStatusBlock(int id, bool status)
        {
            try
            {
                var ur = new UserRepository();
                ur.SetUserStatusBlock(id, status);
                return Ok("Status changed");
            }
            catch(Exception)
            {
                return BadRequest("Status not changed");
            }
        }
        
        [EnableCors]
        [HttpGet, Route("getUserList")]
        public async Task<ActionResult> GetUserList()
        {
            try
            {
                var ar = new AdminRepository();
                return Ok(ar.GetUserList());
            }
            catch(Exception)
            {
                return BadRequest("Unable to return userList");
            }
        }
    }
}