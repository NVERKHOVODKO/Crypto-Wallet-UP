using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using UP.Models;
using Newtonsoft.Json.Linq;

namespace UP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController: ControllerBase
    {
        [HttpPost, Route("blockUser")]
        public async Task<ActionResult> BlockUser(int id, String reason)
        {
            var ar = new Repositories.AdminRepository();
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
            var ar = new Repositories.AdminRepository();
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
        
        [HttpGet, Route("getUserList")]
        public async Task<ActionResult> GetUserList()
        {
            var ar = new Repositories.AdminRepository();
            return Ok(ar.GetUserList());
            /*try
            {
                return Ok(ar.GetUserList());
            }
            catch(Exception)
            {
                return BadRequest("Unable to return userList");
            }*/
        }
    }
}