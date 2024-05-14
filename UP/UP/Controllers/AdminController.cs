using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Exceptions;
using Repository;
using UP.Models;
using UP.ModelsEF;
using UP.Services.Interfaces;

namespace UP.Controllers;

/// <summary>
/// Controller for managing administrative tasks
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly IDbRepository _dbRepository;
    private readonly IAuthService _authService;

    /// <summary>
    /// Constructor for AdminController
    /// </summary>
    /// <param name="dbRepository">Instance of IDbRepository</param>
    /// <param name="authService">Instance of IAuthService</param>
    public AdminController(IDbRepository dbRepository, IAuthService authService)
    {
        _dbRepository = dbRepository;
        _authService = authService;
    }

    /// <summary>
    /// Блокирует пользователя по указанному идентификатору и добавляет причину блокировки.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="reason">Причина блокировки пользователя.</param>
    /// <returns>Сообщение о блокировке пользователя.</returns>
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

    /// <summary>
    /// Удаляет пользователя по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns>Сообщение об удалении пользователя.</returns>
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

    /// <summary>
    /// Устанавливает статус удаления пользователя (активен/удален) по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="status">Новый статус пользователя (активен/удален).</param>
    /// <returns>Сообщение об успешном изменении статуса пользователя.</returns>
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

    /// <summary>
    /// Устанавливает статус блокировки пользователя (активен/заблокирован) по указанному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="status">Новый статус блокировки пользователя (активен/заблокирован).</param>
    /// <returns>Сообщение об успешном изменении статуса пользователя.</returns>
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

    /// <summary>
    /// Получает список всех пользователей.
    /// </summary>
    /// <returns>Список пользователей.</returns>
    [Authorize]
    [HttpGet("getUserList")]
    public Task<ActionResult> GetUserList()
    {
        var users = _dbRepository.Get<User>().ToList();
        if (users == null)
            throw new EntityNotFoundException("Users not found");
        
        return Task.FromResult<ActionResult>(Ok(users));
    }

    /// <summary>
    /// Получает информацию о пользователе по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns>Информация о пользователе.</returns>
    [HttpGet("getUserById")]
    public Task<IActionResult> GetUserById(Guid id)
    {
        var users = _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Id == id);
        if (users == null)
            throw new EntityNotFoundException("Users not found");
        
        return Task.FromResult<IActionResult>(Ok(users));
    }
    
    /// <summary>
    /// Получает список всех монет.
    /// </summary>
    /// <returns>Список монет.</returns>
    [HttpGet("get-all-coins")]
    public async Task<IActionResult> GetCoins()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .ToListAsync();

        return Ok(coinsList);
    }
    
    /// <summary>
    /// Получает список всех активных монет.
    /// </summary>
    /// <returns>Список активных монет.</returns>
    [HttpGet("get-active-coins")]
    public async Task<IActionResult> GetAllCoins()
    {
        var coinsList = await _dbRepository.Get<CoinListInfo>()
            .ToListAsync();

        return Ok(coinsList);
    }
    
    /// <summary>
    /// Получает словарь активных монет в виде "сокращенное название монеты" - "полное название монеты".
    /// </summary>
    /// <returns>Словарь активных монет.</returns>
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

    /// <summary>
    /// Устанавливает статус активации/деактивации монеты по её сокращенному названию.
    /// </summary>
    /// <param name="coinName">Сокращенное название монеты.</param>
    /// <param name="status">Новый статус монеты (активна/неактивна).</param>
    /// <returns>Сообщение о успешном обновлении статуса монеты.</returns>
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
    
    /// <summary>
    /// Get token for specified email
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>Generated token</returns>
    [AllowAnonymous]
    [HttpPost("getToken/{email}")]
    public async Task<IActionResult> GetToken(string email)
    {
        var response = await _authService.GetTokenAsync(email);
        return Ok(response);
    }
}