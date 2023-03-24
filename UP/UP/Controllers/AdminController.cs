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
        [HttpPut, Route("blockUser")]
        public async Task<ActionResult> BlockUser(int id, String reason)
        {
            var ar = new Repositories.AdminRepository();
            try
            {
                //ar.BlockUser(id, reason);
                return Ok(reason);
            }
            catch(Exception)
            {
                return BadRequest("Account not blocked");
            }
        }
    }
}