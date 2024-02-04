using Api.OpenAI.Handlers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Exceptions;
using Repository;
using UP.ModelsEF;

namespace UP.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly IDbRepository _dbRepository;
    private readonly IHashHelpers _hashHelpers;
    private readonly ILogger<UserController> _logger;

    public AdminController(ILogger<UserController> logger, IDbRepository dbRepository, IHashHelpers hashHelpers)
    {
        _logger = logger;
        _dbRepository = dbRepository;
        _hashHelpers = hashHelpers;
    }

    [HttpPost]
    [Route("blockUser")]
    public async Task<ActionResult> BlockUser(Guid id, string reason)
    {
        var existingUser = await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsBlocked = true;

        await _dbRepository.SaveChangesAsync();
        return Ok("Пользователь заблокирован");
    }

    [HttpPost]
    [Route("deleteUser")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var existingUser = await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsDeleted = true;

        await _dbRepository.SaveChangesAsync();
        return Ok("Пользователь удален");
    }

    [HttpPut]
    [Route("setStatusDel")]
    public async Task<IActionResult> SetStatusDel(Guid id, bool status)
    {
        var existingUser = await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsDeleted = status;

        await _dbRepository.SaveChangesAsync();
        return Ok("Пользователь редактирован");
    }

    [HttpPut]
    [Route("setStatusBlock")]
    public async Task<IActionResult> SetStatusBlock(Guid id, bool status)
    {
        var existingUser = await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsBlocked = status;

        await _dbRepository.SaveChangesAsync();
        return Ok("Пользователь редактирован");
    }

    [HttpGet]
    [Route("getUserList")]
    public async Task<ActionResult> GetUserList()
    {
        var users = _dbRepository.Get<User>().ToList();
        if (users == null)
            throw new EntityNotFoundException("Users not found");
        return Ok(users);
    }

    [HttpGet]
    [Route("getUserById")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var users = _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (users == null)
            throw new EntityNotFoundException("Users not found");
        return Ok(users);
    }
}