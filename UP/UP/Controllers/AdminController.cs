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
        public async Task<ActionResult> BlockUser(int id, string reason)
        {
            var ar = new AdminRepository();
            try
            {
                ar.BlockUser(id, reason);
                return Ok("Пользователь заблокирован");
            }
            catch(Exception)
            {
                return BadRequest("Не удалось заблокировать пользователя");
            }
        }
        
        [HttpPost, Route("deleteUser")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var ar = new AdminRepository();
            try
            {
                ar.DeleteUser(id);
                return Ok("Пльзователь удален");
            }
            catch(Exception)
            {
                return BadRequest("Пользователь не был удален");
            }
        }
        
        [HttpPut, Route("setStatusDel")]
        public async Task<IActionResult> SetStatusDel(int id, bool status)
        {
            try
            {
                var ur = new UserRepository();
                ur.SetUserStatusDel(id, status);
                return Ok("Статус изменен");
            }
            catch(Exception)
            {
                return BadRequest("Не удалось изменить статус");
            }
        }

        [HttpPut, Route("setStatusBlock")]
        public async Task<IActionResult> SetStatusBlock(int id, bool status)
        {
            try
            {
                var ur = new UserRepository();
                ur.SetUserStatusBlock(id, status);
                return Ok("Статус изменен");
            }
            catch(Exception)
            {
                return BadRequest("Не удалось изменить статус");
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
                return BadRequest("Не удалось вернуть список пользователя");
            }
        }
    }
}