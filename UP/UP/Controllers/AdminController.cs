using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Exceptions;
using Repository;
using UP.Models;
using UP.ModelsEF;

namespace UP.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly IDbRepository _dbRepository;

    public AdminController(IDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }

    [HttpPost]
    [Authorize]
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
    [Authorize]
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
    [Authorize]
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
    [Authorize]
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
    [Authorize]
    [Route("getUserList")]
    public Task<ActionResult> GetUserList()
    {
        var users = _dbRepository.Get<User>().ToList();
        if (users == null)
            throw new EntityNotFoundException("Users not found");
        return Task.FromResult<ActionResult>(Ok(users));
    }

    [HttpGet]
    [Route("getUserById")]
    public Task<IActionResult> GetUserById(Guid id)
    {
        var users = _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (users == null)
            throw new EntityNotFoundException("Users not found");
        return Task.FromResult<IActionResult>(Ok(users));
    }
    
    [HttpGet]
    [Route("get-all-coins")]
    public async Task<IActionResult> GetCoins()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .ToListAsync();

        return Ok(coinsList);
    }
    
    [HttpGet]
    [Route("get-active-coins")]
    public async Task<IActionResult> GetAllCoins()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .ToListAsync();

        return Ok(coinsList);
    }
    
    [HttpGet]
    [Route("get-active-coins-dict")]
    public async Task<IActionResult> GetCoinsDict()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .Where(x => x.IsActive)
            .ToListAsync();

        var coins = coinsList.ToDictionary(
            coin => coin.ShortName.ToLower(),
            coin => coin.FullName.ToLower()
        );
        return Ok(coins);
    }

    [HttpPatch]
    [Route("set-coin-status")]
    public async Task<IActionResult> SetCoinStatus(string coinName, bool status)

    {
        var coin = await _dbRepository.Get<CoinListInfo>()
            .Where(x => x.ShortName == coinName)
            .FirstOrDefaultAsync();

        if (coin == null)
            throw new EntityNotFoundException("Нет такой монеты");

        coin.IsActive = status;
        coin.DateUpdated = DateTime.UtcNow;
        await _dbRepository.SaveChangesAsync();
        
        return Ok("Статус обновлен");
    }
}