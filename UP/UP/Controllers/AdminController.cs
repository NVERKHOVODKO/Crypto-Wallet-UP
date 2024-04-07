using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Exceptions;
using Repository;
using UP.Models;
using UP.ModelsEF;
using UP.Services.Interfaces;

namespace UP.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly IDbRepository _dbRepository;
    private readonly IAuthService _authService;

    public AdminController(IDbRepository dbRepository, IAuthService authService)
    {
        _dbRepository = dbRepository;
        _authService = authService;
    }

    [HttpPost("blockUser")]
    public async Task<ActionResult> BlockUser(Guid id, string reason)
    {
        var existingUser = await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsBlocked = true;

        await _dbRepository.SaveChangesAsync();
        return Ok("Пользователь заблокирован");
    }

    [HttpPost("deleteUser")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var existingUser = await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsDeleted = true;

        await _dbRepository.SaveChangesAsync();
        return Ok("Пользователь удален");
    }

    [HttpPut("setStatusDel")]
    public async Task<IActionResult> SetStatusDel(Guid id, bool status)
    {
        var existingUser = await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsDeleted = status;

        await _dbRepository.SaveChangesAsync();
        return Ok("Пользователь редактирован");
    }

    [HttpPut("setStatusBlock")]
    public async Task<IActionResult> SetStatusBlock(Guid id, bool status)
    {
        var existingUser = await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (existingUser == null)
            throw new EntityNotFoundException("User not found");

        existingUser.IsBlocked = status;

        await _dbRepository.SaveChangesAsync();
        return Ok("Пользователь редактирован");
    }

    [Authorize]
    [HttpGet("getUserList")]
    public Task<ActionResult> GetUserList()
    {
        var users = _dbRepository.Get<User>().ToList();
        if (users == null)
            throw new EntityNotFoundException("Users not found");
        return Task.FromResult<ActionResult>(Ok(users));
    }

    [HttpGet("getUserById")]
    public Task<IActionResult> GetUserById(Guid id)
    {
        var users = _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (users == null)
            throw new EntityNotFoundException("Users not found");
        return Task.FromResult<IActionResult>(Ok(users));
    }
    
    [HttpGet("get-all-coins")]
    public async Task<IActionResult> GetCoins()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .ToListAsync();

        return Ok(coinsList);
    }
    
    [HttpGet("get-active-coins")]
    public async Task<IActionResult> GetAllCoins()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .ToListAsync();

        return Ok(coinsList);
    }
    
    [AllowAnonymous]
    [HttpGet("get-active-coins-dict")]
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

    [HttpPatch("set-coin-status")]
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
    
    [AllowAnonymous]
    [HttpPost("getToken/{email}")]
    public async Task<IActionResult> GetToken(string email)
    {
        var response = await _authService.GetTokenAsync(email);
        return Ok(response);
    }
}