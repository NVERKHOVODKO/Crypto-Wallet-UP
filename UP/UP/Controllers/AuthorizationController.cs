﻿using System.Text.RegularExpressions;
using Analitique.BackEnd.Handlers;
using Api.OpenAI.Handlers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Exceptions;
using Repository;
using UP.DTO;
using UP.ModelsEF;
using UP.Services.Interfaces;

namespace UP.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IDbRepository _dbRepository;
    private readonly IHashHelpers _hashHelpers;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(ILogger<AuthorizationController> logger, IDbRepository dbRepository,
        IHashHelpers hashHelpers, IAuthService authService)
    {
        _logger = logger;
        _dbRepository = dbRepository;
        _authService = authService;
        _hashHelpers = hashHelpers;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
    {
        _logger.LogInformation("Логин: " + request.Login);
        _logger.LogInformation("Пароль: " + request.Password);

        var users = await _dbRepository.Get<User>()
            .Where(x => x.Login == request.Login)
            .ToListAsync();

        var user = users.FirstOrDefault(x => x.Password == HashHandler.HashPassword(request.Password, x.Salt));

        if (user == null) throw new EntityNotFoundException("Пользователь не найден");

        var record = new LoginHistory
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DateCreated = DateTime.UtcNow,
            IPAddress = LoginHistory.GetIPAddress()
        };
        await _dbRepository.Add(record);
        await _dbRepository.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterNewUser([FromBody] RegisterRequest request)
    {
        if (request.Login == null || request.Password == null || request.Email == null)
            throw new IncorrectDataException("Заполните все поля");
        switch (request.Login.Length)
        {
            case > 20:
                throw new IncorrectDataException("Логин должен быть короче 20 символов");
            case < 4:
                throw new IncorrectDataException("Логин должен быть длиннее 4 символов");
        }

        switch (request.Password.Length)
        {
            case > 40:
                throw new IncorrectDataException("Пароль должен быть короче 40 символов");
            case < 4:
                throw new IncorrectDataException("Пароль должен быть длиннее 4 символов");
        }

        if (!IsEmailValid(request.Email)) throw new IncorrectDataException("Неверный формат email");
        if (await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Email == request.Email) != null)
            throw new IncorrectDataException("Email уже используется");
        if (await _dbRepository.Get<User>().FirstOrDefaultAsync(x => x.Login == request.Login) != null)
            throw new IncorrectDataException("Логин уже используется");

        var salt = _hashHelpers.GenerateSalt(30);
        var entity = new User
        {
            Login = request.Login,
            Password = _hashHelpers.HashPassword(request.Password, salt),
            Email = request.Email,
            DateCreated = DateTime.UtcNow,
            IsDeleted = false,
            IsBlocked = false,
            RoleId = 1,
            Salt = salt
        };
        await _dbRepository.Add(entity);
        await _dbRepository.SaveChangesAsync();

        return Ok("Аккаунт успешно создан");

        bool IsEmailValid(string email)
        {
            const string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }
    }
    
    [HttpPost("getToken")]
    [AllowAnonymous]
    public async Task<IActionResult> GetToken([FromBody] GetTokenRequest request)
    {
        var response = await _authService.GetTokenAsync(request);
        return Ok(response);
    }
}