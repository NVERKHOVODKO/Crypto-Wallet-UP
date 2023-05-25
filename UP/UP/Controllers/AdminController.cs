using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using UP.Models;
using UP.Repositories;

namespace UP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController: ControllerBase
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }
        
        [HttpPost, Route("blockUser")]
        public async Task<ActionResult> BlockUser(int id, string reason)
        {
            var ar = new AdminRepository();
            try
            {
                ar.BlockUser(id, reason);
                _logger.LogInformation($"User(id: " + id + ") is blocked. Reason: " + reason);
                return Ok("Пользователь заблокирован");
            }
            catch(Exception)
            {
                _logger.LogInformation($"Unable to block user");
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
                _logger.LogInformation($"User(id: " + id + ") deleted");
                return Ok("Пльзователь удален");
            }
            catch(Exception)
            {
                _logger.LogInformation($"Unable to delete user");
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
                _logger.LogInformation($"User status changed: id(" + id + ") status(" + status + ")");
                return Ok("Статус изменен");
            }
            catch(Exception)
            {
                _logger.LogInformation($"Unable to change user status");
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
                _logger.LogInformation($"User status changed: id(" + id + ") status(" + status + ")");
                return Ok("Статус изменен");
            }
            catch(Exception)
            {
                _logger.LogInformation($"Unable to change user status");
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
        
        [HttpGet, Route("getUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var ur = new UserRepository();
                var user = ur.GetUserById(id);
                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound("Пользователь не найден");
            }
            catch(Exception)
            {
                return BadRequest("Не удалось получить пользователя");
            }
        }
    }
}